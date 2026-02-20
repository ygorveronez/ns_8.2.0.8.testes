using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_XML_NOTA_FISCAL_PRODUTO", EntityName = "XMLNotaFiscalProduto", Name = "Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto", NameType = typeof(XMLNotaFiscalProduto))]
    public class XMLNotaFiscalProduto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "XFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "XFP_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProduto", Column = "XFP_VALOR_PRODUTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeMedida", Column = "XFP_UNIDADE_MEDIDA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UnidadeMedida { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO_INTERNO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Produto ProdutoInterno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "XFP_CST", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoCompra", Column = "XFP_NUMERO_PEDIDO_COMPRA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NumeroPedidoCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Origem", Column = "XFP_ORIGEM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Origem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoNFCI", Column = "XFP_CODIGO_NFCI", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoNFCI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEAN", Column = "XFP_CODIGO_EAN", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoEAN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "cProd", Column = "XFP_C_PROD", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string cProd { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCM", Column = "XFP_NCM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NCM { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        /// <summary>
        /// Campo utilizado para salvar a quantidade antes de ser multiplicada por caixas no campo de Quantidade, que acontece quando tem a configuração MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeCaixa ativada no tipo de operação (Basicamente na Mattel)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeOriginal", Column = "XFP_QUANTIDADE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeOriginal { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public virtual decimal PesoProduto
        {
            get
            {
                return Quantidade * Produto.PesoUnitario;
            }
        }

        public virtual decimal PesoLiquidoProduto
        {
            get
            {
                return Quantidade * Produto.PesoLiquidoUnitario;
            }
        }

        public virtual decimal ValorTotal
        {
            get
            {
                return Quantidade * ValorProduto;
            }
        }

        public virtual decimal QuantidadeUtilizar
        {
            get { return QuantidadeOriginal > 0 ? QuantidadeOriginal : Quantidade; }
        }

        #endregion Propriedades com Regras

        #region Métodos Públicos

        public virtual XMLNotaFiscalProduto Clonar()
        {
            return (XMLNotaFiscalProduto)this.MemberwiseClone();
        }

        public virtual bool Equals(XMLNotaFiscalProduto other)
        {
            return (other.Codigo == this.Codigo);
        }

        #endregion Métodos Públicos
    }
}
