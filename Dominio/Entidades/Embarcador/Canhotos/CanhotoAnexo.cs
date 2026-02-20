namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANHOTO_ANEXO", EntityName = "CanhotoAnexo", Name = "Dominio.Entidades.Embarcador.Canhotos.CanhotoAnexo", NameType = typeof(CanhotoAnexo))]
    public class CanhotoAnexo : Anexo.Anexo<Canhoto>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Canhoto", Column = "CNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Canhoto EntidadeAnexo { get; set; }

        #endregion
    }
}
