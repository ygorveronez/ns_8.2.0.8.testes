namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CTE_ANEXO", EntityName = "CargaCTeAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaCTeAnexo", NameType = typeof(CargaCTeAnexo))]
    public class CargaCTeAnexo : Anexo.Anexo<Carga>
    {
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Carga EntidadeAnexo { get; set; }
    }
}
