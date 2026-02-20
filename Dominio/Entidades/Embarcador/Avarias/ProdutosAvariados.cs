namespace Dominio.Entidades.Embarcador.Avarias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTOS_AVARIADOS", EntityName = "ProdutosAvariados", Name = "Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados", NameType = typeof(ProdutosAvariados))]
    public class ProdutosAvariados : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoAvaria", Column = "SAV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria SolicitacaoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador ProdutoEmbarcador { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "PAV_NOTA_FISCAL", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string NotaFiscal { get; set; }

        /// <summary>
        /// Quando o produto foi avariado
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PAV_PRODUTO_AVARIADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProdutoAvariado { get; set; }

        /// <summary>
        /// Quando o produto é removido do lote (usado somente na geracao de lote)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PAV_REMOVIDO_LOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemovidoLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAV_REMOVIDO_LOTE_OBS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string RemovidoObservacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoRemocaoLote", Column = "MRL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Avarias.MotivoRemocaoLote MotivoRemocaoLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAV_VALOR_AVARIA", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = true)]
        public virtual decimal ValorAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAV_VALOR_INFORMADO_OPERADOR", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = true)]
        public virtual decimal ValorInformadoOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAV_CAIXAS_AVARIADAS", TypeType = typeof(int), NotNull = true)]
        public virtual int CaixasAvariadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAV_UNIDADES_AVARIADAS", TypeType = typeof(int), NotNull = true)]
        public virtual int UnidadesAvariadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAV_CUSTO_PRIMARIO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal CustoPrimario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAV_GERA_ESTOQUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeraEstoque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Deposito", Column = "DEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.WMS.Deposito LocalArmazenamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscalProduto", Column = "XFP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto ProdutoNotaFiscal { get; set; }
        

        public virtual string Descricao
        {
            get
            {
                return this.ProdutoEmbarcador?.Descricao ?? "";
            }
        }
    }
}