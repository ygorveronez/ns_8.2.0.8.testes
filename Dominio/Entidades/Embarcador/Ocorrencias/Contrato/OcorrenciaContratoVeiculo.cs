namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OCORRENCIA_CONTRATO_VEICULO", EntityName = "OcorrenciaContratoVeiculo", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo", NameType = typeof(OcorrenciaContratoVeiculo))]
    public class OcorrenciaContratoVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia Ocorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCV_VALOR_DIARIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorDiaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCV_VALOR_QUINZENA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorQuinzena { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCV_QUANTIDADE_DIAS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCV_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Total { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCV_QUANTIDADE_DOCUMENTOS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCV_VALOR_DOCUMENTOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorDocumentos { get; set; }
    }
}
