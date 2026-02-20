namespace Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_APROVACAO_ALCADA_SOLICITACAO_GAS", EntityName = "AprovacaoAlcadaSolicitacaoGas", Name = "Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas", NameType = typeof(AprovacaoAlcadaSolicitacaoGas))]
    public class AprovacaoAlcadaSolicitacaoGas : RegraAutorizacao.AprovacaoAlcada<SolicitacaoAbastecimentoGas, RegraAprovacaoSolicitacaoGas>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoAbastecimentoGas", Column = "SAG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override SolicitacaoAbastecimentoGas OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAprovacaoSolicitacaoGas", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAprovacaoSolicitacaoGas RegraAutorizacao { get; set; }

        #endregion
    }
}
