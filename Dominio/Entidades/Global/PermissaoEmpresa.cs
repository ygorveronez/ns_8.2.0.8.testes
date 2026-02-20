namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERMISSAO_EMPRESA", EntityName = "PermissaoEmpresa", Name = "Dominio.Entidades.PermissaoEmpresa", NameType = typeof(PermissaoEmpresa))]
    public class PermissaoEmpresa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pagina", Column = "FOR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pagina Pagina { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermissaoDeAcesso", Column = "PEE_FORM", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string PermissaoDeAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermissaoDeInclusao", Column = "PEE_INCLUIR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string PermissaoDeInclusao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermissaoDeDelecao", Column = "PEE_EXCLUIR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string PermissaoDeDelecao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermissaoDeAlteracao", Column = "PEE_ALTERAR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string PermissaoDeAlteracao { get; set; }
    }
}
