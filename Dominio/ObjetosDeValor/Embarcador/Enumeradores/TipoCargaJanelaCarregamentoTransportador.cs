namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCargaJanelaCarregamentoTransportador
    {
        PorTipoTransportadorCarga = 0,
        PorPrioridadeRotaGrupo = 1,
        PorPrioridadeRota = 2,
        PorMenorValorFreteTabelaCalculado = 3,
        PorGrupoTransportador = 4,
        PorTipoTransportadorCargaSecundario = 5,
        PorTipoTransportadorTerceiroCargaSecundario = 6,
    }

    public static class TipoCargaJanelaCarregamentoTransportadorHelper
    {
        public static string ObterCorLinha(this TipoCargaJanelaCarregamentoTransportador tipo)
        {
            switch (tipo)
            {
                case TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRota: return "#85de7b";
                case TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRotaGrupo: return "#c8e8ff";
                case TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorCarga: return "#ffffff";
                case TipoCargaJanelaCarregamentoTransportador.PorMenorValorFreteTabelaCalculado: return "#e29e23";
                case TipoCargaJanelaCarregamentoTransportador.PorGrupoTransportador: return "#ff66ff";
                case TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorCargaSecundario: return "#b7c30a";
                case TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorTerceiroCargaSecundario: return "#b7c30a";
                default: return string.Empty;
            }
        }
    }
}
