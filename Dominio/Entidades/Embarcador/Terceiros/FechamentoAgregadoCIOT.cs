using System;

namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_AGREGADO_CIOT", EntityName = "FechamentoAgregadoCIOT", Name = "Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT", NameType = typeof(FechamentoAgregadoCIOT))]
    public class FechamentoAgregadoCIOT : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT>
    {
        public FechamentoAgregadoCIOT() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoAgregado", Column = "FAG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado FechamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CIOT", Column = "CIO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Documentos.CIOT CIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAC_CONSOLIDADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Consolidado { get; set; }

        public virtual FechamentoAgregadoCIOT Clonar()
        {
            return (FechamentoAgregadoCIOT)this.MemberwiseClone();
        }

        public virtual bool Equals(FechamentoAgregadoCIOT other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}