namespace Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_NAO_CONFORMIDADE_CFOP", EntityName = "AlcadasNaoConformidade.AlcadaCFOP", Name = "Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaCFOP", NameType = typeof(AlcadaCFOP))]
    public class AlcadaCFOP : RegraAutorizacao.Alcada<RegraAutorizacaoNaoConformidade, CFOP>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CFOP PropriedadeAlcada { get; set; }

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
