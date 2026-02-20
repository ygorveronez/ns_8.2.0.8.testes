namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost
{
    public class RequisicaoEventoRastreamento
    {
        public string JsonRequest { get; set; }
        public string JsonResponse { get; set; }
        public System.Net.HttpStatusCode HttpStatusCode { get; set; }
        public string Mensagem { get; set; }
        public ResponseEventoRastreamento ResponseEventoRastreamento { get; set; }
    }
}
