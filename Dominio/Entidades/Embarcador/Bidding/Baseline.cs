using System;

namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BASELINE", EntityName = "Baseline", Name = "Dominio.Entidades.Embarcador.Bidding.Baseline", NameType = typeof(Baseline))]
    public class Baseline : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BAS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingOfertaRota", Column = "TBR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota BiddingOfertaRota { get; set; }

        [Obsolete("Campo substituído pela Descricao do TipoBaseline.")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "BAS_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "BAS_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoBaseline", Column = "TBS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Bidding.TipoBaseline TipoBaseline { get; set; }
    }
}
