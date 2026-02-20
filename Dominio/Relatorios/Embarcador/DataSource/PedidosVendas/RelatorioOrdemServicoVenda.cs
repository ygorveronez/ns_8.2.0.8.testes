using System;

namespace Dominio.Relatorios.Embarcador.DataSource.PedidosVendas
{
    public class RelatorioOrdemServicoVenda
    {
        #region Propriedades

        public int Numero { get; set; }
        private DateTime DataEmissao { get; set; }
        private DateTime DataEntrega { get; set; }

        public string FantasiaEmpresa { get; set; }
        private string CNPJEmpresa { get; set; }
        public string FoneEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string NumeroEnderecoEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        private string TipoEmpresa { get; set; }

        public string NomePessoa { get; set; }
        private double CNPJPessoa { get; set; }
        public string FonePessoa { get; set; }
        public string EnderecoPessoa { get; set; }
        public string BairroPessoa { get; set; }
        public string CEPPessoa { get; set; }
        public string NumeroEnderecoPessoa { get; set; }
        public string CidadePessoa { get; set; }
        public string EstadoPessoa { get; set; }
        private string TipoPessoa { get; set; }
        public string NomeFuncionario { get; set; }
        public string PessoaSolicitante { get; set; }
        public int CodigoItemBanco { get; set; }
        public string CodigoItem { get; set; }
        public string DescricaoItem { get; set; }
        public decimal QuantidadeItem { get; set; }
        public decimal ValorUnitarioItem { get; set; }
        public decimal ValorTotalItem { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorServicos { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorTotal { get; set; }
        public string Observacao { get; set; }
        public int Tipo { get; set; }
        public int Status { get; set; }
        public int TipoServico { get; set; }
        public string FornecedorServico { get; set; }
        public string FuncionarioServico { get; set; }

        public int KM { get; set; }
        public string Placa { get; set; }
        public string MarcaVeiculo { get; set; }
        public string ModeloVeiculo { get; set; }
        public bool HabilitarTabelaValorOrdemServicoVenda { get; set; }
        public int KMInicial { get; set; }
        public int KMFinal { get; set; }        
        public int KMTotal { get; set; }
        public string HoraInicial { get; set; }
        public string HoraFinal { get; set; }        
        public string HoraTotal { get; set; }
        public string GuidArquivoAssinatura { get; set; }

        public string HoraInicial2 { get; set; }
        public string HoraFinal2 { get; set; }
        public int KMInicial2 { get; set; }
        public int KMFinal2 { get; set; }
        public decimal ValorHora { get; set; }
        public decimal ValorKM { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataEntregaFormatada
        {
            get { return DataEntrega != DateTime.MinValue ? DataEntrega.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string CNPJEmpresaFormatado
        {
            get { return CNPJEmpresa.ObterCpfOuCnpjFormatado(TipoEmpresa); }
        }

        public string CNPJPessoaFormatado
        {
            get { return CNPJPessoa.ToString().ObterCpfOuCnpjFormatado(TipoPessoa); }
        }

        public string QuantidadeOrdem
        {
            get { return this.KMTotal > 0 ? this.KMTotal.ToString("n0") : !string.IsNullOrWhiteSpace(this.HoraTotal) ? this.HoraTotal : this.QuantidadeItem.ToString("n2"); }
        }

        public decimal ValorUnitarioOrdem
        {
            get { return this.ValorKM > 0 ? this.ValorKM : this.ValorHora > 0 ? this.ValorHora : this.ValorUnitarioItem; }
        }


        #endregion
    }
}
