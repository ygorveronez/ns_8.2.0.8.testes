namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class RetornoCancelamentoProvisaoRequest
    {
        public string NumeroFolha { get; set; }
        public string DataFolha { get; set; }
        public bool Cancelado { get; set; }
        public string MensagemRetornoEtapa { get; set; }
    }
}
