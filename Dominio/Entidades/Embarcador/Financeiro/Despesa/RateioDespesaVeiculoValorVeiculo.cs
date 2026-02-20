using System;


namespace Dominio.Entidades.Embarcador.Financeiro.Despesa
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RATEIO_DESPESA_VEICULO_VEICULO", EntityName = "RateioDespecaVeiculoVeiculo", Name = "Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo", NameType = typeof(RateioDespesaVeiculoValorVeiculo ))]
    public class RateioDespesaVeiculoValorVeiculo : EntidadeBase, IEquatable<RateioDespesaVeiculoValorVeiculo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RDV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioDespesaVeiculo", Column = "TRD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RateioDespesaVeiculo DespesaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "RDV_VALOR", TypeType = typeof(Decimal), NotNull = false)]
        public virtual Decimal Valor { get; set; }

        public virtual bool Equals(RateioDespesaVeiculoValorVeiculo other)
        {
           return other.Codigo == this.Codigo ? true : false;
        }
    }

}


