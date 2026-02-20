using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INICIO_DESCARREGAMENTO", EntityName = "InicioDescarregamento", Name = "Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento", NameType = typeof(InicioDescarregamento))]
    public class InicioDescarregamento : EntidadeCargaBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IDR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDR_DATA_DESCARREGAMENTO_INICIADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDescarregamentoIniciado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaInicioDescarregamentoLiberada", Column = "IDR_INICIO_DESCARREGAMENTO_LIBERADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EtapaInicioDescarregamentoLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDR_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoInicioDescarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoInicioDescarregamento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDR_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pesagem", Column = "IDR_PESAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Pesagem { get; set; }

        public virtual string Descricao
        {
            get { return Carga != null ? $"Início do descarregamento da carga {Carga.CodigoCargaEmbarcador}" : $"Início do descarregamento da pré carga {PreCarga.NumeroPreCarga}"; }
        }
    }
}
