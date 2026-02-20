namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_LOTE_CARGA_OCORRENCIA", EntityName = "OcorrenciaLoteCargaOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia", NameType = typeof(OcorrenciaLoteCargaOcorrencia))]
    public class OcorrenciaLoteCargaOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OLC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaLote", Column = "OLO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OcorrenciaLote OcorrenciaLote { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OLC_VALOR_OCORRENCIA_RATEADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOcorrenciaRateado { get; set; }
    }
}
