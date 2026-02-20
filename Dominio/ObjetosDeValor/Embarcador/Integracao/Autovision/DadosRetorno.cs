namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Autovision
{
    public class DadosRetorno
    {
        public string ModuloID { get; set; }
        public Position Position { get; set; }
        public string Date { get; set; }
        public string ReceptionDate { get; set; }
        public int Speed { get; set; }
        public bool EngineOn { get; set; }
        public int Odometer { get; set; }
        public string EquipmentGpsUnitId { get; set; } //placa
    }
}
