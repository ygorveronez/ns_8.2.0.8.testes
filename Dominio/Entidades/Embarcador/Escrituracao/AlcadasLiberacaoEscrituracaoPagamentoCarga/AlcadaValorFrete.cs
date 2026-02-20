namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_LIBERACAO_ESCRITURACAO_PAGAMENTO_CARGA_VALOR_FRETE", EntityName = "AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaValorFrete", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AlcadaValorFrete", NameType = typeof(AlcadaValorFrete))]
    public class AlcadaValorFrete : RegraAutorizacao.Alcada<RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga, decimal>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public override decimal PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}

