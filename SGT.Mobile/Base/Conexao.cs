using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace SGT.Mobile
{
    public class Conexao
    {
        #region Propriedades

        public static string AdminStringConexao
        {
            get
            {
#if DEBUG
                return ObterConnectionAdminMultisoftwareDebug();
#endif
                return Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
            }
        }

        public static string StringConexao(AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente)
        {
            string stringConexao = "";
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao = usuarioMobileCliente.Cliente.ClienteConfiguracao;

            if (usuarioMobileCliente.BaseHomologacao && usuarioMobileCliente.Cliente.ClienteConfiguracaoHomologacao != null)
                configuracao = usuarioMobileCliente.Cliente.ClienteConfiguracaoHomologacao;

            stringConexao = GerarStringConexao(configuracao);

#if DEBUG
            stringConexao = ObterConnectionStringDebug(stringConexao);
#endif

            Servicos.Log.SetStringConexao(stringConexao);
            Servicos.Log.SetCliente(usuarioMobileCliente.Cliente.NomeFantasia + (usuarioMobileCliente.BaseHomologacao ? " Homo" : " Produção"));


            return stringConexao;
        }

        public static string ObterHost
        {
            get
            {
                string host = HttpContext.Current.Request.Url.Host;

                if (HttpContext.Current.Request.Url.Port != 80 && HttpContext.Current.Request.Url.Port != 443 && HttpContext.Current.Request.Url.Port != 8443)
                    host += ":" + HttpContext.Current.Request.Url.Port;

                if (HttpContext.Current.Request.Url.Port == 8443) //Adicionado para validaçao de deploy Bluegren
                    host = host.Replace(":8443", "");

#if DEBUG
                host = ObterArquivoURLDebug().FirstOrDefault();
#endif

                return host;
            }
        }

        #endregion

        #region Metodos Publicos

        public static void ConfigureFileStorage()
        {
            Servicos.IO.FileStorage.ConfigureApplicationFileStorage(AdminStringConexao, ObterHost);
        }

        #endregion

        #region Métodos Privados

        private static string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
        {
            return $"Data Source={configuracao.DBServidor};Initial Catalog={configuracao.DBBase};User Id={configuracao.DBUsuario};Password={configuracao.DBSenha};Max Pool Size=600;";
        }

        private static List<string> ObterArquivoURLDebug()
        {
            string caminho = Utilidades.IO.FileStorageService.LocalStorage.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebugConfig.txt");

            if (Utilidades.IO.FileStorageService.LocalStorage.Exists(caminho))
                return Utilidades.IO.FileStorageService.LocalStorage.ReadLines(caminho).ToList();
            else
                throw new ControllerException("Arquivo DebugConfig.txt não localizado.");
        }

        private static string ObterConnectionAdminMultisoftwareDebug()
        {
            List<string> arquivo = ObterArquivoURLDebug();

            arquivo.RemoveAt(0);

            string json = string.Join("", arquivo);

            if (string.IsNullOrWhiteSpace(json))
                throw new ControllerException("É obrigatório informar a url e string de conexão no arquivo DebugConfig.txt.");

            dynamic configDebug = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            string connectionString = (string)configDebug.AdminMultisoftware;

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ControllerException("String de conexão do AdminMultisoftware não encontrada.");

            return connectionString;
        }

        private static string ObterConnectionStringDebug(string connectionDefault)
        {
            List<string> arquivo = ObterArquivoURLDebug();

            arquivo.RemoveAt(0);

            string json = string.Join("", arquivo);

            if (string.IsNullOrWhiteSpace(json))
                return connectionDefault;

            dynamic configDebug = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            return !string.IsNullOrWhiteSpace((string)configDebug.ConnectionString) ? (string)configDebug.ConnectionString : connectionDefault;
        }

        #endregion
    }
}