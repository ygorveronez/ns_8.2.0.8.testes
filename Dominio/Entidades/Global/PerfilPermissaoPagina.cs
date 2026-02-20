namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERFIL_PERMISSAO_PAGINA", EntityName = "PerfilPermissaoPagina", Name = "Dominio.Entidades.PerfilPermissaoPagina", NameType = typeof(PerfilPermissaoPagina))]
    public class PerfilPermissaoPagina : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pagina", Column = "FOR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pagina Pagina { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PerfilPermissao", Column = "PP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PerfilPermissao PerfilPermissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermissaoDeAcesso", Column = "PPP_FORM", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string PermissaoDeAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermissaoDeInclusao", Column = "PPP_INCLUIR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string PermissaoDeInclusao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermissaoDeDelecao", Column = "PPP_EXCLUIR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string PermissaoDeDelecao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermissaoDeAlteracao", Column = "PPP_ALTERAR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string PermissaoDeAlteracao { get; set; }
    }
}
