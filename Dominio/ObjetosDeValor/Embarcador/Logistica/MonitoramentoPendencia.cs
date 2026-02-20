using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentoPendencia
    {
        public DateTime Data { get; set; }
        public int Monitoramento { get; set; }
        public long? PosicaoAtual { get; set; }
        public long? PosicaoAnterior { get; set; }
    }
}
