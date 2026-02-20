using System;

namespace Dominio.Entidades.Embarcador.GestaoViagem
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_VIAGEM_ENTREGA", EntityName = "ViagemEntrega", Name = "Dominio.Entidades.Embarcador.GestaoViagem.ViagemEntrega", NameType = typeof(ViagemEntrega))]
    public class ViagemEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VEN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "VEN_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "VEN_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioPrevisao", Column = "VEN_DATA_INICIO_PREVISAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicioPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioReprogramada", Column = "VEN_DATA_INICIO_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicioReprogramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "VEN_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimPrevisao", Column = "VEN_DATA_FIM_PREVISAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFimPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimReprogramada", Column = "VEN_DATA_FIM_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFimReprogramada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Viagem", Column = "VIA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GestaoViagem.Viagem Viagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }


        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}
