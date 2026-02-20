using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GUARITA_CHECK_LIST_SERVICO_EQUIPAMENTO", EntityName = "GuaritaCheckListServicoEquipamento", Name = "Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento", NameType = typeof(GuaritaCheckListServicoEquipamento))]
    public class GuaritaCheckListServicoEquipamento : EntidadeBase, IEquatable<GuaritaCheckListServicoEquipamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GSE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoManutencao", Column = "GSE_TIPO_MANUTENCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculoOrdemServicoFrota), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculoOrdemServicoFrota TipoManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoEstimado", Column = "GSE_CUSTO_ESTIMADO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal CustoEstimado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoMedio", Column = "GSE_CUSTO_MEDIO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal CustoMedio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "GSE_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEstimado", Column = "GSE_TEMPO_ESTIMADO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEstimado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoVeiculoFrota", Column = "SEV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.ServicoVeiculoFrota Servico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaServicoVeiculo", Column = "OSS_CODIGO_ULTIMA_MANUTENCAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.OrdemServicoFrotaServicoVeiculo UltimaManutencao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GuaritaCheckList", Column = "GCL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GuaritaCheckList GuaritaCheckList { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Servico?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(GuaritaCheckListServicoEquipamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}