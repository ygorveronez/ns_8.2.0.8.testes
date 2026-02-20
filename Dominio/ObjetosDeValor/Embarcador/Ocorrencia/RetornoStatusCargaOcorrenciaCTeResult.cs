namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public class RetornoStatusCargaOcorrenciaCTeResult
    {
        public int ProtocoloIntegracao { get; set; }

        public bool ProcessadoSucesso { get; set; }

        public string MensagemRetorno { get; set; }
    }
}
