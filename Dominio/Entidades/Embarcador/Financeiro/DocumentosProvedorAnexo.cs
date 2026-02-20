namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_PROVEDOR_ANEXO", EntityName = "DocumentosProvedorAnexo", Name = "Dominio.Entidades.Embarcador.Financeiro.DocumentosProvedorAnexo", NameType = typeof(DocumentosProvedorAnexo))]
    public class DocumentosProvedorAnexo : Anexo.Anexo<PagamentoProvedor>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoProvedor", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PagamentoProvedor EntidadeAnexo { get; set; }

        #endregion
    }
}