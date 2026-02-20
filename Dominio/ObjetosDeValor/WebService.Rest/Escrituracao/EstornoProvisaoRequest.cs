namespace Dominio.ObjetosDeValor.WebService.Rest.Escrituracao
{
    public class EstornoProvisaoRequest
    {
        public int ProtocoloIntegracao { get; set; }

        public bool ProcessadoSucesso { get; set; }

        public string MensagemRetorno { get; set; }
    }
}
