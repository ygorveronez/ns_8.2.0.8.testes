namespace Dominio.Entidades.Embarcador.Usuarios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FUNCIONARIO_FORMULARIO_PERMISSAO_PERSONALIZADA", EntityName = "FuncionarioFormularioPermissaoPersonalizada", Name = "Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada", NameType = typeof(FuncionarioFormularioPermissaoPersonalizada))]
    public class FuncionarioFormularioPermissaoPersonalizada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "FuncionarioFormulario", Column = "FMO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario FuncionarioFormulario { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "CodigoPermissao", Column = "PPS_CODIGO_PERMISSAO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoPermissao { get; set; }
    }
}
