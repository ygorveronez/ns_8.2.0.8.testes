using System;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class DANFE
    {
        public int CodigoNota { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public string RazaoEmpresa { get; set; }
        public string RuaEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string NumeroEmpresa { get; set; }
        public string ComplementoEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string FoneEmpresa { get; set; }
        public int TipoNota { get; set; }
        public string Chave { get; set; }
        public string NaturezaOperacao { get; set; }
        public string IEEmpresa { get; set; }
        public string IESTEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string TipoEmpresa { get; set; }
        public int TipoAmbiente { get; set; }
        public string Protocolo { get; set; }
        public string TipoCliente { get; set; }
        public DateTime DataProcessamento { get; set; }
        public string NomeCliente { get; set; }
        public double CNPJCPFCliente { get; set; }
        public string EnderecoCliente { get; set; }
        public string NumeroCliente { get; set; }
        public string ComplementoCliente { get; set; }
        public string BairroCliente { get; set; }
        public string CidadeCliente { get; set; }
        public string CEPCliente { get; set; }
        public string FoneCliente { get; set; }
        public string EstadoCliente { get; set; }
        public string IECliente { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataSaida { get; set; }
        public DateTime HoraSaida { get; set; }
        public decimal BCICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal BCICMSST { get; set; }
        public decimal ValorICMSST { get; set; }
        public decimal ValorProduto { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorSeguro { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorOutras { get; set; }
        public decimal ValorIPI { get; set; }
        public decimal ValorTotal { get; set; }
        public string RazaoTransportadora { get; set; }
        public int TipoFreteTransportadora { get; set; }
        public string ANTTTransportadora { get; set; }
        public string VeiculoTransportadora { get; set; }
        public string EstadoVeiculoTransportadora { get; set; }
        public string CNPJCPFTransportadora { get; set; }
        public string EnderecoTransportadora { get; set; }
        public string CidadeTransportadora { get; set; }
        public string EstadoTransportadora { get; set; }
        public string IETransportadora { get; set; }
        public string QuantidadeCargaTransportadora { get; set; }
        public string EspecieCargaTransportadora { get; set; }
        public string MarcaCargaTransportadora { get; set; }
        public string NumeroCargaTransportadora { get; set; }
        public decimal PesoBrutoCargaTransportadora { get; set; }
        public decimal PesoLiquidoCargaTransportadora { get; set; }
        public string IMEmpresa { get; set; }
        public decimal ValorServicos { get; set; }
        public decimal BCISS { get; set; }
        public decimal ValorISS { get; set; }
        public string Observacao { get; set; }
        public string InformacoesComplementares { get; set; }
        public int CodigoItem { get; set; }
        public int CodigoProduto { get; set; }
        public string CodigoProdutoIntegracao { get; set; }
        public string DescricaoItem { get; set; }
        public string CodigoNCMItem { get; set; }
        public int CodigoCFOPItem { get; set; }
        public int CSTItem { get; set; }
        public int OrigemItem { get; set; }
        public int UnidadeItem { get; set; }
        public decimal QuantidadeItem { get; set; }
        public decimal ValorUnitarioItem { get; set; }
        public decimal ValorTotalItem { get; set; }
        public decimal BCICMSItem { get; set; }
        public decimal ValorICMSItem { get; set; }
        public decimal ValorIPIItem { get; set; }
        public decimal AliquotaICMSItem { get; set; }
        public decimal AliquotaIPIItem { get; set; }
        public decimal ImpostoIBPT { get; set; }
        public string InformacoesAdicionaisItem { get; set; }
        public string LotesProduto { get; set; }
        private int CasasDecimaisQuantidade { get; set; }
        private int CasasDecimaisValorUnitario { get; set; }


        public bool UtilizarEnderecoRetirada { get; set; }
        public string ClienteRetirada { get; set; }
        public string LocalidadeRetirada { get; set; }
        public string RetiradaLogradouro { get; set; }
        public string RetiradaNumeroLogradouro { get; set; }
        public string RetiradaComplementoLogradouro { get; set; }
        public string RetiradaBairro { get; set; }
        public string RetiradaCEP { get; set; }
        public string RetiradaTelefone { get; set; }
        public string RetiradaEmail { get; set; }
        public string RetiradaIE { get; set; }

        public bool UtilizarEnderecoEntrega { get; set; }
        public string ClienteEntrega { get; set; }
        public string LocalidadeEntrega { get; set; }
        public string EntregaLogradouro { get; set; }
        public string EntregaNumeroLogradouro { get; set; }
        public string EntregaComplementoLogradouro { get; set; }
        public string EntregaBairro { get; set; }
        public string EntregaCEP { get; set; }
        public string EntregaTelefone { get; set; }
        public string EntregaEmail { get; set; }
        public string EntregaIE { get; set; }

        public string TipoClienteRetirada { get; set; }
        public double CNPJCPFClienteRetirada { get; set; }
        public string TipoClienteEntrega { get; set; }
        public double CNPJCPFClienteEntrega { get; set; }

        public string QuantidadeItemFormatada
        {
            get { return CasasDecimaisQuantidade > 0 ? QuantidadeItem.ToString("n" + CasasDecimaisQuantidade) : QuantidadeItem.ToString("n4"); }
        }

        public string ValorUnitarioItemFormatado
        {
            get { return CasasDecimaisValorUnitario > 0 ? ValorUnitarioItem.ToString("n" + CasasDecimaisValorUnitario) : ValorUnitarioItem.ToString("n10"); }
        }
    }
}
