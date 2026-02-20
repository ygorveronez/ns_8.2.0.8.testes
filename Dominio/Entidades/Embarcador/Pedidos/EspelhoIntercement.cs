using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ESPELHO_INTERCEMENT", EntityName = "EspelhoIntercement", Name = "Dominio.Entidades.Embarcador.Pedidos.EspelhoIntercement", NameType = typeof(EspelhoIntercement))]
    public class EspelhoIntercement : EntidadeBase, IEquatable<EspelhoIntercement>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FKNUM", Column = "EIN_FKNUM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string FKNUM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "REBEL", Column = "EIN_REBEL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string REBEL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SIGNI", Column = "EIN_SIGNI", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SIGNI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ERDAT", Column = "EIN_ERDAT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? ERDAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EIN_ERZET", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? ERZET { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VBELN", Column = "EIN_VBELN", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string VBELN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TARIFA", Column = "EIN_TARIFA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TARIFA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DIFPESO", Column = "EIN_DIFPESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal DIFPESO { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CHAPA", Column = "EIN_CHAPA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal CHAPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OUTROS", Column = "EIN_OUTROS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal OUTROS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PISCOFINS", Column = "EIN_PISCOFINS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PISCOFINS { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "TOTAL", Column = "EIN_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal TOTAL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VIAGEM", Column = "EIN_VIAGEM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string VIAGEM { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.VBELN;
            }
        }

        public virtual bool Equals(EspelhoIntercement other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
