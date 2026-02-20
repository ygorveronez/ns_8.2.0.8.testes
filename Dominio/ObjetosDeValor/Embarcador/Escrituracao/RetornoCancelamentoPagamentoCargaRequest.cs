namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class RetornoCancelamentoPagamentoCargaRequest
    {
        public int ProtocoloIntegracao { get; set; }

        public bool ProcessadoSucesso { get; set; }

        public string MensagemRetorno { get; set; }
    }
}
