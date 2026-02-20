using Dominio.Interfaces.Embarcador.Entidade;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CTE", EntityName = "CargaCTe", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaCTe", NameType = typeof(CargaCTe))]
    public class CargaCTe : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaCTe>, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LancamentoNFSManual", Column = "LNM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual LancamentoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreConhecimentoDeTransporteEletronico", Column = "PCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PreConhecimentoDeTransporteEletronico PreCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SistemaEmissor", Column = "PCO_SISTEMA_EMISSOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor SistemaEmissor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoProcessamentoThread", Column = "PCO_SITUACAO_PROCESSAMENTO_THREAD", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoThread), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoThread SituacaoProcessamentoThread { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCheckin", Column = "PCO_SITUACAO_CHECKIN", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCheckin), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCheckin SituacaoCheckin { get; set; }

        /// <summary>
        /// Nos casos onde será emitido CT-e pela filial transportadora (apenas embarcador), aqui será armazenado qual a CargaCTe da filial emissora foi vinculada a CargaCTe do transportador (subcontratação)
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO_FILIAL_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTeFilialEmissora { get; set; }

        /// <summary>
        /// Nos casos onde será emitido CT-e pela filial transportadora (apenas embarcador), aqui será armazenado qual a CargaCTe do transportador (subcontratação) foi gerada para a CargaCTe da filial emissora.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO_SUB_CONTRATACAO_FILIAL_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTeSubContratacaoFilialEmissora { get; set; }

        /// <summary>
        /// Indica com qual CT-e do trecho anterior estava vinculado, para casos onde utiliza o mesmo documento em outro trecho
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO_TRECHO_ANTERIOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTeTrechoAnterior { get; set; }

        /// <summary>
        /// Indica com qual é o próximo trecho vinculado ao CT-e, para casos onde utiliza o mesmo documento em outro trecho
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO_PROXIMO_TRECHO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTeProximoTrecho { get; set; }

        /// <summary>
        /// Data em que um CT-e emitido por outro sistema (importado) foi vinculado à carga
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_DATA_VINCULO_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVinculoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTeComplementoInfo", Column = "CCC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo CargaCTeComplementoInfo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NotasFiscais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedidoXMLNotaFiscalCTe", Column = "CAR_CODIGO")]
        public virtual ICollection<CargaPedidoXMLNotaFiscalCTe> NotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Componentes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CTE_COMPONENTES_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeComponentesFrete", Column = "CCC_CODIGO")]
        public virtual IList<CargaCTeComponentesFrete> Componentes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CIOTs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CIOT_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CIOTCTe", Column = "CIC_CODIGO")]
        public virtual IList<Documentos.CIOTCTe> CIOTs { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_GEROU_MOVIMENTACAO_AUTORIZACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerouMovimentacaoAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_GEROU_MOVIMENTACAO_CANCELAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerouMovimentacaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_GEROU_TITULO_AUTORIZACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerouTituloAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_GEROU_TITULO_GNRE_AUTORIZACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerouTituloGNREAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_GEROU_CONTROLE_FATURAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerouControleFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_GEROU_CANHOTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerouCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTeAgrupado", Column = "CCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado CargaCTeAgrupado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamento", Column = "CAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCancelamento CargaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTeComplementoInfoProduto", Column = "CCP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoProduto CargaCTeComplementoInfoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_REPLICADO_CARGA_FILHO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ReplicadoCargaFilho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoDocumentoContribuinte", Column = "CCT_SITUACAO_DOCUMENTO_CONTRIBUINTE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoContribuinte), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoContribuinte SituacaoDocumentoContribuinte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_VINCULO_MANUAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool VinculoManual { get; set; }

        /// <summary>
        /// Campo para sumarizar se o Lançamento Manual possui ao menos uma nota manual de complemento gerada por uma ocorrência.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_LANCAMENTO_MANUAL_POSSUI_NFS_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? LancamentoManualPossuiNFSOcorrencia { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaCTe)this.MemberwiseClone();
        }

        public virtual string Descricao
        {
            get
            {
                return (this.Carga?.Descricao ?? string.Empty) + " - " + (this.CTe?.Descricao ?? string.Empty);
            }
        }

        public virtual bool Equals(CargaCTe other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
