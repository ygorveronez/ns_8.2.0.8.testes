using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao
{
    public class Item
    {
        public string sesNumber { get; set; }
        public decimal sesNetValue { get; set; }
        public string taxCode { get; set; }
        public string cfopInboundItem { get; set; }
        public decimal valorBaseIcms { get; set; }
        public decimal aliquotaIcms { get; set; }
        public decimal valorIcmsSt { get; set; }
        public List<Taxis> taxes { get; set; }
    }
}
