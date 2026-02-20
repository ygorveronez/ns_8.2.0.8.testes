namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class RetornoModeloVeicular
    {
        public bool success { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public RetornoModeloVeicularLoadDetailLayout loadDetailLayout { get; set; }
    }

    public class RetornoModeloVeicularLoadDetailLayout
    {
        public string _id { get; set; }
    }
}
