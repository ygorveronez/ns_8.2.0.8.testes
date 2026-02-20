using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_VEICULO", EntityName = "AcertoVeiculo", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo", NameType = typeof(AcertoVeiculo))]
    public class AcertoVeiculo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AVV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SegmentoVeiculo", Column = "VSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Veiculos.SegmentoVeiculo SegmentoVeiculo { get; set; }

        public virtual bool Equals(AcertoVeiculo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}