using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class MDFe
    {
        public int NumeroProtocolo { get; set; }

        public string Chave { get; set; }

        public int Numero { get; set; }

        public int Serie { get; set; }

        public string UFOrigem { get; set; }

        public string UFDestino { get; set; }

        public List<int> ProtocolosDeCTe { get; set; }

    }
}
