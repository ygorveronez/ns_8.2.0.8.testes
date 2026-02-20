using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_MOVIMENTACAO_PNEU", EntityName = "Frota.MovimentacaoPneuVeiculo", Name = "Dominio.Entidades.Embarcador.Frota.MovimentacaoPneuVeiculo", NameType = typeof(MovimentacaoPneuVeiculo))]
    public class MovimentacaoPneuVeiculo : EntidadeBase, IEquatable<MovimentacaoPneuVeiculo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MPV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MPV_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MPV_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MPV_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoPneuVeiculo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoPneuVeiculo Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.MovimentacaoPneuVeiculoDadosAdicionais", Column = "MDA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MovimentacaoPneuVeiculoDadosAdicionais DadosAdicionais { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCargaEixoPneu", Column = "MEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCargaEixoPneu EixoPneu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCargaEstepe", Column = "MES_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCargaEstepe Estepe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.Pneu", Column = "PNU_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pneu Pneu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.PneuHistorico", Column = "PNH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PneuHistorico PneuHistorico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        public virtual bool Equals(MovimentacaoPneuVeiculo other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
