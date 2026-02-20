using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaRelatorioAgendamentoEntrega
    {
        public string Carga { get; set; }

        public double CodigoCliente { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoTransportador { get; set; }

        public DateTime DataAgendamentoInicial { get; set; }

        public DateTime DataAgendamentoFinal { get; set; }

        public DateTime DataCarregamentoInicial { get; set; }

        public DateTime DataCarregamentoFinal { get; set; }

        public DateTime DataPrevisaoEntregaInicial { get; set; }

        public DateTime DataPrevisaoEntregaFinal { get; set; }

        public int NFe { get; set; }

        public SituacaoAgendamentoEntregaPedido? SituacaoAgendamento { get; set; }

        public DateTime DataCriacaoPedidoInicial { get; set; }

        public DateTime DataCriacaoPedidoFinal { get; set; }

        public bool? PossuiDataTerminoCarregamento { get; set; }

        public bool? PossuiDataSugestaoEntrega { get; set; }

        public DateTime DataInicialSugestaoEntrega { get; set; }

        public DateTime DataFinalSugestaoEntrega { get; set; }

        public bool ExibirCargasAgrupadas { get; set; }
    }
}
