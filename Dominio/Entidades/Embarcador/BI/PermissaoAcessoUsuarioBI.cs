namespace Dominio.Entidades.Embarcador.BI
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERMISSAO_ACESSO_USUARIO_BI", EntityName = "PermissaoAcessoUsuarioBI", Name = "Dominio.Entidades.Embarcador.BI.PermissaoAcessoUsuarioBI", NameType = typeof(PermissaoAcessoUsuarioBI))]
    public class PermissaoAcessoUsuarioBI : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermissaoPersonalizada", Column = "PAB_PERMISSAO_PERSONALIZADA", TypeType = typeof(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada), NotNull = true)]
        public virtual AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada PermissaoPersonalizada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAB_CODIGO_FORMULARIO_BI", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoFormularioBI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAB_CAMINHO_FORMULARIO", TypeType = typeof(string), Length = 350, NotNull = true)]
        public virtual string CaminhoFormulario { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Permissão de Acesso de Usuário do BI";
            }
        }

    }
}