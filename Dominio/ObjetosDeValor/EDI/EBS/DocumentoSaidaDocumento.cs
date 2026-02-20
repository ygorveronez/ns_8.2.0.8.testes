using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class DocumentoSaidaDocumento
    {
        public DateTime DataLancamento { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public DateTime DataDocumento { get; set; }
        public string Modelo { get; set; }
        public string Serie { get; set; }
        public string SubSerie { get; set; }
        public int Natureza { get; set; }
        public int Variacao { get; set; }
        public int Classificacao1 { get; set; }
        public int Classificacao2 { get; set; }
        public string CPFCNPJDestinatario { get; set; }
        public decimal ValorContabil { get; set; }
        public decimal BasePIS { get; set; }
        public decimal BaseCOFINS { get; set; }
        public decimal BaseCSLL { get; set; }
        public decimal BaseIRPJ { get; set; }
        public decimal BaseICMSA { get; set; }
        public decimal AliquotaICMSA { get; set; }
        public decimal ValorICMSA { get; set; }
        public decimal BaseICMSB { get; set; }
        public decimal AliquotaICMSB { get; set; }
        public decimal ValorICMSB { get; set; }
        public decimal BaseICMSC { get; set; }
        public decimal AliquotaICMSC { get; set; }
        public decimal ValorICMSC { get; set; }
        public decimal BaseICMSD { get; set; }
        public decimal AliquotaICMSD { get; set; }
        public decimal ValorICMSD { get; set; }
        public decimal IsentasICMS { get; set; }
        public decimal OutrasICMS { get; set; }
        public decimal BaseIPI { get; set; }
        public decimal ValorIPI { get; set; }
        public decimal IsentasIPI { get; set; }
        public decimal OutrasIPI { get; set; }
        public decimal MercadoriasST { get; set; }
        public decimal BaseST { get; set; }
        public decimal ICMSST { get; set; }
        public decimal Diferidas { get; set; }
        public decimal BaseISS { get; set; }
        public decimal AliquotaISS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal IsentasISS { get; set; }
        public decimal IRRFRetido { get; set; }
        public string Observacoes { get; set; }
        public string Especie { get; set; }
        public string VendaAVista { get; set; }
        public int NaturezaOperacaoST { get; set; }
        public decimal BasePISCOFINSST { get; set; }
        public int ModalidadeFrete { get; set; }
        public decimal PISRetido { get; set; }
        public decimal COFINSRetido { get; set; }
        public decimal CSLLRetido { get; set; }
        public DateTime DataRecebimento { get; set; }
        public int OperacaoContabil { get; set; }
        public decimal Materiais { get; set; }
        public decimal SubEmpreitada { get; set; }
        public int CodigoServico { get; set; }
        public int CLIFOR { get; set; }
        public string IdentificacaoExterior { get; set; }
        public int Sequencia { get; set; }
        public int NumeroNotaInicial2 { get; set; }
        public int NumeroNotaFinal2 { get; set; }
        public string Observacoes2 { get; set; }
        public int CentroCusto { get; set; }
        public decimal BasePISCOFINSICMSST { get; set; }
        public DocumentoSaidaDocumentoDadoComplementar DadosComplementares { get; set; }
        public List<DocumentoSaidaDocumentoNotaFiscal> NotasFiscais { get; set; }
    }
}
