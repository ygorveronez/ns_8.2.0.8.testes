namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_ARQUIVEI", EntityName = "IntegracaoArquivei", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArquivei", NameType = typeof(IntegracaoArquivei))]
    public class IntegracaoArquivei : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLArquivei", Column = "CIA_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLArquivei { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDArquivei", Column = "CIA_ID", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IDArquivei { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KeyArquivei", Column = "CIA_KEY", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string KeyArquivei { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoInicialConsultaXMLCTeArquivei", Column = "CIA_CODIGO_INICIAL_CONSULTA_XML_CTE", TypeType = typeof(long), NotNull = false)]
        public virtual long CodigoInicialConsultaXMLCTeArquivei { get; set; }
    }
}