using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_MONITORAMENTO_DADOS_SUMARIZADOS", EntityName = "MonitoramentoDadosSumarizados", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoDadosSumarizados", NameType = typeof(MonitoramentoDadosSumarizados))]
    public class MonitoramentoDadosSumarizados : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Monitoramento", Column = "MON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Monitoramento Monitoramento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraQualidadeMonitoramento", Column = "RQM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento RegraQualidadeMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Resultado", Column = "MDS_RESULTADO", TypeType = typeof(string), Length = 3, NotNull = true)]
        public virtual string Resultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePosicoesSumarizadas", Column = "MDS_QUANTIDADE_POSICOES_SUMARIZADAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadePosicoesSumarizadas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Posicoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MONITORAMENTO_DADOS_SUMARIZADOS_POSICAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MDS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Posicao", Column = "POS_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.Posicao> Posicoes { get; set; }

        public virtual string Descricao
        {
            get { return this.RegraQualidadeMonitoramento.Descricao; }
        }
    }
}
