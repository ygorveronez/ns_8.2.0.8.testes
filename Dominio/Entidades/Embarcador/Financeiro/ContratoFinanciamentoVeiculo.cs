using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FINANCIAMENTO_VEICULO", EntityName = "ContratoFinanciamentoVeiculo", Name = "Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoVeiculo", NameType = typeof(ContratoFinanciamentoVeiculo))]
    public class ContratoFinanciamentoVeiculo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoVeiculo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFinanciamento", Column = "CFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFinanciamento ContratoFinanciamento { get; set; }

        public virtual bool Equals(ContratoFinanciamentoVeiculo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
