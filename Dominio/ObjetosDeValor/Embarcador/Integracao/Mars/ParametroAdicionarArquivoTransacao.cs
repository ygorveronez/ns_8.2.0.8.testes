namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class ParametroAdicionarArquivoTransacao
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string JsonRequest {  get; set; }
        public string JsonResponse { get; set; }
        public Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao CargaCancelamentoCargaIntegracao { get; set; }
    }
}
