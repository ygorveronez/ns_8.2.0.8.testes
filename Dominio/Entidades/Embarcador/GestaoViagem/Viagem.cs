using System;

namespace Dominio.Entidades.Embarcador.GestaoViagem
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_VIAGEM", EntityName = "Viagem", Name = "Dominio.Entidades.Embarcador.GestaoViagem.Viagem", NameType = typeof(Viagem))]
    public class Viagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "VIA_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "VIA_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioPrevisao", Column = "VIA_DATA_INICIO_PREVISAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicioPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioReprogramada", Column = "VIA_DATA_INICIO_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicioReprogramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "VIA_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimPrevisao", Column = "VIA_DATA_FIM_PREVISAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFimPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimReprogramada", Column = "VIA_DATA_FIM_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFimReprogramada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}
