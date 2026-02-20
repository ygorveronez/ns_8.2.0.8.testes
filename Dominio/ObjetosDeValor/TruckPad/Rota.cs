namespace Dominio.ObjetosDeValor.TruckPad
{
    public class Rota
    {
        public string source_city_ibge { get; set; }
        public string source_city { get; set; }
        public string source_state { get; set; }
        public string source_cep { get; set; }
        public string destination_city_ibge { get; set; }
        public string destination_city { get; set; }
        public string destination_state { get; set; }
        public string destination_cep { get; set; }
        public string load_nature { get; set; }
        public string load_weight { get; set; }
        public string travel_quantity { get; set; }
    }
}
