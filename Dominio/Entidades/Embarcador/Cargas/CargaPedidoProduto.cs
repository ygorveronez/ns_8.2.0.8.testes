using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_PRODUTO", EntityName = "CargaPedidoProduto", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto", NameType = typeof(CargaPedidoProduto))]
    public class CargaPedidoProduto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoUnitario", Column = "CPP_PESO_UNITARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEmbalagem", Column = "CPP_QUANTIDADE_EMBALAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeEmbalagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotalEmbalagem", Column = "CPP_PESO_TOTAL_EMBALAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotalEmbalagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitarioProduto", Column = "CPP_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnitarioProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "CPP_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPP_QUANTIDADE_PLANEJADA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadePlanejada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Temperatura", Column = "CPP_TEMPERATURA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Temperatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "JustificativaTemperatura", Column = "CJT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura JustificativaTemperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaixa", Column = "CPP_QUANTIDADE_CAIXA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePorCaixaRealizada", Column = "CPP_QUANTIDADE_POR_CAIXA_REALIZADA", TypeType = typeof(int), NotNull = false)]
        public virtual int? QuantidadePorCaixaRealizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaixasVazias", Column = "CPP_QUANTIDADE_CAIXAS_VAZIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCaixasVazias { get; set; }

        /// <summary>
        /// Pelo que entendi a Aurora não está usando esse campo. Ao invés disso, tá usando o "QuantidadeCaixasVazias" como a quantidade planejada. 
        /// Nesse caso, esse campo tá depreciado
        /// </summary>
        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaixasVaziasPlanejadas", Column = "CPP_QUANTIDADE_CAIXAS_VAZIAS_PLANEJADAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCaixasVaziasPlanejadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaixasVaziasRealizada", Column = "CPP_QUANTIDADE_CAIXAS_VAZIAS_REALIZADA", TypeType = typeof(int), NotNull = false)]
        public virtual int? QuantidadeCaixasVaziasRealizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImunoPlanejado", Column = "CPP_IMUNO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ImunoPlanejado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImunoRealizado", Column = "CPP_IMUNO_REALIZADO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ImunoRealizado { get; set; }

        /// <summary>
        /// Observação do <b>produto referente a essa carga/pedido</b>.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCarga", Column = "CPP_OBSERVACAO_CARGA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCarga { get; set; }

        /// <summary>
        /// Campo utilizado para salvar a quantidade antes de ser multiplicada por caixas no campo de Quantidade, que acontece quando tem a configuração MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeCaixa ativada no tipo de operação (Basicamente na Mattel)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeOriginal", Column = "CPP_QUANTIDADE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeOriginal { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public virtual string Descricao
        {
            get
            {
                return $"Carga Pedido - {Produto.Descricao}";
            }
        }

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

        public virtual decimal PesoTotal
        {
            get
            {
                return (Quantidade * PesoUnitario) + PesoTotalEmbalagem;
            }
        }

        public virtual decimal ValorTotal
        {
            get
            {
                return Quantidade * ValorUnitarioProduto;
            }
        }

        public virtual decimal QuantidadeUtilizar
        {
            get { return QuantidadeOriginal > 0 ? QuantidadeOriginal : Quantidade; }
        }

        #endregion Propriedades com Regras

        #region Métodos Públicos

        public virtual CargaPedidoProduto Clonar()
        {
            return (CargaPedidoProduto)this.MemberwiseClone();
        }

        public virtual bool Equals(CargaPedidoProduto other)
        {
            return (other.Codigo == this.Codigo);
        }

        #endregion Métodos Públicos
    }
}
