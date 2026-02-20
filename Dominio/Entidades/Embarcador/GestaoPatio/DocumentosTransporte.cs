using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTOS_TRANSPORTE", EntityName = "DocumentosTransporte", Name = "Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte", NameType = typeof(DocumentosTransporte))]
    public class DocumentosTransporte : EntidadeCargaBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IDT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDT_DATA_DOCUMENTOS_TRANSPORTE_INFORMADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocumentosTransporteInformado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaDocumentosTransporteLiberada", Column = "IDT_DOCUMENTOS_TRANSPORTE_LIBERADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EtapaDocumentosTransporteLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDT_NUMERO_CTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDT_NUMERO_MDFE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDT_BRIX", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Brix { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDT_RATIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Ratio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDT_OLEO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Oleo { get; set; }

        public virtual string Descricao
        {
            get { return Carga != null ? $"Documentos de transporte da carga {Carga.CodigoCargaEmbarcador}" : $"Documentos de transporte da pré carga {PreCarga.NumeroPreCarga}"; }
        }
    }
}
