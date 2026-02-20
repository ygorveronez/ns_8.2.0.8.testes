namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class ModeloPneu
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public DimensaoPneu Dimensao { get; set; }
        public MarcaPneu Marca { get; set; }
    }
}
