using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERMANENCIA_LOCAL", EntityName = "PermanenciaLocal", Name = "Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente", NameType = typeof(PermanenciaLocal))]
    public class PermanenciaLocal : EntidadeBase
    {
        public PermanenciaLocal()
        {
            DataInicio = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PLO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "PLO_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "PLO_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoSegundos", Column = "PLO_TEMPO_SEGUNDOS", TypeType = typeof(int), NotNull = false)]
        public virtual int? TempoSegundos { get; set; }

        public virtual TimeSpan Tempo { 
            get {
                return TimeSpan.FromSeconds(TempoSegundos ?? 0);
            }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Local", Class = "Locais", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Locais Local { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Carga", Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }

    }
}
