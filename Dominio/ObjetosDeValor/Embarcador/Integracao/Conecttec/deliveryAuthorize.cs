using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec
{
    public class deliveryAuthorize
    {
        public string ReserveId { get; set; }
        public int HoseNumber { get; set; }
        public string LimitType { get; set; }
        public double LimitValue { get; set; }
        public double Price { get; set; }
        public int UpdateRate { get; set; }
        public int AuthTimeout { get; set; }
    }

    public class authorizeReturn
    {
        public string AuthId { get; set; }
        public string ReserveId { get; set; }
    }
}
