using System;

namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTORISTA_JORNADA", EntityName = "MotoristaJornada", Name = "Dominio.Entidades.Embarcador.Transportadores.MotoristaJornada", NameType = typeof(MotoristaJornada))]
    public class MotoristaJornada : EntidadeBase, IEquatable<MotoristaJornada>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MJD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "MJD_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "MJD_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        public virtual bool Equals(MotoristaJornada other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
