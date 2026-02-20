using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro.Conciliacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONCILIACAO_BANCARIA", EntityName = "ConciliacaoBancaria", Name = "Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria", NameType = typeof(ConciliacaoBancaria))]
    public class ConciliacaoBancaria : EntidadeBase, IEquatable<ConciliacaoBancaria>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "COB_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "COB_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracaoMovimento", Column = "COB_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataGeracaoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Colaborador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COB_REALIZAR_CONCILIACAO_AUTOMATICA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RealizarConciliacaoAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoConciliacaoBancaria", Column = "COB_SITUACAO", TypeType = typeof(SituacaoConciliacaoBancaria), NotNull = false)]
        public virtual SituacaoConciliacaoBancaria SituacaoConciliacaoBancaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalDebitoExtrato", Column = "COB_VALOR_TOTAL_DEBITO_EXTRATO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalDebitoExtrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalCreditoExtrato", Column = "COB_VALOR_TOTAL_CREDITO_EXTRATO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalCreditoExtrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalExtrato", Column = "COB_VALOR_TOTAL_EXTRATO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalExtrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalDebitoMovimento", Column = "COB_VALOR_TOTAL_DEBITO_MOVIMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalDebitoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalCreditoMovimento", Column = "COB_VALOR_TOTAL_CREDITO_MOVIMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalCreditoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMovimento", Column = "COB_VALOR_TOTAL_MOVIMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnaliticoSintetico", Column = "COB_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico AnaliticoSintetico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_SINTETICO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanoConta PlanoContaSintetico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalGeralDebitoMovimento", Column = "COB_VALOR_TOTAL_GERAL_DEBITO_MOVIMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalGeralDebitoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalGeralCreditoMovimento", Column = "COB_VALOR_TOTAL_GERAL_CREDITO_MOVIMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalGeralCreditoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalGeralMovimento", Column = "COB_VALOR_TOTAL_GERAL_MOVIMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalGeralMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalGeralDebitoExtrato", Column = "COB_VALOR_TOTAL_GERAL_DEBITO_EXTRATO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalGeralDebitoExtrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalGeralCreditoExtrato", Column = "COB_VALOR_TOTAL_GERAL_CREDITO_EXTRATO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalGeralCreditoExtrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalGeralExtrato", Column = "COB_VALOR_TOTAL_GERAL_EXTRATO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalGeralExtrato { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Extratos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONCILIACAO_BANCARIA_EXTRATO_BANCARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ExtratoBancario", Column = "EXB_CODIGO")]
        public virtual ICollection<ExtratoBancario> Extratos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Movimentos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONCILIACAO_BANCARIA_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MovimentoFinanceiroDebitoCredito", Column = "MDC_CODIGO")]
        public virtual ICollection<MovimentoFinanceiroDebitoCredito> Movimentos { get; set; }

        public virtual string DescricaoSituacaoConciliacaoBancaria
        {
            get { return SituacaoConciliacaoBancaria.ObterDescricao(); }
        }

        public virtual string Descricao
        {
            get
            {
                return this.PlanoConta != null ? this.PlanoConta.BuscarDescricao + " - " + this.Codigo.ToString() : this.PlanoContaSintetico.BuscarDescricao + " - " + this.Codigo.ToString();
            }
        }

        public virtual bool Equals(ConciliacaoBancaria other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
