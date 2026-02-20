namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ANEXO_GRUPO_PESSOAS_LAYOUT_EDI", EntityName = "AnexoGrupoPessoasLayoutEDI", Name = "Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI", NameType = typeof(AnexoGrupoPessoasLayoutEDI))]
    public class AnexoGrupoPessoasLayoutEDI : Anexo.Anexo<GrupoPessoasLayoutEDI>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoasLayoutEDI", Column = "GLY_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override GrupoPessoasLayoutEDI EntidadeAnexo { get; set; }

        #endregion
    }
}