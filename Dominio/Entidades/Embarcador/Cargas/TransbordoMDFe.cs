namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRANSBORDO_MDFE", EntityName = "TransbordoMDFe", Name = "Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe", NameType = typeof(TransbordoMDFe))]
    public class TransbordoMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TMD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Transbordo", Column = "TRB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Transbordo Transbordo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmEncerramento", Column = "TMD_EM_ENCERRAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmEncerramento { get; set; }
    }
}
