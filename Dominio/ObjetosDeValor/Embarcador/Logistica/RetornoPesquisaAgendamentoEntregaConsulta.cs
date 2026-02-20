using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class RetornoPesquisaAgendamentoEntregaConsulta
    {
        public int Codigo { get; set; }

        public int CodigoPedido { get; set; }

        public string TipoOperacao { get; set; }

        public string Destino { get; set; }

        public string UFDestino { get; set; }
        public string TipoCarga { get; set; }

        public string Destinatario { get; set; }

        public string CTe { get; set; }

        public string NotaFiscal { get; set; }

        public SituacaoAgendamentoEntregaPedido SituacaoAgendamento { get; set; }

        public DateTime DataAgendamento { get; set; }

        public string ObservacaoAgendamento { get; set; }

        public string ObservacaoReagendamento { get; set; }

        public int Volumes { get; set; }

        public decimal MetrosCubicos { get; set; }

        public bool ExigeAgendamento { get; set; }

        private DateTime DataPrevisaoEntrega { get; set; }

        public string DescricaoExigeAgendamento
        {
            get
            {
                return ExigeAgendamento ? "Sim" : "Não";
            }
        }

        public string SituacaoAgendamentoDescricao
        {
            get
            {
                return this.SituacaoAgendamento.ObterDescricao();
            }
        }

        public string DataAgendamentoFormatada
        {
            get
            {
                if (this.DataAgendamento == DateTime.MinValue)
                    return "";
                
                return this.DataAgendamento.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataPrevisaoEntregaFormatada
        {
            get
            {
                if (this.DataPrevisaoEntrega == DateTime.MinValue)
                    return "";

                return this.DataPrevisaoEntrega.ToString("dd/MM/yyyy HH:mm");
            }
        }
    }
}
