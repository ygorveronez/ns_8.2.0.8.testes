using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentosTrocaAlvoSimplificado
    {
        public int CodigoTrocaAlvo { get; set; }
        public DateTime DataTrocaAlvo { get; set; }
        public int CodigoMonitoramento { get; set; }
        public DateTime? DataCriacaoMonitoramento { get; set; }
        public DateTime? DataInicioMonitoramento { get; set; }
        public DateTime? DataFimMonitoramento { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public bool RealizarBaixaEntradaNoRaio { get; set; }
        public bool NaoProcessarTrocaAlvoViaMonitoramentoTipoOperacao { get; set; }
        public string CodigosClientesEntregas { get; set; }
    }
}
