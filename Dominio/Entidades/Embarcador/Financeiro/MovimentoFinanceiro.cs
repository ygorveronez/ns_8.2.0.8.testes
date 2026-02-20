using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOVIMENTO_FINANCEIRO", EntityName = "MovimentoFinanceiro", Name = "Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro", NameType = typeof(MovimentoFinanceiro))]
    public class MovimentoFinanceiro : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>
    {
        public MovimentoFinanceiro() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// Data em que o movimento ocorreu, ex: data de emissão do CT-e.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataMovimento", Column = "MOV_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataMovimento { get; set; }

        /// <summary>
        /// Somente para informativo da data de inserção do movimento.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracaoMovimento", Column = "MOV_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataGeracaoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "MOV_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Documento", Column = "MOV_DOCUMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Documento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumentoMovimento", Column = "MOV_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento TipoDocumentoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MOV_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_DEBITO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoDeContaDebito { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_CREDITO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoDeContaCredito { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Colaborador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoGeracaoMovimento", Column = "MOV_TIPO_GERACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento TipoGeracaoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Titulo Titulo { get; set; }

        /// <summary>
        /// Data base que ocorreu a movimentação
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBase", Column = "MOV_DATA_BASE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataBase { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAmbiente", Column = "MOV_AMBIENTE", TypeType = typeof(Dominio.Enumeradores.TipoAmbiente), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "MovimentosFinanceirosDebitoCredito", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MovimentoFinanceiroDebitoCredito", Column = "MDC_CODIGO")]
        public virtual IList<MovimentoFinanceiroDebitoCredito> MovimentosFinanceirosDebitoCredito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "MOV_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseCRT", Column = "MOV_DATA_BASE_CRT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaCotacao", Column = "MOV_VALOR_MOEDA_COTACAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginalMoedaEstrangeira", Column = "MOV_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginalMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDespesaFinanceira", Column = "TID_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Despesa.TipoDespesaFinanceira TipoDespesaFinanceira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaTitulo", Column = "MOV_FORMA_TITULO", TypeType = typeof(FormaTitulo), NotNull = false)]
        public virtual FormaTitulo? FormaTitulo { get; set; }

        #region Propriedades Virtuais

        public virtual string DescricaoTipoDocumentoMovimento
        {
            get { return TipoDocumentoMovimento.ObterDescricao(); }
        }

        public virtual string Descricao
        {
            get
            {
                return this.DescricaoTipoDocumentoMovimento;
            }
        }

        public virtual bool Equals(MovimentoFinanceiro other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        #endregion
    }
}
