using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CANCELAMENTO", EntityName = "CargaCancelamento", Name = "Dominio.Entidades.Embarcador.Cargas.CargaCancelamento", NameType = typeof(CargaCancelamento))]
    public class CargaCancelamento : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        public CargaCancelamento()
        {
            TipoCancelamentoCargaDocumento = TipoCancelamentoCargaDocumento.Carga;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_DUPLICADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaDuplicada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCancelamento", Column = "CAC_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoCancelamento", Column = "CAC_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string MotivoCancelamento { get; set; }

        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioERPSolicitouCancelamento", Column = "CAC_USUARIO_ERP_SOLICITOU_CANCELAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioERPSolicitouCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_USUARIO_SOLICITOU_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioSolicitouCancelamento { get; set; }

        /// <summary>
        /// Quando uma carga é cancelada esse campo registra em que situação ela estava quando foi cancelada.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCargaNoCancelamento", Column = "CAC_SITUACAO_NO_CANCELAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCargaNoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CAC_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CAC_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCancelamentoCargaDocumento", Column = "CAC_TIPO_CANCELAMENTO_CARGA_DOCUMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCargaDocumento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCargaDocumento TipoCancelamentoCargaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRejeicaoCancelamento", Column = "CAC_REJEICAO_CANCELAMENTO", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string MensagemRejeicaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerouIntegracao", Column = "CAC_GEROU_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviouMDFesParaCancelamento", Column = "CAC_ENVIOU_MDFES_PARA_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviouMDFesParaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviouCTesParaCancelamento", Column = "CAC_ENVIOU_CTES_PARA_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviouCTesParaCancelamento { get; set; }

        /// <summary>
        /// Inicia como verdadeiro, pois após o cancelamento do CT-e a averbação é enviada automaticamente
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviouAverbacoesCTesParaCancelamento", Column = "CAC_ENVIOU_AVERBACOES_CTES_PARA_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviouAverbacoesCTesParaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviouValePedagiosParaCancelamento", Column = "CAC_ENVIOU_VALE_PEGADIO_PARA_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviouValePedagiosParaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviouIntegracoesDeCancelamento", Column = "CAC_ENVIOU_INTEGRACOES_DE_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviouIntegracoesDeCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "confirmacaoERP", Column = "CAC_CONFIRMACAO_ERP", TypeType = typeof(bool), NotNull = true)]
        public virtual bool confirmacaoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DuplicarCarga", Column = "CAC_DUPLICAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DuplicarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarPedidosParaMontagemCarga", Column = "CAC_LIBERAR_PEDIDOS_PARA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarPedidosParaMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_CANCELAR_DOCUMENTOS_EMITIDOS_NO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CancelarDocumentosEmitidosNoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_LIBERAR_CANCELAMENTO_COM_AVERBACAO_CTE_REJEITADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCancelamentoComAverbacaoCTeRejeitada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_LIBERAR_CANCELAMENTO_COM_AVERBACAO_MDFE_REJEITADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCancelamentoComAverbacaoMDFeRejeitada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_LIBERAR_CANCELAMENTO_COM_INTEGRACAO_REJEITADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCancelamentoComIntegracaoRejeitada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_LIBERAR_CANCELAMENTO_COM_VALE_PEDAGIO_REJEITADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCancelamentoComValePedagioRejeitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_LIBERAR_CANCEALMENTO_COM_CTE_NAO_INUTILIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCancelamentoComCTeNaoInutilizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_LIBERAR_CANCEALMENTO_COM_CIOT_NAO_CANCELADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCancelamentoComCIOTNaoCancelado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvioCancelamento", Column = "CAC_DATA_ENVIO_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvioCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TentativasEnvioCancelamento", Column = "CAC_TENTATIVAS_ENVIO_CANCELAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int TentativasEnvioCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CANCELAMENTO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCancelamentoIntegracao", Column = "CCI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CANCELAMENTO_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCancelamentoAnexo", Column = "ANX_CODIGO")]
        public virtual IList<CargaCancelamentoAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_INTEGROU_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrouTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "JustificativaCancelamentoCarga", Column = "TCJ_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga JustificativaCancelamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_OPERADOR_RESPONSAVEL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario OperadorResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_CONTROLE_INTEGRACAO_EMBARCADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ControleIntegracaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAC_AGUARDANDO_XML_DESACORDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? AguardandoXmlDesacordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerouIntegracaoDados", Column = "CAC_GEROU_INTEGRACAO_DADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouIntegracaoDados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviouCIOTCancelamento", Column = "CAC_ENVIOU_CIOT_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviouCIOTCancelamento { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "CAC_AGUARDANDO_CONFIRMACAO_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool? AguardandoConfirmacaoCancelamento { get; set; }

        public virtual string Descricao
        {
            get { return Carga?.CodigoCargaEmbarcador ?? string.Empty; }
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.Descricao(); }
        }

        public virtual string DescricaoTipo
        {
            get { return Tipo.ObterDescricao(); }
        }
    }
}
