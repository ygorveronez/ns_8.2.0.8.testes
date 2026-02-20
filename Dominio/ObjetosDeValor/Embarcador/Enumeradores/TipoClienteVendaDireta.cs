namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoClienteVendaDireta
    {
        Web = 0,
        CallCenter = 1,
        NaoSeAplica = 2
    }

    public static class TipoClienteVendaDiretaHelper
    {
        public static string ObterDescricao(this TipoClienteVendaDireta tipoCobranca)
        {
            switch (tipoCobranca)
            {
                case TipoClienteVendaDireta.Web: return "Web";
                case TipoClienteVendaDireta.CallCenter: return "Call Center";
                case TipoClienteVendaDireta.NaoSeAplica: return "Não se aplica";
                default: return "";
            }
        }
    }
}
