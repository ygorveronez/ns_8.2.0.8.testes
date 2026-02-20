namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OCORRENCIA_CONTRATO_MOTORISTA", EntityName = "OcorrenciaContratoMotorista", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoMotorista", NameType = typeof(OcorrenciaContratoMotorista))]
    public class OcorrenciaContratoMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia Ocorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCM_VALOR_DIARIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorDiaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCM_VALOR_QUINZENA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorQuinzena { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCM_QUANTIDADE_MOTORISTAS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeMotoristas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCM_QUANTIDADE_DIAS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCM_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Total { get; set; }
    }
}
