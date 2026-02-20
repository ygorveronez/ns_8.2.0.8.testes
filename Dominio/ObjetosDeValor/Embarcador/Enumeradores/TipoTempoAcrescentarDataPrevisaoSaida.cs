namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoTempoAcrescimoDecrescimoDataPrevisaoSaida
    {
        Acrescimo = 0,
        Descrescimo = 1,
    }

    public static class TipoTempoAcrescimoDecrescimoDataPrevisaoSaidaHelper
    {
        public static string ObterDescricao(this TipoTempoAcrescimoDecrescimoDataPrevisaoSaida tipo)
        {
            switch (tipo)
            {
                case TipoTempoAcrescimoDecrescimoDataPrevisaoSaida.Acrescimo: return "Acréscimo";
                case TipoTempoAcrescimoDecrescimoDataPrevisaoSaida.Descrescimo: return "Descréscimo";
                default: return string.Empty;
            }
        }
    }
}
