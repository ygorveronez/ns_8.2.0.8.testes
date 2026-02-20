namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class HttpRequisicaoResposta
    {
        public string conteudoRequisicao { get; set; }
        public string extensaoRequisicao { get; set; }
        public string conteudoResposta { get; set; }
        public string extensaoResposta { get; set; }
        public System.Net.HttpStatusCode httpStatusCode { get; set; }
        public string mensagem { get; set; }
        public bool sucesso { get; set; }
    }
}
