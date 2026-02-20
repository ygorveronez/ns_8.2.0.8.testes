using System;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_VINCULO_KM_REBOQUE", EntityName = "HistoricoVinculoKmReboque", Name = "Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque", NameType = typeof(HistoricoVinculoKmReboque))]
    public class HistoricoVinculoKmReboque : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HVK_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "HVK_CODIGO_TRACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "HVK_CODIGO_REBOQUE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Reboque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMAtual", Column = "HVK_KM_ATUAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KMAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "HVK_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracao", Column = "HVK_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimento", Column = "HVK_TIPO_MOVIMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoKmReboque), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoKmReboque TipoMovimento { get; set; }
    }
}
