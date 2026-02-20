namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac
{
    public abstract class AbstractResponse
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public bool IsLastPage { get; set; }
    }
}
