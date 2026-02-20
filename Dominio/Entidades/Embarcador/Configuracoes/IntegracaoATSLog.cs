namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_ATSLOG", EntityName = "IntegracaoATSLog", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSLog", NameType = typeof(IntegracaoATSLog))]
    public class IntegracaoATSLog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "ATS_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATS_URL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATS_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATS_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATS_SECRET_KEY", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string SecretKey { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATS_CNPJ_COMPANY", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CNPJCompany { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATS_NOME_COMPANY", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeCompany { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Localidade { get; set; }
    }
}