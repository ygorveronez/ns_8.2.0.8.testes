using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentoCargaEntrega
    {
        public int Codigo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega Situacao { get; set; }
        public double CodigoCliente { get; set; }
        public bool Coleta { get; set; }
        public int OrdemPrevista { get; set; }
        public int OrdemRealizada { get; set; }
        public DateTime? DataConfirmacao { get; set; }
        public DateTime? DataPrevista { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataAgendamento { get; set; }
        public DateTime? DataReprogramada { get; set; }
        public DateTime? DataEntradaRaio { get; set; }
        public DateTime? DataSaidaRaio { get; set; }

    }
}
