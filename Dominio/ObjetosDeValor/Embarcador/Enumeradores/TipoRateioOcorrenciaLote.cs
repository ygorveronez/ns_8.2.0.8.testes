namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRateioOcorrenciaLote
    {
        Peso = 1,
        ValorMercadoria = 2,
        QuantidadeCTe = 3
    }

    public static class TipoRateioOcorrenciaLoteHelper
    {
        public static string ObterDescricao(this TipoRateioOcorrenciaLote situacao)
        {
            switch (situacao)
            {
                case TipoRateioOcorrenciaLote.Peso: return "Peso";
                case TipoRateioOcorrenciaLote.ValorMercadoria: return "Valor da Mercadoria";
                case TipoRateioOcorrenciaLote.QuantidadeCTe: return "Quantidade CT-e";
                default: return string.Empty;
            }
        }
    }
}
