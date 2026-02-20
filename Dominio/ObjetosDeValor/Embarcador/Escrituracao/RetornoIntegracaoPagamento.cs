namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class RetornoIntegracaoPagamento
    {
        public int ProtolocoIntegracao { get; set; }

        public bool ProcessadoSucesso { get; set; }

        public string MensagemRetorno { get; set; }
    }
}
