namespace Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_TOLERANCIA_PESAGEM_TIPO_CARGA", EntityName = "AlcadasToleranciaPesagem.AlcadaTipoCarga", Name = "Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaTipoCarga", NameType = typeof(AlcadaTipoCarga))]
    public class AlcadaTipoCarga : RegraAutorizacao.Alcada<RegrasAutorizacaoToleranciaPesagem, Cargas.TipoDeCarga>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.TipoDeCarga PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoToleranciaPesagem", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegrasAutorizacaoToleranciaPesagem RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
