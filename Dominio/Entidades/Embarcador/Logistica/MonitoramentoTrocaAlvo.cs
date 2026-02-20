using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_MONITORAMENTO_TROCA_ALVO", EntityName = "MonitoramentoTrocaAlvo", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoTrocaAlvo", NameType = typeof(MonitoramentoTrocaAlvo))]
    public class MonitoramentoTrocaAlvo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MTA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "MTA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Monitoramento", Column = "MON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Monitoramento Monitoramento { get; set; }

        //[Obsolete("REMOVIDA NÃO UTILIZADA")]
        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Posicao", Column = "POS_CODIGO_ATUAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        //public virtual Dominio.Entidades.Embarcador.Logistica.Posicao PosicaoAtual { get; set; }

        //[Obsolete("REMOVIDA NÃO UTILIZADA")]
        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Posicao", Column = "POS_CODIGO_ANTERIOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        //public virtual Dominio.Entidades.Embarcador.Logistica.Posicao PosicaoAnterior { get; set; }

    }
}
