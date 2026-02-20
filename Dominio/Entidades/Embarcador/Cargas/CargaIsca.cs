namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ISCA", EntityName = "CargaIsca", Name = "Dominio.Entidades.Embarcador.Cargas.CargaIsca", NameType = typeof(CargaIsca))]
    public class CargaIsca : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Isca", Column = "ISC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Isca Isca { get; set; }

        public virtual string Descricao
        {
            get { return $"{Isca.Descricao} - {Carga.CodigoCargaEmbarcador}" ; }
        }
    }
}
