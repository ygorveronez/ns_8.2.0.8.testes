namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    public  class SolicitacaoCancelamentoCarga
    {
        public int protocoloIntegracaoCarga { get; set; }
        public string motivoDoCancelamento { get; set; }
        public string usuarioERPSolicitouCancelamento { get; set; }
        public string ControleIntegracaoEmbarcador { get; set; }
    }
}
