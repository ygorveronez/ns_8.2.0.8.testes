namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_ANEXO", EntityName = "EmpresaAnexo", Name = "Dominio.Entidades.EmpresaAnexo", NameType = typeof(EmpresaAnexo))]
    public class EmpresaAnexo : Dominio.Entidades.Embarcador.Anexo.Anexo<Empresa>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Empresa EntidadeAnexo { get; set; }

        #endregion
    }
}
