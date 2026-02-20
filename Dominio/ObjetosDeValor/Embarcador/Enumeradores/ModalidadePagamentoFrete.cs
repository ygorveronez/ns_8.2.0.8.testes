namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ModalidadePagamentoFrete
    {
        Pago = 0,
        A_Pagar = 1,
        Outros = 2,
        NaoDefinido = 9
    }

    public static class ModalidadePagamentoFreteHelper
    {
        public static string ObterDescricao(this ModalidadePagamentoFrete modo)
        {
            switch (modo)
            {
                case ModalidadePagamentoFrete.Pago: return "Pago";
                case ModalidadePagamentoFrete.A_Pagar: return "À Pagar";
                case ModalidadePagamentoFrete.Outros: return "Outros";
                case ModalidadePagamentoFrete.NaoDefinido: return "Não Definido";
                default: return "";
            }
        }
    }
}
