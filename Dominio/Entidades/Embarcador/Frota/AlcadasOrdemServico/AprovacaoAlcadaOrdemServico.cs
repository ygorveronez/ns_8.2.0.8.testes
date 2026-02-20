namespace Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_ORDEM_SERVICO", EntityName = "AprovacaoAlcadaOrdemServico", Name = "Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico", NameType = typeof(AprovacaoAlcadaOrdemServico))]
    public class AprovacaoAlcadaOrdemServico : RegraAutorizacao.AprovacaoAlcada<OrdemServicoFrota, RegraAutorizacaoOrdemServico>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override OrdemServicoFrota OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoOrdemServico", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoOrdemServico RegraAutorizacao { get; set; }

        #endregion
    }
}
