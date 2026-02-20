using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO", EntityName = "OrdemServicoFrotaServicoVeiculo", Name = "Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo", NameType = typeof(OrdemServicoFrotaServicoVeiculo))]
    public class OrdemServicoFrotaServicoVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OSS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrota OrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoVeiculoFrota", Column = "SEV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ServicoVeiculoFrota Servico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaServicoVeiculo", Column = "OSS_CODIGO_ULTIMA_MANUNTENCAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrotaServicoVeiculo UltimaManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoManutencao", Column = "OSS_TIPO_MANUTENCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculoOrdemServicoFrota), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculoOrdemServicoFrota TipoManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoEstimado", Column = "OSS_CUSTO_ESTIMADO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal CustoEstimado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoMedio", Column = "OSS_CUSTO_MEDIO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal CustoMedio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "OSS_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEstimado", Column = "OSS_TEMPO_ESTIMADO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEstimado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoExecutado", Column = "OSS_TEMPO_EXECUTADO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoExecutado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoExecutado", Column = "OSS_NAO_EXECUTADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExecutado { get; set; }

        #region Da aba Serviços do Fechamento

        [NHibernate.Mapping.Attributes.Property(0, Name = "ServicoConcluido", Column = "OSS_SERVICO_CONCLUIDO", TypeType = typeof(ServicoVeiculoExecutado), NotNull = false)]
        public virtual ServicoVeiculoExecutado ServicoConcluido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoFechamento", Column = "OSS_OBSERVACAO_FECHAMENTO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoFechamento { get; set; }

        #endregion

        #region Propriedades Virtuais

        public virtual string Descricao
        {
            get { return this.Servico?.Descricao ?? string.Empty; }
        }

        public virtual string DescricaoTipoManutencao
        {
            get { return TipoManutencao.ObterDescricao(); }
        }

        #endregion
    }
}
