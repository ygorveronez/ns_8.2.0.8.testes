using System;

namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_DUVIDA", EntityName = "BiddingDuvida", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingDuvida", NameType = typeof(BiddingDuvida))]
    public class BiddingDuvida : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingConvite", Column = "BBC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BiddingConvite BiddingConvite { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "BEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BDC_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BDC_PERGUNTA", TypeType = typeof(string), NotNull = true)]
        public virtual string Pergunta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BDC_RESPOSTA", TypeType = typeof(string), NotNull = false)]
        public virtual string Resposta { get; set; }
    }
}
