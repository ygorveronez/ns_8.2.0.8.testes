namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos
{
    public class Analise
    {
        public int Codigo { get; set; }
        public string DataCriacao { get; set; }
        public string DataRetorno { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Analista Analista { get; set; }
        public string Observacao { get; set; }
    }
}
