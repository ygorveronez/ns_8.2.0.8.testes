namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum NotaFiscalSituacaoGatilho
    {
        SemGatilho = 0,
        ConfirmacaoEntregaNota = 1,
        RejeicaoEntregaNota = 2,
        RetificarEntrega = 3,
    }

    public static class NotaFiscalSituacaoGatilhoHelper
    {
        public static string ObterDescricao(this NotaFiscalSituacaoGatilho gatilho)
        {
            switch (gatilho)
            {
                case NotaFiscalSituacaoGatilho.ConfirmacaoEntregaNota: return "Confirmação da Entrega da Nota";
                case NotaFiscalSituacaoGatilho.RejeicaoEntregaNota: return "Rejeição da Entrega da Nota";
                case NotaFiscalSituacaoGatilho.RetificarEntrega: return "Retificar Entrega da Nota";
                default: return "";
            }
        }
    }
}