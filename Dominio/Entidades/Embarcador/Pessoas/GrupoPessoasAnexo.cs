namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_ANEXO", EntityName = "GrupoPessoasAnexo", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasAnexo", NameType = typeof(GrupoPessoasAnexo))]
    public class GrupoPessoasAnexo : Anexo.Anexo<GrupoPessoas>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override GrupoPessoas EntidadeAnexo { get; set; }

        #endregion
    }
}
