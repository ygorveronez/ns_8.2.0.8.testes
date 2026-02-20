using System;

namespace Dominio.ObjetosDeValor.Embarcador.NFS
{
    public class FiltroPesquisaExistenciaDocumentoNFSManual
    {
        public int Numero { get; set; }
        public int CodigoXMLNotaFiscal { get; set; }
        public int CodigoCTe { get; set; }
        public string Chave { get; set; }
        public int CodigoModeloDocumentoFiscal { get; set; }
        public int CodigoPedidoCTeParaSubContratacao { get; set; }
        public int CodigoDocumentoNFSe { get; set; }
        public int CodigoCargaOcorrencia { get; set; }
        public decimal ValorFrete { get; set; }
        public DateTime DataEmissao { get; set; }
        public string Serie { get; set; }
        public int CodigoCarga { get; set; }
    }
}
