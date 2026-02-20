using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.UVTRN
{
    public class UVTRN
    {
        public List<Dominio.ObjetosDeValor.EDI.UVTRN.MDFe> MDFes { get; set; }

        public string InscricaoEstadual { get; set; }
        public string Estado { get; set; }
        public string Placa { get; set; }
        public string CPFMotorista { get; set; }
        public DateTime DataHoraLancamento { get; set; }
        public DateTime DataHoraBatch { get; set; }
    }
}
