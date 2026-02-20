using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class FreteTerceirizado
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string CNPJEmpresa { get; set; }
        public string Empresa { get; set; }
        public int ContratoFrete { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroCTes { get; set; }
        public string NumeroMDFes { get; set; }
        public string NumeroCIOT { get; set; }
        public string ModeloVeicular { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public decimal ValorPedagio { get; set; }

        public string PISPASEPTerceiro { get; set; }
        private double CPFCNPJTerceiro { get; set; }
        private string TipoTerceiro { get; set; }
        public string CPFCNPJRemetente { get; set; }
        public string CPFCNPJDestinatario { get; set; }
        public string Terceiro { get; set; }
        public string DataNascimentoTerceiro { get; set; }
        private RegimeTributario RegimeTributarioTerceiro { get; set; }
        private string TipoPessoaTerceiro { get; set; }

        public string Remetente { get; set; }
        public string LocalidadeRemetente { get; set; }
        public string Destinatario { get; set; }
        public string LocalidadeDestinatario { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal ValorBruto { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorSaldo { get; set; }
        public decimal ValorINSS { get; set; }
        public decimal ValorIRRF { get; set; }
        public decimal ValorSEST { get; set; }
        public decimal ValorSENAT { get; set; }
        public decimal ValorAcrescimos { get; set; }
        public decimal ValorDescontos { get; set; }
        public decimal PercentualAdiantamento { get; set; }
        public decimal ValorAdiantamento { get; set; }
        public decimal PercentualAbastecimento { get; set; }
        public decimal ValorAbastecimento { get; set; }
        public decimal ValorFreteMenosAbastecimento { get; set; }
        public DateTime DataEmissao { get; set; }
        public string DataEncerramento { get; set; }
        public string DataAutorizacaoPagamento { get; set; }
        public string DataVencimentoAdiantamento { get; set; }
        public string DataPagamentoAdiantamento { get; set; }
        public string DataVencimentoValor { get; set; }
        public string DataPagamentoValor { get; set; }
        public string Veiculo { get; set; }
        public string SegmentoVeiculo { get; set; }
        public string Motorista { get; set; }
        public string CPFMotorista { get; set; }
        public string RG { get; set; }
        public decimal ValorComponente1 { get; set; }
        public decimal ValorComponente2 { get; set; }
        public decimal ValorComponente3 { get; set; }
        public decimal ValorComponente4 { get; set; }
        public decimal ValorComponente5 { get; set; }
        public decimal ValorComponente6 { get; set; }
        public decimal ValorComponente7 { get; set; }
        public decimal ValorComponente8 { get; set; }
        public decimal ValorComponente9 { get; set; }
        public decimal ValorComponente10 { get; set; }
        public decimal ValorComponente11 { get; set; }
        public decimal ValorComponente12 { get; set; }
        public decimal ValorComponente13 { get; set; }
        public decimal ValorComponente14 { get; set; }
        public decimal ValorComponente15 { get; set; }
        private string CPFCNPJFilialEmissora { get; set; }
        public string FilialEmissora { get; set; }
        public string UFFilialEmissora { get; set; }
        private SituacaoContratoFrete SituacaoContratoFrete { get; set; }
        private DateTime DataAprovacao { get; set; }
        public string TipoOperacao { get; set; }
        public string DataAberturaCIOT { get; set; }
        public string DataEncerramentoCIOT { get; set; }
        public int PercentualVariacao { get; set; }
        public string CentroResultado { get; set; }
        public string Titulos { get; set; }
        public string DataSaqueAdiantamento { get; set; }
        public decimal ValorSaqueAdiantamento { get; set; }
        public string TipoFornecedor { get; set; }
        public int MesCompetencia { get; set; }
        public int AnoCompetencia { get; set; }
        public string CentroResultadoEmpresa { get; set; }
        public int MunicipioLancamento { get; set; }
        public string CodigoEstabelecimento { get; set; }
        public string CodigoEmpresa { get; set; }
        public string ProtocoloCIOT { get; set; }
        public string CodigoVerificadorCIOT { get; set; }
        public string Expedidor { get; set; }
        public string LocalidadeExpedidor { get; set; }
        public string UFExpedidor { get; set; }
        public string SerieCTes { get; set; }
        public string StatusCTes { get; set; }
        public decimal BCICMS { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal PesoKg { get; set; }
        public string NumeroValePedagio { get; set; }
        public string ProdutoPredominante { get; set; }
        public decimal ValorFreteNegociado { get; set; }
        public decimal ValorFreteTerceiro { get; set; }
        public decimal TotalDescontos { get; set; }
        public decimal Impostos { get; set; }
        public string Tomador { get; set; }
        public string CPFCNPJTomador { get; set; }
        public decimal ValorPedagioManual { get; set; }
        public SituacaoCarga SituacaoCarga { get; set; }
        public string LocalidadeEmpresaFilial { get; set; }
        public decimal ValorReceberCTes { get; set; }
        public decimal ValorTotalProdutosNotaFiscal { get; set; }
        public string NumeroDocAnterior { get; set; }
        public string TipoCarga { get; set; }
        public DateTime DataCarga { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public int TipoDaConta { get; set; }     
        public string Titular { get; set; }   
        public string TipoDocumento { get; set; }

        public string CodigoRetornoST { get; set; }
        public string CodigoRetornoSU { get; set; }
        public string CodigoIntegracao { get; set; }
        public OperadoraCIOT Operadora { get; set; }
        public string CentroCustoViagem { get; set; }

        #endregion

        #region Propriedades com Regras
        public decimal SaldoFrete
        {
            get
            {
                return ValorPago - Impostos - ValorAdiantamento - TotalDescontos;
            }
        }

        public string CPFCNPJTerceiroFormatado
        {
            get
            {
                if (this.TipoTerceiro == "E")
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    return this.TipoTerceiro == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJTerceiro) : string.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJTerceiro);
                }
            }
        }

        public string CPFCNPJFilialEmissoraFormatado
        {
            get { return string.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJFilialEmissora); }
        }

        public string DescricaoSituacaoContratoFrete
        {
            get { return SituacaoContratoFrete.ObterDescricao(); }
        }

        public string DataAprovacaoFormatada
        {
            get { return DataAprovacao != DateTime.MinValue ? DataAprovacao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataCargaFormatada
        {
            get { return DataCarga != DateTime.MinValue ? DataCarga.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string RegimeTributarioTerceiroFormatado
        {
            get { return RegimeTributarioTerceiro.ObterDescricao(); }
        }

        public string TipoPessoaTerceiroFormatado
        {
            get { return string.IsNullOrWhiteSpace(TipoPessoaTerceiro) ? string.Empty : TipoPessoaTerceiro.Equals("F") ? "Física" : TipoPessoaTerceiro.Equals("J") ? "Jurídica" : "Exterior"; }
        }

        public string DescricaoServico
        {
            get { return "Prestação Serviços motorista."; }
        }

        public string Verba
        {
            get { return "1"; }
        }

        public string CPFCNPJTomadorFormatado
        {
            get
            {
                string retorno = "";
                if (this.CPFCNPJTomador != string.Empty && this.CPFCNPJTomador != null)
                {
                    string[] cpfCnpjTomadores = this.CPFCNPJTomador.Split(',');

                    for (var i = 0; i < cpfCnpjTomadores.Length; i++)
                    {
                        string cpfCnpj = cpfCnpjTomadores[i].Trim();

                        if (i > 0)
                            retorno += ", ";

                        retorno += cpfCnpj.ObterCpfOuCnpjFormatado();
                    }
                }
                return retorno;
            }
        }

        public decimal ValorLiquidoSemAdiantamento
        {
            get { return this.ValorPago - this.TotalDescontos - this.ValorIRRF - this.ValorINSS - this.ValorSEST - this.ValorSENAT; }
        }

        public decimal ValorSubcontratacao
        {
            get { return this.ValorBruto - this.ValorPedagio; }
        }

        public string SituacaoCargaDescricao
        { 
            get { return SituacaoCarga.ObterDescricao(); } 
        }

        public string CodigoRetornoSUFormatado 
        {
            get
            {
                string retorno = "";
                if (this.CodigoRetornoSU != string.Empty && this.CodigoRetornoSU != null)
                {
                    string[] codigoRetornoSU = this.CodigoRetornoSU.Split('!');

                    if (codigoRetornoSU.Length > 0)
                    {
                        retorno = codigoRetornoSU[1];
                    }
                }
                return retorno;
            }
        }

        public string OperadoraDescricao
        {
            get { return Operadora.ObterDescricao(); }
        }
        #endregion
    }
}
