using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.MDFe
{
    public class MDFeManual
    {
        public List<int> ProtocolosDasCargas { get; set; }
        public List<Dominio.ObjetosDeValor.MDFe.ValePedagio> ListaValePedagio { get; set; }
        public List<Dominio.ObjetosDeValor.MDFe.CIOT> ListaCIOT { get; set; }        
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista> Motoristas { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo Veiculo { get; set; }
    }
}
