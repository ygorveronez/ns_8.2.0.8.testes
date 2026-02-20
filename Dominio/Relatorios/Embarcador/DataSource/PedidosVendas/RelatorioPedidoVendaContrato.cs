namespace Dominio.Relatorios.Embarcador.DataSource.PedidosVendas
{
    public class RelatorioPedidoVendaContrato
    {
        public int Numero { get; set; }
        public string RazaoEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string IEEmpresa { get; set; }
        public string FoneEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string NumeroEnderecoEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        public string ComplementoEmpresa { get; set; }
        public string SiteEmpresa { get; set; }
        public string NomePessoa { get; set; }
        public double CNPJPessoa { get; set; }
        public string IEPessoa { get; set; }
        public string EnderecoPessoa { get; set; }
        public string BairroPessoa { get; set; }
        public string CEPPessoa { get; set; }
        public string NumeroEnderecoPessoa { get; set; }
        public string CidadePessoa { get; set; }
        public string EstadoPessoa { get; set; }
        public string EmailPessoa { get; set; }
        public string FonePessoa { get; set; }
        public string FoneSecundarioPessoa { get; set; }
        public string DescricaoItem { get; set; }
        public string ValorTotalItem { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorServicos { get; set; }
        public decimal ValorTotal { get; set; }
        public string Observacao { get; set; }
        public string FormaPagamento { get; set; }
        public string DataVencimentoParcela { get; set; }
        public string SequenciaParcela { get; set; }
        public string Referencia { get; set; }
    }
}