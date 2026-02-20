namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECKLIST_ANEXO", EntityName = "CheckListAnexo", Name = "Dominio.Entidades.Embarcador.GestaoPatio.CheckListAnexo", NameType = typeof(CheckListAnexo))]
    public class CheckListAnexo : Anexo.Anexo<CheckListCarga>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListCarga", Column = "CLC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CheckListCarga EntidadeAnexo { get; set; }

        #endregion
    }
}
