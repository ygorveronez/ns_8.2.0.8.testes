using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_FISCAL", EntityName = "DocumentoFiscal", Name = "Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal", NameType = typeof(DocumentoFiscal))]
    public class DocumentoFiscal : EntidadeCargaBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IDF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDF_DATA_DOCUMENTO_FISCAL_INFORMADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocumentoFiscalInformado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaDocumentoFiscalLiberada", Column = "IDF_DOCUMENTO_FISCAL_LIBERADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EtapaDocumentoFiscalLiberada { get; set; }

        /// <summary>
        /// Números dos documentos sumarizados
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "IDF_NUMERO_DOCUMENTO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "NumerosDocumentos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_FISCAL_NUMERO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IDF_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "IDF_NUMERO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual ICollection<string> NumerosDocumentos { get; set; }

        public virtual string Descricao
        {
            get
            { 
                return Carga != null ? $"Documento fiscal da carga {Carga.CodigoCargaEmbarcador}" : $"Documento fiscal da pré carga {PreCarga.NumeroPreCarga}";
            }
        }
    }
}
