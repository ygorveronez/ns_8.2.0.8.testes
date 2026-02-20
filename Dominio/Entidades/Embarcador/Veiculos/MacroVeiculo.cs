using System;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MACRO_VEICULO", EntityName = "MacroVeiculo", Name = "Dominio.Entidades.Embarcador.Veiculos.MacroVeiculo", NameType = typeof(MacroVeiculo))]
    public class MacroVeiculo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Veiculos.MacroVeiculo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Macro", Column = "MCR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.Macro Macro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "MCV_LATIDUDE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "MCV_LONGITUDE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "MCV_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataMacro", Column = "MCV_DATA_MACRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime? DataMacro { get; set; }

        public virtual string Descricao
        {
            get { return Macro?.Descricao ?? string.Empty; }
        }

        public virtual bool Equals(MacroVeiculo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
        
    }
}
