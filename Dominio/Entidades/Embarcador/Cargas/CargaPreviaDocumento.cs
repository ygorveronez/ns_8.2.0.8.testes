using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PREVIA_DOCUMENTO", EntityName = "CargaPreviaDocumento", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento", NameType = typeof(CargaPreviaDocumento))]
    public class CargaPreviaDocumento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PREVIA_DOCUMENTO_DOCUMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPreviaDocumentoDocumento", Column = "CPN_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumentoDocumento> Documentos { get; set; }
    }
}
