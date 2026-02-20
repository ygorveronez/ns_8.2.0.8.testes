using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaConsultaPorNotaFiscal
    {
        public DateTime? DataCarregamentoInicial { get; set; }
        public DateTime? DataCarregamentoFinal { get; set; }
        public DateTime? DataAgendamentoInicial { get; set; }
        public DateTime? DataAgendamentoFinal { get; set; }
        public SituacaoAgendamentoEntregaPedido? SituacaoAgendamento { get; set; }
        public string NumeroCarga { get; set; }
        public int TipoOperacao { get; set; }
        public double Cliente { get; set; }
        public int Transportador { get; set; }
        public DateTime? DataPrevisaoEntregaInicial { get; set; }
        public DateTime? DataPrevisaoEntregaFinal { get; set; }
        public int NumeroNota { get; set; }
    }
}
