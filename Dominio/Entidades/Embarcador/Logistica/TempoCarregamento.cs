using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_TEMPO_CARREGAMENTO", EntityName = "TempoCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.TempoCarregamento", NameType = typeof(TempoCarregamento))]
    public class TempoCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tempo", Column = "TEC_TEMPO", TypeType = typeof(int), NotNull = true)]
        public virtual int Tempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicio", Column = "TEC_HORA_INICIO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraTermino", Column = "TEC_HORA_TERMINO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraTermino { get; set; }

        /// <summary>
        /// Atributo utilizado para processo de montagem de carga automático, para validar o máximo de destinos de um carregamento por tipo de carga e modelo veicular...
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TEC_QTDE_MAX_ENTREGAS_ROTEIRIZAR", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeMaximaEntregasRoteirizar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TEC_QUANTIDADE_VAGAS_OCUPAR_NA_GRADE_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeVagasOcuparGradeNaCarregamento { get; set; }


        /// <summary>
        /// Atributo utilizado para processo de montagem de carga automático, para validar o minimo de destinos de um carregamento modelo veicular...
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TEC_QTDE_MIN_ENTREGAS_ROTEIRIZAR", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeMinimaEntregasRoteirizar { get; set; }

        public virtual string Descricao
        {
            get { return this.TipoCarga.Descricao + " - " + ModeloVeicular.Descricao; }
        }

        public virtual string PeriodoCarregamento
        {
            get
            {
                if (HoraInicio.HasValue && HoraTermino.HasValue)
                    return $"Das {HoraInicio.Value.ToString(@"hh\:mm")} até as {HoraTermino.Value.ToString(@"hh\:mm")}";

                if (HoraInicio.HasValue)
                    return $"À partir das {HoraInicio.Value.ToString(@"hh\:mm")}";

                if (HoraTermino.HasValue)
                    return $"Até as {HoraTermino.Value.ToString(@"hh\:mm")}";

                return string.Empty;
            }
        }
    }
}
