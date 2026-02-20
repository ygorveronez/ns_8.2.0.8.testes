using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_MUNICIPIO_DESCARREGAMENTO", EntityName = "MunicipioDescarregamentoMDFe", Name = "Dominio.Entidades.MunicipioDescarregamentoMDFe", NameType = typeof(MunicipioDescarregamentoMDFe))]
    public class MunicipioDescarregamentoMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Municipio { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MDD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoMunicipioDescarregamentoMDFe", Column = "MDO_CODIGO")]
        public virtual IList<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> Documentos { get; set; }
    }
}
