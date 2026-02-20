using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento
{
    public class OcorrenciaCancelamento
    {
        public int ProtocoloCancelamento { get; set; }
        public int ProtocoloOcorrecia { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public string MotivoCancelamento { get; set; }
        public bool PossuiDocumentoCancelado { get; set; }

        public List<Dominio.ObjetosDeValor.WebService.CTe.CTe> Conhecimentos { get; set; }
    }
}
