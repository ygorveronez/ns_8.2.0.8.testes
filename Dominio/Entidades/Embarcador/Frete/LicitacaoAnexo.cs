namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LICITACAO_ANEXO", EntityName = "LicitacaoAnexo", Name = "Dominio.Entidades.Embarcador.Frete.LicitacaoAnexo", NameType = typeof(LicitacaoAnexo))]
    public class LicitacaoAnexo : Anexo.Anexo<Licitacao>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Licitacao", Column = "LIC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Licitacao EntidadeAnexo { get; set; }

        #endregion
    }
}
