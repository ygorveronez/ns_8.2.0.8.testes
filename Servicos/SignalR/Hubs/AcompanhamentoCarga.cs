using Dominio.MSMQ;

namespace Servicos.SignalR.Hubs
{
	public enum AcompanhamentoCargaHubs
    {
        CargaAtualizada = 1,
        CargaInserida = 2,
        ListaCargasAtualizadas = 3,
        InformarRetornoProcessamentoDocumentosFiscais = 4
    }

    public class AcompanhamentoCarga : SignalRBase<AcompanhamentoCarga>
    {
        public AcompanhamentoCarga()
        {
        }

        public static string GetHub(AcompanhamentoCargaHubs metodo)
        {
            switch (metodo)
            {
                case AcompanhamentoCargaHubs.CargaAtualizada:
                    return "CargaAtualizada";
                case AcompanhamentoCargaHubs.CargaInserida:
                    return "CargaInserida";
                case AcompanhamentoCargaHubs.ListaCargasAtualizadas:
                    return "CargasAtualizadas";
                case AcompanhamentoCargaHubs.InformarRetornoProcessamentoDocumentosFiscais:
                    return "InformarRetornoProcessamentoDocumentosFiscais";
                default:
                    return "";
            }
        }

        public void CargasAtualizadas(Notification notification)
        {
            dynamic Objeto = notification.Content;
            var retorno = new
            {
                Cards = Objeto.objetoCard,
                Inserido = false
            };

            SendToAll("informarListaCardAtualizado", retorno);
        }

        public void CargaAtualizada(Notification notification)
        {
            var retorno = new
            {
                Card = notification.Content,
                Inserido = false
            };

			SendToAll("informarCardAtualizado", retorno);
        }

        public void CargaInserida(Notification notification)
        {
            var retorno = new
            {
                Card = notification.Content,
                Inserido = true
            };

			SendToAll("informarCardAtualizado", retorno);
        }

        public override string GetKey()
        {
            return Context.User.Identity.Name;
        }

        public override void ProcessarNotificacao(Notification notification)
        {
            switch (notification.Service)
            {
                case "CargaAtualizada":
                    CargaAtualizada(notification);
                    break;
                case "CargaInserida":
                    CargaInserida(notification);
                    break;
                case "CargasAtualizadas":
                    CargasAtualizadas(notification);
                    break;
                case "InformarRetornoProcessamentoDocumentosFiscais":
                    InformarRetornoProcessamentoDocumentosFiscais(notification);
                    break;
                default:
                    break;
            }
        }

        public void InformarCardAtualizado(Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga.Carga cardCarga, bool Inserido)
        {
            var retorno = new
            {
                Card = cardCarga,
                Inserido = Inserido
            };

			SendToAll("informarCardAtualizado", retorno);
        }

        public void InformarMensagensAtualizadas()
        {
			SendToAll("atualizarCardMensagens");
        }


        public void InformarRetornoProcessamentoDocumentosFiscais(Notification notification)
        {
            var ret = new
            {
                CodigoCarga = notification.Content.CodigoCarga,
                Retorno = notification.Content.Retorno
            };

            SendToAll("InformarRetornoProcessamentoDocumentosFiscais", ret);
        }
    }
}
