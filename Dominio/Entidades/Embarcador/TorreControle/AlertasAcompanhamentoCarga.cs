using System;

namespace Dominio.Entidades.Embarcador.TorreControle
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_ALERTA_ACOMPANHAMENTO_CARGA", EntityName = "AlertasAcompanhamentoCarga", Name = "Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga", NameType = typeof(AlertasAcompanhamentoCarga))]
    public class AlertasAcompanhamentoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEvento", Column = "AAC_DATA_EVENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "AAC_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEvento", Column = "ALC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento CargaEvento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AlertaMonitor", Column = "ALE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.AlertaMonitor AlertaMonitor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertaTratado", Column = "AAC_TRATADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlertaTratado { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
