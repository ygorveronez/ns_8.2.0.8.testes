namespace Dominio.ObjetosDeValor.WebService.Rest.Escrituracao
{
    public class LiberacaoPagamentoRequest
    {
        public int ProtocoloIntegracao { get; set; }

        public bool ProcessadoSucesso { get; set; }

        public string MensagemRetorno { get; set; }
    }
}
