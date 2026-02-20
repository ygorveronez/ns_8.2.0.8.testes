using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Dominio.Excecoes.Embarcador;
using Microsoft.Extensions.DependencyInjection;

namespace Servicos.SignalR
{
    public abstract class SignalRBase<THubEntity> : Hub where THubEntity : Hub
	{
		protected static readonly ConnectionMapping<string> _connections = new ConnectionMapping<string>();

		public static void ProcessHubNotification(Dominio.MSMQ.Notification notification)
		{
			switch (notification.Hub)
			{
				case Dominio.SignalR.Hubs.Mobile:
					Servicos.SignalR.Mobile serMobile = new Mobile();
					serMobile.ProcessarNotificacao(notification);
					break;
				case Dominio.SignalR.Hubs.ControleColetaEntrega:
					Servicos.SignalR.Hubs.ControleColetaEntrega serControleColetaEntrega = new Hubs.ControleColetaEntrega();
					serControleColetaEntrega.ProcessarNotificacao(notification);
					break;
				case Dominio.SignalR.Hubs.GestaoPatio:
					Hubs.GestaoPatio servicoGestaoPatio = new Hubs.GestaoPatio();
					servicoGestaoPatio.ProcessarNotificacao(notification);
					break;
				case Dominio.SignalR.Hubs.Pedidos:
					Servicos.SignalR.Hubs.Pedidos hupPedidos = new Servicos.SignalR.Hubs.Pedidos();
					hupPedidos.ProcessarNotificacao(notification);
					break;
				case Dominio.SignalR.Hubs.Notificacao:
					Servicos.Embarcador.Hubs.Notificacao hubNotificacao = new Servicos.Embarcador.Hubs.Notificacao();
					hubNotificacao.NotificarUsuarioMSMQ(notification);
					break;
				case Dominio.SignalR.Hubs.IntegracaoMercadoLivre:
					Servicos.Embarcador.Hubs.IntegracaoMercadoLivre hubIntegracaoMercadoLivre = new Servicos.Embarcador.Hubs.IntegracaoMercadoLivre();
					hubIntegracaoMercadoLivre.ProcessarNotificacao(notification);
					break;
				case Dominio.SignalR.Hubs.AcompanhamentoCarga:
					Hubs.AcompanhamentoCarga hubacompanhamentoCarga = new Hubs.AcompanhamentoCarga();
					hubacompanhamentoCarga.ProcessarNotificacao(notification);
					break;

				case Dominio.SignalR.Hubs.EtapasCarga:
					Servicos.SignalR.Hubs.EtapasCarga hubEtapasCarga = new Servicos.SignalR.Hubs.EtapasCarga();
					hubEtapasCarga.ProcessarNotificacao(notification);
					break;

				case Dominio.SignalR.Hubs.Monitoramento:
					Hubs.Monitoramento hubMonitoramento = new Hubs.Monitoramento();
					hubMonitoramento.ProcessarNotificacao(notification);
					break;

                case Dominio.SignalR.Hubs.GestaoDevolucao:
                    Hubs.GestaoDevolucao hubGestaoDevolucao = new Hubs.GestaoDevolucao();
                    hubGestaoDevolucao.ProcessarNotificacao(notification);
                    break;

                default:
                    break;
            }
        }

		private IHubContext<THubEntity> GetContext()
		{

/* Unmerged change from project 'Servicos (net472)'
Before:
			return Servicos.ServiceProviderContext.ServiceProvider.GetRequiredService<IHubContext<THubEntity>>();
After:
			return ServiceProviderContext.ServiceProvider.GetRequiredService<IHubContext<THubEntity>>();
*/
			return Servicos.Providers.ServiceProviderContext.ServiceProvider.GetRequiredService<IHubContext<THubEntity>>();
		}

