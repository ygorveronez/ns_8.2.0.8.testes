namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_MONITORAMENTO_PROCESSAR_MONITORAMENTOS", EntityName = "ConfiguracaoMonitoramentoProcessarMonitoramentos", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoMonitoramentoProcessarMonitoramentos", NameType = typeof(ConfiguracaoMonitoramentoProcessarMonitoramentos))]
    public class ConfiguracaoMonitoramentoProcessarMonitoramentos : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_TEMPO_SLEEP_THREAD", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoSleepThread { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_LIMITE_REGISTROS", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteRegistros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_INTERVALO_CALCULO_PREVISAO_ENTREGA_MIN", TypeType = typeof(int), NotNull = false)]
        public virtual int IntervaloCalculoPrevisaoCargaEntregaMinutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_POSSUI_COLETA_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiColetaContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_ENVIAR_NOTIFICACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarNotificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMP_REGRAS_TRANSITO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegrasTransito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_CODIGO_INTEGRACAO_VIA_TRANSPORTE_MARITIMA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CodigoIntegracaoViaTansporteMaritima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_ARQUIVO_NIVEL_LOG", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ArquivoNivelLog { get; set; }
    }
}
