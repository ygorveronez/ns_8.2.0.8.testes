namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_LIBERACAO_ESCRITURACAO_PAGAMENTO_CARGA_TIPO_OPERACAO", EntityName = "AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaTipoOperacao", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaTipoOperacao", NameType = typeof(AlcadaTipoOperacao))]
    public class AlcadaTipoOperacao : RegraAutorizacao.Alcada<RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga, Pedidos.TipoOperacao>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Pedidos.TipoOperacao PropriedadeAlcada { get; set; }

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
