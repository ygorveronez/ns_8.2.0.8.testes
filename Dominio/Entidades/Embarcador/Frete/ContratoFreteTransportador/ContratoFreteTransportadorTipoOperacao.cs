namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_TIPO_OPERACAO", EntityName = "ContratoFreteTransportadorTipoOperacao", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoOperacao", NameType = typeof(ContratoFreteTransportadorTipoOperacao))]
    public class ContratoFreteTransportadorTipoOperacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFreteTransportador ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }


        public virtual string Descricao
        {
            get
            {
                return ContratoFrete.Descricao + " - " + TipoOperacao.Descricao;
            }
        }
    }
}
