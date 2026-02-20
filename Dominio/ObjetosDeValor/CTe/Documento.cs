namespace Dominio.ObjetosDeValor.CTe
{
    public class Documento
    {
        public Dominio.Enumeradores.TipoDocumentoCTe Tipo { get; set; }

        public int protocoloNFe { get; set; }

        public string Numero { get; set; }

        public string NumeroReferenciaEDI { get; set; }
        public string NumeroControleCliente { get; set; }

        public string Serie { get; set; }

        public decimal Valor { get; set; }

        public string ModeloDocumentoFiscal { get; set; }

        public string DataEmissao { get; set; }

        public int Volume { get; set; }
        
        public decimal Peso { get; set; }

        public decimal BaseCalculoICMS { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal BaseCalculoICMSST { get; set; }

        public decimal ValorICMSST { get; set; }

        public decimal ValorProdutos { get; set; }

        public string CFOP { get; set; }

        public string ItemPrincipal { get; set; }

        public string ChaveNFE { get; set; }

        public string ProtocoloAutorizacao { get; set; }

        public string PINSuframa { get; set; }

        public string Descricao { get; set; }

        public string NumeroRomaneio { get; set; }

        public string NumeroPedido { get; set; }

        public string Modalidade { get; set; }

        public string NCMPredominante { get; set; }

        public string NomeDestinatario { get; set; }
        public string IEDestinatario { get; set; }
        public string IERemetente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe? ClassificacaoNFe { get; set; }
    }
}
