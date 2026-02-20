using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INICIO_CARREGAMENTO", EntityName = "InicioCarregamento", Name = "Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento", NameType = typeof(InicioCarregamento))]
    public class InicioCarregamento : EntidadeCargaBase
    {  
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICR_DATA_CARREGAMENTO_INICIADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarregamentoIniciado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaInicioCarregamentoLiberada", Column = "ICR_INICIO_CARREGAMENTO_LIBERADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EtapaInicioCarregamentoLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICR_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoInicioCarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoInicioCarregamento Situacao { get; set; }

        [Obsolete("Não utilizar, será deletada. Migrada para a observação da entidade FluxoGestaoPatioEtapas")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "ICR_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pesagem", Column = "ICR_PESAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Pesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ICR_DATA_LACRE_INICIO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLacreInicioCarregamento { get; set; }

        public virtual string Descricao
        {
            get { return Carga != null ? $"Início do carregamento da carga {Carga.CodigoCargaEmbarcador}" : $"Início do carregamento da pré carga {PreCarga.NumeroPreCarga}"; }
        }  
    }
}
