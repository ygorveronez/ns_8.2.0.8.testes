namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrigemRateioDespesaVeiculo
    {
        ContratoFinanciamento = 1,
        DocumentoEntrada = 2,
        Infracao = 3,
        MovimentoFinanceiro = 4,
        Manual = 5,
        Titulo = 6,
        PagamentoMotorista = 7
    }

    public static class OrigemRateioDespesaVeiculoHelper
    {
        public static string ObterDescricao(this OrigemRateioDespesaVeiculo origem)
        {
            switch (origem)
            {
                case OrigemRateioDespesaVeiculo.ContratoFinanciamento: return "Contrato de Financiamento";
                case OrigemRateioDespesaVeiculo.DocumentoEntrada: return "Documento de Entrada";
                case OrigemRateioDespesaVeiculo.Infracao: return "Infração";
                case OrigemRateioDespesaVeiculo.MovimentoFinanceiro: return "Movimento Financeiro";
                case OrigemRateioDespesaVeiculo.Manual: return "Manual";
                case OrigemRateioDespesaVeiculo.Titulo: return "Título";
                case OrigemRateioDespesaVeiculo.PagamentoMotorista: return "Pagamento Motorista";
                default: return string.Empty;
            }
        }
    }
}
