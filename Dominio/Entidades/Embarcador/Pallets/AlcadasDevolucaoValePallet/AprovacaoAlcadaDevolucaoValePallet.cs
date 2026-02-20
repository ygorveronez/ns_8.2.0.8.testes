namespace Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_DEVOLUCAO_VALE_PALLET", EntityName = "AprovacaoAlcadaDevolucaoValePallet", Name = "Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet", NameType = typeof(AprovacaoAlcadaDevolucaoValePallet))]
    public class AprovacaoAlcadaDevolucaoValePallet : RegraAutorizacao.AprovacaoAlcada<DevolucaoValePallet, RegraAutorizacaoDevolucaoValePallet>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DevolucaoValePallet", Column = "DVP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override DevolucaoValePallet OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoDevolucaoValePallet", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoDevolucaoValePallet RegraAutorizacao { get; set; }

        #endregion
    }
}
