namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERMIFUN", EntityName = "PaginaUsuario", Name = "Dominio.Entidades.PaginaUsuario", NameType = typeof(PaginaUsuario))]
    public class PaginaUsuario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PER_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pagina", Column = "FOR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pagina Pagina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermissaoDeAcesso", Column = "PER_FORM", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string PermissaoDeAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermissaoDeInclusao", Column = "PER_INCLUIR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string PermissaoDeInclusao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermissaoDeDelecao", Column = "PER_EXCLUIR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string PermissaoDeDelecao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermissaoDeAlteracao", Column = "PER_ALTERAR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string PermissaoDeAlteracao { get; set; }
    }
}
