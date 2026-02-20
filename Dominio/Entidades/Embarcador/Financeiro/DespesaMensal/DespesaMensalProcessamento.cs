using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Financeiro.DespesaMensal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DESPESA_MENSAL_PROCESSAMENTO", EntityName = "DespesaMensalProcessamento", Name = "Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento", NameType = typeof(DespesaMensalProcessamento))]
    public class DespesaMensalProcessamento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DMP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mes", Column = "DMP_MES", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.Mes), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.Mes Mes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "DMP_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPagamento", Column = "DMP_DATA_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDespesaFinanceira", Column = "TID_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Despesa.TipoDespesaFinanceira TipoDespesaFinanceira { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DESPESA_MENSAL_PROCESSAMENTO_DESPESAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DespesaMensalProcessamentoDespesas", Column = "DPD_CODIGO")]
        public virtual IList<DespesaMensalProcessamentoDespesas> Despesas { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Mês: " + this.Mes.ToString() + " - Data Geração: " + this.Data.ToString("dd/MM/yyyy");
            }
        }

        public virtual int QuantidadeDespesas
        {
            get
            {
                int qtdTotal = 0;

                if (this.Despesas != null)
                    qtdTotal = (from o in Despesas.ToList() select o.Codigo).Count();

                return qtdTotal;
            }
        }

        public virtual decimal ValorTotalPagar
        {
            get
            {
                decimal valorTotal = 0;

                if (this.Despesas != null)
                    valorTotal = (from o in Despesas.ToList() select o.ValorPago).Sum();

                return valorTotal;
            }
        }

        public virtual bool Equals(DespesaMensalProcessamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
