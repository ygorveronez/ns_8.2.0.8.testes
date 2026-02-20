namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ModoCompraValePedagioTarget
    {
        CartaoTarget = 1,
        PedagioTagViaFacil = 2,
        PedagioTagVeloe = 5,
        PedagioTagMoveMais = 6,
        PedagioConectCar = 7,
        PedagioTagAmbipar = 8,
        PedagioTagRepom = 9,
        PedagioTagTicketLog = 10,
        PedagioTagEdenred = 11,
        PedagioTagTaggy = 12,
        PedagioTagAutomatico = 13,
        PedagioTagIndefinido = 14
    }

    public static class ModoCompraValePedagioTargetHelper
    {
        public static string ObterValorTagEmissorValePedagio(this ModoCompraValePedagioTarget modoCompraValePedagioTarget)
        {
            switch (modoCompraValePedagioTarget)
            {
                case ModoCompraValePedagioTarget.PedagioConectCar: return "9992";
                case ModoCompraValePedagioTarget.PedagioTagViaFacil: return "9993";
                case ModoCompraValePedagioTarget.PedagioTagMoveMais: return "9996";
                case ModoCompraValePedagioTarget.PedagioTagVeloe: return "9997";
                default: return string.Empty;
            }
        }
        public static string ObterDescricao(this ModoCompraValePedagioTarget modoCompraValePedagioTarget)
        {
            switch (modoCompraValePedagioTarget)
            {
                case ModoCompraValePedagioTarget.CartaoTarget: return "Cartão Target";
                case ModoCompraValePedagioTarget.PedagioTagViaFacil: return "Tag Via Fácil (Sem Parar)";
                case ModoCompraValePedagioTarget.PedagioTagVeloe: return "Tag Veloe";
                case ModoCompraValePedagioTarget.PedagioTagMoveMais: return "Tag Move mais";
                case ModoCompraValePedagioTarget.PedagioConectCar: return "Conect Car";
                case ModoCompraValePedagioTarget.PedagioTagAmbipar: return "Tag Ambipar";
                case ModoCompraValePedagioTarget.PedagioTagRepom: return "Repom";
                case ModoCompraValePedagioTarget.PedagioTagTicketLog: return "Ticket Log";
                case ModoCompraValePedagioTarget.PedagioTagEdenred: return "Edenred";
                case ModoCompraValePedagioTarget.PedagioTagTaggy: return "Tag Taggy";
                case ModoCompraValePedagioTarget.PedagioTagAutomatico: return "Tag Automático";
                case ModoCompraValePedagioTarget.PedagioTagIndefinido: return "Tag Indefinido";
                default: return string.Empty;
            }
        }
    }
}