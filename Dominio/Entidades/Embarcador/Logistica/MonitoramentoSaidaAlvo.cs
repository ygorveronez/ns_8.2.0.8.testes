using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_MONITORAMENTO_SAIDA_ALVO", EntityName = "MonitoramentoSaidaAlvo", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoSaidaAlvo", NameType = typeof(MonitoramentoSaidaAlvo))]
    public class MonitoramentoSaidaAlvo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MSA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Monitoramento", Column = "MON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Monitoramento Monitoramento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Posicao", Column = "POS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Posicao Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "MSA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "MSA_LATITUDE", TypeType = typeof(double), NotNull = false)]
        public virtual double Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "MSA_LONGITUDE", TypeType = typeof(double), NotNull = false)]
        public virtual double Longitude { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

    }
}
