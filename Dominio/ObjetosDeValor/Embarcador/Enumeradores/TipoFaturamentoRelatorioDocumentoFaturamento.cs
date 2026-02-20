namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFaturamentoRelatorioDocumentoFaturamento
    {
        EmFatura = 1,
        Faturado = 2,
        NaoFaturado = 3
    }

    public static class TipoFaturamentoRelatorioDocumentoFaturamentoHelper
    {
        public static string ObterDescricao(this TipoFaturamentoRelatorioDocumentoFaturamento tipoFaturamento)
        {
            switch (tipoFaturamento)
            {
                case TipoFaturamentoRelatorioDocumentoFaturamento.EmFatura:
                    return "Em Fatura";
                case TipoFaturamentoRelatorioDocumentoFaturamento.Faturado:
                    return "Faturado";
                case TipoFaturamentoRelatorioDocumentoFaturamento.NaoFaturado:
                    return "Não Faturado";
                default:
                    return string.Empty;
            }
        }
    }
}
