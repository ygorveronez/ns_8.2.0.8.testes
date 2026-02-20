namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_LACRE", EntityName = "CargaLacre", Name = "Dominio.Entidades.Embarcador.Cargas.CargaLacre", NameType = typeof(CargaLacre))]
    public class CargaLacre : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CLA_NUMERO", TypeType = typeof(string), Length = 60, NotNull = true)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoLacre", Column = "TLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoLacre TipoLacre { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        public virtual string Descricao
        {
            get { return Numero.ToString(); }
        }
    }
}
