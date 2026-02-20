namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMovimentoExportacao
    {
        Emissao = 1,
        Cancelamento = 2,
        BaixaTituloReceber = 3,
        AcrescimoDescontoBaixaTituloReceber = 4,
        CancelamentoBaixaTituloReceber = 5,
        CancelamentoAcrescimoDescontoBaixaTituloReceber = 6,
        AprovacaoContratoFrete = 7,
        ReversaoContratoFrete = 8,
        PagamentoContratoFrete = 9,
        ReversaoPagamentoContratoFrete = 10,
        AcrescimoDescontoFatura = 11,
        CancelamentoAcrescimoDescontoFatura = 12
    }

    public static class TipoMovimentoExportacaoHelper
    {
        public static string ObterDescricao(this TipoMovimentoExportacao tipo)
        {
            switch (tipo)
            {
                case TipoMovimentoExportacao.Emissao: return "Emissão";
                case TipoMovimentoExportacao.Cancelamento: return "Cancelamento";
                case TipoMovimentoExportacao.BaixaTituloReceber: return "Baixa de Título a Receber";
                case TipoMovimentoExportacao.CancelamentoBaixaTituloReceber: return "Cancelamento da Baixa de Título a Receber";
                case TipoMovimentoExportacao.AcrescimoDescontoBaixaTituloReceber: return "Acréscimo/Desconto da Baixa de Título a Receber";
                case TipoMovimentoExportacao.CancelamentoAcrescimoDescontoBaixaTituloReceber: return "Cancelamento de Acréscimo/Desconto da Baixa de Título a Receber";
                case TipoMovimentoExportacao.AprovacaoContratoFrete: return "Aprovação do Contrato de Frete";
                case TipoMovimentoExportacao.ReversaoContratoFrete: return "Reversão/Cancelamento do Contrato de Frete";
                case TipoMovimentoExportacao.PagamentoContratoFrete: return "Pagamento do Contrato de Frete";
                case TipoMovimentoExportacao.ReversaoPagamentoContratoFrete: return "Reversão do Pagamento do Contrato de Frete";
                case TipoMovimentoExportacao.AcrescimoDescontoFatura: return "Acréscimo/Desconto da Fatura";
                case TipoMovimentoExportacao.CancelamentoAcrescimoDescontoFatura: return "Cancelamento de Acréscimo/Desconto da Fatura";
                default: return "";
            }
        }
    }
}
