using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class ConsultaEntregaAtrasada
    {
        public int Codigo { get; set; }
        public int CodigoEntrega { get; set; }
        public string Carga { get; set; }
        public string Cliente { get; set; }
        public string TipoOperacao { get; set; }
        public decimal Peso { get; set; }
        public DateTime DataAgendamento { get; set; }
        public DateTime DataConfirmacaoEntrega { get; set; }
        public string Transportador { get; set; }
        public int TipoResponsavel { get; set; }
        public string DescricaoResponsavel { get; set; }
        public string DataConfirmacaoEntregaFormatada
        {
            get
            {
                return DataConfirmacaoEntrega != DateTime.MinValue ? DataConfirmacaoEntrega.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }
        public string DataAgendamentoFormatada
        {
            get
            {
                return DataAgendamento != DateTime.MinValue ? DataAgendamento.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

    }
}
