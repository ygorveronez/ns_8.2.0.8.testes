namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_PERIODO_CARREGAMENTO_TIPO_OPERACAO_SIMULTANEO", EntityName = "PeriodoCarregamentoTipoOperacaoSimultaneo", Name = "Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo", NameType = typeof(PeriodoCarregamentoTipoOperacaoSimultaneo))]
    public class PeriodoCarregamentoTipoOperacaoSimultaneo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PTS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PeriodoCarregamento", Column = "PEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PeriodoCarregamento PeriodoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PTS_CAPACIDADE_CARREGAMENTO_SIMULTANEO", TypeType = typeof(int), NotNull = true)]
        public virtual int CapacidadeCarregamentoSimultaneo { get; set; }

        public virtual string Descricao => TipoOperacao.Descricao;
    }
}
