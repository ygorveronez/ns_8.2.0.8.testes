using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class ContratadoRNTRC
    {
        public string CNPJCPFContratado { get; set; }
        public string RNTRCContratado { get; set; }
        public List<VeiculosRNTRC> veiculos { get; set; }
    }
}