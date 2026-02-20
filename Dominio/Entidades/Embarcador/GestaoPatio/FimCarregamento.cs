using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FIM_CARREGAMENTO", EntityName = "FimCarregamento", Name = "Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento", NameType = typeof(FimCarregamento))]
    public class FimCarregamento : EntidadeCargaBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FCR_DATA_CARREGAMENTO_FINALIZADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarregamentoFinalizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaFimCarregamentoLiberada", Column = "FCR_FIM_CARREGAMENTO_LIBERADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EtapaFimCarregamentoLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FCR_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFimCarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFimCarregamento Situacao { get; set; }

        [Obsolete("Não utilizar, será deletada. Migrada para a observação da entidade FluxoGestaoPatioEtapas")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "FCR_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pesagem", Column = "FCR_PESAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Pesagem { get; set; }

        public virtual string Descricao
        {
            get { return Carga != null ? $"Fim do carregamento da carga {Carga.CodigoCargaEmbarcador}" : $"Fim do carregamento da pré carga {PreCarga.NumeroPreCarga}"; }
        }
    }
}
