using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class ChecklistGeofence
    {
        public string type { get; set; }
        public object coordinates { get; set; } // Pode ser List<double> ou List<List<double>>
        public Radius? radius { get; set; }
        public string? stopCondition { get; set; }
    }
}