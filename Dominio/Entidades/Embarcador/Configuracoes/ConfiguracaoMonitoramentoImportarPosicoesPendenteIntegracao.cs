namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_MONITORAMENTO_POSICOES_PENDENTE_INTEGRACAO", EntityName = "ConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao", NameType = typeof(ConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao))]
    public class ConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMI_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMI_TEMPO_SLEEP_THREAD", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoSleepThread { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMI_LIMITE_REGISTROS", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteRegistros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMI_MINUTOS_DIFERENCA_MINIMA_ENTRE_POSICOES", TypeType = typeof(int), NotNull = false)]
        public virtual int MinutosDiferencaMinimaEntrePosicoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMI_ARQUIVO_NIVEL_LOG", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ArquivoNivelLog { get; set; }
    }
}
