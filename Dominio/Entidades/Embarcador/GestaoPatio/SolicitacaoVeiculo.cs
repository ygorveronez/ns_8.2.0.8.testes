using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SOLICITACAO_VEICULO", EntityName = "SolicitacaoVeiculo", Name = "Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo", NameType = typeof(SolicitacaoVeiculo))]
    public class SolicitacaoVeiculo : EntidadeCargaBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SOV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SOV_DATA_SOLICITACAO_VEICULO_INICIADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSolicitacaoVeiculoIniciada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaSolicitacaoVeiculoLiberada", Column = "SOV_SOLICITACAO_VEICULO_LIBERADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EtapaSolicitacaoVeiculoLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SOV_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoVeiculo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoVeiculo Situacao { get; set; }

        [Obsolete("Não utilizar, será deletada. Migrada para a observação da entidade FluxoGestaoPatioEtapas")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "SOV_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacoes { get; set; }

        public virtual string Descricao
        {
            get { return Carga != null ? $"Solicitação de veículo da carga {Carga.CodigoCargaEmbarcador}" : $"Solicitação de veículo da pré carga {PreCarga.NumeroPreCarga}"; }
        }
    }
}
