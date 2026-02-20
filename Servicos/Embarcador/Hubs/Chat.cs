using Dominio.ObjetosDeValor.Embarcador.Frota;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Hubs
{
    public class Chat : HubBase<Chat>
    {
        protected readonly static ConnectionMapping<string> _connectionsChat = new ConnectionMapping<string>();

        public override Task OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;

            if (!string.IsNullOrWhiteSpace(name))
                _conexoes.Add(name, Context.ConnectionId);

            AtualizarStatusUsuario(name, true);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string name = Context.User.Identity.Name;

            _conexoes.Remove(name, Context.ConnectionId);

            AtualizarStatusUsuario(name, false);

            return base.OnDisconnectedAsync(exception);
        }

        public void NotificarMensagemUsuario(int codigoUsuarioReceiver, int codigoFuncionarioLogado, string mensagem, string dataHoraEnvio, string quemEnviou, bool enviarTodos = false)
        {
            var retorno = new
            {
                idLink = "chat" + codigoFuncionarioLogado.ToString(),
                idChat = "box_" + codigoFuncionarioLogado.ToString(),
                msg = mensagem,
                dataHoraEnvio,
                quemEnviou,
                codigoRemetente = codigoFuncionarioLogado
            };
            if (!enviarTodos)
                SendToClient(codigoUsuarioReceiver.ToString(), "NotificarMensagemUsuario", retorno);
            else
                SendToAll("NotificarMensagemUsuario", retorno);
        }

        public bool VerificarUsuarioOnline(int codigoUsuarioReceiver)
        {
            return IsConexaoAtiva(codigoUsuarioReceiver.ToString());
        }

        public void AtualizarStatusUsuario(string codigoUsuario, bool conectado)
        {
            var retorno = new
            {
                codigoUsuario,
                conectado
            };

            SendToAll("AtualizarStatusUsuario", retorno);
        }

        public void CriarNotificaoChatOutroAmbiente(int codigoMensagem, string urlBaseOrigemRequisicao)
        {
            if (!string.IsNullOrWhiteSpace(urlBaseOrigemRequisicao))
            {
                try
                {
                    string url = $"{urlBaseOrigemRequisicao}/Chat/SendMessageOutroAmbiente?CodigoChat={codigoMensagem}";

                    WebRequest requisicao = WebRequest.Create(url);

                    requisicao.Method = "GET";

                    WebResponse resposta = requisicao.GetResponse();

                    resposta.Close();
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);
                }
            }
        }
    }
}
