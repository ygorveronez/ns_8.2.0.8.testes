using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_FORMULARIO", EntityName = "TransportadorFormulario", Name = "Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario", NameType = typeof(TransportadorFormulario))]
    public class TransportadorFormulario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EFM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "CodigoFormulario", Column = "EFM_CODIGO_FORMULARIO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoFormulario { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "SomenteLeitura", Column = "EFM_SOMENTE_LEITURA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SomenteLeitura { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TransportadorFormularioPermissaoesPersonalizadas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_FORMULARIO_PERMISSAO_PERSONALIZADA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EFM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TransportadorFormularioPermissaoPersonalizada", Column = "EPP_CODIGO")]
        public virtual ICollection<TransportadorFormularioPermissaoPersonalizada> TransportadorFormularioPermissaoesPersonalizadas { get; set; }

    }
}



