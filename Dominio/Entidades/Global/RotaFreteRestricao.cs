using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_FRETE_RESTRICAO", EntityName = "RotaFreteRestricao", Name = "Dominio.Entidades.RotaFreteRestricao", NameType = typeof(RotaFreteRestricao))]
    public class RotaFreteRestricao : EntidadeBase, IEquatable<RotaFreteRestricao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicio", Column = "RFR_HORA_INICIO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraTermino", Column = "RFR_HORA_TERMINO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraTermino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Segunda", Column = "RFR_SEGUNDA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Segunda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Terca", Column = "RFR_TERCA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Terca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quarta", Column = "RFR_QUARTA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Quarta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quinta", Column = "RFR_QUINTA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Quinta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sexta", Column = "RFR_SEXTA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Sexta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sabado", Column = "RFR_SABADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Sabado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Domingo", Column = "RFR_DOMINGO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Domingo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        public virtual bool Equals(RotaFreteRestricao other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual string ObterDescricaoDias()
        {
            List<string> dias = new List<string>();

            if (Segunda) dias.Add("Segunda-Feira");
            if (Terca)  dias.Add("Terça-Feira");
            if (Quarta) dias.Add("Quarta-Feira");
            if (Quinta) dias.Add("Quinta-Feira");
            if (Sexta) dias.Add("Sexta-Feira");
            if (Sabado) dias.Add("Sábado");
            if (Domingo) dias.Add("Domingo");

            if (dias.Count == 7)
                return "Todos os dias";

            return string.Join(",", dias);
        }
    }
}
