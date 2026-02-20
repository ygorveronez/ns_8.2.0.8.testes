namespace Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_TAXA_DESCARGA_TIPO_DE_CARGA", EntityName = "AlcadasTaxaDescarga.AlcadaTipoDeCarga", Name = "Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTipoDeCarga", NameType = typeof(AlcadaTipoDeCarga))]
    public class AlcadaTipoDeCarga : RegraAutorizacao.Alcada<RegraAutorizacaoTaxaDescarga, Entidades.Embarcador.Cargas.TipoDeCarga>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Cargas.TipoDeCarga PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoTaxaDescarga", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoTaxaDescarga RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
