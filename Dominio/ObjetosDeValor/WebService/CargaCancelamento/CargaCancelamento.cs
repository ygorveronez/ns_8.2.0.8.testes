using System;

namespace Dominio.ObjetosDeValor.WebService.CargaCancelamento
{
    public class CargaCancelamento
    {
        public int ProtocoloCancelamento { get; set; }
        public int ProtocoloCarga { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public string MotivoCancelamento { get; set; }
        public bool PossuiDocumentoCancelado { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCargaDocumento TipoCancelamentoCargaDocumento { get; set; }
    }
}
