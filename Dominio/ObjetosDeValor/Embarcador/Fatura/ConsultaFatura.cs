namespace Dominio.ObjetosDeValor.Embarcador.Fatura
{
    public class ConsultaFatura
    {
        public int Codigo { get; set; }
        public int Descricao { get; set; }
        public int Numero { get; set; }
        public string NumeroBoletos { get; set; }
        public string DescricaoPeriodo { get; set; }
        public string Pessoa { get; set; }
        public string DescricaoSituacao { get; set; }
        public string PeriodoVencimento { get; set; }
        public string PeriodoEmissao { get; set; }
        public string Valor { get; set; }
        public string PedidoViagemNavio { get; set; }
        public string TerminalOrigem { get; set; }
        public string TipoOperacao { get; set; }
        public string NumerosControle { get; set; }
        public string NumerosFiscais { get; set; }
        public double CNPJPessoa { get; set; }
        public int CodigoGrupoPessoa { get; set; }
        public double CodigoTomador { get; set; }
        public int CodigoCentroResultado { get; set; }
        public string Transportadora { get; set; }
        public string NumeroCarga { get; set; }
    }
}
