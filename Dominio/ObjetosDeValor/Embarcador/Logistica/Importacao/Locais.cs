namespace Dominio.ObjetosDeValor.Embarcador.Logistica.Importacao
{
    public class Locais
    {
        public string Descricao { get; set; }
        public Enumeradores.TipoLocal Tipo { get; set; }
        public Enumeradores.TipoArea TipoArea { get; set; }
        public string Observacao { get; set; }
        public string Municipio { get; set; }
        public int CodigoIBGE { get; set; }
        public string UF { get; set; }
        public string CNPJFilial { get; set; }
        public int Indice { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
