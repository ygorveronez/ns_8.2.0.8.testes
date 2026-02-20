namespace Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_AVARIA_PALLET", EntityName = "AprovacaoAlcadaAvaria", Name = "Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria", NameType = typeof(AprovacaoAlcadaAvaria))]
    public class AprovacaoAlcadaAvaria : RegraAutorizacao.AprovacaoAlcada<Avaria.AvariaPallet, RegraAutorizacaoAvaria>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AvariaPallet", Column = "PAV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Avaria.AvariaPallet OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoAvaria", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoAvaria RegraAutorizacao { get; set; }

        #endregion
    }
}
