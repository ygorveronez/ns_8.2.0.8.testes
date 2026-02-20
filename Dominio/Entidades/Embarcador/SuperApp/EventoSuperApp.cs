using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.SuperApp
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_EVENTO_SUPER_APP", EntityName = "EventoSuperApp", Name = "Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp", NameType = typeof(EventoSuperApp))]
    public class EventoSuperApp : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ESA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "ESA_TIPO", TypeType = typeof(TipoEventoSuperApp), NotNull = true)]
        public virtual TipoEventoSuperApp Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Titulo", Column = "ESA_TITULO", TypeType = typeof(string), NotNull = true, Length = 100)]
        public virtual string Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Obrigatorio", Column = "ESA_OBRIGATORIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Obrigatorio { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "ESA_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEventoCustomizado", Column = "ESA_TIPO_EVENTO_CUSTOMIZADO", TypeType = typeof(TipoCustomEventAppTrizy), NotNull = false)]
        public virtual TipoCustomEventAppTrizy TipoEventoCustomizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoParada", Column = "ESA_TIPO_PARADA", TypeType = typeof(TipoParadaEventoSuperApp), NotNull = true)]
        public virtual TipoParadaEventoSuperApp TipoParada { get; set; }
       
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChecklistSuperApp", Column = "CSA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp ChecklistSuperApp { get; set; }

    }
}
