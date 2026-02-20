namespace Dominio.Entidades.Embarcador.Fatura.AlcadasFatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_FATURA_TOMADOR", EntityName = "AlcadasFatura.AlcadaTomador", Name = "Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaTomador", NameType = typeof(AlcadaTomador))]
    public class AlcadaTomador : RegraAutorizacao.Alcada<RegraAutorizacaoFatura, Cliente>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cliente PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoFatura", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoFatura RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
