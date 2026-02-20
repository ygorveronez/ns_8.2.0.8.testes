using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_INTERCAB", EntityName = "IntegracaoIntercab", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab", NameType = typeof(IntegracaoIntercab))]
    public class IntegracaoIntercab : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoIntercab", Column = "CIN_POSSUI_INTEGRACAO_INTERCAB", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoIntercab { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCargas", Column = "CIN_ATIVAR_INTEGRACAO_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarNovoHomeDash", Column = "CIN_ATIVAR_NOVO_HOME_DASH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarNovoHomeDash { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoDocumentacaoCarga", Column = "CIN_INTEGRACAO_DOCUMENTACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoDocumentacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCartaCorrecao", Column = "CIN_ATIVAR_INTEGRACAO_CARTA_CORRECAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCartaCorrecao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTipoOperacao", Column = "CIN_CODIGO_TIPO_OPERACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntercab", Column = "CIN_URL_INTERCAB", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string URLIntercab { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenIntercab", Column = "CIN_TOKEN_INTERCAB", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string TokenIntercab { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoMercante", Column = "CIN_ATIVAR_INTEGRACAO_MERCANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoMercante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCancelamentoCarga", Column = "CIN_ATIVAR_INTEGRACAO_CANCELAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCancelamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCteManual", Column = "CIN_ATIVAR_INTEGRACAO_CTE_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCteManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoMDFeAquaviario", Column = "CIN_ATIVAR_INTEGRACAO_MDFE_AQUAVIARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoMDFeAquaviario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoCargaAtualParaNovo", Column = "CIN_ATIVAR_INTEGRACAO_CARGA_ATUAL_PARA_NOVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoCargaAtualParaNovo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoOcorrencias", Column = "CIN_ATIVAR_INTEGRACAO_OCORRENCIAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoOcorrencias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DefinirModalPeloTipoCarga", Column = "CIN_DEFINIR_MODAL_PELO_TIPO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DefinirModalPeloTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoFatura", Column = "CIN_ATIVAR_INTEGRACAO_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BuscarTipoServicoModeloDocumentoVinculadoCarga", Column = "CIN_BUSCAR_TIPO_SERVICO_MODELO_DOCUMENTO_VINCULADO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarTipoServicoModeloDocumentoVinculadoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIN_MODIFICAR_TIMELINE_DE_ACORDO_COM_TIPO_SERVICO_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ModificarTimelineDeAcordoComTipoServicoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarTimelineIntegracaoFaturaCarga", Column = "CIN_HABILITAR_TIMELINE_INTEGRACAO_FATURA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTimelineIntegracaoFaturaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarTimelineFaturamentoCarga", Column = "CIN_HABILITAR_TIMELINE_FATURAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTimelineFaturamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarTimelineMercanteCarga", Column = "CIN_HABILITAR_TIMELINE_MERCANTE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTimelineMercanteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarTimelineMDFeAquaviario", Column = "CIN_HABILTAR_TIMELINE_MDFE_AQUAVIARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTimelineMDFeAquaviario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarNovosFiltrosConsultaCarga", Column = "CIN_ATIVAR_NOVOS_FILTROS_CONSULTA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarNovosFiltrosConsultaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AjustarLayoutFiltrosTelaCarga", Column = "CIN_AJUSTAR_LAYOUT_FILTROS_TELA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AjustarLayoutFiltrosTelaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModificarTimelineIntegracaoCarga", Column = "CIN_MODIFICAR_TIMELINE_INTEGRACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ModificarTimelineIntegracaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarPreFiltrosTelaCarga", Column = "CIN_ATIVAR_PRE_FILTROS_TELA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarPreFiltrosTelaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDiasParaDataInicial", Column = "CIN_QUANTIDADE_DIAS_PARA_DATA_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasParaDataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "SituacoesCarga", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_INTEGRACAO_INTERCAB_SITUACAO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CIN_SITUACAO_CARGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaMercante), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaMercante> SituacoesCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarTimelineCargaPortoPorto", Column = "CIN_HABILITAR_TIMELINE_CARGA_PORTO_PORTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTimelineCargaPortoPorto { get; set; }        
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarTimelineCargaPorta", Column = "CIN_HABILITAR_TIMELINE_CARGA_PORTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTimelineCargaPorta { get; set; }        
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarTimelineCargaSVMProprio", Column = "CIN_HABILITAR_TIMELINE_CARGA_SVM_PROPRIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTimelineCargaSVMProprio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarTimelineCargaFeeder", Column = "CIN_HABILITAR_TIMELINE_CARGA_FEEDER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTimelineCargaFeeder { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RemoverObrigacaoCodigoEmbarcacaoCadastroNavio", Column = "CIN_REMOVER_OBRIGACAO_CODIGO_EMBARCACAO_CADASTRO_NAVIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemoverObrigacaoCodigoEmbarcacaoCadastroNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarControleDashRegiaoOperador", Column = "CIN_ATIVAR_CONTROLE_DASH_REGIAO_OPERADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarControleDashRegiaoOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SelecionarTipoOperacao", Column = "CIN_SELECIONAR_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SelecionarTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarGeracaoCCePelaRolagemWS", Column = "CIN_ATIVAR_GERACAO_CCE_PELA_ROLAGEM_WS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarGeracaoCCePelaRolagemWS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO_PADRAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCargaPadrao { get; set; }
    }
}
