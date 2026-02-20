using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TURNO_HORARIO_ACESSO", EntityName = "TurnoHorarioAcesso", Name = "Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso", NameType = typeof(TurnoHorarioAcesso))]
    public class TurnoHorarioAcesso : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "THA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Turno", Column = "TUR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Turno Turno { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "DiasDaSemana", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TURNO_HORARIO_ACESSO_DIA_SEMANA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "THA_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "THA_DIA_SEMANA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = true)]
        public virtual ICollection<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> DiasDaSemana { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicial", Column = "THA_HORA_INICIAL", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraFinal", Column = "THA_HORA_FINAL", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraFinal { get; set; }

        public virtual string Descricao
        {
            get { return $"Horário de acesso {Codigo}"; }
        }
    }
}
