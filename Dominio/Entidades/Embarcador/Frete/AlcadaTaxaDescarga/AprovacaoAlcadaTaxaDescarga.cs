namespace Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_TAXA_DESCARGA", EntityName = "AprovacaoAlcadaTaxaDescarga", Name = "Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga", NameType = typeof(AprovacaoAlcadaTaxaDescarga))]
    public class AprovacaoAlcadaTaxaDescarga : RegraAutorizacao.AprovacaoAlcada<ConfiguracaoDescargaCliente, RegraAutorizacaoTaxaDescarga>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoDescargaCliente", Column = "CDC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ConfiguracaoDescargaCliente OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoTaxaDescarga", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoTaxaDescarga RegraAutorizacao { get; set; }

        #endregion
    }
}
