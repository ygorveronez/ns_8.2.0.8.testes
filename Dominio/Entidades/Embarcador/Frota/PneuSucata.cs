using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_PNEU_SUCATA", EntityName = "Frota.PneuSucata", Name = "Dominio.Entidades.Embarcador.Frota.PneuSucata", NameType = typeof(PneuSucata))]
    public class PneuSucata : EntidadeBase, IEquatable<PneuSucata>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNS_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNS_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PNS_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCarcaca", Column = "PNS_VALOR_CARCACA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorCarcaca { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.MotivoSucateamentoPneu", Column = "PMS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoSucateamentoPneu Motivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.Pneu", Column = "PNU_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pneu Pneu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.PneuHistorico", Column = "PNH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PneuHistorico PneuHistorico { get; set; }

        public virtual bool Equals(PneuSucata other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
