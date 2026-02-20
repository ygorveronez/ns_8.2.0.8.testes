using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trixlog
{
    public class Positions
    {   
        public int moduloID { get; set; }
        public DateTime Date { get; set; }
        public DateTime ReceptionDate { get; set; }
        public int Speed { get; set; }
        public bool EngneOn { get; set; }
        public long Odometer { get; set; }
        public string equipmentGpsUnitId { get; set; }
        public Position Position { get; set; }
        
    }
}
