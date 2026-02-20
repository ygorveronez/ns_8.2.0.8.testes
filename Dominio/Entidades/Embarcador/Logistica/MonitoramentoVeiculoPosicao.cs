namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONITORAMENTO_VEICULO_POSICAO", EntityName = "MonitoramentoVeiculoPosicao", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao", NameType = typeof(MonitoramentoVeiculoPosicao))]
    public class MonitoramentoVeiculoPosicao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MVP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MonitoramentoVeiculo", Column = "MOV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo MonitoramentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Posicao", Column = "POS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Posicao Posicao { get; set; }
    }
}
