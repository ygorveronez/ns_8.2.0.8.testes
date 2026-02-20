namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FormaTitulo
    {
        Todos = -1,
        Outros = 0,
        Boleto = 1,
        Deposito = 2,
        Cheque = 3,
        Cartao = 4,
        Dinheiro = 5,
        Financiamento = 6,
        DebitoAutomatico = 7,
        Transferencia = 8,
        CobrancaJudicial = 9,
        PagamentoSalario = 10,
        Pix = 11
    }

    public static class FormaTituloHelper
    {
        public static string ObterDescricao(this FormaTitulo formaTitulo)
        {
            switch (formaTitulo)
            {
                case FormaTitulo.Outros: return "Outros";
                case FormaTitulo.Boleto: return "Boleto";
                case FormaTitulo.Deposito: return "Depósito";
                case FormaTitulo.Cheque: return "Cheque";
                case FormaTitulo.Cartao: return "Cartão";
                case FormaTitulo.Dinheiro: return "Dinheiro";
                case FormaTitulo.Financiamento: return "Financiamento";
                case FormaTitulo.DebitoAutomatico: return "Débito Automático";
                case FormaTitulo.Transferencia: return "Transferência";
                case FormaTitulo.CobrancaJudicial: return "Cobrança Judicial";
                case FormaTitulo.PagamentoSalario: return "Pagamento Salário";
                case FormaTitulo.Pix: return "PIX";
                default: return string.Empty;
            }
        }

        public static FormaTitulo ObterFormaTitulo(string formaTitulo)
        {
            if (string.IsNullOrWhiteSpace(formaTitulo))
                return FormaTitulo.Outros;

            switch (formaTitulo)
            {
                case "Outros": return FormaTitulo.Outros;
                case "Boleto": return FormaTitulo.Boleto;
                case "Depósito": return FormaTitulo.Deposito;
                case "Cheque": return FormaTitulo.Cheque;
                case "Cartão": return FormaTitulo.Cartao;
                case "Dinheiro": return FormaTitulo.Dinheiro;
                case "Financiamento": return FormaTitulo.Financiamento;
                case "Débito Automático": return FormaTitulo.DebitoAutomatico;
                case "Transferência": return FormaTitulo.Transferencia;
                case "Cobrança Judicial": return FormaTitulo.CobrancaJudicial;
                case "Pagamento Salário": return FormaTitulo.PagamentoSalario;
                case "PIX": return FormaTitulo.Pix;
                default: return FormaTitulo.Outros;
            }
        }
    }
}