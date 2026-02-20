using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_PARAMETRO_OCORRENCIA", EntityName = "OcorrenciaParametroOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia", NameType = typeof(OcorrenciaParametroOcorrencia))]
    public class OcorrenciaParametroOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OPO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPO_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPO_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPO_OBSERVACAO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPO_TIPO_PARAMETRO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia TipoParametro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPO_DATA_OCORRENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPO_HORA_INCIAL", TypeType = typeof(TimeSpan), NotNull = false)]
        public virtual TimeSpan HoraInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPO_HORA_FINAL", TypeType = typeof(TimeSpan), NotNull = false)]
        public virtual TimeSpan HoraFinal{ get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPO_KM_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KmInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPO_KM_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KmFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPO_OBSERVACAO_CTE", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string ObservacaoCTe { get; set; }


        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }
    }
}
