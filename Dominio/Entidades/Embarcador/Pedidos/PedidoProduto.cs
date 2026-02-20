using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_PRODUTO", EntityName = "PedidoProduto", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoProduto", NameType = typeof(PedidoProduto))]

    public class PedidoProduto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProduto", Column = "PRP_VALOR_PRODUTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoUnitario", Column = "PRP_PESO_UNITARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrecoUnitario", Column = "PRP_PRECO_UNITARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PrecoUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEmbalagem", Column = "PRP_QUANTIDADE_EMBALAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeEmbalagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotalEmbalagem", Column = "PRP_PESO_TOTAL_EMBALAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotalEmbalagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PRP_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRP_QUANTIDADE_PLANEJADA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadePlanejada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePalet", Column = "PRP_QUANTIDADE_PALET", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadePalet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlturaCM", Column = "PRP_ALTURA_CM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal AlturaCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LarguraCM", Column = "PRP_LARGURA_CM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal LarguraCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComprimentoCM", Column = "PRP_COMPRIMENTO_CM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ComprimentoCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetroCubico", Column = "PRP_METRO_CUBICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetroCubico { get; set; }

        /// <summary>
        /// Essa observação é sempre a observação do PRODUTO relacionado a esse objeto. Não confundir com a ObservacaoCarga abaixo.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PRP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        /// <summary>
        /// Essa observação é sempre relativa a esse pedido/carga em específico. É populada no método AdicionarCarga do SGT.WebService.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCarga", Column = "PRP_OBSERVACAO_CARGA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PalletFechado", Column = "PRP_PALLET_FECHADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PalletFechado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SetorLogistica", Column = "PRP_SETOR_LOGISTICA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SetorLogistica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClasseLogistica", Column = "PRP_CLASSE_LOGISTICA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ClasseLogistica { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LinhaSeparacao", Column = "CLS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.LinhaSeparacao LinhaSeparacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EnderecoProduto", Column = "CEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.EnderecoProduto EnderecoProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoEmbalagem", Column = "TE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.TipoEmbalagem TipoEmbalagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaixaPorPallet", Column = "PRP_QUANTIDADE_CAIXA_POR_PALLET", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCaixaPorPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeUnidadePorCaixa", Column = "PRP_QUANTIDADE_UNIDADE_POR_CAIXA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeUnidadePorCaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaixa", Column = "PRP_QUANTIDADE_CAIXA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaixasVazias", Column = "PRP_QUANTIDADE_CAIXAS_VAZIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCaixasVazias { get; set; }

        /// <summary>
        /// Parece que a Aurora não está usando esse campo como planejados, e sim o "QuantidadeCaixasVazias".
        /// Nesse caso, esse campo está depreciado
        /// </summary>
        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaixasVaziasPlanejadas", Column = "PRP_QUANTIDADE_CAIXAS_VAZIAS_PLANEJADAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCaixasVaziasPlanejadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CanalDistribuicao", Column = "PRP_CANAL_DISTRIBUICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CanalDistribuicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SiglaModalidade", Column = "PRP_SIGLA_MODALIDADE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SiglaModalidade { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PedidoProdutoONUs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_PRODUTO_ONU")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoProdutoONU", Column = "PPO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU> PedidoProdutoONUs { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImunoPlanejado", Column = "PRP_IMUNO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ImunoPlanejado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImunoRealizado", Column = "PRP_IMUNO_REALIZADO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ImunoRealizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRP_SITUACAO_SEPARACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSeparacaoPedidoProduto), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSeparacaoPedidoProduto SituacaoSeparacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRP_ID_DEMANDA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IdDemanda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRP_CANAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Canal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRP_CODIGO_ORGANIZACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoOrganizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRP_SETOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Setor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRP_CODIGO_PRODUTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRP_PESO_UNITARIO_PRODUTO", TypeType = typeof(Decimal), NotNull = false)]
        public virtual Decimal PesoUnitarioProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilialArmazem", Column = "FIA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.FilialArmazem FilialArmazem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desmembrar", Column = "PRP_DESMEMBRAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Desmembrar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRP_CAMPOS_PERSONALIZADOS", Type = "StringClob", NotNull = false)]
        public virtual string CamposPersonalizados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeMedidaSecundaria", Column = "PRP_UNIDADE_MEDIDA_SECUNDARIA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string UnidadeMedidaSecundaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeSecundaria", Column = "PRP_QUANTIDADE_SECUNDARIA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeSecundaria { get; set; }

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
                return (Quantidade * PrecoUnitario);
            }
        }

        #endregion Propriedades com Regras

        #region Métodos Públicos

        public virtual PedidoProduto Clonar()
        {
            return (PedidoProduto)this.MemberwiseClone();
        }

        public virtual bool Equals(PedidoProduto other)
        {
            return (other.Codigo == this.Codigo);
        }

        #endregion Métodos Públicos
    }
}
