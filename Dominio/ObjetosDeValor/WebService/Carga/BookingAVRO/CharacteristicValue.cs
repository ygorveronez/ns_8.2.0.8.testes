using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga.BookingAVRO
{
    public class CharacteristicValue
    {
        public string characteristicName { get; set; }
        public string childName { get; set; }
        public List<AlternateCodes> alternateCodes { get; set; }
    }
}
