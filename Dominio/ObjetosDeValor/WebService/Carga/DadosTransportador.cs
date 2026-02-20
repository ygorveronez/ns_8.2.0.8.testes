using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class DadosTransportador
    {
        public int ProtocoloCarga { get; set; }

        public string CNPJtransportador { get; set; } // opcional 

        public string CodigoIntegracaoTransportador { get; set; }
    }
}
