using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_DESCARREGAMENTO_HORARIO_APROVACAO_AUTOMATICA_DESCARREGAMETO", EntityName = "HorarioAprovacaoAutomaticaDescarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento", NameType = typeof(HorarioAprovacaoAutomaticaDescarregamento))]
    public class HorarioAprovacaoAutomaticaDescarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CHA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "CHA_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "CHA_DATA_FINAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFinal { get; set; }
    }
}
