namespace Dominio.ObjetosDeValor.WebService.OrdemEmbarque
{
    public sealed class OrdemEmbarqueSituacaoRetorno
    {
        public string DataAtualizacao { get; set; }

        public string NumeroOrdemEmbarque { get; set; }

        public int ProtocoloIntegracaoCarga { get; set; }

        public string ProtocoloSituacao { get; set; }
    }
}
