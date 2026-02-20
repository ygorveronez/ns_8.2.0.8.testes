using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Usuarios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERFIL_FORMULARIO", EntityName = "PerfilFormulario", Name = "Dominio.Entidades.Embarcador.Usuarios.PerfilFormulario", NameType = typeof(PerfilFormulario))]
    public class PerfilFormulario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "PerfilAcesso", Column = "PAC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso PerfilAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(Column = "FOR_CODIGO_FORMULARIO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoFormulario { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "SomenteLeitura", Column = "PAF_SOMENTE_LEITURA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SomenteLeitura { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "FormularioPermissaoPersonalizada", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PERFIL_FORMULARIO_PERMISSAO_PERSONALIZADA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PerfilFormularioPermissaoPersonalizada", Column = "PPP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Usuarios.PerfilFormularioPermissaoPersonalizada> FormularioPermissaoPersonalizada { get; set; }

    }
}



