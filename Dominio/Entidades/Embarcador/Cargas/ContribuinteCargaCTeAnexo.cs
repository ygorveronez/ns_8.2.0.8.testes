namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRIBUINTE_CARGA_CTE_ANEXO", EntityName = "ContribuinteCargaCTeAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.ContribuinteCargaCTeAnexo", NameType = typeof(ContribuinteCargaCTeAnexo))]
    public class ContribuinteCargaCTeAnexo : Anexo.Anexo<Dominio.Entidades.Empresa>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Empresa EntidadeAnexo { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaCTe CargaCTe { get; set; }

    }
}
