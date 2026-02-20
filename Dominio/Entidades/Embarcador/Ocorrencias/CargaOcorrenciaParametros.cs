using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OCORRENCIA_PARAMETROS", EntityName = "CargaOcorrenciaParametros", Name = "Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros", NameType = typeof(CargaOcorrenciaParametros))]
    public class CargaOcorrenciaParametros : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParametroOcorrencia", Column = "POC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia ParametroOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Booleano", Column = "COP_BOOLEANO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Booleano { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "COC_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "COC_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalHoras", Column = "COC_TOTAL_HORAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalHoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "COC_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Texto", Column = "COC_TEXTO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string Texto { get; set; }

        public virtual bool Equals(CargaOcorrenciaParametros other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
