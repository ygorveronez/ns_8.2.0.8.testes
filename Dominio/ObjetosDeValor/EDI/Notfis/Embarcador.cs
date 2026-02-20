using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.Notfis
{
    public class Embarcador
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }

        public Expedidor Expedidor { get; set; }

        public List<Destinatario> Destinatarios { get; set; }
        public DateTime DataEmbarqueMercadoria { get; set; }

        public string NumeroDT { get; set; }

        public string Filler { get; set; }
        
        public virtual Embarcador Clonar()
        {
            return (Embarcador)this.MemberwiseClone();
        }
    }
}
