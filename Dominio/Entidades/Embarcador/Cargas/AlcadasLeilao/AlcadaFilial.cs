namespace Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_LEILAO_FILIAL", EntityName = "AlcadasLeilao.AlcadaFilial", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaFilial", NameType = typeof(AlcadaFilial))]
    public class AlcadaFilial : RegraAutorizacao.Alcada<RegraAutorizacaoLeilao, Filiais.Filial>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Filiais.Filial PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoLeilao", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoLeilao RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
