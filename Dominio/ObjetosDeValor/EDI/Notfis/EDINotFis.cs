using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.Notfis
{
    public class EDINotFis
    {
        public string Emitente { get; set; }
        public string Destinatario { get; set; }
        public DateTime Data { get; set; }
        public DateTime SetHora
        {
            set
            {
                this.Data = new DateTime(this.Data.Year, this.Data.Month, this.Data.Day, value.Hour, value.Minute, value.Second);
            }
        }
        public string Intercambio { get; set; }
        public string Filler { get; set; }

        public List<NotaFiscal> NotasFiscais { get; set; }
        public CabecalhoDocumento CabecalhoDocumento { get; set; }
        public List<CabecalhoDocumento> CabecalhosDocumento { get; set; }

        public string NumeroCTeAnterior { get; set; }
        public string ChaveCTeAnterior { get; set; }
        public string SerieCTEAnterior { get; set; }
        public Participantes Participantes { get; set; }
    }
}
