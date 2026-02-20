using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Hubs
{
    public abstract class HubBase<THubEntity> : Hub where THubEntity : Hub
	{
		#region Atributos

		protected readonly static Hubs.ConnectionMapping<string> _conexoes = new Hubs.ConnectionMapping<string>();

		#endregion Atributos

		#region Métodos Protegidos

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

		protected bool IsConexaoAtiva(string chaveUsuario)
		{
			try
			{
				List<string> idConexoes = _conexoes.GetConnections(chaveUsuario).ToList();

				return idConexoes.Count > 0;
			}
			catch (Exception excecao)
			{
				Log.TratarErro($"Hub {typeof(THubEntity).Name}: {excecao}", "SignalR");
				return false;
			}
		}

		protected void SendToAll(string method, object obj)
		{
			try
			{
				IHubContext<THubEntity> contexto = GetContext();

                contexto.Clients.All.SendAsync(method, obj).GetAwaiter().GetResult();
            }
            catch (Exception excecao)
			{
				Log.TratarErro($"Hub {typeof(THubEntity).Name}: {excecao}", "SignalR");
			}
		}

		protected void SendToClient(string userKey, string method, object obj)
		{
			try
			{
				List<string> idConexoes = _conexoes.GetConnections(userKey).ToList();

				if (idConexoes.Count > 0)
				{
					IHubContext<THubEntity> contexto = GetContext();

					contexto.Clients.Clients(idConexoes).SendAsync(method, obj).GetAwaiter().GetResult();
				}
			}
			catch (Exception excecao)
			{
				Log.TratarErro($"Hub {typeof(THubEntity).Name}: {excecao}", "SignalR");
			}
		}

		protected void SendToAllExcept(string userKey, string method, object obj)
		{
			try
			{
				List<string> idConexoes = _conexoes.GetConnections(userKey).ToList();

				if (idConexoes.Count > 0)
				{
					IHubContext<THubEntity> contexto = GetContext();

					contexto.Clients.AllExcept(idConexoes).SendAsync(method, obj).GetAwaiter().GetResult();
				}
			}
			catch (Exception excecao)
			{
				Log.TratarErro($"Hub {typeof(THubEntity).Name}: {excecao}", "SignalR");
			}
		}

		#endregion Métodos Protegidos

		#region Métodos Públicos Sobrescritos

		public override Task OnConnectedAsync()
		{
			string chaveUsuario = Context.User.Identity.Name;

			if (!string.IsNullOrWhiteSpace(chaveUsuario))
				_conexoes.Add(chaveUsuario, Context.ConnectionId);

			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			string chaveUsuario = Context.User.Identity.Name;

			_conexoes.Remove(chaveUsuario, Context.ConnectionId);

			return base.OnDisconnectedAsync(exception);
		}

		#endregion Métodos Públicos Sobrescritos
	}
}