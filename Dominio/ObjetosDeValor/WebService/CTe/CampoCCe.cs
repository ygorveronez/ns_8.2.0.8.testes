using Dominio.Enumeradores;

namespace Dominio.ObjetosDeValor.WebService.CTe
{
    public class CampoCCe
    {
        public string ValorAlterado { get; set; }
        public int NumeroItemAlterado { get; set; }
        public string Descricao { get; set; }
        public string NomeCampo { get; set; }
        public string GrupoCampo { get; set; }
        public bool IndicadorRepeticao { get; set; }
        public Enumeradores.TipoCampoCCe TipoCampo { get; set; }
        public int QuantidadeInteiros { get; set; }
        public int QuantidadeDecimais { get; set; }
        public int QuantidadeCaracteres { get; set; }
        public TipoCampoCCeAutomatico TipoCampoCCeAutomatico { get; set; }
    }
}
