namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCalculoComponenteTabelaFrete
    {
        Nenhum = 0,
        Percentual = 1,
        Peso = 2,
        QuantidadeDocumentos = 3,
        Tempo = 4,
        ValorFixo = 5,
        Eixo = 6,
        Cubagem = 7,
        ParametroFixo = 8,
        Volume = 9
    }

    public static class TipoCalculoComponenteTabelaFreteHelper
    {
        public static string ObterDescricao(this TipoCalculoComponenteTabelaFrete tipoCalculoComponenteTabelaFrete)
        {
            switch (tipoCalculoComponenteTabelaFrete)
            {
                case TipoCalculoComponenteTabelaFrete.Nenhum:
                    return "Nenhum";
                case TipoCalculoComponenteTabelaFrete.Percentual:
                    return "Percentual";
                case TipoCalculoComponenteTabelaFrete.Peso:
                    return "Peso";
                case TipoCalculoComponenteTabelaFrete.QuantidadeDocumentos:
                    return "Quantidade de Documentos";
                case TipoCalculoComponenteTabelaFrete.Tempo:
                    return "Tempo";
                case TipoCalculoComponenteTabelaFrete.ValorFixo:
                    return "Valor Fixo";
                case TipoCalculoComponenteTabelaFrete.Eixo:
                    return "Eixo";
                case TipoCalculoComponenteTabelaFrete.Cubagem:
                    return "Cubagem";
                case TipoCalculoComponenteTabelaFrete.ParametroFixo:
                    return "Por Parametro Fixo";
                case TipoCalculoComponenteTabelaFrete.Volume:
                    return "Volume";
                default:
                    return string.Empty;
            }
        }
    }
}
