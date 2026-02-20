using System;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_PROVISAO", EntityName = "DocumentoProvisao", Name = "Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao", NameType = typeof(DocumentoProvisao))]
    public class DocumentoProvisao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>
    {
        public DocumentoProvisao() { }
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DPV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LancamentoNFSManual", Column = "LNM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual LancamentoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CTe.CTeTerceiro CTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoCTeParaSubContratacao", Column = "PSC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao PedidoCTeParaSubContratacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Stage", Column = "STA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Stage Stage { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImpostoValorAgregado", Column = "IVA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Contabeis.ImpostoValorAgregado ImpostoValorAgregado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "CON_MODELODOC", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumento", Column = "DPV_NUMERO_DOCUMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieDocumento", Column = "DPV_SERIE_DOCUMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int SerieDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete FechamentoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pagamento", Column = "PAG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.Pagamento Pagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoFaturamento", Column = "DFA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento DocumentoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Provisao", Column = "PRV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.Provisao Provisao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CancelamentoProvisao", Column = "CPV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao CancelamentoProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoBruto", Column = "DPV_PESO_BRUTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoBruto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MovimentoFinanceiroGerado", Column = "PRV_MOVIMENTO_FINANCEIRO_GERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MovimentoFinanceiroGerado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraEscrituracao", Column = "RES_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraEscrituracao RegraEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_PROVISAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_PROVISAO_COMPETENCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorProvisaoCompetencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_BASE_CALCULO_ICMS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_BASE_CALCULO_ICMS_COMPETENCIA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMSCompetencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_PERCENTUAL_ALICOTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_PERCENTUAL_ALICOTA_COMPETENCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaCompetencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_ICMS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_ICMS_COMPETENCIA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSCompetencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_CST", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_CST_COMPETENCIA", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CSTCompetencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_ISS_COMPETENCIA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorISSCompetencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_BASE_CALCULO_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_BASE_CALCULO_ISS_COMPETENCIA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoISSCompetencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_ICMS_INCLUSO_BC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ICMSInclusoBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_ICMS_INCLUSO_BC_COMPETENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ICMSInclusoBCCompetencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_ISS_INCLUSO_BC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ISSInclusoBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_ISS_INCLUSO_BC_COMPETENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ISSInclusoBCCompetencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_PERCENTUAL_ALICOTA_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_PERCENTUAL_ALICOTA_ISS_COMPETENCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaISSCompetencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_RETENCAO_ISS_COMPETENCIA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoISSCompetencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_AD_VALOREM", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdValorem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_DESCARGA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_GRIS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorGris { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_ENTREGA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_PERNOITE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPernoite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_CONTRATO_FRETE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_FRETE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_TIPO_VALOR_FRETE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoValorFreteDocumentoProvisao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoValorFreteDocumentoProvisao TipoValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFolha", Column = "DPV_NUMERO_FOLHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroFolha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_DATA_FOLHA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFolha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_CANCELADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Cancelado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }



        #region Imposto IBS/CBS

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OutrasAliquotas", Column = "TOA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas OutrasAliquotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBS", Column = "DPV_CST_IBSCBS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBS", Column = "DPV_CLASSIFICACAO_TRIBUTARIA_IBSCBS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBS", Column = "DPV_BASE_CALCULO_IBSCBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadual", Column = "DPV_ALIQUOTA_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadual", Column = "DPV_PERCENTUAL_REDUCAO_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadual", Column = "DPV_VALOR_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipal", Column = "DPV_ALIQUOTA_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipal", Column = "DPV_PERCENTUAL_REDUCAO_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipal", Column = "DPV_VALOR_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBS", Column = "DPV_ALIQUOTA_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBS", Column = "DPV_PERCENTUAL_REDUCAO_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBS", Column = "DPV_VALOR_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBS { get; set; }

        #endregion Imposto CBS/IBS


        #region Propriedades com Regras
        public virtual Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao Clonar()
        {
            return (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao)this.MemberwiseClone();
        }

        public virtual string Descricao
        {
            get { return this.NumeroDocumento.ToString(); }
        }

        public virtual decimal ObterValorICMS()
        {
            if (CTe == null)
                return 0m;

            return ((CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Enumeradores.TipoDocumento.CTe) && (CTe.CST != "60")) ? CTe.ValorICMSIncluso : 0m;
        }

        public virtual decimal ObterValorISS()
        {
            if (CTe == null)
                return 0m;

            return ((CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Enumeradores.TipoDocumento.CTe) && !CTe.ISSRetido && (CTe.IncluirISSNoFrete == Enumeradores.OpcaoSimNao.Sim)) ? CTe.ValorISS : 0m;
        }

        public virtual decimal ObterValorLiquido()
        {
            if (CTe == null)
                return 0m;

            return CTe.ValorAReceber - ObterValorICMS() - ObterValorISS() - ObterValorPISCOFINS();
        }

        public virtual decimal ObterValorPISCOFINS()
        {
            if (CTe == null)
                return 0m;

            decimal aliquotaCOFINS = Empresa.EmpresaPai?.Configuracao?.AliquotaCOFINS ?? (decimal)7.60;
            decimal aliquotaPIS = Empresa.EmpresaPai?.Configuracao?.AliquotaPIS ?? (decimal)1.65;

            return Math.Round(CTe.BaseCalculoImposto * ((aliquotaCOFINS + aliquotaPIS) / 100), 2, MidpointRounding.AwayFromZero);
        }

        public virtual bool Equals(DocumentoProvisao other)
        {
            return (other.Codigo == this.Codigo);
        }
        #endregion Propriedades com Regras
    }
}
