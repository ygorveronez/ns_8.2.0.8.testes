namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_MONITORAMENTO_MONITORAR_THREADS", EntityName = "ConfiguracaoMonitoramentoMonitorarThreads", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoMonitoramentoMonitorarThreads", NameType = typeof(ConfiguracaoMonitoramentoMonitorarThreads))]
    public class ConfiguracaoMonitoramentoMonitorarThreads : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMT_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMT_TEMPO_SLEEP_THREAD", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoSleepThread { get; set; }
    }
}
