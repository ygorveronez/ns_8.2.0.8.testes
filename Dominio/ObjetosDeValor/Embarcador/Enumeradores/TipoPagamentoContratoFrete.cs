namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPagamentoContratoFrete
    {
        SobreFreteCarga = 1,
        TabelaFrete = 2
    }

    public static class TipoPagamentoContratoFreteHelper
    {
        public static string Descricao(this TipoPagamentoContratoFrete tipoPagamentoContratoFrete)
        {
            switch (tipoPagamentoContratoFrete)
            {
                case TipoPagamentoContratoFrete.SobreFreteCarga:
                    return "Sobre o valor do frete da carga";
                case TipoPagamentoContratoFrete.TabelaFrete:
                    return "Calculado pela tabela de frete";
                default:
                    return string.Empty;
            }
        }
    }
}
