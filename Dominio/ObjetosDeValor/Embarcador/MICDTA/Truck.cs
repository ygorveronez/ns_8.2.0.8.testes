using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.MICDTA
{
    public class Truck
    {
        public string placa { get; set; }
        public string tara { get; set; }
        public List<Lacre> lacres { get; set; }
    }
}
