using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Usuarios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FUNCIONARIO_FORMULARIO", EntityName = "FuncionarioFormulario", Name = "Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario", NameType = typeof(FuncionarioFormulario))]
    public class FuncionarioFormulario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FMO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "CodigoFormulario", Column = "FOR_CODIGO_FORMULARIO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoFormulario { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "SomenteLeitura", Column = "FMO_SOMENTE_LEITURA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SomenteLeitura { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "FuncionarioFormularioPermissaoesPersonalizadas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FUNCIONARIO_FORMULARIO_PERMISSAO_PERSONALIZADA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FMO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FuncionarioFormularioPermissaoPersonalizada", Column = "FPP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada> FuncionarioFormularioPermissaoesPersonalizadas { get; set; }
        
    }
}



