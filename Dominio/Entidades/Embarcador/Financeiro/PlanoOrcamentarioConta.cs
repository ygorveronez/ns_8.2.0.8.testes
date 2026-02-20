using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PLANO_ORCAMENTARIO_CONTA", EntityName = "PlanoOrcamentarioConta", Name = "Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentarioConta", NameType = typeof(PlanoOrcamentarioConta))]
    public class PlanoOrcamentarioConta : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentarioConta>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "POC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Percentual", Column = "POC_PERCENTUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "POC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoOrcamentario", Column = "POR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario PlanoOrcamentario { get; set; }

        public virtual bool Equals(PlanoOrcamentarioConta other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
