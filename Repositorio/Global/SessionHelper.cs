using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Repositorio
{
    class SessionHelper
    {
        private static ISessionFactory _Factory;
        private static ISessionFactory SessionFactory(string stringConexao)
        {
            lock (typeof(SessionHelper))
            {
                if (_Factory == null)
                {
                    NHibernate.Cfg.Configuration config = default(NHibernate.Cfg.Configuration);

                    config = new NHibernate.Cfg.Configuration();

                    config.DataBaseIntegration(x =>
                    {
                        x.ConnectionString = stringConexao;
                        x.Dialect<CustomDialect>();
                        x.Driver<NHibernate.Driver.SqlClientDriver>();
                        //x.IsolationLevel = System.Data.IsolationLevel.Snapshot;
#if DEBUG
                        x.LogFormattedSql = true;
                        x.LogSqlInConsole = true;
#else
                        x.LogFormattedSql = false;
                        x.LogSqlInConsole = false;
#endif

                    })
                    .Proxy(x => x.ProxyFactoryFactory<NHibernate.Bytecode.StaticProxyFactoryFactory>())
                    .LinqToHqlGeneratorsRegistry<ExtendedLinqtoHqlGeneratorsRegistry>();

                    config.BeforeBindMapping += (sender, args) => args.Mapping.autoimport = false;

                    NHibernate.Mapping.Attributes.HbmSerializer.Default.Validate = false;
                    NHibernate.Mapping.Attributes.HbmSerializer.Default.HbmAutoImport = false;

#if DEBUG
                    using System.IO.MemoryStream stream = ObterSream();
#else
                    using System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    NHibernate.Mapping.Attributes.HbmSerializer.Default.Serialize(stream, typeof(Dominio.Entidades.EntidadeBase).Assembly);
#endif

                    stream.Position = 0;
                    config.AddInputStream(stream);
                    stream.Close();

                    _Factory = config.BuildSessionFactory();

                    //Código feito para salvar o script de banco de dados. Deletar o arquivo scriptBanco.sql da pasta para que ele gere um arquivo novo.
#if DEBUG
                    FileInfo fileInfo = new FileInfo(string.Concat(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
                    DirectoryInfo parentDir = fileInfo.Directory.Parent;

                    string caminhoScript = Utilidades.IO.FileStorageService.LocalStorage.Combine(@"D:\admin multi", "schemaME.sql");

                    if (!Utilidades.IO.FileStorageService.LocalStorage.Exists(caminhoScript))
                    {
                        SchemaExport schema = new SchemaExport(config);
                        using (System.IO.StreamWriter streamEscrita = new System.IO.StreamWriter(Utilidades.IO.FileStorageService.LocalStorage.OpenWrite(caminhoScript)))
                        {
                            schema.SetDelimiter(";");
                            schema.Execute(true, false, false, null, streamEscrita);
                        }
                    }
#endif

                    if (_Factory == null)
                    {
                        throw new InvalidOperationException("Não foi possível criar o SessionFactory!");
                    }
                }
            }


            return _Factory;
        }
        private static MemoryStream ObterSream()
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            try
            {
                string pathApp = AppDomain.CurrentDomain.BaseDirectory;
                string pathBin = pathApp.Contains(@"\bin\") ? pathApp : Utilidades.IO.FileStorageService.LocalStorage.Combine(pathApp, "bin");
                string pathCache = Utilidades.IO.FileStorageService.LocalStorage.Combine(pathBin, "cacheNHibernateME.cache");

                if (VefificarDataDominioAlterado(pathBin))
                {
                    if (Utilidades.IO.FileStorageService.LocalStorage.Exists(pathCache))
                        Utilidades.IO.FileStorageService.LocalStorage.Delete(pathCache);
                }

                if (Utilidades.IO.FileStorageService.LocalStorage.Exists(pathCache))
                {
                    LoadStream(stream, pathCache);
                    return stream;
                }

                NHibernate.Mapping.Attributes.HbmSerializer.Default.Serialize(stream, typeof(Dominio.Entidades.EntidadeBase).Assembly);

                SaveMemoryStream(stream, pathCache);
                SaveControlCache(pathBin);

                return stream;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                NHibernate.Mapping.Attributes.HbmSerializer.Default.Serialize(stream, typeof(Dominio.Entidades.EntidadeBase).Assembly);
                return stream;
            }

        }
        private static void SaveMemoryStream(MemoryStream ms, string FileName)
        {
            using (Stream outStream = Utilidades.IO.FileStorageService.LocalStorage.OpenWrite(FileName))
                ms.WriteTo(outStream);
        }
        private static void SaveControlCache(string path)
        {
            string pathContoleCache = Utilidades.IO.FileStorageService.LocalStorage.Combine(path, "cacheNHibernateME.txt");
            string fileDominio = Utilidades.IO.FileStorageService.LocalStorage.Combine(path, "Dominio.dll");

            DateTime dataDominio = Utilidades.IO.FileStorageService.LocalStorage.GetLastWriteTime(fileDominio);

            Utilidades.IO.FileStorageService.LocalStorage.WriteAllText(pathContoleCache, dataDominio.ToString());
        }
        private static bool VefificarDataDominioAlterado(string path)
        {
            string fileContoleCache = Utilidades.IO.FileStorageService.LocalStorage.Combine(path, "cacheNHibernateME.txt");
            string fileDominio = Utilidades.IO.FileStorageService.LocalStorage.Combine(path, "Dominio.dll");

            DateTime dataDominio = Utilidades.IO.FileStorageService.LocalStorage.GetLastWriteTime(fileDominio);

            string dataTexto = string.Empty;
            if (Utilidades.IO.FileStorageService.LocalStorage.Exists(fileContoleCache))
                dataTexto = Utilidades.IO.FileStorageService.LocalStorage.ReadAllText(fileContoleCache);

            if (dataTexto == string.Empty)
                return true;

            return dataDominio.ToString() != dataTexto;
        }
        private static void LoadStream(MemoryStream ms, string FileName)
        {
            using (Stream source = Utilidades.IO.FileStorageService.LocalStorage.OpenRead(FileName))
            {
                ms.Position = 0;
                source.CopyTo(ms);
            }
        }

        public static ISession OpenSession(string stringConexao)
        {
            ISession session = default(ISession);

            System.Data.SqlClient.SqlConnection conexao = new System.Data.SqlClient.SqlConnection(stringConexao);

            conexao.Open();

            session = SessionFactory(stringConexao).WithOptions().Connection(conexao).OpenSession();

            if (session == null)
                throw new InvalidOperationException("Não foi possível abrir a sessão!");

            return session;
        }

        [Obsolete("Utilizar apenas para testes de performance das threads")]
        public static ISession OpenSession(string stringConexao, System.Data.SqlClient.SqlConnection conexao)
        {
            ISession session = default(ISession);
            if (conexao.State != ConnectionState.Open)
            {
                conexao.Open();
            }

            session = SessionFactory(stringConexao).WithOptions().Connection(conexao).OpenSession();

            if (session == null)
                throw new InvalidOperationException("Não foi possível abrir a sessão!");

            return session;
        }
    }
}

class ExtendedLinqtoHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
{
    public ExtendedLinqtoHqlGeneratorsRegistry()
    {
        this.Merge(new AddDaysGenerator());
        this.Merge(new AddHoursGenerator());
        this.Merge(new AddMinutesGenerator());
        this.Merge(new FKG_NRO_PEDIDO_EMBARCADOR());
        //this.Merge(new RegexMatchGenerator());
    }
}

class AddDaysGenerator : BaseHqlGeneratorForMethod
{
    public AddDaysGenerator()
    {
        SupportedMethods = new[]
        {
            NHibernate.Util.ReflectHelper.GetMethodDefinition<DateTime>(x => x.AddDays(0)),
        };
    }

    public override HqlTreeNode BuildHql(
        MethodInfo method,
        Expression targetObject,
        ReadOnlyCollection<Expression> arguments,
        HqlTreeBuilder treeBuilder,
        IHqlExpressionVisitor visitor)
    {
        return treeBuilder.MethodCall("AddDays",
                                      visitor.Visit(targetObject).AsExpression(),
                                      visitor.Visit(arguments[0]).AsExpression());
    }
}


class AddHoursGenerator : BaseHqlGeneratorForMethod
{
    public AddHoursGenerator()
    {
        SupportedMethods = new[]
        {
            NHibernate.Util.ReflectHelper.GetMethodDefinition<DateTime>(x => x.AddHours(0))
        };
    }

    public override HqlTreeNode BuildHql(
        MethodInfo method,
        Expression targetObject,
        ReadOnlyCollection<Expression> arguments,
        HqlTreeBuilder treeBuilder,
        IHqlExpressionVisitor visitor)
    {
        return treeBuilder.MethodCall("AddHours",
                                      visitor.Visit(targetObject).AsExpression(),
                                      visitor.Visit(arguments[0]).AsExpression());
    }
}

class AddMinutesGenerator : BaseHqlGeneratorForMethod
{
    public AddMinutesGenerator()
    {
        SupportedMethods = new[]
        {
            NHibernate.Util.ReflectHelper.GetMethodDefinition<DateTime>(x => x.AddMinutes(0))
        };
    }

    public override HqlTreeNode BuildHql(
        MethodInfo method,
        Expression targetObject,
        ReadOnlyCollection<Expression> arguments,
        HqlTreeBuilder treeBuilder,
        IHqlExpressionVisitor visitor)
    {
        return treeBuilder.MethodCall("AddMinutes",
                                      visitor.Visit(targetObject).AsExpression(),
                                      visitor.Visit(arguments[0]).AsExpression());
    }
}

class CustomDialect : MsSql2012Dialect
{
    public CustomDialect()
    {
        RegisterFunction(
            "AddDays",
            new SQLFunctionTemplate(
                NHibernateUtil.DateTime,
                "dateadd(day,?2,?1)"
                )
            );

        RegisterFunction(
            "AddMinutes",
            new SQLFunctionTemplate(
                NHibernateUtil.DateTime,
                "dateadd(minute,?2,?1)"
                )
            );

        RegisterFunction(
            "AddHours",
            new SQLFunctionTemplate(
                NHibernateUtil.DateTime,
                "dateadd(hour,?2,?1)"
                )
            );
    }
}

class FKG_NRO_PEDIDO_EMBARCADOR : BaseHqlGeneratorForMethod
{
    public FKG_NRO_PEDIDO_EMBARCADOR()
    {
        var methodDefinition = NHibernate.Util.ReflectHelper.GetMethodDefinition(() => Extensions.TryParseLong(null));
        SupportedMethods = new[] { methodDefinition };
    }

    public override HqlTreeNode BuildHql(
        MethodInfo method,
        Expression targetObject,
        ReadOnlyCollection<Expression> arguments,
        HqlTreeBuilder treeBuilder,
        IHqlExpressionVisitor visitor)
    {
        return treeBuilder.MethodCall("dbo.FKG_NRO_PEDIDO_EMBARCADOR",
                                      visitor.Visit(targetObject).AsExpression(),
                                      visitor.Visit(arguments[0]).AsExpression());
    }
}

static class Extensions
{
    //public static bool RegexMatch(this string s, string pattern)
    //{
    //    return System.Text.RegularExpressions.Regex.IsMatch(s, pattern);
    //}

    //public static string RegexMatchValue(this string s, string pattern)
    //{
    //    return System.Text.RegularExpressions.Regex.Match(s, pattern)?.Value;
    //}

    public static long TryParseLong(this string s)
    {
        long tmp = 0;
        string t = System.Text.RegularExpressions.Regex.Replace(s, @"[^\d]", "");
        t = System.Text.RegularExpressions.Regex.Match(t, @"\d+")?.Value;
        long.TryParse(t, out tmp);
        return tmp;
    }
}

//public class RegexMatchGenerator : BaseHqlGeneratorForMethod
//{
//    public RegexMatchGenerator()
//    {
//        var methodDefinition = ReflectionHelper.GetMethodDefinition(() => Extensions.RegexMatch(null, null));
//        var methodDefinitionValue = ReflectionHelper.GetMethodDefinition(() => Extensions.RegexMatchValue(null, null));

//        SupportedMethods = new[] { methodDefinition, methodDefinitionValue };
//    }

//    public override HqlTreeNode BuildHql(
//      MethodInfo method,
//      Expression targetObject,
//      ReadOnlyCollection<Expression> arguments,
//      HqlTreeBuilder treeBuilder,
//      IHqlExpressionVisitor visitor
//    )
//    {
//        return treeBuilder.Equality(
//          treeBuilder.MethodCall("dbo.RegexMatch", new[]
//          {
//             visitor.Visit(arguments[0]).AsExpression(),
//             visitor.Visit(arguments[1]).AsExpression()
//          }),
//          treeBuilder.Constant(1)
//        );
//    }
//}