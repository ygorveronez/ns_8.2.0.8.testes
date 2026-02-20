using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.INTDNE
{
    public  class INTDNE 
    {
        public string Interface { get; set; }
        public string Versao { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public string Ambiente { get; set; }
        public List<Parceiro> Parceiros { get; set; }
    }
}
