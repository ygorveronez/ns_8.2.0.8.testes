namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_TRANSPORTADOR", EntityName = "ConfiguracaoTransportador", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador", NameType = typeof(ConfiguracaoTransportador))]
    public class ConfiguracaoTransportador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_PERMITIR_CADASTRAR_TRANSPORTADOR_INFORMACOES_MINIMAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirCadastrarTransportadorInformacoesMinimas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_PERMITIR_INFORMAR_EMPRESA_FAVORECIDA_NOS_DADOS_BANCARIOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarEmpresaFavorecidaNosDadosBancarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_EXISTE_TRANSPORTADOR_PADRAO_CONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExisteTransportadorPadraoContratacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_PADRAO_CONTRATACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa TransportadorPadraoContratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_ATIVAR_CONTROLE_CARREGAMENTO_NAVIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarControleCarregamentoNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_EXIGIR_RETENCAO_ISS_QUANDO_MUNICIPIO_PRESTACAO_FOR_DIFERENTE_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirRetencaoISSQuandoMunicipioPrestacaoForDiferenteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_PERMITIR_TRANSPORTADOR_RETORNAR_ETAPA_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransportadorRetornarEtapaNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_NAO_ATUALIZAR_NOME_FANTASIA_CLIENTE_ALTERAR_DADOS_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAtualizarNomeFantasiaClienteAlterarDadosTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_ENVIAR_EMAIL_DOCUMENTO_REIJEITADO_AUDITORIA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailDocumentoRejeitadoAuditoriaFrete { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_NAO_GERAR_AUTOMATICAMENTE_USUARIO_ACESSO_PORTAL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarAutomaticamenteUsuarioAcessoPortalTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_NAO_HABILITAR_DETALHES_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoHabilitarDetalhesCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_NOTIFICAR_TRANSPORTADOR_PROCESSO_SHARE_ROTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarTransportadorProcessoShareRotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_HABILITAR_SPOT_CARGA_APOS_LIMITE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarSpotCargaAposLimiteHoras { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração de Transportador/Empresa"; }
        }
    }
}
