using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_USUARIO_CARGA", EntityName = "AcertoUsuarioCarga", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoUsuarioCarga", NameType = typeof(AcertoUsuarioCarga))]
    public class AcertoUsuarioCarga : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoUsuarioCarga>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AUC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHora", Column = "AUC_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        public virtual bool Equals(AcertoUsuarioCarga other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}