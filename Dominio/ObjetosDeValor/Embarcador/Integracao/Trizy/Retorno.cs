namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Retorno
    {
        public bool success { get; set; }
        public int code { get; set; }
        public int requisicao_id { get; set; }
        public string message { get; set; }
        public string error { get; set; }
        public Travel travel { get; set; }
        
        public RetornoDetalhes result { get; set; }
    }
}
