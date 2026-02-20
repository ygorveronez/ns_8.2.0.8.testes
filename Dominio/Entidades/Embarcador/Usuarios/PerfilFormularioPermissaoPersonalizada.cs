namespace Dominio.Entidades.Embarcador.Usuarios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERFIL_FORMULARIO_PERMISSAO_PERSONALIZADA", EntityName = "PerfilFormularioPermissaoPersonalizada", Name = "Dominio.Entidades.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada", NameType = typeof(PerfilFormularioPermissaoPersonalizada))]
    public class PerfilFormularioPermissaoPersonalizada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "PerfilFormulario", Column = "PAF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario PerfilFormulario { get; set; }

        [NHibernate.Mapping.Attributes.Property(Column = "PPP_CODIGO_PERMISSAO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoPermissao { get; set; }
    }
}
