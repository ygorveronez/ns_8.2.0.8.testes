namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRotaFreteRepom
    {
        NaoEspecificado = 0,
        Ida = 1,
        IdaVolta = 2
    }

    public static class TipoRotaFreteRepomHelper
    {
        public static string ObterDescricao(this TipoRotaFreteRepom tipoRotaFreteRepom)
        {
            switch (tipoRotaFreteRepom)
            {
                case TipoRotaFreteRepom.NaoEspecificado: return "Não Especificado";
                case TipoRotaFreteRepom.Ida: return "Ida";
                case TipoRotaFreteRepom.IdaVolta: return "Ida e Volta";
                default: return string.Empty;
            }
        }
    }
}
