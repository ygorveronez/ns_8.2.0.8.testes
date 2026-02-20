namespace Dominio.ObjetosDeValor
{
    public class CampoCCe
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string GrupoCampo { get; set; }
        public bool IndicadorRepeticao { get; set; }
        public string NomeCampo { get; set; }
        public int QuantidadeCaracteres { get; set; }
        public int QuantidadeDecimais { get; set; }
        public int QuantidadeInteiros { get; set; }
        public Dominio.Enumeradores.TipoCampoCCe TipoCampo { get; set; }
        public string Status { get; set; }
    }
}
