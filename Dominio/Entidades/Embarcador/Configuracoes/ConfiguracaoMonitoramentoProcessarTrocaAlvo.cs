namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_MONITORAMENTO_PROCESSAR_TROCA_ALVO", EntityName = "ConfiguracaoMonitoramentoProcessarTrocaAlvo", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoMonitoramentoProcessarTrocaAlvo", NameType = typeof(ConfiguracaoMonitoramentoProcessarTrocaAlvo))]
    public class ConfiguracaoMonitoramentoProcessarTrocaAlvo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMT_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMT_TEMPO_SLEEP_THREAD", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoSleepThread { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMT_LIMITE_REGISTROS", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteRegistros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMT_LIMITE_FILA_ARQUIVOS", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteFilaArquivos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMT_GEOLOZALIZACAO_APENAS_JURIDICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeolocalizacaoApenasJuridico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMT_GERAR_PERMANCIA_LOCAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPermanenciaLocais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMT_DIRETORIO_FILA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string DiretorioFila { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMT_ARQUIVO_FILA_PREFIXO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string ArquivoFilaPrefixo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMT_ARQUIVO_NIVEL_LOG", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ArquivoNivelLog { get; set; }



    }
}
