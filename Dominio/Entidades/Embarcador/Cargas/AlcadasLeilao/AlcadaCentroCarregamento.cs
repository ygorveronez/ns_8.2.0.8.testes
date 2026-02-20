namespace Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_LEILAO_CENTRO_CARREGAMENTO", EntityName = "AlcadasLeilao.AlcadaCentroCarregamento", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AlcadaCentroCarregamento", NameType = typeof(AlcadaCentroCarregamento))]
    public class AlcadaCentroCarregamento : RegraAutorizacao.Alcada<RegraAutorizacaoLeilao, Logistica.CentroCarregamento>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Logistica.CentroCarregamento PropriedadeAlcada { get; set; }

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
