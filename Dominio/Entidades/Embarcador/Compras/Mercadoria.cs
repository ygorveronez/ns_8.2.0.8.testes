namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MERCADORIA", EntityName = "Mercadoria", Name = "Dominio.Entidades.Embarcador.Compras.Mercadoria", NameType = typeof(Mercadoria))]
    public class Mercadoria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MER_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEstoque", Column = "PRE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque ProdutoEstoque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RequisicaoMercadoria", Column = "RME_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria RequisicaoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MER_MODO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria Modo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MER_QUANTIDADE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MER_SALDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Saldo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.LocalArmazenamentoProduto LocalArmazenamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MER_CUSTO_UNITARIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal CustoUnitario { get; set; }

        public virtual decimal CustoTotal
        {
            get { return this.CustoUnitario * this.Quantidade; }
        }

        public virtual string Descricao
        {
            get { return this.ProdutoEstoque?.Produto?.Descricao ?? string.Empty; }
        }
    }
}
