namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO_CANCELAMENTO_ANEXO", EntityName = "TituloCancelamentoAnexo", Name = "Dominio.Entidades.Embarcador.Financeiro.TituloCancelamentoAnexo", NameType = typeof(TituloCancelamentoAnexo))]
    public class TituloCancelamentoAnexo : Anexo.Anexo<Titulo>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Titulo EntidadeAnexo { get; set; }

        #endregion
    }
}