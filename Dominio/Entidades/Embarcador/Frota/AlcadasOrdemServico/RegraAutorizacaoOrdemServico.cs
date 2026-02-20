using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_ORDEM_SERVICO", EntityName = "RegraAutorizacaoOrdemServico", Name = "Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico", NameType = typeof(RegraAutorizacaoOrdemServico))]
    public class RegraAutorizacaoOrdemServico : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFornecedor", Column = "RAT_FORNECEDOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorOperador", Column = "RAT_OPERADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValor", Column = "RAT_VALOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOrdemServico", Column = "RAT_TIPO_ORDEM_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTipoOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFornecedor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ORDEM_SERVICO_FORNECEDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasOrdemServico.AlcadaFornecedor", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFornecedor> AlcadasFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasOperador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ORDEM_SERVICO_OPERADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasOrdemServico.AlcadaOperador", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaOperador> AlcadasOperador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ORDEM_SERVICO_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasOrdemServico.AlcadaValor", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValor> AlcadasValor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoOrdemServico", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ORDEM_SERVICO_TIPO_ORDEM_SERVICO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasOrdemServico.AlcadaTipoOrdemServico", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoOrdemServico> AlcadasTipoOrdemServico { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_ORDEM_SERVICO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorFornecedor || RegraPorOperador || RegraPorValor;
        }

        public override void LimparAlcadas()
        {
            AlcadasFornecedor?.Clear();
            AlcadasOperador?.Clear();
            AlcadasValor?.Clear();
        }

        #endregion
    }
}
