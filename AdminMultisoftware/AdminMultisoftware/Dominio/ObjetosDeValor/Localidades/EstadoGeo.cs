namespace AdminMultisoftware.Dominio.ObjetosDeValor.Localidades
{
    public class EstadoGeo
    {
        public string cd_ibge { get; set; }   
        public string nm_uf { get; set;}
        public string sigla_uf { get; set;}
        public string nm_regiao { get; set;}
        public string area_km2 { get; set; }
        public string geom { get; set; }

        public int indice { get; set; }
        public string point { get; set; }
    }
}
