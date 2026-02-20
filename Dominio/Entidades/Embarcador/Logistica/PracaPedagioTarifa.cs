using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRACA_PEDAGIO_TARIFA", EntityName = "PracaPedagioTarifa", Name = "Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa", NameType = typeof(PracaPedagioTarifa))]
    public class PracaPedagioTarifa : EntidadeBase, IEquatable<PracaPedagioTarifa>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PPT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PracaPedagio", Column = "PRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.PracaPedagio PracaPedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PPT_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tarifa", Column = "PPT_TARIFA", TypeType = typeof(decimal), NotNull = true, Scale = 4, Precision = 12)]
        public virtual decimal Tarifa { get; set; }

        public virtual bool Equals(PracaPedagioTarifa other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
