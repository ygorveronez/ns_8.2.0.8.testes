using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga.BookingAVRO
{
    public class ProductList
    {
        public string productName { get; set; }
        public List<CharacteristicValue> characteristicValueList { get; set; }
    }
}
