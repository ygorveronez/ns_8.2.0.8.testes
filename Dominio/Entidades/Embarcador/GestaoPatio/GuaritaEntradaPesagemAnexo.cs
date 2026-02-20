namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GUARITA_ENTRADA_PESAGEM_ANEXO", EntityName = "GuaritaEntradaPesagemAnexo", Name = "Dominio.Entidades.Embarcador.GestaoPatio.GuaritaEntradaPesagemAnexo", NameType = typeof(GuaritaEntradaPesagemAnexo))]
    public class GuaritaEntradaPesagemAnexo : Anexo.Anexo<Cargas.CargaJanelaCarregamentoGuarita>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoGuarita", Column = "JCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.CargaJanelaCarregamentoGuarita EntidadeAnexo { get; set; }

        #endregion
    }
}
