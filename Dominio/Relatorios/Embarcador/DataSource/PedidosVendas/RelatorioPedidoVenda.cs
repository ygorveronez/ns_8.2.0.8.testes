using System;

namespace Dominio.Relatorios.Embarcador.DataSource.PedidosVendas
{
    public class RelatorioPedidoVenda
    {
        public int Numero { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataEntrega { get; set; }

        public string FantasiaEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string FoneEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string NumeroEnderecoEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        public string TipoEmpresa { get; set; }
        public int CasasQuantidadeProdutoNFe { get; set; }
        public int CasasValorProdutoNFe { get; set; }

        public string NomePessoa { get; set; }
        public double CNPJPessoa { get; set; }
        public string FonePessoa { get; set; }
        public string EnderecoPessoa { get; set; }
        public string BairroPessoa { get; set; }
        public string CEPPessoa { get; set; }
        public string NumeroEnderecoPessoa { get; set; }
        public string CidadePessoa { get; set; }
        public string EstadoPessoa { get; set; }
        public string TipoPessoa { get; set; }

        public string NomeFuncionario { get; set; }
        public int CodigoItemBanco { get; set; }
        public int CodigoProduto { get; set; }
        public int CodigoServico { get; set; }
        public string CodigoItem { get; set; }
        public string DescricaoItem { get; set; }
        public string CodigoNCM { get; set; }
        public decimal QuantidadeItem { get; set; }
        public decimal ValorUnitarioItem { get; set; }
        public decimal ValorTotalItem { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorServicos { get; set; }
        public decimal ValorTotal { get; set; }

        public string Observacao { get; set; }
        public int Tipo { get; set; }
        public int Status { get; set; }
        public string FormaPagamento { get; set; }
        public int QtdParcela { get; set; }
        public bool PossuiFoto { get; set; }
        public byte[] Foto { get; set; }

        public string Referencia { get; set; }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao.ToString("dd/MM/yyyy"); }
        }

        public string DataEntregaFormatada
        {
            get { return DataEntrega.ToString("dd/MM/yyyy"); }
        }

        public string CNPJEmpresaFormatado
        {
            get { return CNPJEmpresa.ObterCpfOuCnpjFormatado(TipoEmpresa); }
        }

        public string CNPJPessoaFormatado
        {
            get { return CNPJPessoa.ToString().ObterCpfOuCnpjFormatado(TipoPessoa); }
        }
    }
}