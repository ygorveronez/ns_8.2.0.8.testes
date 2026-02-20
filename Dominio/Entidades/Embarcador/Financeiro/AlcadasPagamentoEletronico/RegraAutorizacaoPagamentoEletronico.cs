using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_PAGAMENTO_ELETRONICO", EntityName = "RegraAutorizacaoPagamentoEletronico", Name = "Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico", NameType = typeof(RegraAutorizacaoPagamentoEletronico))]
    public class RegraAutorizacaoPagamentoEletronico : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFornecedor", Column = "RAT_FORNECEDOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorBoletoConfiguracao", Column = "RAT_BOLETO_CONFIGURACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorBoletoConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValor", Column = "RAT_VALOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFornecedor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_PAGAMENTO_ELETRONICO_FORNECEDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasPagamentoEletronico.AlcadaFornecedor", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFornecedor> AlcadasFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasBoletoConfiguracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_PAGAMENTO_ELETRONICO_BOLETO_CONFIGURACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasPagamentoEletronico.AlcadaBoletoConfiguracao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaBoletoConfiguracao> AlcadasBoletoConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_PAGAMENTO_ELETRONICO_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasPagamentoEletronico.AlcadaValor", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValor> AlcadasValor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PAGAMENTO_ELETRONICO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorFornecedor || RegraPorBoletoConfiguracao || RegraPorValor;
        }

        public override void LimparAlcadas()
        {
            AlcadasFornecedor?.Clear();
            AlcadasBoletoConfiguracao?.Clear();
            AlcadasValor?.Clear();
        }

        #endregion
    }
}
