using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Linq;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class MonitoramentoTempoVeiculo
    {
        #region Propriedades
        public int Codigo { get; set; }
        public string NumeroPedidoProvisorio { get; set; }
        public string NumeroCarga { get; set; }
        public string Filial { get; set; }
        public string TipoOperacao { get; set; }
        public string Natureza { get; set; }
        public string TipoCarga { get; set; }
        public string Pedido { get; set; }
        public string NumeroEXP { get; set; }
        public string Cliente { get; set; }
        public string UFDestino { get; set; }
        public string PaisDestino { get; set; }
        public string DataColeta { get; set; }
        public string Transportador { get; set; }
        public string Veiculo { get; set; }
        public string Reboque { get; set; }
        public string ModeloReboque { get; set; }
        public string ModeloVeiculo { get; set; }
        private DateTime DataPrevisaoChegadaOrigem { get; set; }
        public string DataInicioEntregaReprogramada { get; set; }
        public string DataInicioEntrega { get; set; }
        public string DataEntrega { get; set; }
        public string StatusViagem { get; set; }
        public string DataInicioViagem { get; set; }
        public string DataPrevisaoEntrega { get; set; }
        public string DataEntregaRecalculada { get; set; }
        public string DataEntradaRaio { get; set; }
        public string DataSaidaRaio { get; set; }
        private string SituacaoEntrega { get; set; }
        public decimal PercentualViagem { get; set; }
        public decimal Distancia { get; set; }
        public decimal KMRestante { get; set; }
        public decimal PesoTotal { get; set; }
        public string NumeroNotas { get; set; }
        public decimal ValorNotas { get; set; }
        public decimal ValorFreteAPagar { get; set; }
        public string DataDeadLCargaNavioViagem { get; set; }
        public string DataDeadLineNavioViagem { get; set; }
        public decimal TaxaOcupacaoCarregado { get; set; }
        public string TabelaFrete { get; set; }
        public int TempoDeslocamentoParaPlanta { get; set; }
        public int TempoAguardandoHorarioCarregamento { get; set; }
        public int TempoAguardandoCarregamento { get; set; }
        public int TempoEmCarregamento { get; set; }
        public int TempoEmLiberacao { get; set; }
        public int TempoTransito { get; set; }
        public int TempoAguardandoHorarioDescarga { get; set; }
        public int TempoAguardandoDescarga { get; set; }
        public int TempoDescarga { get; set; }
        public double CapacidadeKGVeiculo { get; set; }
        public DateTime DataCriacaoCarga { get; set; }
        public DateTime DataSalvamentoDadosTransporte { get; set; }
        public DateTime DataConfirmacaoEnvioDocumentos { get; set; }
        public DateTime DataConfirmacaoValorFrete { get; set; }
        public DateTime DataInicioEmissaoDocumentos { get; set; }
        public DateTime DataFimEmissaoDocumentos { get; set; }
        public DateTime DataEnvioCTeOcorrencia { get; set; }
        public DateTime DataPrevisaoSaidaPedidoAjustada { get; set; }
        public DateTime DataPrevisaoEntregaPedidoAjustada { get; set; }
        public DateTime DataPrevisaoEntregaPedidoRecalculada { get; set; }
        public string CidadeOrigemPedido { get; set; }
        public string CidadeDestinoPedido { get; set; }
        public string ClienteOrigemPedido { get; set; }
        public string ClienteDestinoPedido { get; set; }
        public string GrupoPessoasTomador { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataCriacaoCargaFormatada
        {
            get
            {
                return DataCriacaoCarga != DateTime.MinValue ? DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }
        
        public string DataSalvamentoDadosTransporteFormatada
        {
            get
            {
                return DataSalvamentoDadosTransporte != DateTime.MinValue ? DataSalvamentoDadosTransporte.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }
        
        public string DataConfirmacaoEnvioDocumentosFormatada
        {
            get
            {
                return DataConfirmacaoEnvioDocumentos != DateTime.MinValue ? DataConfirmacaoEnvioDocumentos.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }
        
        public string DataConfirmacaoValorFreteFormatada
        {
            get
            {
                return DataConfirmacaoValorFrete != DateTime.MinValue ? DataConfirmacaoValorFrete.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }
        
        public string DataInicioEmissaoDocumentosFormatada
        {
            get
            {
                return DataInicioEmissaoDocumentos != DateTime.MinValue ? DataInicioEmissaoDocumentos.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataFimEmissaoDocumentosFormatada
        {
            get
            {
                return DataFimEmissaoDocumentos != DateTime.MinValue ? DataFimEmissaoDocumentos.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataEnvioCTeOcorrenciaFormatada
        {
            get
            {
                return DataEnvioCTeOcorrencia != DateTime.MinValue ? DataEnvioCTeOcorrencia.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }
        
        public string DataPrevisaoSaidaPedidoAjustadaFormatada
        {
            get
            {
                return DataPrevisaoSaidaPedidoAjustada != DateTime.MinValue ? DataPrevisaoSaidaPedidoAjustada.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }
        
        public string DataPrevisaoEntregaPedidoAjustadaFormatada
        {
            get
            {
                return DataPrevisaoEntregaPedidoAjustada != DateTime.MinValue ? DataPrevisaoEntregaPedidoAjustada.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }
        
        public string DataPrevisaoEntregaPedidoRecalculadaFormatada
        {
            get
            {
                return DataPrevisaoEntregaPedidoRecalculada != DateTime.MinValue ? DataPrevisaoEntregaPedidoRecalculada.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataPrevisaoChegadaOrigemFormatada
        {
            get { return DataPrevisaoChegadaOrigem != DateTime.MinValue ? DataPrevisaoChegadaOrigem.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string SituacaoEntregaFormatada
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SituacaoEntrega))
                    return SituacaoEntrega;

                string[] listaSituacoes = SituacaoEntrega.Split(',');

                return string.Join(", ", (from situacao in listaSituacoes select SituacaoEntregaHelper.ObterDescricao((SituacaoEntrega)situacao.ToInt())));
            }
        }

        #endregion
    }
}
