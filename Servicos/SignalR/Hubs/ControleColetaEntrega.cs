using Dominio.MSMQ;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.SignalR.Hubs
{

	public enum ControleColetaEntregaHubs
    {
        MensagemChatEnviada = 1,
        MensagemRecebida = 2,

    }

    public class ControleColetaEntrega : SignalRBase<ControleColetaEntrega>
    {
        public ControleColetaEntrega()
        {
        }

        public static string GetHub(ControleColetaEntregaHubs metodo)
        {
            switch (metodo)
            {
                case ControleColetaEntregaHubs.MensagemChatEnviada:
                    return "MensagemChatEnviada";
                case ControleColetaEntregaHubs.MensagemRecebida:
                    return "MensagemRecebida";
                default:
                    return "";
            }
        }

        public override string GetKey()
        {
            return Context.User.Identity.Name;
        }

        public override void ProcessarNotificacao(Notification notification)
        {
            switch (notification.Service)
            {
                case "MensagemChatEnviada":
                    MensagemChatEnviada(notification);
                    break;
                case "MensagemRecebida":
                    MensagemRecebida(notification);
                    break;
                default:
                    break;
            }
        }

        public void MensagemRecebida(Dominio.MSMQ.Notification notification)
        {
            List<string> connectionId = new List<string>();
            foreach (int idUsuario in notification.UsersID)
                connectionId.AddRange(_connections.GetConnections(idUsuario.ToString()).ToList());

            if (connectionId.Count > 0)
            {
                SendToConnectionIds(connectionId, "mensagemRecebida", JsonConvert.SerializeObject(notification.Content));

                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();
                servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();
            }
        }

        public void MensagemChatEnviada(Dominio.MSMQ.Notification notification)
        {
            SendToAll("mensagemChatEnviada", JsonConvert.SerializeObject(notification.Content));
            
            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();
            servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();
        }

        public void ConfirmarLeituraMensagem(int codigoMensagem)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(SignalRConnection.GetInstance().ConnectionString);
            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();

            try
            {
                Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem = repChatMobileMensagem.BuscarPorCodigo(codigoMensagem);

                if (chatMobileMensagem != null && !chatMobileMensagem.MensagemLida)
                {
                    Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(int.Parse(GetKey()));
                    if (chatMobileMensagem.Remetente.Codigo != usuario.Codigo)
                    {
                        chatMobileMensagem.MensagemLida = true;
                        chatMobileMensagem.DataConfirmacaoLeitura = DateTime.Now;
                        repChatMobileMensagem.Atualizar(chatMobileMensagem);
                        Servicos.Embarcador.Chat.ChatMensagem.NotificarMensagemRecebida(chatMobileMensagem, SignalRConnection.GetInstance().CodigoClienteMultisoftware, unitOfWork);
                    }

                    servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
