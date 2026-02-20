using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERMANENCIA_SUBAREA", EntityName = "PermanenciaSubarea", Name = "Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea", NameType = typeof(PermanenciaSubarea))]
    public class PermanenciaSubarea : EntidadeBase
    {
        public PermanenciaSubarea()
        {
            DataInicio = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PSA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "PSA_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "PSA_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoSegundos", Column = "PSA_TEMPO_SEGUNDOS", TypeType = typeof(int), NotNull = false)]
        public virtual int? TempoSegundos { get; set; }

        public virtual TimeSpan Tempo { 
            get {
                return TimeSpan.FromSeconds(TempoSegundos ?? 0);
            }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Subarea", Class = "SubareaCliente", Column = "SAC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.SubareaCliente Subarea { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "CargaEntrega", Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

    }
}
