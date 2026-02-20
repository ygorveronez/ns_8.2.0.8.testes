using Dominio.Entidades.Embarcador.Frota;
using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_VIAGEM_TACOGRAFO", EntityName = "AcertoViagemTacografo", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoViagemTacografo", NameType = typeof(AcertoViagemTacografo))]
    public class AcertoViagemTacografo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoViagemTacografo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AVT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ControleTacografo", Column = "CTA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ControleTacografo ControleTacografo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Excesso", Column = "AVT_EXCESSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Excesso { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(AcertoViagemTacografo other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
