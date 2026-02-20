using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO", EntityName = "MovimentoFinanceiroDebitoCredito", Name = "Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito", NameType = typeof(MovimentoFinanceiroDebitoCredito))]
    public class MovimentoFinanceiroDebitoCredito : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataMovimento", Column = "MDC_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracaoMovimento", Column = "MDC_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataGeracaoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "MDC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MovimentoFinanceiro", Column = "MOV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro MovimentoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoDeConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DebitoCredito", Column = "MDC_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito DebitoCredito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDC_MOVIMENTO_CONCOLIDADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MovimentoConcolidado { get; set; }

        public virtual string DescricaoDebitoCredito
        {
            get
            {
                switch (this.DebitoCredito)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito:
                        return "Débito";
                    case ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito:
                        return "Crédito";                    
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.MovimentoFinanceiro?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(MovimentoFinanceiroDebitoCredito other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
