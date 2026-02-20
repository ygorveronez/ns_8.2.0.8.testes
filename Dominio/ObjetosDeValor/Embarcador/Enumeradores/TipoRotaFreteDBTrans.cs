namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRotaFreteDBTrans
    {
        NaoEspecificado = 0,
        Ida = 1,
        IdaVolta = 2
    }

    public static class TipoRotaFreteDBTransHelper
    {
        public static string ObterDescricao(this TipoRotaFreteDBTrans tipoRotaFreteDBTrans)
        {
            switch (tipoRotaFreteDBTrans)
            {
                case TipoRotaFreteDBTrans.NaoEspecificado: return "Não Especificado";
                case TipoRotaFreteDBTrans.Ida: return "Ida";
                case TipoRotaFreteDBTrans.IdaVolta: return "Ida e Volta";
                default: return string.Empty;
            }
        }
    }
}
