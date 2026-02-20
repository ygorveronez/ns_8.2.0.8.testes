using System;

namespace Dominio.Entidades.Embarcador.Financeiro.Despesa
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RATEIO_DESPESA_VEICULO_LANCAMENTO_DIA", EntityName = "RateioDespesaVeiculoLancamentoDia", Name = "Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia", NameType = typeof(RateioDespesaVeiculoLancamentoDia))]
    public class RateioDespesaVeiculoLancamentoDia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "RLD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioDespesaVeiculoLancamento", Column = "TRL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RateioDespesaVeiculoLancamento Lancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RLD_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RLD_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
