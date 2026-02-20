using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_PNEU", EntityName = "VeiculoPneu", Name = "Dominio.Entidades.VeiculoPneu", NameType = typeof(VeiculoPneu))]
    public class VeiculoPneu : EntidadeBase, IEquatable<VeiculoPneu>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VPN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCargaEixoPneu", Column = "MEP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Cargas.ModeloVeicularCargaEixoPneu EixoPneu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.Pneu", Column = "PNU_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Frota.Pneu Pneu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataMovimentacaoPneu", Column = "VPN_DATA_MOVIMENTACAO_PNEU", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataMovimentacaoPneu { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataMovimentacao", Column = "VPN_DATA_MOVIMENTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataMovimentacao { get; set; }

        public virtual string Descricao
        {
            get { return Pneu.NumeroFogo; }
        }

        public virtual bool Equals(VeiculoPneu other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
