namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ANEXO_GRUPO_PESSOAS_FATURA_CANHOTO", EntityName = "AnexoGrupoPessoasFaturaCanhoto", Name = "Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto", NameType = typeof(AnexoGrupoPessoasFaturaCanhoto))]
    public class AnexoGrupoPessoasFaturaCanhoto : Anexo.Anexo<GrupoPessoasFaturaCanhoto>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoasFaturaCanhoto", Column = "GFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override GrupoPessoasFaturaCanhoto EntidadeAnexo { get; set; }

        #endregion
    }
}