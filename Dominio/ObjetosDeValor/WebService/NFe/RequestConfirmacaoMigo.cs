namespace Dominio.ObjetosDeValor.WebService.NFe
{
    public class RequestConfirmacaoMigo
    {
        public string CodigoIdentificador { get; set; }
        public int ProtocoloIntegracaoCarga { get; set; }
        public int ProtocoloIntegracaoPedido { get; set; }
        public string ChaveNFe { get; set; }
        public string NumeroOcorrencia { get; set; }
        public string DataEnvio { get; set; }
    }
}
