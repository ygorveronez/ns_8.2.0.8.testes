namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_AVERBACAO", EntityName = "TransportadorAverbacao", Name = "Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao", NameType = typeof(TransportadorAverbacao))]
    public class TransportadorAverbacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguro", Column = "APS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro ApoliceSeguro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EAV_DESCONTO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal? Desconto { get; set; }
        public virtual string Descricao
        {
            get
            {
                return this.TipoOperacao?.Descricao ?? string.Empty;
            }
        }
    }
}
