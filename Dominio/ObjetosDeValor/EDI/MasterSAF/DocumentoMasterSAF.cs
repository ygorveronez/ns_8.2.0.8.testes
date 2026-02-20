using System;

namespace Dominio.ObjetosDeValor.EDI.MasterSAF
{
    public class DocumentoMasterSAF
    {
        public decimal ValorZerado { get; set; }
        public string CodigoEmpresa { get; set; }
        //public string CodigoEstabelecimento { get; set; }
        public string NormalDevolucao { get; set; }
        public string TipoDocumento { get; set; }
        public string IndicadorFiscaJuridica { get; set; }
        public string CodigoDestinatarioEmitenteRemetente { get; set; }
        public string NumeroDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public string SubSerieDocumento { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataSaidaRecebimento { get; set; }
        public string ClassificacaoFiscal { get; set; }
        public string ModeloDocumento { get; set; }
        public string CFOP { get; set; }
        public string CFOPEntrada { get; set; }
        public string NaturezaOperacao { get; set; }
        public string SerieDocumentoReferencia { get; set; }
        public string SubSerieDocumentoReferencia { get; set; }
        public string NumeroDeclaracaoImportada { get; set; }
        public DateTime DataRecebimento { get; set; }
        public string InscricaoEstaudalST { get; set; }
        public decimal ValorProdutosServico { get; set; }
        public decimal ValorTotalDocumentoFiscal { get; set; }
        public decimal ValorDoFrete { get; set; }
        public decimal ValorDoSeguro { get; set; }
        public decimal ValorOutrasDespesas { get; set; }
        public decimal ValorDiferencaAliquotasFrete { get; set; }
        public decimal ValorDescontos { get; set; }
        public string Contribuinte { get; set; }
        public string SituacaoNota { get; set; }
        public string IndiceConversao { get; set; }
        public decimal ValorDocumentoIndice { get; set; }
        public string ContaContabil { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal DiferencaAliquotaICMS { get; set; }
        public string ObservacaoICMS { get; set; }
        public string CodigoApuracaoICMS { get; set; }
        public decimal AliquotaIPI { get; set; }
        public decimal ValorIPI { get; set; }
        public decimal ObservacaoIPI { get; set; }
        public decimal CodigoApuracaoIPI { get; set; }
        public decimal AliquotaIR { get; set; }
        public decimal ValorIR { get; set; }

        public decimal AliquotaICMSST { get; set; }
        public decimal ValorICMSST { get; set; }
        public decimal ObservacaoICMSST { get; set; }
        public decimal CodigoApuracaoICMSST { get; set; }
        public decimal BaseICMSTributada { get; set; }
        public decimal BaseICMSIsenta { get; set; }
        public decimal BaseICMSOutras { get; set; }
        public decimal BaseReducaoICMS { get; set; }

        public decimal BaseIPITributada { get; set; }
        public decimal BaseIPIIsenta { get; set; }
        public decimal BaseIPIOutras { get; set; }
        public decimal BaseReducaoIPI { get; set; }

        public decimal BaseIRTributada { get; set; }
        public decimal BaseIRsenta { get; set; }
        public decimal BaseIROutras { get; set; }
        public decimal BaseReducaoIR { get; set; }

        public decimal AliquotaISS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal BaseISSTributada { get; set; }
        public decimal BaseISSIsenta { get; set; }
        public decimal BaseISSRealizadosPorTerceiros { get; set; }
        public decimal ValorAliquotaISS { get; set; }

        public decimal AliquotaISSRetido { get; set; }
        public decimal ValorISSRetido { get; set; }
        public decimal ValorBaseISSRetido { get; set; }


        public decimal ValorDeducaoISS { get; set; }

        public decimal BaseICMSST { get; set; }
        public string ModeloCupomFiscal { get; set; }
        public decimal ValorTotalServicos { get; set; }
        public decimal IndicadorTipoDeFrete { get; set; }
        public string CodigoMunicipioISS { get; set; }
        public string CodigoMunicipioISSSimplificado { get; set; }
        public string NotaFiscalCreditoDebito { get; set; }
        public decimal ValorServicoTransporte { get; set; }
        public string CodigoRegiao { get; set; }

        public DateTime DataAutorizacaoNF { get; set; }

        public decimal ValorBaseINSS { get; set; }

        public decimal ValorINSSRetido { get; set; }

        public DateTime? DataCancelamento { get; set; }
        public string TipoFaturamento { get; set; }

        public string TipoFrete { get; set; }
        public string TipoFreteNumerico { get; set; }

        public decimal BasePIS { get; set; }
        public decimal ValorPIS { get; set; }

        public decimal BaseCOFINS { get; set; }
        public decimal ValorCOFINS { get; set; }

        public decimal BaseICMSOrigemDestino { get; set; }
        public decimal ValorICMSOrigemDestino { get; set; }
        public decimal AliquotaICMSOrigemDestino { get; set; }

        public decimal PercentualReducaoBaseCalculoICMS { get; set; }
        public string IndicadorMedidaJudicial { get; set; }

        public string UFOrigem { get; set; }
        public string TipoCompraVenda { get; set; }
        public string UFDestino { get; set; }

        public string InscricaoEstadual { get; set; }
        public string IndicadorTipoNotaFiscalServico { get; set; }

        public string CodigoMotivoCancelamentoNF { get; set; }
        public string MotivoCancelamentoNF { get; set; }
        public string IndicadorTipoTomador { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Expedidor { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Recebedor { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal NotaFiscal { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Remetente { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Tomador { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Empresa.Empresa Transportador { get; set; }

        public decimal aliquotaPIS { get; set; }
        public decimal aliquotaCOFINS { get; set; }


        public string CodigoVeficacaoDaNF { get; set; }

        public string TipoRPS { get; set; }

        public string NumeroRPS { get; set; }
        public string SerieRPS { get; set; }
        public string ChaveNFs { get; set; }
        public DateTime DataEmissaoRPS { get; set; }
        public string DiscriminacaoDosServicosNFS { get; set; }
        public string NumeroDigitoVerificadorNFs { get; set; }
        public string ModeloNFAtendMunicipal { get; set; }
        public string CodigoModeloNF { get; set; }
        public DateTime DataEmissaoNFS { get; set; }

        public string IdentificadorDocumentoFiscal { get; set; }

        public string CodigoSistemaOrigem { get; set; }

        //SAF09
        public decimal QuantidadeServico { get; set; }
        public decimal ValorUnitario { get; set; }

        public string CodigoEstabelecimento { get; set; }
        public string CodigoFilial { get; set; }
        public string CodigoSituacaoCOFINS { get; set; }
        public string CodigoSituacaoPIS { get; set; }
        public string IndicadorMunicipioISS { get; set; }
        public string IndicadorResponsavelRecolhimentoISS { get; set; }
        public string IndicadorTipoRetensao { get; set; }
        public int NumeroItem { get; set; }
    }
}
