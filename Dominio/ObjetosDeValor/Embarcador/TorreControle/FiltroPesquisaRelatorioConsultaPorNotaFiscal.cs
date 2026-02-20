using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaRelatorioConsultaPorNotaFiscal
    {
        public DateTime? DataCarregamentoInicial { get; set; }
        public DateTime? DataCarregamentoFinal { get; set; }
        public DateTime? DataAgendamentoInicial { get; set; }
        public DateTime? DataAgendamentoFinal { get; set; }
        public SituacaoAgendamentoEntregaPedido? SituacaoAgendamento { get; set; }
        public string NumeroCarga { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public double CnpjCpfCliente { get; set; }
        public int CodigoTransportador { get; set; }
        public DateTime? DataPrevisaoEntregaInicial { get; set; }
        public DateTime? DataPrevisaoEntregaFinal { get; set; }
        public int NumeroNota { get; set; }
        public int Filial { get; set; }
        public int TipoTrecho { get; set; }
        public double Expedidor { get; set; }
        public double Recebedor { get; set; }
    }
}
