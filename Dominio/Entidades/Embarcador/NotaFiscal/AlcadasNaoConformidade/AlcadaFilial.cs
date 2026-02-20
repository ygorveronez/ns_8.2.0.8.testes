namespace Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_NAO_CONFORMIDADE_FILIAL", EntityName = "AlcadasNaoConformidade.AlcadaFilial", Name = "Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaFilial", NameType = typeof(AlcadaFilial))]
    public class AlcadaFilial : RegraAutorizacao.Alcada<RegraAutorizacaoNaoConformidade, Filiais.Filial>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Filiais.Filial PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoNaoConformidade", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoNaoConformidade RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
