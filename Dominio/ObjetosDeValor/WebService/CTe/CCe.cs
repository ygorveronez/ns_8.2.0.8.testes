using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.CTe
{
    public class CCe
    {
        public string ChaveCTe { get; set; }
        public int NumeroSequencialEvento { get; set; }
        public string Protocolo { get; set; }
        public DateTime? DataEmissao { get; set; }
        public DateTime? DataRetornoSefaz { get; set; }
        public int CodigoIntegrador { get; set; }
        public string MensagemRetornoSefaz { get; set; }
        public int CodigoErroSefaz { get; set; }
        public string Log { get; set; }
        public Enumeradores.StatusCCe Status { get; set; }
        public List<CampoCCe> CampoCCe { get; set; }
    }
}
