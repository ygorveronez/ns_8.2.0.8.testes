namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GUARITA_ENTRADA_PESAGEM_FINAL_ANEXO", EntityName = "GuaritaEntradaPesagemFinalAnexo", Name = "Dominio.Entidades.Embarcador.GestaoPatio.GuaritaEntradaPesagemFinalAnexo", NameType = typeof(GuaritaEntradaPesagemFinalAnexo))]
    public class GuaritaEntradaPesagemFinalAnexo : Anexo.Anexo<Cargas.CargaJanelaCarregamentoGuarita>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoGuarita", Column = "JCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.CargaJanelaCarregamentoGuarita EntidadeAnexo { get; set; }

        #endregion
    }
}
