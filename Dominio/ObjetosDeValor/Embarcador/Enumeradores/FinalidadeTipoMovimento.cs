namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FinalidadeTipoMovimento
    {
        Todas = 0,
        TituloFinanceiro = 1,
        MultiploTitulo = 2,
        NaturezaOperacao = 3,
        DocumentoEntrada = 4,
        Abastecimento = 5,
        Pedagio = 6,
        FaturamentoMensal = 7,
        Justificativa = 8,
        MovimentoFinanceiro = 9
    }

    public static class FinalidadeTipoMovimentoHelper
    {
        public static string ObterDescricao(this FinalidadeTipoMovimento finalidadeTipoMovimento)
        {
            switch (finalidadeTipoMovimento)
            {
                case FinalidadeTipoMovimento.Abastecimento:
                    return "Abastecimento";
                case FinalidadeTipoMovimento.DocumentoEntrada:
                    return "Documento de Entrada";
                case FinalidadeTipoMovimento.FaturamentoMensal:
                    return "Faturamento Mensal";
                case FinalidadeTipoMovimento.Justificativa:
                    return "Justificativa";
                case FinalidadeTipoMovimento.MovimentoFinanceiro:
                    return "Movimento Financeiro";
                case FinalidadeTipoMovimento.MultiploTitulo:
                    return "Múltiplos Títulos";
                case FinalidadeTipoMovimento.NaturezaOperacao:
                    return "Natureza da Operação";
                case FinalidadeTipoMovimento.Pedagio:
                    return "Pedágio";
                case FinalidadeTipoMovimento.TituloFinanceiro:
                    return "Título Financeiro";
                default:
                    return "Todas";
            }
        }
    }
}
