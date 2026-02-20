namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ISCA", EntityName = "Isca", Name = "Dominio.Entidades.Embarcador.Cargas.Isca", NameType = typeof(Isca))]
    public class Isca : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ISC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ISC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "ISC_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "ISC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEmpresaIsca", Column = "ISC_CODIGO_EMPRESA_ISCA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoEmpresaIsca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Site", Column = "ISC_SITE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Site { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Login", Column = "ISC_LOGIN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Login { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "ISC_SENHA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Senha { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
