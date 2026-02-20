using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Camil
{
    public class Contabilizacao
    {
        public long ProtocoloIntegracao { get; set; }
        public string CodigoEstabelecimento { get; set; }
        public string CodigoFornecedor { get; set; }
        public string NaturezaOperacao { get; set; }
        public string NumeroDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public string DataEmissao { get; set; }
        public string DataVencimento { get; set; }
        public string CodigoChaveAcesso { get; set; }
        public TipoTributacaoICMS TipoTributacaoICMS { get; set; }
        public decimal? ValorICMSOutros { get; set; }
        public decimal? ValorDocumento { get; set; }
        public bool TituloIntermediario { get; set; }
        public string CodigoIBGEOrigem { get; set; }
        public string UFOrigem { get; set; }
        public string CodigoIBGEDestino { get; set; }
        public string UFDestino { get; set; }
        public string XMLCTE { get; set; }
        public string TipoOperacao { get; set; }
        public int CodigoOcorrencia { get; set; }
        public List<Imposto> Imposto { get; set; }
        public List<NotaFiscal> NotaFiscal { get; set; }
        public List<Documento> Documento { get; set; }
    }
}
