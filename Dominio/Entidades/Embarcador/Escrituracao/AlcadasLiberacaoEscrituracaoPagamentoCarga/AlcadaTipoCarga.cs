namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_LIBERACAO_ESCRITURACAO_PAGAMENTO_CARGA_TIPO_CARGA", EntityName = "AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaTipoCarga", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaTipoCarga", NameType = typeof(AlcadaTipoCarga))]
    public class AlcadaTipoCarga : RegraAutorizacao.Alcada<RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga, Cargas.TipoDeCarga>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.TipoDeCarga PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
