namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ANEXO_PRODUTOR", EntityName = "AnexoProdutor", Name = "Dominio.Entidades.Embarcador.GestaoPatio.AnexoProdutor", NameType = typeof(AnexoProdutor))]
    public class AnexoProdutor : Anexo.Anexo<Cargas.Carga>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga EntidadeAnexo { get; set; }

        #endregion
    }
}
