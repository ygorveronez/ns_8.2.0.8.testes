using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EVENTOS_DO_TRANSPORTE", EntityName = "EventosDT", Name = "Dominio.Entidades.Embarcador.Logistica.EventosDT", NameType = typeof(EventosDT))]
    public class EventosDT : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EDT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Qualificador", Column = "EDT_QUALIFICADOR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Qualificador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDT_DATA_INICIO_PREVISTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioPrevisto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDT_DATA_FIM_PREVISTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimPrevisto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDT_DATA_INICIO_REAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioReal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDT_DATA_FIM_REAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimReal { get; set; }
    }
}