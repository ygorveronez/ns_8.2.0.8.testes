using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class NotaEntradaNotaFiscal
    {
        public string TipoRegistro { get; set; }
        public DateTime DataLancamento { get; set; }
        public string NumeroNota { get; set; }
        public DateTime DataDocumento { get; set; }
        public string Modelo { get; set; }
        public string Serie { get; set; }
        public string SubSerie { get; set; }
        public string Natureza { get; set; }
        public string Variacao { get; set; }
        public string Classificacao1 { get; set; }
        public string Classificacao2 { get; set; }
        public string CNPJCPFEmitente { get; set; }
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
        public decimal IsentaICMS { get; set; }
        public decimal OutraICMS { get; set; }
        public decimal BaseIPI { get; set; }
        public decimal ValorIPI { get; set; }
        public decimal IsentaIPI { get; set; }
        public decimal OutraIPI { get; set; }
        public decimal MercadoriaST { get; set; }
        public decimal BaseST { get; set; }
        public decimal ICMSST { get; set; }
        public decimal Diferidas { get; set; }
        public string Observacao { get; set; }
        public string Especie { get; set; }
        public string VendaAVista { get; set; }
        public string NaturezaOperacaoST { get; set; }
        public decimal BasePISCOFINSST { get; set; }
        public decimal BaseISS { get; set; }
        public decimal AliquotaISS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal IsentaISS { get; set; }
        public decimal IRRFRetido { get; set; }
        public decimal PISRetido { get; set; }
        public decimal COFINSRetido { get; set; }
        public decimal CSLLRetido { get; set; }
        public string DataPagamento { get; set; }
        public string OperacaoContabil { get; set; }
        public string CliFor { get; set; }
        public string IdentificacaoExterior { get; set; }
        public decimal INSSRetido { get; set; }
        public decimal FunRuralRetido { get; set; }
        public string CodigoServico { get; set; }
        public string ISSRetido { get; set; }
        public string ISSDevidoPrestacao { get; set; }
        public string UFPrestacao { get; set; }
        public string MunicipioPrestacao { get; set; }
        public string TipoEmissao { get; set; }
        public string ModalidadeFrete { get; set; }
        public string Brancos { get; set; }
        public string UseEBS { get; set; }
        public string Sequencia { get; set; }
        public string NumeroNota2 { get; set; }
        public string Observacao2 { get; set; }
        public string CentroCusto { get; set; }
        public decimal BasePISCOFINSICMSST { get; set; }
        public string DataEmissaoRPS { get; set; }
        public decimal ICMSRelativoFCP { get; set; }
        public decimal ICMSUFDestino { get; set; }
        public decimal ICMSUFOrigem { get; set; }
        public decimal BaseICMSE { get; set; }
        public decimal AliquotaICMSE { get; set; }
        public decimal ValorICMSE { get; set; }
        public decimal BaseICMSF { get; set; }
        public decimal AliquotaICMSF { get; set; }
        public decimal ValorICMSF { get; set; }
        public List<NotaEntradaItem> Itens { get; set; }
        public NotaEntradaDadoComplementar DadosComplementares { get; set; }
        public List<NotaEntradaParcela> Parcelas { get; set; }
    }
}
