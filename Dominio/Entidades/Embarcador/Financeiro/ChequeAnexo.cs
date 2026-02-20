namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHEQUE_ANEXO", EntityName = "ChequeAnexo", Name = "Dominio.Entidades.Embarcador.Financeiro.ChequeAnexo", NameType = typeof(ChequeAnexo))]
    public class ChequeAnexo : Anexo.Anexo<Cheque>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cheque", Column = "CHQ_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cheque EntidadeAnexo { get; set; }

        #endregion
    }
}
