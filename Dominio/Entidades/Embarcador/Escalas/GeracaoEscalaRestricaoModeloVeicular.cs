using System;

namespace Dominio.Entidades.Embarcador.Escalas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GERACAO_ESCALA_RESTRICAO_MODELO_VEICULAR", EntityName = "GeracaoEscalaRestricaoModeloVeicular", Name = "Dominio.Entidades.Embarcador.Escalas.GeracaoEscalaRestricaoModeloVeicular", NameType = typeof(GeracaoEscalaRestricaoModeloVeicular))]
    public class GeracaoEscalaRestricaoModeloVeicular : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Escalas.GeracaoEscalaRestricaoModeloVeicular>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ERM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GeracaoEscala", Column = "GES_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escalas.GeracaoEscala GeracaoEscala { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicioRestricao", Column = "ERM_HORA_INICIO_RESTRICAO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraInicioRestricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraFimRestricao", Column = "ERM_HORA_TERMINO_RESTRICAO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraFimRestricao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ModeloVeicularCarga?.Descricao ?? string.Empty;
            }
        }
        public virtual bool Equals(GeracaoEscalaRestricaoModeloVeicular other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
