using System;

namespace Dominio.Entidades.Embarcador.Financeiro.Despesa
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RATEIO_DESPESA_VEICULO_CENTRO_RESULTADO", EntityName = "RateioDespecaVeiculoCentroResultado", Name = "Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado", NameType = typeof(RateioDespesaVeiculoValorCentroResultado))]
    public class RateioDespesaVeiculoValorCentroResultado : EntidadeBase, IEquatable<RateioDespesaVeiculoValorCentroResultado>
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioDespesaVeiculo", Column = "TRD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RateioDespesaVeiculo DespesaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "RDC_VALOR", TypeType = typeof(Decimal), NotNull = false)]
        public virtual Decimal Valor { get; set; }

        public virtual bool Equals(RateioDespesaVeiculoValorCentroResultado other)
        {
            return other.Codigo == this.Codigo ? true : false;
        }
    }
}
