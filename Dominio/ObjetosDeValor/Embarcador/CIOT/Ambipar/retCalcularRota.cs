using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public class retCalcularRota
    {
        public int status { get; set; }
        public retCalcularRotaData data { get; set; }
    }

    public class retCalcularRotaData
    {
        public retCalcularRotaDataTest fastest { get; set; }
        public retCalcularRotaDataTest shortest { get; set; }
    }

    public class retCalcularRotaDataTest
    {
        public int totalDistance { get; set; }
        public int totalNominalDuration { get; set; }
        public decimal averageSpeed { get; set; }
        public int totalSpeedProfilePenalty { get; set; }
        public int totalTolls { get; set; }
        public decimal tollCosts { get; set; }
        public int tripId { get; set; }
        public decimal totalTollsValue { get; set; }
    }
}