namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Camil
{
    public class CancelamentoContabilizacao
    {
        public long ProtocoloIntegracao { get; set; }
        public string CodigoEstabelecimento { get; set; }
        public string CodigoFornecedor { get; set; }
        public string NaturezaOperacao { get; set; }
        public string NumeroDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public bool LimpaProtocolo { get; set; }
        public string CodigoTipoOperacao { get; set; }
        public string CodigoCancelOcorrencia { get; set; }
    }
}
