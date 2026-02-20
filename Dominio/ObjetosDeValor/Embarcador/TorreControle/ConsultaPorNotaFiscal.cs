using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class ConsultaPorNotaFiscal
    {
        #region Propriedades 
        
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public string Carga { get; set; }
        public string SituacaoNotaFiscal { get; set; }
        public string TipoOperacao { get; set; }
        public SituacaoAgendamentoEntregaPedido SituacaoAgendamento { get; set; }
        public string ObservacaoReagendamento { get; set; }
        public string Cliente { get; set; }
        public double ClienteCnpj { get; set; }
        public string Destino { get; set; }
        public string Transportador { get; set; }
        public string TransportadorCnpj { get; set; }
        public DateTime SugestaoDataEntrega { get; set; }
        public DateTime DataAgendamento { get; set; }
        public DateTime DataReagendamento { get; set; }
        public DateTime DataTerminoCarregamento { get; set; }
        public DateTime DataInicioEntrega { get; set; }
        public DateTime DataFimEntrega { get; set; }
        public string SituacaoViagem { get; set; }
        public string ContatoCliente { get; set; }
        public string ContatoTransportador { get; set; }
        public string Ocorrencia { get; set; }
        public int SituacaoEntrega { get; set; }
        public SituacaoNotaFiscal SituacaoEntregaNotaFiscal { get; set; }

        #endregion

        #region Propriedades com Regras

        public string SugestaoDataEntregaFormatada
        {
            get
            {
                return SugestaoDataEntrega != DateTime.MinValue ? SugestaoDataEntrega.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataAgendamentoFormatada
        {
            get
            {
                return DataAgendamento != DateTime.MinValue ? DataAgendamento.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataReagendamentoFormatada
        {
            get
            {
                return DataReagendamento != DateTime.MinValue ? DataReagendamento.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataTerminoCarregamentoFormatada
        {
            get
            {
                return DataTerminoCarregamento != DateTime.MinValue ? DataTerminoCarregamento.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataInicioEntregaFormatada
        {
            get
            {
                return DataInicioEntrega != DateTime.MinValue ? DataInicioEntrega.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataFimEntregaFormatada
        {
            get
            {
                return DataFimEntrega != DateTime.MinValue ? DataFimEntrega.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string SituacaoEntregaDescricao
        {
            get
            {
                switch (SituacaoEntrega)
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
            get
            {
                return SituacaoEntregaNotaFiscal.ObterDescricao();
            }
        }

        public string SituacaoAgendamentoDescricao
        {
            get
            {
                return SituacaoAgendamento.ObterDescricao();
            }
        }
        
        public string ClienteDescricao
        {
            get
            {
                return $"{Cliente} - {ClienteCnpj.ToString().ObterCnpjFormatado()}";
            }
        }

        public string TransportadorDescricao
        {
            get
            {
                return $"{Transportador} - {TransportadorCnpj.ObterCnpjFormatado()}";
            }
        }

        #endregion
    }
}
