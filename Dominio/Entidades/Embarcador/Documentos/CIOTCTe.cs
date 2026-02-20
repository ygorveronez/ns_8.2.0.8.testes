namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_CTE", EntityName = "CIOTCTe", Name = "Dominio.Entidades.Embarcador.Documentos.CIOTCTe", NameType = typeof(CIOTCTe))]
    public class CIOTCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CIOT", Column = "CIO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Documentos.CIOT CIOT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = true, Unique = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "CIC_INTEGRADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIC_PROTOCOLO_DOCUMENTO_FISCAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ProtocoloDocumentoFiscal { get; set; }
    }
}
