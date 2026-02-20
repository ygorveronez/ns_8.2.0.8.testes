namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PagamentoMaritimo
    {
        NaoDefinido = 0,
        Collect = 1,
        Prepaid = 2,
        PrepaidAbroad = 3
    }

    public static class EnumPagamentoMaritimoHelper
    {
        public static string ObterDescricao(this PagamentoMaritimo situacao)
        {
            switch (situacao)
            {
                case PagamentoMaritimo.Collect: return "Collect";
                case PagamentoMaritimo.Prepaid: return "Prepaid";
                case PagamentoMaritimo.PrepaidAbroad: return "Prepaid Abroad";
                default: return string.Empty;
            }
        }
    }
}
