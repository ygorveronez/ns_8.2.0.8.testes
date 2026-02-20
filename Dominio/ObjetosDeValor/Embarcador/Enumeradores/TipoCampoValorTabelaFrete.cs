namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCampoValorTabelaFrete
    {
        ValorFixoArredondadoParaCima = -1,
        ValorFixo = 0,
        AumentoPercentual = 1,
        AumentoValor = 2,
        PercentualSobreValorNotaFiscal = 3,
        Desabilitado = 4
    }

    public static class TipoCampoValorTabelaFreteHelper
    {
        public static bool IsPermiteSomar(this TipoCampoValorTabelaFrete tipoCampoValorTabelaFrete)
        {
            switch (tipoCampoValorTabelaFrete)
            {
                case TipoCampoValorTabelaFrete.ValorFixo:
                case TipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima:
                case TipoCampoValorTabelaFrete.AumentoValor:
                    return true;

                default:
                    return false;
            }
        }

        public static string ObterDescricao(this TipoCampoValorTabelaFrete tipoCampoValorTabelaFrete)
        {
            switch (tipoCampoValorTabelaFrete)
            {
                case TipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima: return "Valor Fixo Arredondando o parâmetro para cima";
                case TipoCampoValorTabelaFrete.ValorFixo: return "Valor Fixo";
                case TipoCampoValorTabelaFrete.AumentoPercentual: return "Aumento Percentual";
                case TipoCampoValorTabelaFrete.AumentoValor: return "Acrescenta o Valor";
                case TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal: return "Percentual sobre o Valor da Mercadoria";
                case TipoCampoValorTabelaFrete.Desabilitado: return "Desabilitado";
                default: return "";
            }
        }
    }
}
