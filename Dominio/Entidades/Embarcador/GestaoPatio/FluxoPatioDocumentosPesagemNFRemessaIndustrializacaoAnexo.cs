namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_PATIO_DOCUMENTOS_PESAGEM_NF_REMESSA_INDUSTRIALIZACAO_ANEXO", EntityName = "FluxoPatioDocumentosPesagemNFRemessaIndustrializacaoAnexo", Name = "Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioDocumentosPesagemNFRemessaIndustrializacaoAnexo", NameType = typeof(FluxoPatioDocumentosPesagemNFRemessaIndustrializacaoAnexo))]
    public class FluxoPatioDocumentosPesagemNFRemessaIndustrializacaoAnexo : Anexo.Anexo<FluxoGestaoPatio>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override FluxoGestaoPatio EntidadeAnexo { get; set; }

        #endregion
    }
}
