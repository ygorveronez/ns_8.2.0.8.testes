namespace Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_NAO_CONFORMIDADE_NAO_CONFORMIDADE", EntityName = "AlcadasNaoConformidade.AlcadaNaoConformidade", Name = "Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaNaoConformidade", NameType = typeof(AlcadaNaoConformidade))]
    public class AlcadaNaoConformidade : RegraAutorizacao.Alcada<RegraAutorizacaoNaoConformidade, ItemNaoConformidade>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ItemNaoConformidade", Column = "INC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ItemNaoConformidade PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoNaoConformidade", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
