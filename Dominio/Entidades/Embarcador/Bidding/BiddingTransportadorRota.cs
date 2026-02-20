using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_TRANSPORTADOR_ROTA", EntityName = "BiddingTransportadorRota", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota", NameType = typeof(BiddingTransportadorRota))]

    public class BiddingTransportadorRota : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTR_RODADA", TypeType = typeof(Int32), NotNull = true)]
        public virtual int Rodada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "TTR_TRANSPORTADOR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingOfertaRota", Column = "TTR_ROTA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BiddingOfertaRota Rota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTR_STATUS_ROTA", TypeType = typeof(StatusBiddingRota), NotNull = true)]
        public virtual StatusBiddingRota Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTR_RANKING", TypeType = typeof(Int32), NotNull = false)]
        public virtual int Ranking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TTR_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Target", Column = "TTR_TARGET", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Target { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAnterior", Column = "TTR_VALOR_ANTERIOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TransportadorRejeitado", Column = "TTR_TRANSPORTADOR_REJEITADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TransportadorRejeitado { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo + " - " + this.Transportador.Descricao;
            }
        }
    }
}
