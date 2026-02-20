using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTAINER_TIPO_ASSOCIADO", EntityName = "ContainerTipoAssociado", Name = "Dominio.Entidades.Embarcador.Pedidos.ContainerTipoAssociado", NameType = typeof(ContainerTipoAssociado))]
    public class ContainerTipoAssociado : EntidadeBase, IEquatable<ContainerTipoAssociado>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContainerTipo", Column = "CTI_CODIGO_VINCULADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContainerTipo ContainerTipoVinculado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContainerTipo", Column = "CTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContainerTipo ContainerTipo { get; set; }

        public virtual bool Equals(ContainerTipoAssociado other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
