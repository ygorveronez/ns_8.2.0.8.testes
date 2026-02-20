using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LANCAMENTO_NFS_MANUAL", EntityName = "LancamentoNFSManual", Name = "Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual", NameType = typeof(LancamentoNFSManual))]
    public class LancamentoNFSManual : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LNM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LNM_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LNM_SITUACAO", TypeType = typeof(SituacaoLancamentoNFSManual), NotNull = false)]
        public virtual SituacaoLancamentoNFSManual Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LNM_SITUACAO_NO_CANCELAMENTO", TypeType = typeof(SituacaoLancamentoNFSManual), NotNull = false)]
        public virtual SituacaoLancamentoNFSManual SituacaoNoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete FechamentoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DadosNFSManual", Column = "NSM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DadosNFSManual DadosNFS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadePrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LNM_ALCADA_COM_REQUISITO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlcadaComRequisito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LNM_ALCADA_COM_REQUISITO_APROVADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlcadaComRequisitoAprovadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LNM_NFS_RESIDUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NFSResidual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LNM_NFS_EMITIDA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NFSEmitidaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetornoNFSAutomaticamente", Column = "LNM_MENSAGEM_RETORNO_NFS_AUTOMATICAMENTE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensagemRetornoNFSAutomaticamente { get; set; }

        /// <summary>
        /// Define se a origem dos documento são de NFS-es integradas pelo MultiCTe
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CargasMultiCTe", Column = "LNM_CARGAS_MULTICTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargasMultiCTe { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_NFE_PARA_EMISSAO_NFS_MANUAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LNM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaDocumentoParaEmissaoNFSManual", Column = "NEM_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> Documentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "IntegracoesNatura", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NFS_MANUAL_CTE_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LNM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NFSManualIntegracaoNatura", Column = "NIN_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoNatura> IntegracoesNatura { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "LancamentoNFSAutorizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LANCAMENTO_NFS_AUTORIZACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LNM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LancamentoNFSAutorizacao", Column = "LAA_CODIGO")]
        public virtual ICollection<LancamentoNFSAutorizacao> LancamentoNFSAutorizacoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NFS_MANUAL_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LNM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LancamentoNFSIntegracao", Column = "ILN_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao> Integracoes { get; set; }

        /// <summary>
        /// Está gerando os registros de integrações de documentos. Nesta situação não deve permitir fazer nenhum tipo de alteração na carga.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "GerandoIntegracoes", Column = "LNM_GERANDO_INTEGRACOES", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerandoIntegracoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoServico", Column = "LNM_CODIGO_SERVICO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoCliente", Formula = @"(SUBSTRING((SELECT DISTINCT ', ' + c.NEM_NUMERO_PEDIDO_CLIENTE
	                                                                                        FROM T_LANCAMENTO_NFS_MANUAL l
	                                                                                        inner join T_CARGA_NFE_PARA_EMISSAO_NFS_MANUAL c ON c.LNM_CODIGO = l.LNM_CODIGO
	                                                                                        WHERE c.LNM_CODIGO = LNM_CODIGO FOR XML PATH('')), 3, 1000))", TypeType = typeof(string), Lazy = true)]

        public virtual string NumeroPedidoCliente { get; set; }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public virtual string Descricao
        {
            get { return this.DadosNFS?.Numero.ToString() ?? "0"; }
        }
    }
}
