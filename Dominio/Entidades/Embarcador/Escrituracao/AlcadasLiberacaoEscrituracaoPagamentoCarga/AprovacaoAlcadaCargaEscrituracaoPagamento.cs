using NHibernate.Mapping.Attributes;

namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga
{
    [Class(0, Table = "T_AUTORIZACAO_ALCADA_LIBERACAO_ESCRITURACAO_PAGAMENTO_CARGA", EntityName = "AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga", NameType = typeof(AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga))]
    public class AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga : RegraAutorizacao.AprovacaoAlcada<Cargas.Carga, RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga>
    {
        #region Propriedades Sobrescritas

        [ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = Laziness.Proxy)]
        public override Cargas.Carga OrigemAprovacao { get; set; }

        [ManyToOne(0, Class = "RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga", Column = "RAT_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public override RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga RegraAutorizacao { get; set; }

        #endregion
    }
}
