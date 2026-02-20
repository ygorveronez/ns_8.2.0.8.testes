namespace Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_CONTRATO_FRETE_TERCEIRO_ACRESCIMO_DESCONTO", EntityName = "AprovacaoAlcadaContratoFreteAcrescimoDesconto", Name = "Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto", NameType = typeof(AprovacaoAlcadaContratoFreteAcrescimoDesconto))]
    public class AprovacaoAlcadaContratoFreteAcrescimoDesconto : RegraAutorizacao.AprovacaoAlcada<ContratoFreteAcrescimoDesconto, RegraAutorizacaoContratoFreteAcrescimoDesconto>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteAcrescimoDesconto", Column = "CAD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ContratoFreteAcrescimoDesconto OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoContratoFreteAcrescimoDesconto", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoContratoFreteAcrescimoDesconto RegraAutorizacao { get; set; }

        #endregion
    }
}
