namespace Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_LEILAO", EntityName = "AprovacaoAlcadaLeilao", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao", NameType = typeof(AprovacaoAlcadaLeilao))]
    public class AprovacaoAlcadaLeilao : RegraAutorizacao.AprovacaoAlcada<CargaJanelaCarregamento, RegraAutorizacaoLeilao>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamento", Column = "CJC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CargaJanelaCarregamento OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoLeilao", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoLeilao RegraAutorizacao { get; set; }

        #endregion
    }
}
