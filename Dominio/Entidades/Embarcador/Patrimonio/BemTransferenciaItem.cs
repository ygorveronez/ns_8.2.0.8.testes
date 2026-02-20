using System;

namespace Dominio.Entidades.Embarcador.Patrimonio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BEM_TRANSFERENCIA_ITEM", EntityName = "BemTransferenciaItem", Name = "Dominio.Entidades.Embarcador.Patrimonio.BemTransferenciaItem", NameType = typeof(BemTransferenciaItem))]
    public class BemTransferenciaItem : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Patrimonio.BemTransferenciaItem>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BTI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Bem", Column = "BEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Bem Bem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BemTransferencia", Column = "BTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BemTransferencia BemTransferencia { get; set; }

        public virtual bool Equals(BemTransferenciaItem other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
