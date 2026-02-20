using System;

namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONDICAO_PAGAMENTO", EntityName = "CondicaoPagamento", Name = "Dominio.Entidades.Embarcador.Compras.CondicaoPagamento", NameType = typeof(CondicaoPagamento))]
    public class CondicaoPagamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeParcelas", Column = "COP_QUANTIDADE_PARCELAS", TypeType = typeof(int), NotNull = false)]
		public virtual int QuantidadeParcelas { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "IntervaloDias", Column = "COP_INTERVALO_DIAS", TypeType = typeof(string), NotNull = false)]
		public virtual string IntervaloDias { get; set; }

		//[NHibernate.Mapping.Attributes.Property(0, Name = "PrimeiroVencimento", Column = "COP_PRIMEIROVENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
		//public virtual DateTime PrimeiroVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasParaPrimeiroVencimento", Column = "COP_DIAS_PARA_PRIMEIRO_VENCIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasParaPrimeiroVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
