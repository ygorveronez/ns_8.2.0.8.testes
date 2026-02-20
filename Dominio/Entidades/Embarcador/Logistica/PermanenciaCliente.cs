using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERMANENCIA_CLIENTE", EntityName = "PermanenciaCliente", Name = "Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente", NameType = typeof(PermanenciaCliente))]
    public class PermanenciaCliente : EntidadeBase
    {
        public PermanenciaCliente()
        {
            DataInicio = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "PCL_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "PCL_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoSegundos", Column = "PCL_TEMPO_SEGUNDOS", TypeType = typeof(int), NotNull = false)]
        public virtual int? TempoSegundos { get; set; }

        public virtual TimeSpan Tempo { 
            get {
                return TimeSpan.FromSeconds(TempoSegundos ?? 0);
            }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Cliente", Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "CargaEntrega", Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }

    }
}
