namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_AGENDAMENTO_COLETA_ENTREGA", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoAgendamentoColetaEntrega", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoAgendamentoColetaEntrega", NameType = typeof(ConfiguracaoTipoOperacaoAgendamentoColetaEntrega))]
    public class ConfiguracaoTipoOperacaoAgendamentoColetaEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_UTILIZAR_DATA_SAIDA_GUARITA_COMO_TERMINO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataSaidaGuaritaComoTerminoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_OBRIGAR_INFORMAR_CTE_PORTAL_FORNECEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarInformarCTePortalFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_EXIGIR_NUMERO_ISIS_RETURN_PARA_AGENDAR_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirNumeroIsisReturnParaAgendarEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_REMOVER_ETAPA_AGENDAMENTO_DO_AGENDAMENTO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemoverEtapaAgendamentoDoAgendamentoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_NAO_OBRIGAR_INFORMAR_MODELO_VEICULAR_AGENDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoObrigarInformarModeloVeicularAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_NAO_OBRIGAR_INFORMAR_TRANSPORTADOR_AGENDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoObrigarInformarTransportadorAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_AVANCAR_ETAPA_NFE_CARGA_AO_CONFIRMAR_AGENDAMENTO_RETIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvancarEtapaNFeCargaAoConfirmarAgendamentoRetira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_EXIGIR_QUE_CD_DESTINO_SEJA_INFORMADO_AGENDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirQueCDDestinoSejaInformadoAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_ENVIAR_EMAIL_AO_CLIENTE_COM_LINK_DE_AGENDAMENTO_QUANDO_GERAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailAoCLienteComLinkDeAgendamentoQuandoGerarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_CONSIDERAR_DATA_ENTREGA_COMO_INICIO_FLUXO_PATIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarDataEntregaComoInicioDoFluxoPatio { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configurações para agendamento de coleta/entrega";
            }
        }
    }
}
