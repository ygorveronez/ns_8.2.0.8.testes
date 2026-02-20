using System;

namespace Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONSULTA_ABASTECIMENTO_ANGELLIRA_VEICULO", EntityName = "ConsultaAbastecimentoAngelLiraVeiculo", Name = "Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLiraVeiculo", NameType = typeof(ConsultaAbastecimentoAngelLiraVeiculo))]

    public class ConsultaAbastecimentoAngelLiraVeiculo : EntidadeBase, IEquatable<ConsultaAbastecimentoAngelLiraVeiculo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConsultaAbastecimentoAngelLira", Column = "CAA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConsultaAbastecimentoAngelLira ConsultaAbastecimentoAngelLira { get; set; }

        public virtual string Descricao
        {
            get { return this.Veiculo.Descricao; }
        }

        public virtual bool Equals(ConsultaAbastecimentoAngelLiraVeiculo other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
