namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ANEXO", EntityName = "CargaAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaAnexo", NameType = typeof(CargaAnexo))]
    public class CargaAnexo : Anexo.Anexo<Carga>
    {
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Carga EntidadeAnexo { get; set; }

    }
}
