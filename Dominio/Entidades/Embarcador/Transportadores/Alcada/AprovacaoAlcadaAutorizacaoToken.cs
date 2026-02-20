
namespace Dominio.Entidades.Embarcador.Transportadores.Alcada
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_APROVACAO_ALCADA_AUTORIZACAO_TOKEN", EntityName = "AprovacaoAlcadaAutorizacaoToken", Name = "Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken", NameType = typeof(AprovacaoAlcadaAutorizacaoToken))]
    public class AprovacaoAlcadaAutorizacaoToken : RegraAutorizacao.AprovacaoAlcada<SolicitacaoToken, RegraAutorizacaoToken>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoToken", Column = "STO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override SolicitacaoToken OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoToken", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoToken RegraAutorizacao { get; set; }

        #endregion
    }
}
