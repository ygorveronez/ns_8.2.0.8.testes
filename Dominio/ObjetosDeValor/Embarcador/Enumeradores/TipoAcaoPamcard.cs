namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAcaoPamcard
    {
        SomenteCompra = 0,
        ConsultaCompra = 1,
        SomenteConsulta = 2
    }

    public static class TipoAcaoPamcardHelper
    {
        public static string ObterDescricao(this TipoAcaoPamcard tipoAcao)
        {
            switch (tipoAcao)
            {
                case TipoAcaoPamcard.SomenteCompra: return "Somente Compra";
                case TipoAcaoPamcard.ConsultaCompra: return "Consulta Compra";
                case TipoAcaoPamcard.SomenteConsulta: return "Somente Consulta";
                default: return string.Empty;
            }
        }
    }
}