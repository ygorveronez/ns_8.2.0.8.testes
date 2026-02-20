namespace Dominio.Entidades.Embarcador.Fatura.AlcadasFatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_FATURA", EntityName = "AprovacaoAlcadaFatura", Name = "Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura", NameType = typeof(AprovacaoAlcadaFatura))]
    public class AprovacaoAlcadaFatura : RegraAutorizacao.AprovacaoAlcada<Fatura, RegraAutorizacaoFatura>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Fatura OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoFatura", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoFatura RegraAutorizacao { get; set; }

        #endregion
    }
}
