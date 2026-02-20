using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaRelatorioCargaEntregaPedido
    {
        public bool? CargaAgendada { get; set; }

        public List<int> CodigosFilial { get; set; }

        public List<double> CodigosRecebedor { get; set; }

        public List<int> CodigosTransportadora { get; set; }

        public DateTime DataFinalCriacao { get; set; }

        public DateTime DataFinalEntrega { get; set; }

        public DateTime DataInicialCriacao { get; set; }

        public DateTime DataInicialEntrega { get; set; }

        public OpcaoSimNaoPesquisa PossuiRedespacho { get; set; }

        public List<SituacaoCarga> SituacaoCargas { get; set; }

        public string TipoDestino { get; set; }
        public DateTime PrevisaoEntregaPlanejadaInicio { get; set; }

        public DateTime PrevisaoEntregaPlanejadaFinal { get; set; }
        public List<MonitoramentoStatus> StatusMonitoramento { get; set; }
    }
}
