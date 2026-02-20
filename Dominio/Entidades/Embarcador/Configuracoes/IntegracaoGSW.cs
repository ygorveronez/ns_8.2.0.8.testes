namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_GSW", EntityName = "IntegracaoGSW", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGSW", NameType = typeof(IntegracaoGSW))]
    public class IntegracaoGSW : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIG_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIG_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIG_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoInicialConsultaXMLCTe", Column = "CIG_CODIGO_INICIAL_CONSULTA_XML_CTE", TypeType = typeof(long), NotNull = false)]
        public virtual long CodigoInicialConsultaXMLCTe { get; set; }
    }
}
