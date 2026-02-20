namespace AdminMultisoftware.Dominio.ObjetosDeValor.Localidades
{
    public class Geo
    {
        public int cep { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public decimal latitude_nominatim { get; set; }
        public decimal longitude_nominatim { get; set; }
        public string display_name_nominatim { get; set; }
        public decimal distancia { get; set; }
        public string logradouro { get; set; }
        public string tipo_logradouro { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public int id { get; set; }
    }
}
