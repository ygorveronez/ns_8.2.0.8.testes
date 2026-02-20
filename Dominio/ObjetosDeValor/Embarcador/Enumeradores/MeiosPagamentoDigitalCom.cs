namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MeiosPagamentoDigitalCom
    {
        SEM_PARAR = 0,
        DBTRANS = 1,
        REPOM = 2,
        TARGET_MP = 3,
        MOVE_MAIS = 4,
        VELOE = 5,
        CONECTCAR = 6,
        NOT_DEFINED = 7,
        AMBIPAR = 8,
        TICKET_LOG = 9,
        VELOE_PARCEIROS = 10
    }

    public static class MeiosPagamentoDigitalComHelper
    {
        public static string ObterDescricao(this MeiosPagamentoDigitalCom aplicacaoTabela)
        {
            switch (aplicacaoTabela)
            {
                case MeiosPagamentoDigitalCom.SEM_PARAR: return "Sem Parar";
                case MeiosPagamentoDigitalCom.DBTRANS: return "DBTrans";
                case MeiosPagamentoDigitalCom.REPOM: return "Repom";
                case MeiosPagamentoDigitalCom.TARGET_MP: return "Target MP";
                case MeiosPagamentoDigitalCom.MOVE_MAIS: return "Move Mais";
                case MeiosPagamentoDigitalCom.VELOE: return "Veloe";
                case MeiosPagamentoDigitalCom.CONECTCAR: return "Conectcar";
                case MeiosPagamentoDigitalCom.NOT_DEFINED: return "Não definido";
                case MeiosPagamentoDigitalCom.AMBIPAR: return "Ambipar";
                case MeiosPagamentoDigitalCom.TICKET_LOG: return "Ticket Log";
                case MeiosPagamentoDigitalCom.VELOE_PARCEIROS: return "Veloe Parceiros";
                default: return "Nenhum";
            }
        }
    }
}
