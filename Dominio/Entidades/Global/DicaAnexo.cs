namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DICA_ANEXOS", EntityName = "DicaAnexo", Name = "Dominio.Entidades.DicaAnexo", NameType = typeof(DicaAnexo))]
    public class DicaAnexo : Embarcador.Anexo.Anexo<Dica>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Dica", Column = "DIC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dica EntidadeAnexo { get; set; }

        #endregion

    }
}
