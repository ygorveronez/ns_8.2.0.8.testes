namespace Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CARREGAMENTO_TIPO_CARGA", EntityName = "AlcadasMontagemCarga.AlcadaTipoCarga", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaTipoCarga", NameType = typeof(AlcadaTipoCarga))]
    public class AlcadaTipoCarga : RegraAutorizacao.Alcada<RegraAutorizacaoCarregamento, TipoDeCarga>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override TipoDeCarga PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCarregamento", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCarregamento RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}

