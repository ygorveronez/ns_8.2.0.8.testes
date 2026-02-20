using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public class envCalcularRota
    {
        public int axis { get; set; }
        public List<envCalcularRotaPoint> points { get; set; }
    }

    public class envCalcularRotaPoint
    {
        public string cep { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public string siteId { get; set; }
        public int? liftedAxles { get; set; }
    }
}