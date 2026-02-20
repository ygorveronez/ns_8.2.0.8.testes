namespace Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_CONTRATO_PRESTACAO_SERVICO", EntityName = "AprovacaoAlcadaContratoPrestacaoServico", Name = "Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AprovacaoAlcadaContratoPrestacaoServico", NameType = typeof(AprovacaoAlcadaContratoPrestacaoServico))]
    public class AprovacaoAlcadaContratoPrestacaoServico : RegraAutorizacao.AprovacaoAlcada<ContratoPrestacaoServico, RegraAutorizacaoContratoPrestacaoServico>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoPrestacaoServico", Column = "CPS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ContratoPrestacaoServico OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoContratoPrestacaoServico", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoContratoPrestacaoServico RegraAutorizacao { get; set; }

        #endregion
    }
}
