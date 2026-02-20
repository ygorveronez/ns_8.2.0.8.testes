namespace Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_TRANSFERENCIA_PALLET", EntityName = "AprovacaoAlcadaTransferenciaPallet", Name = "Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AprovacaoAlcadaTransferenciaPallet", NameType = typeof(AprovacaoAlcadaTransferenciaPallet))]
    public class AprovacaoAlcadaTransferenciaPallet : RegraAutorizacao.AprovacaoAlcada<Transferencia.TransferenciaPallet, RegraAutorizacaoTransferenciaPallet>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TransferenciaPallet", Column = "PAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Transferencia.TransferenciaPallet OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoTransferenciaPallet", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoTransferenciaPallet RegraAutorizacao { get; set; }

        #endregion
    }
}
