namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_MONITORAMENTO_PROCESSAR_EVENTOS_SINAL", EntityName = "ConfiguracaoMonitoramentoProcessarEventosSinal", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoMonitoramentoProcessarEventosSinal", NameType = typeof(ConfiguracaoMonitoramentoProcessarEventosSinal))]
    public class ConfiguracaoMonitoramentoProcessarEventosSinal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CME_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CME_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CME_TEMPO_SLEEP_THREAD", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoSleepThread { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CME_LIMITE_REGISTROS", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteRegistros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CME_ARQUIVO_NIVEL_LOG", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ArquivoNivelLog { get; set; }
    }
}
