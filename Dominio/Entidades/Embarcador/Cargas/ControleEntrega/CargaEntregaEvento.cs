using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ENTREGA_EVENTO", EntityName = "CargaEntregaEvento", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento", NameType = typeof(CargaEntregaEvento))]
    public class CargaEntregaEvento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataOcorrencia", Column = "CEE_DATA_OCORRENCIA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoDeOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EventoColetaEntrega", Column = "CEE_EVENTO_COLETA_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega? EventoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "CEE_LATITUDE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "CEE_LONGITUDE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPosicao", Column = "CEE_DATA_POSICAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPosicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoRecalculada", Column = "CEE_DATA_PREVISAO_RECALCULADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoRecalculada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Origem", Column = "CEE_ORIGEM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega Origem { get; set; }

        public virtual string Descricao
        {
            get { return $"Eventro de carga/entrega {CargaEntrega?.Descricao ?? Carga?.CodigoCargaEmbarcador}"; }
        }
    }
}