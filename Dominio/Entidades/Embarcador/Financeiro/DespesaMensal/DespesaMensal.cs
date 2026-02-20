using System;

namespace Dominio.Entidades.Embarcador.Financeiro.DespesaMensal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DESPESA_MENSAL", EntityName = "DespesaMensal", Name = "Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal", NameType = typeof(DespesaMensal))]
    public class DespesaMensal : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DME_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "DME_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaProvisao", Column = "DME_DIA_PROVISAO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProvisao", Column = "DME_VALOR_PROVISAO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "DME_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "DME_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoPagamentoRecebimento", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoPagamentoRecebimento TipoPagamentoRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDespesaFinanceira", Column = "TID_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Despesa.TipoDespesaFinanceira TipoDespesaFinanceira { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual bool Equals(DespesaMensal other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
