namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GUARITA_ANEXO", EntityName = "GuaritaAnexo", Name = "Dominio.Entidades.Embarcador.GestaoPatio.GuaritaAnexo", NameType = typeof(GuaritaAnexo))]
    public class GuaritaAnexo : Anexo.Anexo<Cargas.CargaJanelaCarregamentoGuarita>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoGuarita", Column = "CLC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.CargaJanelaCarregamentoGuarita EntidadeAnexo { get; set; }

        #endregion
    }
}
