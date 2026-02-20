namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_LOG", EntityName = "ConfiguracaoLog", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLog", NameType = typeof(ConfiguracaoLog))]
    public class ConfiguracaoLog : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizaLogArquivo", Column = "CLO_UTILIZA_LOG_ARQUIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizaLogArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizaLogWeb", Column = "CLO_UTILIZA_LOG_WEB", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaLogWeb { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloLogWeb", Column = "CLO_PROTOCOLO_LOG_WEB", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ProtocoloLogWeb), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ProtocoloLogWeb? ProtocoloLogWeb { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GravarLogError", Column = "CLO_GRAVAR_LOG_ERROR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GravarLogError { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GravarLogInfo", Column = "CLO_GRAVAR_LOG_INFO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GravarLogInfo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GravarLogAdvertencia", Column = "CLO_GRAVAR_LOG_ADVERTENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GravarLogAdvertencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GravarLogDebug", Column = "CLO_GRAVAR_LOG_DEBUG", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GravarLogDebug { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Url", Column = "CLO_URL", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Url { get; set; }       

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "CLO_PORTA", TypeType = typeof(int), NotNull = true)]
        public virtual int Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizaGraylog", Column = "CLO_UTILIZA_LOG_GRAYLOG", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaGraylog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloLogGraylog", Column = "CLO_PROTOCOLO_LOG_GRAYLOG", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ProtocoloLogWeb), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ProtocoloLogWeb? ProtocoloLogGraylog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlGraylog", Column = "CLO_URL_GRAYLOG", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string UrlGraylog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PortaGraylog", Column = "CLO_PORTA_GRAYLOG", TypeType = typeof(int), NotNull = false)]
        public virtual int PortaGraylog { get; set; }
    }
}
