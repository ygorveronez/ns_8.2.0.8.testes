namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_DESCARREGAMENTO_PEDIDO_PRODUTO", EntityName = "CargaJanelaDescarregamentoPedidoProduto", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto", NameType = typeof(CargaJanelaDescarregamentoPedidoProduto))]
    public class CargaJanelaDescarregamentoPedidoProduto : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaDescarregamentoPedido", Column = "JDP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaDescarregamentoPedido CargaJanelaDescarregamentoPedido { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoProduto", Column = "PRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoProduto PedidoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "CDP_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeAgendada", Column = "CDP_QUANTIDADE_AGENDADA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeAgendada { get; set; }

        #endregion Propriedades

        #region Métodos Públicos

        public virtual string Descricao
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion Métodos Públicos
    }
}