		protected void SendToConnectionIds(List<string> connectionIds, string method, object obj)
		{
			IHubContext<THubEntity> contexto = GetContext();

			contexto.Clients.Clients(connectionIds).SendAsync(method, obj);
		}

		protected void SendToAll(string method, object obj)
		{
			try
			{
				IHubContext<THubEntity> contexto = GetContext();

				contexto.Clients.All.SendAsync(method, obj);
			}
			catch (Exception excecao)
			{
				Log.TratarErro($"Hub {typeof(THubEntity).Name}: {excecao}", "SignalR");
			}
		}

		protected void SendToAll(string method)
		{
			try
			{
				IHubContext<THubEntity> contexto = GetContext();

				contexto.Clients.All.SendAsync(method);
			}
			catch (Exception excecao)
			{
				Log.TratarErro($"Hub {typeof(THubEntity).Name}: {excecao}", "SignalR");
			}
		}

		protected void SendToClient(string method, object obj)
		{
			try
			{
				List<string> idConexoes = _connections.GetConnections(GetKey()).ToList();

				if (idConexoes.Count > 0)
				{
					IHubContext<THubEntity> contexto = GetContext();

					contexto.Clients.Clients(idConexoes).SendAsync(method, obj);
				}
			}
			catch (Exception excecao)
			{
				Log.TratarErro($"Hub {typeof(THubEntity).Name}: {excecao}", "SignalR");
			}
		}

		protected void SendToAllExcept(string method, object obj, string connectionKey = null)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(connectionKey))
					connectionKey = GetKey();

				List<string> idConexoes = _connections.GetConnections(connectionKey).ToList();

				if (idConexoes.Count > 0)
				{
					IHubContext<THubEntity> contexto = GetContext();

					contexto.Clients.AllExcept(idConexoes).SendAsync(method, obj);
				}
			}
			catch (Exception excecao)
			{
				Log.TratarErro($"Hub {typeof(THubEntity).Name}: {excecao}", "SignalR");
			}
		}

		public override Task OnConnectedAsync()
		{
			string userKey = GetKey();

			if (!string.IsNullOrWhiteSpace(userKey))
				_connections.Add(userKey, Context.ConnectionId);

			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			string userKey = GetKey();

			if (!string.IsNullOrWhiteSpace(userKey) && !_connections.GetConnections(userKey).Contains(Context.ConnectionId))
				_connections.Remove(userKey, Context.ConnectionId);

			return base.OnDisconnectedAsync(exception);
		}

		public abstract string GetKey();

		public abstract void ProcessarNotificacao(Dominio.MSMQ.Notification notification);

		protected static string AdminStringConexao
		{
			get
			{
#if DEBUG
				return ObterConnectionAdminMultisoftwareDebug();
#endif

				return Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
			}
		}

		private static List<string> ObterArquivoURLDebug()
		{
			string caminho = Utilidades.IO.FileStorageService.LocalStorage.Combine(Servicos.FS.GetPath(AppDomain.CurrentDomain.BaseDirectory), "DebugConfig.txt");

			if (Utilidades.IO.FileStorageService.LocalStorage.Exists(caminho))
				return Utilidades.IO.FileStorageService.LocalStorage.ReadLines(caminho).ToList();
			else
				throw new CustomException("Arquivo DebugConfig.txt não localizado.");
		}

		private static string ObterConnectionAdminMultisoftwareDebug()
		{
			List<string> arquivo = ObterArquivoURLDebug();

			arquivo.RemoveAt(0);

			string json = string.Join("", arquivo);

			if (string.IsNullOrWhiteSpace(json))
				throw new CustomException("É obrigatório informar a url e string de conexão no arquivo DebugConfig.txt.");

			dynamic configDebug = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

			string connectionString = (string)configDebug.AdminMultisoftware;

			if (string.IsNullOrWhiteSpace(connectionString))
				throw new CustomException("String de conexão do AdminMultisoftware não encontrada.");

			return connectionString;
		}
	}
}
