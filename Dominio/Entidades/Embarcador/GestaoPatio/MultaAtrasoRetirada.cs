using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MULTA_ATRASO_RETIRADA", EntityName = "MultaAtrasoRetirada", Name = "Dominio.Entidades.Embarcador.GestaoPatio.MultaAtrasoRetirada", NameType = typeof(MultaAtrasoRetirada))]
    public class MultaAtrasoRetirada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MAT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasMultaAtrasoRetirada", Column = "RMA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasMultaAtrasoRetirada RegrasMultaAtrasoRetirada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "MAT_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiberacaoCarga", Column = "MAT_DATA_LIBERACAO_CARGA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataLiberacaoCarga{ get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetiradaCarga", Column = "MAT_DATA_RETIRADA_CARGA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRetiradaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicioPeriodo", Column = "MAT_HORA_INICIO_PERIODO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan HoraInicioPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraTerminoPeriodo", Column = "MAT_HORA_TERMINO_PERIODO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan HoraTerminoPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCargas", Column = "MAT_QUANTIDADE_CARGAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeHorasContrato", Column = "MAT_QUANTIDADE_HORAS_CONTRATO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeHorasContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetiradaNoPeriodo", Column = "MAT_RETIRADA_NO_PERIODO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RetiradaNoPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarOcorrencia", Column = "MAT_GERAR_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrencia { get; set; }
    }
}
