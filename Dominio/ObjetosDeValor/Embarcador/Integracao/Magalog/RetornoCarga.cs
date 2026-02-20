using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog
{
    public class RetornoCarga
    {
        public Protocolos protocol { get; set; }

        public long idMessage { get; set; }

        public DateTime createAt { get; set; }

        public string message { get; set; }
    }
}
