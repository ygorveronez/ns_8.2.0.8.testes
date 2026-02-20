namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURA_CANCELAMENTO_ANEXO", EntityName = "FaturaCancelamentoAnexo", Name = "Dominio.Entidades.Embarcador.Fatura.FaturaCancelamentoAnexo", NameType = typeof(FaturaCancelamentoAnexo))]
    public class FaturaCancelamentoAnexo : Anexo.Anexo<Fatura>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Fatura EntidadeAnexo { get; set; }

        #endregion
    }
}
