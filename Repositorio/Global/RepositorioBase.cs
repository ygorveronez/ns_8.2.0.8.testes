using Dominio.ObjetosDeValor.Enumerador;
using NHibernate.Linq;
using NHibernate.Mapping.Attributes;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class RepositorioBase<T> : Dominio.Interfaces.Repositorios.Generico<T> where T : Dominio.Entidades.EntidadeBase
    {
        #region Atributos Privados

        private readonly UnitOfWork _unitOfWork;
        private readonly CancellationToken _cancellationToken;

        #endregion Atributos Privados

        #region Atributos Protegidos

        protected string StringConexao
        {
            get { return _unitOfWork.StringConexao; }
        }

        protected NHibernate.ISession SessionNHiBernate
        {
            get
            {
                return _unitOfWork.Sessao;

            }
        }

        protected UnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        protected CancellationToken CancellationToken { get { return _cancellationToken; } }

        #endregion Atributos Protegidos

        #region Construtores

        public RepositorioBase(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public RepositorioBase(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;
            _cancellationToken = cancellationToken;
        }

        #endregion Construtores

        #region Métodos Privados Auditoria

        private void AtualizarHistoricoBag(Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai, dynamic obj, string propriedade, Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjetoGerado)
        {
            Dominio.Entidades.Auditoria.HistoricoPropriedade historicoPropriedade = new Dominio.Entidades.Auditoria.HistoricoPropriedade(propriedade, obj.Descricao, obj.Descricao, historicoObjetoGerado);
            historicoPai.Propriedades.Add(historicoPropriedade);
        }

        private Dominio.Entidades.Auditoria.HistoricoObjeto Auditar(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, dynamic entidade, List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, AcaoBancoDados acao, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai, string descricaoAcao = "")
        {
            Auditoria.HistoricoObjeto repositorioHistoricoObjeto = new Auditoria.HistoricoObjeto(UnitOfWork);
            string nomeEntidade = entidade.GetType().Name.Replace("ProxyForFieldInterceptor", "").Replace("Proxy", "");

            if (historioPai == null)
            {
                Dominio.Entidades.Auditoria.HistoricoObjeto historico = new Dominio.Entidades.Auditoria.HistoricoObjeto
                {
                    CodigoObjeto = (long)entidade.Codigo,
                    Data = DateTime.Now,
                    Objeto = nomeEntidade,
                    Acao = acao,
                    Descricao = Utilidades.String.Left((string)entidade.Descricao, 300),
                    DescricaoAcao = !string.IsNullOrWhiteSpace(descricaoAcao) ? descricaoAcao : acao.ObterDescricao(),
                    Empresa = Auditado.Empresa,
                    Usuario = Auditado.Usuario,
                    UsuarioMultisoftware = Auditado.Usuario?.DescricaoUsuarioInterno.Left(200) ?? string.Empty,
                    IP = Auditado.IP,
                    Integradora = Auditado.Integradora,
                    TipoAuditado = Auditado.TipoAuditado,
                    OrigemAuditado = Auditado.OrigemAuditado
                };

                if (alteracoes != null)
                    historico.Propriedades = alteracoes;

                repositorioHistoricoObjeto.Inserir(historico);

                return historico;
            }
            else
            {
                switch (acao)
                {
                    case AcaoBancoDados.Insert:
                        InserirHistoricoBag(historioPai, entidade, nomeEntidade);
                        repositorioHistoricoObjeto.Atualizar(historioPai);
                        return historioPai;

                    case AcaoBancoDados.Delete:
                        ExcluirHistoricoBag(historioPai, entidade, nomeEntidade);
                        repositorioHistoricoObjeto.Atualizar(historioPai);
                        return historioPai;

                    case AcaoBancoDados.Update:
                        if ((alteracoes?.Count ?? 0) > 0)
                        {
                            Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjetoFilho = Auditar(Auditado, entidade, alteracoes, acao, null);
                            AtualizarHistoricoBag(historioPai, entidade, nomeEntidade, historicoObjetoFilho);
                            repositorioHistoricoObjeto.Atualizar(historioPai);
                            return historicoObjetoFilho;
                        }
                        else
                            return historioPai;

                    default:
                        return null;
                }
            }
        }

        private async Task<Dominio.Entidades.Auditoria.HistoricoObjeto> AuditarAsync(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, dynamic entidade, List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, AcaoBancoDados acao, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai, string descricaoAcao = "")
        {
            Auditoria.HistoricoObjeto repositorioHistoricoObjeto = new Auditoria.HistoricoObjeto(UnitOfWork, _cancellationToken);
            string nomeEntidade = entidade.GetType().Name.Replace("ProxyForFieldInterceptor", "").Replace("Proxy", "");

            if (historioPai == null)
            {
                Dominio.Entidades.Auditoria.HistoricoObjeto historico = new Dominio.Entidades.Auditoria.HistoricoObjeto
                {
                    CodigoObjeto = (long)entidade.Codigo,
                    Data = DateTime.Now,
                    Objeto = nomeEntidade,
                    Acao = acao,
                    Descricao = Utilidades.String.Left((string)entidade.Descricao, 300),
                    DescricaoAcao = !string.IsNullOrWhiteSpace(descricaoAcao) ? descricaoAcao : acao.ObterDescricao(),
                    Empresa = Auditado.Empresa,
                    Usuario = Auditado.Usuario,
                    UsuarioMultisoftware = Auditado.Usuario?.DescricaoUsuarioInterno.Left(200) ?? string.Empty,
                    IP = Auditado.IP,
                    Integradora = Auditado.Integradora,
                    TipoAuditado = Auditado.TipoAuditado,
                    OrigemAuditado = Auditado.OrigemAuditado
                };

                if (alteracoes != null)
                    historico.Propriedades = alteracoes;

                await repositorioHistoricoObjeto.InserirAsync(historico);

                return historico;
            }
            else
            {
                switch (acao)
                {
                    case AcaoBancoDados.Insert:
                        InserirHistoricoBag(historioPai, entidade, nomeEntidade);
                        await repositorioHistoricoObjeto.AtualizarAsync(historioPai);
                        return historioPai;

                    case AcaoBancoDados.Delete:
                        ExcluirHistoricoBag(historioPai, entidade, nomeEntidade);
                        await repositorioHistoricoObjeto.AtualizarAsync(historioPai);
                        return historioPai;

                    case AcaoBancoDados.Update:
                        if ((alteracoes?.Count ?? 0) > 0)
                        {
                            Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjetoFilho = await AuditarAsync(Auditado, entidade, alteracoes, acao, null);
                            AtualizarHistoricoBag(historioPai, entidade, nomeEntidade, historicoObjetoFilho);
                            await repositorioHistoricoObjeto.AtualizarAsync(historioPai);
                            return historicoObjetoFilho;
                        }
                        else
                            return historioPai;

                    default:
                        return null;
                }
            }
        }

        private void ExcluirHistoricoBag(Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai, dynamic obj, string propriedade)
        {
            Dominio.Entidades.Auditoria.HistoricoPropriedade historicoPropriedade = new Dominio.Entidades.Auditoria.HistoricoPropriedade(propriedade, obj.Descricao, "");
            historicoPai.Propriedades.Add(historicoPropriedade);
        }

        private void InserirHistoricoBag(Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai, dynamic obj, string propriedade)
        {
            Dominio.Entidades.Auditoria.HistoricoPropriedade historicoPropriedade = new Dominio.Entidades.Auditoria.HistoricoPropriedade(propriedade, "", obj.Descricao);
            historicoPai.Propriedades.Add(historicoPropriedade);
        }

        #endregion Métodos Privados Auditoria

        #region Métodos Privados

        private List<Repositorio.Global.Dicionario.MapProperty> GetDicionarioPropriedades(T entidade, ref string tabelaDb)
        {

            foreach (var at in entidade.GetType().GetCustomAttributesData())
                foreach (var item in at.NamedArguments)
                    if (item.MemberName == "Table")
                    {
                        tabelaDb = item.TypedValue.Value.ToString();
                        break;
                    }

            var classAttribute = entidade.GetType().GetCustomAttributes(typeof(ClassAttribute), true)
                .FirstOrDefault() as ClassAttribute;
            if (classAttribute != null)
            {
                string nomeDaTabela = classAttribute.Table;
            }



            List<Repositorio.Global.Dicionario.MapProperty> dicionarioPropriedades = new List<Repositorio.Global.Dicionario.MapProperty>();   // guardo o nome da coluna e o indice da propriedade
            PropertyInfo[] propriedades = entidade.GetType().GetProperties();
            int i = -1;
            foreach (PropertyInfo property in propriedades)
            {
                i++;
                if (!property.CanWrite)
                    continue;
                try
                {

                    Repositorio.Global.Dicionario.MapProperty _property = new Repositorio.Global.Dicionario.MapProperty();
                    Repositorio.Global.Dicionario.MapProperty _propertyClass = new Repositorio.Global.Dicionario.MapProperty();

                    var KeyAttribute = property.CustomAttributes.Where(x => x.AttributeType.Name == "KeyAttribute").FirstOrDefault();
                    if (KeyAttribute != null)
                        continue;

                    var GeneratorAttribute = property.CustomAttributes.Where(x => x.AttributeType.Name == "GeneratorAttribute").FirstOrDefault();
                    if (GeneratorAttribute != null && property.Name == "Codigo")
                        _property.ScopeIdentity = true;


                    foreach (var customAttribute in property.CustomAttributes)
                    {
                        bool isClass = false;
                        foreach (var namedArgument in customAttribute.NamedArguments)
                        {
                            if (namedArgument.MemberName == "Formula")
                                continue;

                            if (!isClass && namedArgument.MemberName == "Class")
                            {
                                isClass = true;
                                continue;
                            }
                            if (namedArgument.MemberName == "Column")
                            {
                                _property.DBName = namedArgument.TypedValue.Value.ToString();
                                _property.PropoertyName = property.Name;


                                if (property.Name == "Codigo")
                                    _property.IsKey = true;

                                _property.Index = i;
                                if (isClass)
                                {

                                    _propertyClass.DBName = namedArgument.TypedValue.Value.ToString();
                                    _propertyClass.PropoertyName = "Codigo";
                                    _propertyClass.Index = i;
                                    _property.IsClass = _propertyClass;
                                }
                                var atributLen = customAttribute.NamedArguments.Where(x => x.MemberName == "Length").FirstOrDefault();
                                if (atributLen != null && atributLen.MemberInfo != null && atributLen.MemberName == "Length")
                                    _property.MaxLen = (int)atributLen.TypedValue.Value;

                                var atributNotNull = customAttribute.NamedArguments.Where(x => x.MemberName == "NotNull").FirstOrDefault();
                                if (atributNotNull != null && atributNotNull.MemberInfo != null && atributNotNull.MemberName == "NotNull")
                                    _property.NotNull = Convert.ToBoolean(atributNotNull.TypedValue.Value);

                                dicionarioPropriedades.Add(_property);
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            return dicionarioPropriedades;
        }

        //private string GetValorSql(dynamic valor, Repositorio.Global.Dicionario.MapProperty mapProperty)
        //{
        //    string _valor = "";
        //    if (mapProperty.IsClass != null && valor != null)
        //        valor = valor.GetType().GetProperty("Codigo").GetValue(valor, null);

        //    if (valor == null)
        //        _valor = "NULL";

        //    else if (valor is bool)
        //        _valor = (bool)valor ? "1" : "0";

        //    else if (valor is int)
        //        _valor = Convert.ToString(valor);
        //    else if (valor is int || valor is double || valor is decimal || valor is long)
        //    {
        //        _valor = Convert.ToString(valor);
        //        _valor = _valor.Replace(',', '.');
        //    }
        //    else if (valor is DateTime)
        //        _valor = $@"'{((DateTime)valor).ToString("yyyyMMdd HH:mm:ss")}'";

        //    else if (valor is string)
        //        _valor = $@"'{valor.Replace("'", "")}'";

        //    else if (valor.GetType().IsEnum)
        //        _valor = Convert.ToString(Convert.ToInt32(valor));
        //    else
        //        _valor = "NULL";

        //    return _valor;
        //}

        private ParametroSQL GetValorSql(dynamic valor, Repositorio.Global.Dicionario.MapProperty mapProperty, string nomeParametro)
        {
            object valorProcessado = null;

            if (mapProperty.IsClass != null && valor != null)
                valor = valor.GetType().GetProperty("Codigo").GetValue(valor, null);

            if (valor == null)
                valorProcessado = null;
            else if (valor is bool)
                valorProcessado = (bool)valor ? 1 : 0;
            else if (valor is int)
                valorProcessado = valor;
            else if (valor is double || valor is decimal)
                valorProcessado = valor;
            else if (valor is long)
                valorProcessado = valor;
            else if (valor is DateTime)
                valorProcessado = valor;
            else if (valor is string)
                valorProcessado = valor.ToString();
            else if (valor.GetType().IsEnum)
                valorProcessado = Convert.ToInt32(valor);
            else
                valorProcessado = null;

            return new ParametroSQL
            (
                 nomeParametro,
                 valorProcessado
            );
        }





        #endregion Métodos Privados

        #region Métodos Protegidos

        protected List<TEntidade> ObterLista<TEntidade>(IQueryable<TEntidade> consulta, string propriedadeOrdenar, string direcaoOrdenar, int inicioRegistros, int limiteRegistros)
        {
            var parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoOrdenar = direcaoOrdenar,
                InicioRegistros = inicioRegistros,
                LimiteRegistros = limiteRegistros,
                PropriedadeOrdenar = propriedadeOrdenar
            };

            return ObterLista(consulta, parametrosConsulta);
        }

        protected async Task<List<TEntidade>> ObterListaAsync<TEntidade>(IQueryable<TEntidade> consulta, string propriedadeOrdenar, string direcaoOrdenar, int inicioRegistros, int limiteRegistros)
        {
            var parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoOrdenar = direcaoOrdenar,
                InicioRegistros = inicioRegistros,
                LimiteRegistros = limiteRegistros,
                PropriedadeOrdenar = propriedadeOrdenar
            };

            return await ObterListaAsync(consulta, parametrosConsulta);
        }

        protected List<TEntidade> ObterLista<TEntidade>(IQueryable<TEntidade> consulta, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            if (parametroConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
                    consulta = consulta.OrderBy(parametroConsulta.PropriedadeOrdenar + (parametroConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

                if (parametroConsulta.InicioRegistros > 0)
                    consulta = consulta.Skip(parametroConsulta.InicioRegistros);

                if (parametroConsulta.LimiteRegistros > 0)
                    consulta = consulta.Take(parametroConsulta.LimiteRegistros);
            }


            return consulta.ToList();
        }

        protected async Task<List<TEntidade>> ObterListaAsync<TEntidade>(IQueryable<TEntidade> consulta, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            if (parametroConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
                    consulta = consulta.OrderBy(parametroConsulta.PropriedadeOrdenar + (parametroConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

                if (parametroConsulta.InicioRegistros > 0)
                    consulta = consulta.Skip(parametroConsulta.InicioRegistros);

                if (parametroConsulta.LimiteRegistros > 0)
                    consulta = consulta.Take(parametroConsulta.LimiteRegistros);
            }


            return await consulta.ToListAsync(_cancellationToken);
        }

        protected void ProcessarBatch(Action fnExecute)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                fnExecute.Invoke();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    fnExecute.Invoke();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        protected async Task ProcessarBatchAsync(Func<Task> fnExecuteAsync)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                await fnExecuteAsync.Invoke();
            }
            else
            {
                try
                {
                    await UnitOfWork.StartAsync(_cancellationToken);

                    await fnExecuteAsync.Invoke();

                    await UnitOfWork.CommitChangesAsync(_cancellationToken);
                }
                catch
                {
                    await UnitOfWork.RollbackAsync(_cancellationToken);
                    throw;
                }
            }
        }

        #endregion Métodos Protegidos

        #region Métodos Públicos

        public Dominio.Entidades.Auditoria.HistoricoObjeto Atualizar(T objeto, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null, string descricaoAcao = "")
        {
            Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = null;

            if (UnitOfWork.IsActiveTransaction())
            {
                if (Auditado != null)
                    historicoObjeto = Auditar(Auditado, objeto, objeto.GetChanges(), Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update, historioPai, descricaoAcao);

                SessionNHiBernate.Update(objeto);
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    if (Auditado != null)
                        historicoObjeto = Auditar(Auditado, objeto, objeto.GetChanges(), Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update, historioPai, descricaoAcao);

                    UnitOfWork.Sessao.Update(objeto);

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }

            return historicoObjeto;
        }

        public async Task<Dominio.Entidades.Auditoria.HistoricoObjeto> AtualizarAsync(T objeto, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null, string descricaoAcao = "")
        {
            Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = null;

            if (UnitOfWork.IsActiveTransaction())
            {
                if (Auditado != null)
                    historicoObjeto = await AuditarAsync(Auditado, objeto, objeto.GetChanges(), Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update, historioPai, descricaoAcao);

                await SessionNHiBernate.UpdateAsync(objeto, _cancellationToken);
            }
            else
            {
                try
                {
                    await UnitOfWork.StartAsync(_cancellationToken);

                    if (Auditado != null)
                        historicoObjeto = await AuditarAsync(Auditado, objeto, objeto.GetChanges(), Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update, historioPai, descricaoAcao);

                    await UnitOfWork.Sessao.UpdateAsync(objeto, _cancellationToken);

                    await UnitOfWork.CommitChangesAsync(_cancellationToken);
                }
                catch
                {
                    await UnitOfWork.RollbackAsync(_cancellationToken);
                    throw;
                }
            }

            return historicoObjeto;
        }

        public T BuscarPorCodigo(int codigo, bool auditavel)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();

            criteria.Add(NHibernate.Criterion.Expression.Eq("Codigo", codigo));

            T objeto = criteria.UniqueResult<T>();

            if (auditavel)
                objeto?.Initialize();

            return objeto;
        }

        public async Task<T> BuscarPorCodigoAsync(int codigo, bool auditavel)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();

            criteria.Add(NHibernate.Criterion.Expression.Eq("Codigo", codigo));

            T objeto = await criteria.UniqueResultAsync<T>(_cancellationToken);

            if (auditavel)
                objeto?.Initialize();

            return objeto;
        }

        public T BuscarPorCodigo(long codigo, bool auditavel)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();

            criteria.Add(NHibernate.Criterion.Expression.Eq("Codigo", codigo));

            T objeto = criteria.UniqueResult<T>();

            if (auditavel)
                objeto.Initialize();

            return objeto;
        }

        public async Task<T> BuscarPorCodigoAsync(long codigo, bool auditavel)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();

            criteria.Add(NHibernate.Criterion.Expression.Eq("Codigo", codigo));

            T objeto = await criteria.UniqueResultAsync<T>(_cancellationToken);

            if (auditavel)
                objeto.Initialize();

            return objeto;
        }

        public List<T> BuscarPorCodigos(IList<int> codigos, bool auditavel)
        {
            List<T> objetos = BuscarPorCodigosIList(codigos, auditavel).ToList();

            return objetos;
        }

        public IList<T> BuscarPorCodigosIList<T2>(IList<T2> codigos, bool auditavel)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();

            criteria.Add(NHibernate.Criterion.Expression.In("Codigo", (System.Collections.ICollection)codigos));

            IList<T> objetos = criteria.List<T>();

            if (auditavel)
            {
                for (int i = 0; i < objetos.Count; i++)
                    objetos[i]?.Initialize();
            }

            return objetos;
        }

        public IList<T> BuscarPorEmpresa(int codigoEmpresa, int inicioRegistros, int maximoRegistros, string propriedadeOrdenar, bool decrescente = false)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();
            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar))
            {
                if (decrescente)
                    criteria.AddOrder(NHibernate.Criterion.Order.Desc(propriedadeOrdenar));
                else
                    criteria.AddOrder(NHibernate.Criterion.Order.Asc(propriedadeOrdenar));
            }
            criteria.Add(NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<T>();
        }

        public T BuscarPrimeiroRegistro()
        {
            var query = this.SessionNHiBernate.Query<T>();

            return query
                .FirstOrDefault();
        }

        public async Task<T> BuscarPrimeiroRegistroAsync()
        {
            return await this.SessionNHiBernate.Query<T>().FirstOrDefaultAsync(_cancellationToken);
        }

        public IList<T> BuscarTodos(int inicioRegistros, int maximoRegistros, string propriedadeOrdenar, bool decrescente = false)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();
            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar))
            {
                if (decrescente)
                    criteria.AddOrder(NHibernate.Criterion.Order.Desc(propriedadeOrdenar));
                else
                    criteria.AddOrder(NHibernate.Criterion.Order.Asc(propriedadeOrdenar));
            }
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<T>();
        }

        public List<T> BuscarTodos()
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();
            // TODO: ToList 91 referências
            return criteria.List<T>().ToList();
        }

        public async Task<List<T>> BuscarTodosAsync()
        {
            IQueryable<T> query = SessionNHiBernate.Query<T>();

            return await query.ToListAsync(_cancellationToken);
        }

        public int ContarPorEmpresa(int codigoEmpresa)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();
            criteria.Add(NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public int ContarPorEmpresaETipo(int codigoEmpresa, Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var result = (from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.TipoAcesso == tipoAcesso && obj.Tipo.Equals("U") select obj);

            return result.Count();
        }

        public int ContarTodos()
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public int ContarPorFilial(int codigoFilial)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();
            criteria.Add(NHibernate.Criterion.Expression.Eq("Filial.Codigo", codigoFilial));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public IList<T> BuscarPorFilial(int codigoFilial, int inicioRegistros, int maximoRegistros, string propriedadeOrdenar, bool decrescente = false)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<T>();
            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar))
            {
                if (decrescente)
                    criteria.AddOrder(NHibernate.Criterion.Order.Desc(propriedadeOrdenar));
                else
                    criteria.AddOrder(NHibernate.Criterion.Order.Asc(propriedadeOrdenar));
            }
            criteria.Add(NHibernate.Criterion.Expression.Eq("Filial.Codigo", codigoFilial));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<T>();
        }

        public void Deletar(T objeto, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    if (Auditado != null)
                        Auditar(Auditado, objeto, null, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Delete, historioPai);

                    UnitOfWork.Sessao.Delete(objeto);
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        if (Auditado != null)
                            Auditar(Auditado, objeto, null, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Delete, historioPai);

                        UnitOfWork.Sessao.Delete(objeto);

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        public async Task DeletarAsync(T objeto, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    if (Auditado != null)
                        await AuditarAsync(Auditado, objeto, null, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Delete, historioPai);

                    await UnitOfWork.Sessao.DeleteAsync(objeto, _cancellationToken);
                }
                else
                {
                    try
                    {
                        await UnitOfWork.StartAsync();

                        if (Auditado != null)
                            await AuditarAsync(Auditado, objeto, null, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Delete, historioPai);

                        await UnitOfWork.Sessao.DeleteAsync(objeto, _cancellationToken);

                        await UnitOfWork.CommitChangesAsync(_cancellationToken);
                    }
                    catch
                    {
                        await UnitOfWork.RollbackAsync(_cancellationToken);
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        public long Inserir(T objeto, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null, string descricaoAcao = "")
        {
            object returning = null;
            if (UnitOfWork.IsActiveTransaction())
            {
                returning = UnitOfWork.Sessao.Save(objeto);

                if (Auditado != null)
                    Auditar(Auditado, objeto, null, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Insert, historioPai, descricaoAcao);

            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    returning = UnitOfWork.Sessao.Save(objeto);

                    if (Auditado != null)
                        Auditar(Auditado, objeto, null, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Insert, historioPai, descricaoAcao);

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }

            return Convert.ToInt64(returning);
        }

        public async Task<long> InserirAsync(T objeto, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null, string descricaoAcao = "")
        {
            object returning;

            if (UnitOfWork.IsActiveTransaction())
            {
                returning = await UnitOfWork.Sessao.SaveAsync(objeto, _cancellationToken).ConfigureAwait(false);
                if (Auditado != null)
                    await AuditarAsync(Auditado, objeto, null, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Insert, historioPai);
            }
            else
            {
                try
                {
                    await UnitOfWork.StartAsync(_cancellationToken);
                    returning = await UnitOfWork.Sessao.SaveAsync(objeto, _cancellationToken).ConfigureAwait(false);
                    if (Auditado != null)
                        await AuditarAsync(Auditado, objeto, null, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Insert, historioPai);

                    await UnitOfWork.CommitChangesAsync(_cancellationToken);
                }
                catch
                {
                    await UnitOfWork.RollbackAsync(_cancellationToken);
                    throw;
                }
            }

            return Convert.ToInt64(returning);
        }

        public void DeletarPorEntidade(Dominio.Entidades.EntidadeBase entidade)
        {
            try
            {
                dynamic entidadeBase = entidade;

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery($"DELETE FROM {this.GetType().Name} c WHERE c.{entidadeBase.GetType().Name}.Codigo = :codigo").SetInt32("codigo", entidadeBase.Codigo).ExecuteUpdate(); // SQL-INJECTION-SAFE
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery($"DELETE FROM {this.GetType().Name} c WHERE c.{entidade.GetType().Name}.Codigo = :codigo").SetInt32("codigo", entidadeBase.Codigo).ExecuteUpdate(); // SQL-INJECTION-SAFE

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        public async Task DeletarPorEntidadeAsync(Dominio.Entidades.EntidadeBase entidade)
        {
            try
            {
                dynamic entidadeBase = entidade;

                if (UnitOfWork.IsActiveTransaction())
                {
                    await UnitOfWork.Sessao.CreateQuery($"DELETE FROM {this.GetType().Name} c WHERE c.{entidadeBase.GetType().Name}.Codigo = :codigo").SetInt32("codigo", entidadeBase.Codigo).ExecuteUpdateAsync(_cancellationToken);
                }
                else
                {
                    try
                    {
                        await UnitOfWork.StartAsync(_cancellationToken);

                        await UnitOfWork.Sessao.CreateQuery($"DELETE FROM {this.GetType().Name} c WHERE c.{entidade.GetType().Name}.Codigo = :codigo").SetInt32("codigo", entidadeBase.Codigo).ExecuteUpdateAsync(_cancellationToken);

                        await UnitOfWork.CommitChangesAsync(_cancellationToken);
                    }
                    catch
                    {
                        await UnitOfWork.RollbackAsync(_cancellationToken);
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        public List<Global.Dicionario.MapProperty> Validator(T entidade)
        {
            string tabela = "";
            List<Global.Dicionario.MapProperty> lstError = new List<Global.Dicionario.MapProperty>();
            List<Global.Dicionario.MapProperty> dicionarioPropriedades = GetDicionarioPropriedades(entidade, ref tabela);
            foreach (var propriedadeDicionario in dicionarioPropriedades.Where(x => x.ScopeIdentity == false).ToList())
            {
                PropertyInfo propertyInfo = entidade.GetType().GetProperty(propriedadeDicionario.PropoertyName);
                dynamic valor = propertyInfo.GetValue(entidade, null);
                string _valor = "";

                if (propriedadeDicionario.NotNull && valor == null)
                {
                    propriedadeDicionario.MsgError = $"Campo não pode ser nullo.";
                    lstError.Add(propriedadeDicionario);
                }

                if (valor is string)
                {
                    _valor = valor.TrimEnd();
                    if (_valor.Length > propriedadeDicionario.MaxLen)
                    {
                        propriedadeDicionario.MsgError = $"Quantidade de caracteres {_valor.Length} maior que permitido {propriedadeDicionario.MaxLen}.";
                        propriedadeDicionario.Value = _valor;
                        lstError.Add(propriedadeDicionario);
                    }
                }

                //if (propriedadeDicionario.IsClass != null && valor != null)
                //    valor = valor.GetType().GetProperty("Codigo").GetValue(valor, null);
                //
                //if (valor == null)
                //    _valor = "NULL";
                //
                //else if (valor is bool)
                //    _valor = (bool)valor ? "1" : "0";
                //
                //else if (valor is int)
                //    _valor = Convert.ToString(valor);
                //else if (valor is int || valor is double || valor is decimal || valor is long)
                //{
                //    _valor = Convert.ToString(valor);
                //    _valor = _valor.Replace(',', '.');
                //}
                //else if (valor is DateTime)
                //    _valor = $@"'{((DateTime)valor).ToString("yyyyMMdd HH:mm:ss")}'";
                //
                //else if (valor.GetType().IsEnum)
                //    _valor = Convert.ToString(Convert.ToInt32(valor));
                //else
                //    _valor = "NULL";
                //


            }
            return lstError;
        }

        #endregion Métodos Públicos

        #region Métodos Públicos - Batch

        public void Atualizar(List<T> listaEntidade, List<string> camposParaAlterar, string tabela = "", int batchSize = 1000)
        {
            if (listaEntidade == null || listaEntidade.Count == 0)
                return;

            ProcessarBatch(() =>
            {
                List<Repositorio.Global.Dicionario.MapProperty> dicionarioPropriedades = GetDicionarioPropriedades(listaEntidade.FirstOrDefault(), ref tabela);

                for (int l = 0; l < listaEntidade.Count; l += batchSize)
                {
                    IEnumerable<T> lote = listaEntidade.Skip(l).Take(batchSize);

                    //List<string> updates = new List<string>();
                    var parametros = new List<ParametroSQL>();
                    var comandosUpdate = new List<string>();
                    int contadorParametro = 0;

                    for (int i = 0; i < lote.Count(); i++)
                    {
                        T _entidade = lote.ElementAt(i);

                        //StringBuilder sqlSet = new StringBuilder();
                        //StringBuilder sqlWhere = new StringBuilder();
                        //string virgulaSet = "";
                        //string virgulaWhere = "";
                        var condicoesSet = new List<string>();
                        var condicoesWhere = new List<string>();

                        for (int j = 0; j < dicionarioPropriedades.Count; j++)
                        {
                            Global.Dicionario.MapProperty propriedadeDicionario = dicionarioPropriedades[j];
                            PropertyInfo propertyInfo = _entidade.GetType().GetProperty(propriedadeDicionario.PropoertyName);
                            //string valor = GetValorSql(propertyInfo.GetValue(_entidade, null), propriedadeDicionario);
                            var nomeParam = $":p{contadorParametro++}";
                            var parametroSql = GetValorSql(propertyInfo.GetValue(_entidade, null), propriedadeDicionario, nomeParam);


                            if (!propriedadeDicionario.ScopeIdentity && camposParaAlterar.Contains(propriedadeDicionario.PropoertyName))
                            {
                                //sqlSet.Append($"{virgulaSet} {propriedadeDicionario.DBName} = {valor}");
                                //virgulaSet = ",";
                                parametros.Add(parametroSql);
                                condicoesSet.Add($"{propriedadeDicionario.DBName} = {nomeParam}");
                            }
                            else if (propriedadeDicionario.ScopeIdentity)
                            {
                                //sqlWhere.Append($"{virgulaWhere} {propriedadeDicionario.DBName} = {valor}");
                                //virgulaWhere = " and ";
                                parametros.Add(parametroSql);
                                condicoesWhere.Add($"{propriedadeDicionario.DBName} = {nomeParam}");
                            }
                        }

                        //if (sqlWhere.Length > 0)
                        //    updates.Add($@"UPDATE {tabela} SET {sqlSet} WHERE {sqlWhere}");
                        if (condicoesWhere.Count > 0)
                            comandosUpdate.Add($@"UPDATE {tabela} SET {string.Join(", ", condicoesSet)} WHERE {string.Join(" AND ", condicoesWhere)}");

                    }

                    //ComandSql(updates);
                    if (comandosUpdate.Count > 0)
                    {
                        string sqlUpdate = string.Join("; ", comandosUpdate);

                        NHibernate.ISQLQuery query = UnitOfWork.Sessao.CreateSQLQuery(sqlUpdate);

                        foreach (var param in parametros)
                        {
                            query.SetParameter(param.NomeParametro.Replace(":", ""), param.Valor, param.TipoDb);
                        }

                        query.SetTimeout(300).ExecuteUpdate();
                    }
                }
            });
        }

        public void AtualizarSomenteCamposAlterados(List<T> listaEntidade, string tabela = "", int batchSize = 1000)
        {
            if (listaEntidade == null || listaEntidade.Count == 0)
                return;

            ProcessarBatch(() =>
            {
                List<Repositorio.Global.Dicionario.MapProperty> dicionarioPropriedades = GetDicionarioPropriedades(listaEntidade.FirstOrDefault(), ref tabela);

                for (int l = 0; l < listaEntidade.Count; l += batchSize)
                {
                    IEnumerable<T> lote = listaEntidade.Skip(l).Take(batchSize);
                    //List<string> updates = new List<string>();
                    var parametros = new List<ParametroSQL>();
                    var comandosUpdate = new List<string>();
                    int contadorParametro = 0;

                    foreach (T entidade in lote)
                    {
                        var alteracoes = entidade.GetChanges();
                        if (alteracoes == null || alteracoes.Count == 0)
                            continue;

                        var condicoesSet = new List<string>();
                        var condicoesWhere = new List<string>();

                        //StringBuilder sqlSet = new StringBuilder();
                        //StringBuilder sqlWhere = new StringBuilder();
                        //string virgulaSet = "";
                        //string virgulaWhere = "";

                        foreach (var propriedadeDicionario in dicionarioPropriedades)
                        {
                            PropertyInfo propertyInfo = entidade.GetType().GetProperty(propriedadeDicionario.PropoertyName);
                            //string valor = GetValorSql(propertyInfo.GetValue(entidade, null), propriedadeDicionario);
                            var nomeParam = $":p{contadorParametro++}";
                            var parametroSql = GetValorSql(propertyInfo.GetValue(entidade, null), propriedadeDicionario, nomeParam);

                            if (!propriedadeDicionario.ScopeIdentity &&
                                alteracoes.Any(a => a.Propriedade == propriedadeDicionario.PropoertyName))
                            {
                                //sqlSet.Append($"{virgulaSet} {propriedadeDicionario.DBName} = {valor}");
                                //virgulaSet = ",";
                                parametros.Add(parametroSql);
                                condicoesSet.Add($"{propriedadeDicionario.DBName} = {nomeParam}");
                            }
                            else if (propriedadeDicionario.ScopeIdentity)
                            {
                                //sqlWhere.Append($"{virgulaWhere} {propriedadeDicionario.DBName} = {valor}");
                                //virgulaWhere = " and ";
                                parametros.Add(parametroSql);
                                condicoesWhere.Add($"{propriedadeDicionario.DBName} = {nomeParam}");
                            }
                        }

                        //if (sqlSet.Length > 0 && sqlWhere.Length > 0)
                        //    updates.Add($@"UPDATE {tabela} SET {sqlSet} WHERE {sqlWhere}");
                        if (condicoesSet.Count > 0 && condicoesWhere.Count > 0)
                            comandosUpdate.Add($@"UPDATE {tabela} SET {string.Join(", ", condicoesSet)} WHERE {string.Join(" AND ", condicoesWhere)}");

                    }

                    //ComandSql(updates);
                    if (comandosUpdate.Count > 0)
                    {
                        string sqlUpdate = string.Join("; ", comandosUpdate);

                        NHibernate.ISQLQuery query = UnitOfWork.Sessao.CreateSQLQuery(sqlUpdate);

                        foreach (var param in parametros)
                        {
                            query.SetParameter(param.NomeParametro.Replace(":", ""), param.Valor, param.TipoDb);
                        }

                        query.SetTimeout(300).ExecuteUpdate();
                    }
                }
            });
        }

        public void ComandSql(List<string> listComandSql, int batchSize = 1000)
        {
            if (listComandSql == null || listComandSql.Count == 0)
                return;

            ProcessarBatch(() =>
            {
                for (int l = 0; l < listComandSql.Count; l += batchSize)
                {
                    IEnumerable<string> lote = listComandSql.Skip(l).Take(batchSize);
                    string sqlComand = string.Join("; ", lote);
                    UnitOfWork.Sessao.CreateSQLQuery(sqlComand).SetTimeout(300).ExecuteUpdate();
                }
            });
        }

        public void ComandSql(string sqlComand, List<ParametroSQL> parametros)
        {
            if (string.IsNullOrEmpty(sqlComand) || parametros == null)
                return;

            ProcessarBatch(() =>
            {
                NHibernate.ISQLQuery query = UnitOfWork.Sessao.CreateSQLQuery(sqlComand);

                foreach (var param in parametros)
                {
                    query.SetParameter(param.NomeParametro.Replace(":", ""), param.Valor, param.TipoDb);
                }

                query.SetTimeout(300).ExecuteUpdate();
            });
        }

        public async Task ComandSqlAsync(List<string> listComandSql, int batchSize = 1000)
        {
            if (listComandSql == null || listComandSql.Count == 0)
                return;

            await ProcessarBatchAsync(async () =>
            {
                for (int l = 0; l < listComandSql.Count; l += batchSize)
                {
                    IEnumerable<string> lote = listComandSql.Skip(l).Take(batchSize);
                    string sqlComand = string.Join("; ", lote);
                    await UnitOfWork.Sessao.CreateSQLQuery(sqlComand).SetTimeout(300).ExecuteUpdateAsync(_cancellationToken);
                }
            });
        }

        private async Task ExecutarComandoParametrizadoAsync(string sql, List<ParametroSQL> parametros, int batchSize = 1000)
        {
            if (string.IsNullOrEmpty(sql) || parametros == null)
                return;

            await ProcessarBatchAsync(async () =>
            {
                NHibernate.ISQLQuery query = UnitOfWork.Sessao.CreateSQLQuery(sql);

                foreach (var param in parametros)
                {
                    query.SetParameter(param.NomeParametro.Replace(":", ""), param.Valor, param.TipoDb);
                }

                await query.SetTimeout(300).ExecuteUpdateAsync(_cancellationToken);
            });
        }

        public void Deletar(List<T> listaEntidade, string tabela = "", int batchSize = 1000)
        {
            if (listaEntidade == null || listaEntidade.Count == 0)
                return;

            ProcessarBatch(() =>
            {
                List<Global.Dicionario.MapProperty> dicionarioPropriedades = GetDicionarioPropriedades(listaEntidade.FirstOrDefault(), ref tabela);

                for (int l = 0; l < listaEntidade.Count; l += batchSize)
                {
                    IEnumerable<T> lote = listaEntidade.Skip(l).Take(batchSize);
                    //List<string> where = new List<string>();
                    var parametros = new List<ParametroSQL>();
                    var comandosDelete = new List<string>();
                    int contadorParametro = 0;

                    for (int i = 0; i < lote.Count(); i++)
                    {
                        T _entidade = lote.ElementAt(i);
                        //StringBuilder sqlWhere = new StringBuilder();
                        var condicoesWhere = new List<string>();


                        for (int j = 0; j < dicionarioPropriedades.Count; j++)
                        {
                            Global.Dicionario.MapProperty propriedadeDicionario = dicionarioPropriedades[j];
                            if (propriedadeDicionario.ScopeIdentity)
                            {
                                PropertyInfo propertyInfo = _entidade.GetType().GetProperty(propriedadeDicionario.PropoertyName);
                                //string valor = GetValorSql(propertyInfo.GetValue(_entidade, null), propriedadeDicionario);
                                //if (sqlWhere.Length > 0)
                                //    sqlWhere.Append(" AND ");
                                //sqlWhere.Append($"{propriedadeDicionario.DBName} = {valor}");
                                var nomeParam = $":p{contadorParametro++}";
                                var parametroSql = GetValorSql(propertyInfo.GetValue(_entidade, null), propriedadeDicionario, nomeParam);

                                parametros.Add(parametroSql);
                                condicoesWhere.Add($"{propriedadeDicionario.DBName} = {nomeParam}");
                            }
                        }

                        //where.Add($"DELETE FROM {tabela} WHERE {sqlWhere}"); 

                        if (condicoesWhere.Count > 0)
                        {
                            comandosDelete.Add($"DELETE FROM {tabela} WHERE {string.Join(" AND ", condicoesWhere)}");
                        }


                    }

                    //if (where.Count > 0)
                    //{
                    //    string sqlDelete = string.Join("; ", where);
                    //    UnitOfWork.Sessao.CreateSQLQuery(sqlDelete).SetTimeout(300).ExecuteUpdate();
                    //}
                    if (comandosDelete.Count > 0)
                    {
                        string sqlDelete = string.Join("; ", comandosDelete);

                        NHibernate.ISQLQuery query = UnitOfWork.Sessao.CreateSQLQuery(sqlDelete);

                        foreach (var param in parametros)
                        {
                            query.SetParameter(param.NomeParametro.Replace(":", ""), param.Valor, param.TipoDb);
                        }

                        query.SetTimeout(300).ExecuteUpdate();
                    }
                }
            });
        }

        public void Inserir(List<T> listaEntidade, string tabela = "", int batchSize = 1000)
        {
            if (listaEntidade == null || listaEntidade.Count == 0)
                return;

            ProcessarBatch(() =>
            {
                List<Global.Dicionario.MapProperty> dicionarioPropriedades = GetDicionarioPropriedades(listaEntidade.FirstOrDefault(), ref tabela);
                List<string> propertyNames = dicionarioPropriedades.Where(x => !x.ScopeIdentity).Select(x => x.DBName).ToList();
                Global.Dicionario.MapProperty scopeIdentity = dicionarioPropriedades.Find(x => x.IsKey);

                for (int l = 0; l < listaEntidade.Count; l += batchSize)
                {
                    IEnumerable<T> lote = listaEntidade.Skip(l).Take(batchSize);
                    //StringBuilder sqlValues = new StringBuilder();
                    //List<string> values = new List<string>();
                    var parametros = new List<ParametroSQL>();
                    var placeholdersLote = new List<string>();
                    int contadorParametro = 0;

                    for (int j = 0; j < lote.Count(); j++)
                    {
                        T _entidade = lote.ElementAt(j);
                        var placeholdersLinha = new List<string>();

                        for (int k = 0; k < dicionarioPropriedades.Count; k++)
                        {
                            Global.Dicionario.MapProperty propriedadeDicionario = dicionarioPropriedades[k];
                            if (!propriedadeDicionario.ScopeIdentity)
                            {
                                PropertyInfo propertyInfo = _entidade.GetType().GetProperty(propriedadeDicionario.PropoertyName);
                                //string valor = GetValorSql(propertyInfo.GetValue(_entidade, null), propriedadeDicionario);
                                //values.Add(valor);
                                var nomeParam = $":p{contadorParametro++}";
                                var parametroSql = GetValorSql(propertyInfo.GetValue(_entidade, null), propriedadeDicionario, nomeParam);

                                parametros.Add(parametroSql);
                                placeholdersLinha.Add(nomeParam);
                            }
                        }

                        //sqlValues.Append($"({string.Join(",", values)}),");
                        //values.Clear();
                        placeholdersLote.Add($"({string.Join(",", placeholdersLinha)})");

                    }

                    //if (sqlValues.Length > 0)
                    //    sqlValues.Length--; // Remove the last comma

                    string valuesClause = string.Join(",", placeholdersLote);


                    if (scopeIdentity != null)
                    {
                        //string sqlInsert = $@"
                        //DECLARE @ChavesPrimarias_{l} TABLE (ID INT);
                        //INSERT INTO {tabela} ({string.Join(",", propertyNames)})
                        //OUTPUT INSERTED.{scopeIdentity.DBName} INTO @ChavesPrimarias_{l}
                        //VALUES {sqlValues}
                        //SELECT ID FROM @ChavesPrimarias_{l};"; 
                        string sqlInsert = $@"
                        DECLARE @ChavesPrimarias_{l} TABLE (ID INT);
                        INSERT INTO {tabela} ({string.Join(",", propertyNames)})
                        OUTPUT INSERTED.{scopeIdentity.DBName} INTO @ChavesPrimarias_{l}
                        VALUES {valuesClause};
                        SELECT ID FROM @ChavesPrimarias_{l};";

                        NHibernate.ISQLQuery query = UnitOfWork.Sessao.CreateSQLQuery(sqlInsert);

                        foreach (var param in parametros)
                        {
                            query.SetParameter(param.NomeParametro.Replace(":", ""), param.Valor, param.TipoDb);
                        }

                        IList<int> ids = query.SetTimeout(300).List<int>();
                        ids = ids.OrderBy(i => i).ToList();

                        int i = 0;
                        for (int m = 0; m < lote.Count(); m++)
                        {
                            T item = lote.ElementAt(m);
                            PropertyInfo propriedade = item.GetType().GetProperty(scopeIdentity.PropoertyName);
                            propriedade.SetValue(item, ids[i]);
                            i++;
                        }
                    }
                    else
                    {
                        //string sqlInsert = $@"
                        //INSERT INTO {tabela} ({string.Join(",", propertyNames)})
                        //VALUES {sqlValues}"; 
                        string sqlInsert = $@"
                        INSERT INTO {tabela} ({string.Join(",", propertyNames)})
                        VALUES {valuesClause}";

                        //ComandSql(new List<string> { sqlInsert });
                        ComandSql(sqlInsert, parametros);

                    }
                }
            });
        }

        public async Task InserirAsync(List<T> listaEntidade, string tabela = "", int batchSize = 1000)
        {
            if (listaEntidade == null || listaEntidade.Count == 0)
                return;

            await ProcessarBatchAsync(async () =>
            {
                List<Global.Dicionario.MapProperty> dicionarioPropriedades = GetDicionarioPropriedades(listaEntidade.FirstOrDefault(), ref tabela);
                List<string> propertyNames = dicionarioPropriedades.Where(x => !x.ScopeIdentity).Select(x => x.DBName).ToList();
                Global.Dicionario.MapProperty scopeIdentity = dicionarioPropriedades.Find(x => x.IsKey);

                var propriedadesEntidade = typeof(T)
                    .GetProperties()
                    .ToDictionary(p => p.Name, p => p);

                var lista = listaEntidade.ToList();

                for (int l = 0; l < lista.Count; l += batchSize)
                {
                    var lote = lista.Skip(l).Take(batchSize).ToList();
                    //var sqlValues = new StringBuilder();
                    //var valoresLote = new List<string>();

                    var parametros = new List<ParametroSQL>();
                    var placeholdersLote = new List<string>();
                    int contadorParametro = 0;

                    foreach (var entidade in lote)
                    {
                        //var valoresLinha = new List<string>();

                        var placeholdersLinha = new List<string>();

                        foreach (var prop in dicionarioPropriedades.Where(p => !p.ScopeIdentity))
                        {
                            //var valor = GetValorSql(propriedadesEntidade[prop.PropoertyName].GetValue(entidade), prop);
                            //valoresLinha.Add(valor);
                            var nomeParam = $":p{contadorParametro++}";
                            var parametroSql = GetValorSql(
                                propriedadesEntidade[prop.PropoertyName].GetValue(entidade),
                                prop,
                                nomeParam
                            );

                            parametros.Add(parametroSql);
                            placeholdersLinha.Add(nomeParam);

                        }

                        //valoresLote.Add($"({string.Join(",", valoresLinha)})");
                        placeholdersLote.Add($"({string.Join(",", placeholdersLinha)})");

                    }

                    //sqlValues.Append(string.Join(",", valoresLote));
                    string valuesClause = string.Join(",", placeholdersLote);


                    if (scopeIdentity != null)
                    {
                        //    string sqlInsert = $@"
                        //DECLARE @ChavesPrimarias_{l} TABLE (ID INT);
                        //INSERT INTO {tabela} ({string.Join(",", propertyNames)})
                        //OUTPUT INSERTED.{scopeIdentity.DBName} INTO @ChavesPrimarias_{l}
                        //VALUES {sqlValues};
                        //SELECT ID FROM @ChavesPrimarias_{l};"; 

                        string sqlInsert = $@"
                    DECLARE @ChavesPrimarias_{l} TABLE (ID INT);
                    INSERT INTO {tabela} ({string.Join(",", propertyNames)})
                    OUTPUT INSERTED.{scopeIdentity.DBName} INTO @ChavesPrimarias_{l}
                    VALUES {valuesClause};
                    SELECT ID FROM @ChavesPrimarias_{l};";

                        NHibernate.ISQLQuery query = UnitOfWork.Sessao.CreateSQLQuery(sqlInsert);

                        foreach (var param in parametros)
                        {
                            query.SetParameter(param.NomeParametro.Replace(":", ""), param.Valor, param.TipoDb);
                        }

                        IList<int> ids = await query.SetTimeout(300).ListAsync<int>(_cancellationToken);
                        ids = ids.OrderBy(i => i).ToList();

                        //var joinsPorTabela = new Dictionary<string, List<string>>();
                        var joinsPorTabela = new Dictionary<string, List<ParametroSQL>>();
                        var joinsPlaceholders = new Dictionary<string, List<string>>();

                        for (int i = 0; i < lote.Count; i++)
                        {
                            var item = lote[i];
                            var idGerado = ids[i];

                            var propKey = propriedadesEntidade[scopeIdentity.PropoertyName];
                            propKey.SetValue(item, idGerado);

                            foreach (var prop in item.GetType().GetProperties())
                            {
                                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType)
                                    && prop.PropertyType != typeof(string))
                                {
                                    var listaFilhos = prop.GetValue(item) as System.Collections.IEnumerable;
                                    if (listaFilhos == null) continue;

                                    Type tipoFilho = prop.PropertyType.GetGenericArguments().FirstOrDefault();
                                    if (tipoFilho == null) continue;

                                    if (prop.GetCustomAttributes(typeof(NHibernate.Mapping.Attributes.ManyToManyAttribute), true).FirstOrDefault() is NHibernate.Mapping.Attributes.ManyToManyAttribute manyToManyAttr &&
                                        prop.GetCustomAttributes(typeof(NHibernate.Mapping.Attributes.SetAttribute), true).FirstOrDefault() is NHibernate.Mapping.Attributes.SetAttribute setAttr &&
                                        prop.GetCustomAttributes(typeof(NHibernate.Mapping.Attributes.KeyAttribute), true).FirstOrDefault() is NHibernate.Mapping.Attributes.KeyAttribute keyAttr)
                                    {
                                        string nomeTabelaJoin = setAttr.Table;
                                        string nomeColunaPai = keyAttr.Column;
                                        string nomeColunaFilho = manyToManyAttr.Column;
                                        string chaveJoin = $"{nomeTabelaJoin}|{nomeColunaPai}|{nomeColunaFilho}";

                                        if (!joinsPorTabela.ContainsKey(chaveJoin))
                                        {
                                            //joinsPorTabela[chaveJoin] = new List<string>();
                                            joinsPorTabela[chaveJoin] = new List<ParametroSQL>();
                                            joinsPlaceholders[chaveJoin] = new List<string>();

                                        }

                                        PropertyInfo propIdFilho = tipoFilho.GetProperty("Codigo") ??
                                                                tipoFilho.GetProperty("Sigla") ??
                                                                tipoFilho.GetProperty("UF_SIGLA") ??
                                                                tipoFilho.GetProperty("CPF_CNPJ");

                                        if (propIdFilho == null) continue;

                                        foreach (var filho in listaFilhos)
                                        {
                                            var idFilho = propIdFilho.GetValue(filho);
                                            //joinsPorTabela[chaveJoin].Add($"('{idGerado}', '{idFilho}')");

                                            var paramPai = $":jp{joinsPorTabela[chaveJoin].Count}";
                                            var paramFilho = $":jf{joinsPorTabela[chaveJoin].Count}";

                                            joinsPorTabela[chaveJoin].Add(new ParametroSQL
                                            (
                                                paramPai,
                                                idGerado
                                            ));

                                            joinsPorTabela[chaveJoin].Add(new ParametroSQL
                                            (
                                                paramFilho,
                                                idFilho
                                            ));

                                            joinsPlaceholders[chaveJoin].Add($"({paramPai}, {paramFilho})");
                                        }
                                    }
                                }
                            }
                        }

                        // Executa todos os joins acumulados (1 insert por tabela)
                        foreach (var join in joinsPorTabela)
                        {
                            if (join.Value.Count > 0)
                            {
                                var partes = join.Key.Split('|');
                                //var sqlInsertJoin = $@"
                                //INSERT INTO {partes[0]} ({partes[1]}, {partes[2]})
                                //VALUES {string.Join(",", join.Value)}"; 
                                var sqlInsertJoin = $@"
                                INSERT INTO {partes[0]} ({partes[1]}, {partes[2]})
                                VALUES {string.Join(",", joinsPlaceholders[join.Key])}";

                                //await ComandSqlAsync(new List<string> { sqlInsertJoin });
                                await ExecutarComandoParametrizadoAsync(sqlInsertJoin, join.Value);

                            }
                        }
                    }
                    else
                    {
                        //string sqlInsert = $@"
                        //INSERT INTO {tabela} ({string.Join(",", propertyNames)})
                        //VALUES {sqlValues}"; 
                        string sqlInsert = $@"
                        INSERT INTO {tabela} ({string.Join(",", propertyNames)})
                        VALUES {valuesClause}";

                        //await ComandSqlAsync(new List<string> { sqlInsert });
                        await ExecutarComandoParametrizadoAsync(sqlInsert, parametros);

                    }
                }
            });
        }

        #endregion Métodos Públicos - Batch
    }
}
