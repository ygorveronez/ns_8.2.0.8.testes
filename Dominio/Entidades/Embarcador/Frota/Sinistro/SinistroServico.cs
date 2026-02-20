namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SINISTRO_SERVICO", EntityName = "SinistroServico", Name = "Dominio.Entidades.Embarcador.Frota.SinistroServico", NameType = typeof(SinistroServico))]
    public class SinistroServico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SSE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoManutencao", Column = "SSE_TIPO_MANUTENCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculoOrdemServicoFrota), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculoOrdemServicoFrota TipoManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoEstimado", Column = "SSE_CUSTO_ESTIMADO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal CustoEstimado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoMedio", Column = "SSE_CUSTO_MEDIO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal CustoMedio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "SSE_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEstimado", Column = "SSE_TEMPO_ESTIMADO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEstimado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoVeiculoFrota", Column = "SEV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ServicoVeiculoFrota Servico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaServicoVeiculo", Column = "OSS_CODIGO_ULTIMA_MANUTENCAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrotaServicoVeiculo UltimaManutencao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SinistroDados", Column = "SDS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SinistroDados Sinistro { get; set; }

        public virtual string Descricao
        {
            get { return Servico?.Descricao ?? string.Empty; }
        }
    }
}
