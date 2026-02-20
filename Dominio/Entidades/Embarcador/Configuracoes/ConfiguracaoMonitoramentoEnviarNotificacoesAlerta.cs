namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_MONITORAMENTO_NOTIFICACAO_ALERTA", EntityName = "ConfiguracaoMonitoramentoEnviarNotificacoesAlerta", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoMonitoramentoEnviarNotificacoesAlerta", NameType = typeof(ConfiguracaoMonitoramentoEnviarNotificacoesAlerta))]
    public class ConfiguracaoMonitoramentoEnviarNotificacoesAlerta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMN_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMN_TEMPO_SLEEP_THREAD", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoSleepThread { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMN_LIMITE_REGISTROS", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteRegistros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMN_ARQUIVO_NIVEL_LOG", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ArquivoNivelLog { get; set; }
    }
}
