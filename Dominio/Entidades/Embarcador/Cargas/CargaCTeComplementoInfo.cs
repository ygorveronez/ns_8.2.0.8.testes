using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    /// <summary>
    /// Representa um conjunto de dados para a geração de um complemento de CTe ou CargaDocumentoParaEmissaoNFSManual.
    /// Antigamente ele complementava apenas o CTe, mas depois de 02/07/2021 ele pode também estar complementando uma CargaDocumentoParaEmissaoNFSManual,
    /// que depois processado se tornará outra CargaDocumentoParaEmissaoNFSManual (o complemento).
    /// </summary>
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CTE_COMPLEMENTO_INFO", DynamicUpdate = true, EntityName = "CargaCTeComplementoInfo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo", NameType = typeof(CargaCTeComplementoInfo))]
    public class CargaCTeComplementoInfo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO_COMPLEMENTADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTeComplementado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaDocumentoParaEmissaoNFSManual", Column = "NEM_CODIGO_COMPLEMENTADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual CargaDocumentoParaEmissaoNFSManualComplementado { get; set; }

        /// <summary>
        /// CTe que foi EMITIDO COMO COMPLEMENTO a partir do CargaCTeComplementado. 
        /// É null até o momento em que essa instância é processada na Thread ConsultaEmissoesPendentes.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        /// <summary>
        /// CargaDocumentoParaEmissaoNFSManual que foi EMITIDO COMO COMPLEMENTO a partir do CargaDocumentoParaEmissaoNFSManualComplementado. 
        /// É null até o momento em que essa instância é processada na Thread ConsultaEmissoesPendentes.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaDocumentoParaEmissaoNFSManual", Column = "NEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual CargaDocumentoParaEmissaoNFSManualGerado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreConhecimentoDeTransporteEletronico", Column = "PCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PreConhecimentoDeTransporteEletronico PreCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaNFS", Column = "CNS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaNFS CargaNFS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaComplementoFrete", Column = "CCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete CargaComplementoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete FechamentoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComplemento", Column = "CCC_VALOR_OCORRENCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorComplemento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTe", Column = "CCC_OBSERVACAO_CTE", TypeType = typeof(string), NotNull = true, Length = 2000)]
        public virtual string ObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirICMSFrete", Column = "CCC_INCLUIR_ICMS_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IncluirICMSFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComplementoIntegradoEmbarcador", Column = "CCC_COMPLEMENTO_INTEGRADO_EMBARCADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ComplementoIntegradoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ErroIntegracaoComGPA", Column = "CCC_ERRO_INTEGRACAO_COM_GPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ErroIntegracaoComGPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_MENSAGEM_PENDENCIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MensagemPendencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_INTEGRADO_COM_GPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegradoComGPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComplementoFilialEmissora", Column = "CCC_COMPLEMENTO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComplementoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorCTeGlobalizado", Column = "CCC_INDICADOR_CTE_GLOBALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicadorCTeGlobalizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProvisaoPelaNotaFiscal", Column = "CCC_PROVISAO_PELA_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProvisaoPelaNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "CCC_VALOR_ICMS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCofins", Column = "CCC_VALOR_COFINS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCofins { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPis", Column = "CCC_VALOR_PIS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_PERCENTUAL_CREDITO_PRESUMIDO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualCreditoPresumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_VALOR_CREDITO_PRESUMIDO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCreditoPresumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquota", Column = "CCC_PERCENTUAL_ALICOTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCofins", Column = "CCC_ALICOTA_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCofins { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPis", Column = "CCC_ALICOTA_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "CCC_BASE_CALCULO_ICMS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "CCC_CST", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoBC", Column = "CCC_PERCENTUAL_REDUCAO_BC", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualIncluirBaseCalculo", Column = "CCC_PERCENTUAL_INCLUIR_BASE_CALCULO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualIncluirBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISS", Column = "CCC_VALOR_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoISS", Column = "CCC_BASE_CALCULO_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquotaISS", Column = "CCC_PERCENTUAL_ALICOTA_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualRetencaoISS", Column = "CCC_PERCENTUAL_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirISSBaseCalculo", Column = "CCC_INCLUIR_ISS_BASE_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirISSBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRetencaoISS", Column = "CCC_VALOR_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReterIR", Column = "CCC_RETER_IR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReterIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIR", Column = "CCC_ALIQUOTA_IR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIR", Column = "CCC_BASE_CALCULO_IR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIR", Column = "CCC_VALOR_IR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIR { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TOMADOR_PAGADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente TomadorPagador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTomador", Column = "CCC_TIPO_TOMADOR", TypeType = typeof(Enumeradores.TipoTomador), NotNull = false)]
        public virtual Enumeradores.TipoTomador TipoTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "CCC_TIPO_PAGAMENTO", TypeType = typeof(Enumeradores.TipoTomador), NotNull = false)]
        public virtual Enumeradores.TipoPagamento TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServico", Column = "CCC_TIPO_SERVICO", TypeType = typeof(Enumeradores.TipoTomador), NotNull = false)]
        public virtual Enumeradores.TipoServico TipoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCTE", Column = "CCC_TIPO_CTE", TypeType = typeof(Enumeradores.TipoTomador), NotNull = false)]
        public virtual Enumeradores.TipoCTE TipoCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_ALIQUOTA_PIS_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPISCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_VALOR_PIS_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPISCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_ICMS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoICMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_PIS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoPIS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_COFINS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_ESCRITURACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_VALOR_MAXIMO_CENTRO_CONTABILIZACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMaximoCentroContabilizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ItemServico", Column = "CCC_ITEM_SERVICO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string ItemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_REEMITIR_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReemitirCTe { get; set; }

        [Obsolete("O campo não deve mais ser utilizado")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_NUMERO_TENTATIVA_REEMITIR_CTE", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativaReemitirCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraICMS", Column = "RIC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.ICMS.RegraICMS RegraICMS { get; set; }

        #region Imposto IBS/CBS

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OutrasAliquotas", Column = "TOA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas OutrasAliquotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_NBS", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string NBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_CODIGO_INDICADOR_OPERACAO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string CodigoIndicadorOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBS", Column = "CCC_CST_IBSCBS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBS", Column = "CCC_CLASSIFICACAO_TRIBUTARIA_IBSCBS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBS", Column = "CCC_BASE_CALCULO_IBSCBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadual", Column = "CCC_ALIQUOTA_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadual", Column = "CCC_PERCENTUAL_REDUCAO_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadual", Column = "CCC_VALOR_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipal", Column = "CCC_ALIQUOTA_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipal", Column = "CCC_PERCENTUAL_REDUCAO_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipal", Column = "CCC_VALOR_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBS", Column = "CCC_ALIQUOTA_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBS", Column = "CCC_PERCENTUAL_REDUCAO_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBS", Column = "CCC_VALOR_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBS { get; set; }

        #endregion Imposto CBS/IBS

        #region Propriedades com Regras

        public virtual void SetarRegraICMS(int codigoRegraICMS)
        {
            if (codigoRegraICMS > 0)
                RegraICMS = new Dominio.Entidades.Embarcador.ICMS.RegraICMS() { Codigo = codigoRegraICMS };
            else
                RegraICMS = null;
        }

        public virtual void SetarRegraOutraAliquota(int codigoOutraAliquota)
        {
            OutrasAliquotas = codigoOutraAliquota > 0 ? new Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas() { Codigo = codigoOutraAliquota } : null;
        }

        public virtual string Descricao
        {
            get { return CTe?.Descricao ?? string.Empty; }
        }

        /// <summary>
        /// Valor a receber do complementado, seja ele um CargaCTeComplementado ou um CargaDocumentoParaEmissaoNFSManualComplementado
        /// </summary>
        public virtual decimal ValorAReceberComplementado
        {
            get
            {
                if (CargaCTeComplementado != null)
                {
                    return CargaCTeComplementado.CTe?.ValorAReceber ?? 0;
                }

                if (CargaDocumentoParaEmissaoNFSManualComplementado != null)
                {
                    return CargaDocumentoParaEmissaoNFSManualComplementado.CTe?.ValorAReceber ?? 0;
                }

                return 0;
            }
        }

        /// <summary>
        /// CTe do complementado, seja ele um CargaCTeComplementado ou um CargaDocumentoParaEmissaoNFSManualComplementado
        /// </summary>
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTeComplementado
        {
            get
            {
                if (CargaCTeComplementado != null)
                {
                    return CargaCTeComplementado.CTe;
                }

                if (CargaDocumentoParaEmissaoNFSManualComplementado != null)
                {
                    return CargaDocumentoParaEmissaoNFSManualComplementado.CargaCTe?.CTe;
                }

                return null;
            }
        }

        /// <summary>
        /// Carga do complementado, seja ele um CargaCTeComplementado ou um CargaDocumentoParaEmissaoNFSManualComplementado
        /// </summary>
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaComplementado
        {
            get
            {
                if (CargaCTeComplementado != null)
                {
                    return CargaCTeComplementado.Carga;
                }

                if (CargaDocumentoParaEmissaoNFSManualComplementado != null)
                {
                    return CargaDocumentoParaEmissaoNFSManualComplementado.Carga;
                }

                return null;
            }
        }

        public virtual bool Equals(CargaCTeComplementoInfo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        #endregion
    }
}
