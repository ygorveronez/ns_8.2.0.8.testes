using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_PEDAGIO", EntityName = "AcertoPedagio", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoPedagio", NameType = typeof(AcertoPedagio))]
    public class AcertoPedagio : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedagio", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedagio.Pedagio Pedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LancadoManualmente", Column = "ACV_LANCADO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancadoManualmente { get; set; }

        public virtual bool Equals(AcertoPedagio other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}