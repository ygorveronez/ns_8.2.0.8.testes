using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_NFE_PARA_EMISSAO_NFS_MANUAL", EntityName = "CargaDocumentoParaEmissaoNFSManual", Name = "Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual", NameType = typeof(CargaDocumentoParaEmissaoNFSManual))]
    public class CargaDocumentoParaEmissaoNFSManual : EntidadeBase, IEquatable<CargaDocumentoParaEmissaoNFSManual>
    {
        public CargaDocumentoParaEmissaoNFSManual() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NEM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga CargaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaPedido CargaPedido { get; set; }

        /// <summary>
        /// Está preenchido quando esta entidade é um complemento gerado a partir de uma Ocorrência.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaDocumentoParaEmissaoNFSManual", Column = "NEM_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaDocumentoParaEmissaoNFSManual CargaDocumentoParaEmissaoNFSManualOcorrenciaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoCTeParaSubContratacao", Column = "PSC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoCTeParaSubContratacao PedidoCTeParaSubContratacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTeComplementoInfo", Column = "CCC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo CargaCTeComplementoInfo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentosNFSe", Column = "DNS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentosNFSe DocumentosNFSe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Fechamento.FechamentoFrete FechamentoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LancamentoNFSManual", Column = "LNM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFS.LancamentoNFSManual LancamentoNFSManual { get; set; }

        /// <summary>
        /// Aqui fica armazenado a nota de serviço emitida manualmente, seja nfse, ou dados de uma nfse
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TOMADOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_DESTINATARIO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_REMETENTE", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_NUMERO_PEDIDO_CLIENTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NumeroPedidoCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_PRESTACAO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadePrestacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_CHAVE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_SERIE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_VALOR_FRETE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_VALOR_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_VALOR_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_VALOR_PRESTACAO_SERVICO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_BASE_CALCULO_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_PERCENTUAL_ALIQUOTA_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OutrasAliquotas", Column = "TOA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas OutrasAliquotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_NBS", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string NBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_CODIGO_INDICADOR_OPERACAO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string CodigoIndicadorOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBS", Column = "NEM_CST_IBSCBS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBS", Column = "NEM_CLASSIFICACAO_TRIBUTARIA_IBSCBS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBS", Column = "NEM_BASE_CALCULO_IBSCBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadual", Column = "NEM_ALIQUOTA_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadual", Column = "NEM_PERCENTUAL_REDUCAO_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadual", Column = "NEM_VALOR_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipal", Column = "NEM_ALIQUOTA_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipal", Column = "NEM_PERCENTUAL_REDUCAO_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipal", Column = "NEM_VALOR_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBS", Column = "NEM_ALIQUOTA_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBS", Column = "NEM_PERCENTUAL_REDUCAO_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBS", Column = "NEM_VALOR_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_PESO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_RATEOU_VALOR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RateouValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_DOC_RESIDUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocResidual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NEM_ALTEROU_VALOR_FRETE_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlterouValorFreteNFsManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvioUltimoAlertaNFsePendente", Column = "NEM_DATA_ENVIO_ULTIMO_ALERTA_NFSE_PENDENTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvioUltimoAlertaNFsePendente { get; set; }


        public virtual NFSe NFSe
        {
            get { return this.DocumentosNFSe?.NFSe; }
        }

        public virtual bool Equals(CargaDocumentoParaEmissaoNFSManual other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual CargaDocumentoParaEmissaoNFSManual Clonar()
        {
            CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManualClonada = (CargaDocumentoParaEmissaoNFSManual)this.MemberwiseClone();
            cargaDocumentoParaEmissaoNFSManualClonada.PercentualAliquotaISS = 0;
            cargaDocumentoParaEmissaoNFSManualClonada.BaseCalculoISS = 0;
            cargaDocumentoParaEmissaoNFSManualClonada.RateouValorFrete = false;
            cargaDocumentoParaEmissaoNFSManualClonada.ValorFrete = 0;
            cargaDocumentoParaEmissaoNFSManualClonada.ValorISS = 0;
            cargaDocumentoParaEmissaoNFSManualClonada.ValorPrestacaoServico = 0;
            cargaDocumentoParaEmissaoNFSManualClonada.ValorRetencaoISS = 0;
            cargaDocumentoParaEmissaoNFSManualClonada.ValorTotalMoeda = 0;
            cargaDocumentoParaEmissaoNFSManualClonada.DocResidual = false;
            return cargaDocumentoParaEmissaoNFSManualClonada;
        }
    }
}
