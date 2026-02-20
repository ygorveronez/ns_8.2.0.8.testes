using System;

namespace Dominio.Entidades.Embarcador.Financeiro.DespesaMensal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DESPESA_MENSAL_PROCESSAMENTO_DESPESAS", EntityName = "DespesaMensalProcessamentoDespesas", Name = "Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas", NameType = typeof(DespesaMensalProcessamentoDespesas))]
    public class DespesaMensalProcessamentoDespesas : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DPD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPago", Column = "DPD_VALOR_PAGO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorPago { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DespesaMensal", Column = "DME_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DespesaMensal DespesaMensal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DespesaMensalProcessamento", Column = "DMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DespesaMensalProcessamento DespesaMensalProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Titulo Titulo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.DespesaMensal.Descricao.ToString() + " - Valor Pago: " + this.ValorPago.ToString("n2");
            }
        }

        public virtual bool Equals(DespesaMensalProcessamentoDespesas other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
