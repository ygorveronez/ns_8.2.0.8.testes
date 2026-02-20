using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentoSaidaDeAlvo
    {

        public int CodigoSaidaAlvo { get; set; }
        public int CodigoMonitoramento { get; set; }
        public long CodigoUltimaPosicaoMonitoramento { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public bool NaoProcessarTrocaAlvoViaMonitoramentoTipoOperacao { get; set; }
        public long CodigoPosicao { get; set; }
        public DateTime DataVeiculoPosicao { get; set; }
        public double LatitudePosicao { get; set; }
        public double LongitudePosicao { get; set; }
        public int CodigoCargaEntrega { get; set; }
        public double CodigoCliente { get; set; }
        public string CodigosClientesEntregas { get; set; }
    }
}
