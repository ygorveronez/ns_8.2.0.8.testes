namespace Dominio.Relatorios.Embarcador.DataSource.Localidades
{
    public class Localidade
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public int Ibge { get; set; }
        public string Cep { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Regiao { get; set; }
    }
}
