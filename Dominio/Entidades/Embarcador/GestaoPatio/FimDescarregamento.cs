using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FIM_DESCARREGAMENTO", EntityName = "FimDescarregamento", Name = "Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento", NameType = typeof(FimDescarregamento))]
    public class FimDescarregamento : EntidadeCargaBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FDR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FDR_DATA_DESCARREGAMENTO_FINALIZADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDescarregamentoFinalizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaFimDescarregamentoLiberada", Column = "FDR_FIM_DESCARREGAMENTO_LIBERADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EtapaFimDescarregamentoLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FDR_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFimDescarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFimDescarregamento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FDR_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pesagem", Column = "FDR_PESAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Pesagem { get; set; }

        public virtual string Descricao
        {
            get { return Carga != null ? $"Fim do descarregamento da carga {Carga.CodigoCargaEmbarcador}" : $"Fim do descarregamento da pré carga {PreCarga.NumeroPreCarga}"; }
        }
    }
}
