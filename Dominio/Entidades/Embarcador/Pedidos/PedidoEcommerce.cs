using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_ECOMMERCE", EntityName = "PedidoEcommerce", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoEcommerce", NameType = typeof(PedidoEcommerce))]
    public class PedidoEcommerce : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlturaPedido", Column = "PEC_ALTURA_PEDIDO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal AlturaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LarguaPedido", Column = "PEC_LARGURA_PEDIDO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal LarguaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComprimentoPedido", Column = "PEC_COMPRIMENTO_PEDIDO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ComprimentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiametroPedido", Column = "PEC_DIAMETRO_PEDIDO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal DiametroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CategoriaPrincipalProduto", Column = "PEC_CATEGORIA_PRINCIPAL_PRODUTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CategoriaPrincipalProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieNFe", Column = "PEC_SERIE_NFE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SerieNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveAcessoNFe", Column = "PEC_CHAVE_ACESSO_NFE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveAcessoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaturezaGeralMercadorias", Column = "PEC_NATUREZA_GERAL_MERCADORIAS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NaturezaGeralMercadorias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoGeralMercadorias", Column = "PEC_TIPO_GERAL_MERCADORIAS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TipoGeralMercadorias { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "PrazoEntregaLoja", Column = "PEC_PRAZO_ENTREGA_LOJA", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoEntregaLoja { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFrete", Column = "PEC_TIPO_FRETE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TipoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPagamentoPedido", Column = "PEC_DATA_PAGAMENTO_PEDIDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPagamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModalidadeEntrega", Column = "PEC_MODALIDADE_ENTREGA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ModalidadeEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTabelaFreteSistemaFIS", Column = "PEC_CODIGO_TABELA_FRETE_SISTEMA_FIS", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoTabelaFreteSistemaFIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOPPredominanteNFe", Column = "PEC_CFOP_PREDOMINANTE_NFE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CFOPPredominanteNFe { get; set; }
    }
}