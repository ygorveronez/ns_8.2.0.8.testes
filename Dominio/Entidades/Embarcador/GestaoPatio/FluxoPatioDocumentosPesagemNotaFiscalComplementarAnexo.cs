namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_PATIO_DOCUMENTOS_PESAGEM_NOTA_FISCAL_COMPLEMENTAR_ANEXO", EntityName = "FluxoPatioDocumentosPesagemNotaFiscalComplementarAnexo", Name = "Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioDocumentosPesagemNotaFiscalComplementarAnexo", NameType = typeof(FluxoPatioDocumentosPesagemNotaFiscalComplementarAnexo))]
    public class FluxoPatioDocumentosPesagemNotaFiscalComplementarAnexo : Anexo.Anexo<FluxoGestaoPatio>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override FluxoGestaoPatio EntidadeAnexo { get; set; }

        #endregion
    }
}
