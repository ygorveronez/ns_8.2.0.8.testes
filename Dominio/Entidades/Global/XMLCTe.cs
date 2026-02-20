namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_XML", EntityName = "XMLCTe", Name = "Dominio.Entidades.XMLCTe", NameType = typeof(XMLCTe))]
    public class XMLCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XML", Column = "CTX_XML", Type = "StringClob", NotNull = true)]
        public virtual string XML { get; set; }

        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CTX_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoXMLCTe), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoXMLCTe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XMLArmazenadoEmArquivo", Column = "CTX_XML_ARMAZENADO_EM_ARQUIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool XMLArmazenadoEmArquivo { get; set; }
    }
}
