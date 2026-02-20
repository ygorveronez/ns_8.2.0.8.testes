using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.TorreControle
{
    public class RelatorioConsultaPorNotaFiscal
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int Numero { get; set; }
        public string Carga { get; set; }
        public string SituacaoNotaFiscal { get; set; }
        public string TipoOperacao { get; set; }
        private SituacaoAgendamentoEntregaPedido SituacaoAgendamento { get; set; }
        public string ObservacaoReagendamento { get; set; }
        private string Cliente { get; set; }
        private double ClienteCnpj { get; set; }
        public string Destino { get; set; }
        private string Transportador { get; set; }
        private string TransportadorCnpj { get; set; }
        private DateTime SugestaoDataEntrega { get; set; }
        private DateTime DataAgendamento { get; set; }
        private DateTime DataReagendamento { get; set; }
        private DateTime DataTerminoCarregamento { get; set; }
        private DateTime DataInicioEntrega { get; set; }
        private DateTime DataFimEntrega { get; set; }
        private DateTime DataPrevisaoEntrega { get; set; }
        public string SituacaoViagem { get; set; }
        public string ContatoCliente { get; set; }
        public string ContatoTransportador { get; set; }
        public string Ocorrencia { get; set; }
        public int SituacaoEntregaAgendamento { get; set; }
        public string ResponsavelAgenda { get; set; }
        public string Holding { get; set; }
        public string Categoria { get; set; }
        public string MotivoReagendamento { get; set; }
        private SituacaoNotaFiscal SituacaoEntregaNotaFiscal { get; set; }
        public int ISISReturn { get; set; }
        public string ResponsavelReagendamento { get; set; }
        public string ModeloVeicular { get; set; }
        public string ObservacaoMotivoReagendamento { get; set; }
        public string Trecho { get; set; }
        public string NomeFantasia { get; set; }
        public string CategoriaExpedidor { get; set; }
        public string CNPJFilialEmissora { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public string CidadeOrigemPedido { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string CidadeDestinatario { get; set; }
        public string ClienteEndereco { get; set; }
        public string PlacaVeiculo { get; set; }
        public string TransportadorCodigoIntegracao { get; set; }
        public string AnalistaResponsavelMonitoramento { get; set; }
        public int SituacaoEntregaPrevisao { get; set; }
        public DateTime DataEntregaPrevista { get; set; }
        public int OrdemPrevista { get; set; }
        public int SequenciaEntrega { get; set; }
        public decimal PesoCarga { get; set; }
        public string CNPJFilial { get; set; }
        public string NomeFilial { get; set; }
        public string RotaFreteDescricao { get; set; }
        public string RotaFreteCodigoIntegracao { get; set; }

        #endregion

        #region Propriedades com Regras

        public string SugestaoDataEntregaFormatada
        {
            get { return SugestaoDataEntrega != DateTime.MinValue ? SugestaoDataEntrega.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataAgendamentoFormatada
        {
            get { return DataAgendamento != DateTime.MinValue ? DataAgendamento.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataEntregaPrevistaFormatada
        {
            get { return DataEntregaPrevista != DateTime.MinValue ? DataEntregaPrevista.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataReagendamentoFormatada
        {
            get { return DataReagendamento != DateTime.MinValue ? DataReagendamento.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataTerminoCarregamentoFormatada
        {
            get { return DataTerminoCarregamento != DateTime.MinValue ? DataTerminoCarregamento.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataInicioEntregaFormatada
        {
            get { return DataInicioEntrega != DateTime.MinValue ? DataInicioEntrega.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataFimEntregaFormatada
        {
            get
            {
                if (this.SituacaoEntregaNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.Entregue 
                    && this.SituacaoEntregaNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.EntregueParcial
                    && this.SituacaoEntregaNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.DevolvidaParcial
                    && this.SituacaoViagem != "ENTREGUE")
                    return "";
                else
                    return DataFimEntrega != DateTime.MinValue ? DataFimEntrega.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataPrevisaoEntregaFormatada
        {
            get { return DataPrevisaoEntrega != DateTime.MinValue ? DataPrevisaoEntrega.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string SituacaoEntregaDescricao
        {
            //Situacao da entrega calculada com base na data de agendamento
            get
            {
                switch (SituacaoEntregaAgendamento)
                {
                    case 1:
                        return "On time";
                    case 2:
                        return "Not on time";
                    default:
                        return "";
                }
            }
        }

        public string SituacaoEntregaPrevisaoDescricao
        {
            //Situacao da entrega calculada com base na data de previsao de entrega
            get
            {
                switch (SituacaoEntregaPrevisao)
                {
                    case 1:
                        return "On time";
                    case 2:
                        return "Not on time";
                    default:
                        return "";
                }
            }
        }

        public string SituacaoEntregaNotaFiscalDescricao
        {
            get { return SituacaoEntregaNotaFiscal.ObterDescricao(); }
        }

        public string SituacaoAgendamentoDescricao
        {
            get { return SituacaoAgendamento.ObterDescricao(); }
        }

        public string ClienteDescricao
        {
            get { return $"{Cliente} - {ClienteCnpj.ToString().ObterCnpjFormatado()}"; }
        }

        public string TransportadorDescricao
        {
            get { return $"{Transportador} - {TransportadorCnpj.ObterCnpjFormatado()}"; }
        }


        public string CNPJFilialEmissoraFormatado
        {
            get { return this.CNPJFilialEmissora?.ToString().ObterCnpjFormatado() ?? ""; }
        }

        public string CNPJFilialFormatado
        {
            get { return this.CNPJFilial?.ToString().ObterCnpjFormatado() ?? ""; }
        }

        #endregion
    }
}
