using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.Cache;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Database
{
	public class ConnectionString
	{
		private static readonly Lazy<ConnectionString> _instance = new Lazy<ConnectionString>(() => new ConnectionString());

		public static ConnectionString Instance => _instance.Value;

		public string GetDatabaseConnectionString(string database)
		{

#if DEBUG
			return ObterConnectionAdminDebug();
#endif

			string cacheKey = database + "ConnectionString";

			string connectionString = CacheProvider.Instance.Get<string>(cacheKey);

			if (!string.IsNullOrWhiteSpace(connectionString))
				return connectionString;

			SecretManagement.ISecretManager secretManager = new SecretManagement.AzureKeyVaultSecretManager();

			connectionString = secretManager.GetSecretValue(database);

            CacheProvider.Instance.Add(cacheKey, connectionString, TimeSpan.FromHours(12));

			return connectionString;
		}

        public string? GetHangfireWorkerConnectionString()
        {
			try
			{
#if DEBUG
                return ObterHangfireWorkerConnectionStringDebug();
#endif
                //TODO: alterar isso para buscar dinâmicamente
                var connectionString = "Data Source=sql-multisoftware-hml.database.windows.net;Initial Catalog=sqldb-hangfire-dev;User Id=Desenvolvimento;Password=Dev@MultiSoftware$2021;";
                //string cacheKey = "HangfireWorkerConnection";

                //string connectionString = Cache.MemoryCacheManager.Instance.Get(cacheKey) as string;

                //if (!string.IsNullOrWhiteSpace(connectionString))
                //    return connectionString;

                //SecretManagement.ISecretManager secretManager = new SecretManagement.AzureKeyVaultSecretManager();

                //connectionString = secretManager.GetSecretValue(cacheKey);

                //Cache.MemoryCacheManager.Instance.Add(cacheKey, connectionString, DateTime.Now.AddHours(12));

                return connectionString;
            }
			catch (Exception ex)
			{
                Servicos.Log.TratarErro(ex);

                return null;
			}
        }

        private static string ObterConnectionAdminDebug()
		{
			List<string> arquivo = ObterArquivoURLDebug();

			arquivo.RemoveAt(0);

			string json = string.Join("", arquivo);

			if (string.IsNullOrWhiteSpace(json))
				throw new ControllerException("É obrigatório informar a url e string de conexão no arquivo DebugConfig.txt.");

			dynamic configDebug = json.FromJson<dynamic>();

			string connectionString = (string)configDebug.AdminMultisoftware;

			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ControllerException("String de conexão do AdminMultisoftware não encontrada.");

			return connectionString;
		}

        private static string ObterHangfireWorkerConnectionStringDebug()
        {
            List<string> arquivo = ObterArquivoURLDebug();

            arquivo.RemoveAt(0);

            string json = string.Join("", arquivo);

            if (string.IsNullOrWhiteSpace(json))
                throw new ControllerException("É obrigatório informar a url e string de conexão no arquivo DebugConfig.txt.");

            dynamic configDebug = json.FromJson<dynamic>();

            string connectionString = (string)configDebug.HangfireWorkerConexao;

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ControllerException("String de conexão do AdminMultisoftware não encontrada.");

            return connectionString;
        }

        private static List<string> ObterArquivoURLDebug()
		{
			string caminho = Utilidades.IO.FileStorageService.LocalStorage.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebugConfig.txt");

			if (Utilidades.IO.FileStorageService.LocalStorage.Exists(caminho))
				return Utilidades.IO.FileStorageService.LocalStorage.ReadLines(caminho).ToList();
			else
				throw new ControllerException("Arquivo DebugConfig.txt não localizado.");
		}

        public string GetRedisConnectionString(string database)
        {
            string connectionString = CacheProvider.Instance.Get<string>(database);

            if (!string.IsNullOrWhiteSpace(connectionString))
                return connectionString;

            try
            {
                SecretManagement.ISecretManager secretManager = new SecretManagement.AzureKeyVaultSecretManager();

                connectionString = secretManager.GetSecretValue(database);

                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    CacheProvider.Instance.Add(database, connectionString, TimeSpan.FromHours(12));
                    return connectionString;
                }
            }
            catch
            {
                //vamos logar?
            }

            return null;
        }

    }
}