using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO_TEMPO_EXECUCAO", EntityName = "OrdemServicoFrotaServicoVeiculoTempoExecucao", Name = "Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao", NameType = typeof(OrdemServicoFrotaServicoVeiculoTempoExecucao))]
    public class OrdemServicoFrotaServicoVeiculoTempoExecucao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OTE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrota OrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoVeiculoFrota", Column = "SEV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ServicoVeiculoFrota Servico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaServicoVeiculo", Column = "OSS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrotaServicoVeiculo Manutencao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_MECANICO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Mecanico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "OTE_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicio", Column = "OTE_HORA_INICIO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraFim", Column = "OTE_HORA_FIM", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoExecutado", Column = "OTE_TEMPO_EXECUTADO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoExecutado { get; set; }
    }
}
