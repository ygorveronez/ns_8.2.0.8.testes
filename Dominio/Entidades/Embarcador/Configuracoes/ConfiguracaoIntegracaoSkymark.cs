namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_SKYMARK", EntityName = "IntegracaoSkymark", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoSkymark", NameType = typeof(ConfiguracaoIntegracaoSkymark))]
    public class ConfiguracaoIntegracaoSkymark : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Url", Column = "CIS_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Url { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integracao", Column = "CIS_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Integracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Contrato", Column = "CIS_CONTRATO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Contrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveUm", Column = "CIS_CHAVE_UM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ChaveUm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveDois", Column = "CIS_CHAVE_DOIS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ChaveDois { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarIntegracao", Column = "CIS_HABILITAR_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarIntegracao { get; set; }
    }
}
