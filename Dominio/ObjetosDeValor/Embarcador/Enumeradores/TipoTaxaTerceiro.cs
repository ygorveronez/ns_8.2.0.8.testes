namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoTaxaTerceiro
    {
        PorKM = 1,
        PorVeiculo = 2,
        PorTerceiro = 3
    }

    public static class TipoTaxaTerceiroHelper
    {
        public static string ObterDescricao(this TipoTaxaTerceiro tipoTaxaTerceiro)
        {
            switch (tipoTaxaTerceiro)
            {
                case TipoTaxaTerceiro.PorKM: return "Por KM";
                case TipoTaxaTerceiro.PorVeiculo: return "Por Veículo";
                case TipoTaxaTerceiro.PorTerceiro: return "Por Terceiro";
                default: return string.Empty;
            }
        }
    }
}
