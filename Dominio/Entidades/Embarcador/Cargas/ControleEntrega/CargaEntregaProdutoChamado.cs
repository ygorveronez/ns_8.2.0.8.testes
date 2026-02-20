using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA_PRODUTO_CHAMADO", EntityName = "CargaEntregaProdutoChamado", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado", NameType = typeof(CargaEntregaProdutoChamado))]
    public class CargaEntregaProdutoChamado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoUnitario", Column = "CPP_PESO_UNITARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "CPP_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPP_QUANTIDADE_PLANEJADA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadePlanejada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDevolucao", Column = "CPP_QUANTIDADE_DEVOLUCAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeConferencia", Column = "CPP_QUANTIDADE_CONFERENCIA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeConferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoProdutoDevolucao", Column = "CPP_OBSERVACAO_PRODUTO_DEVOLUCAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoProdutoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Lote", Column = "CPP_LOTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Lote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCritica", Column = "CPP_DATA_CRITICA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCritica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDevolucao", Column = "CPP_VALOR_DEVOLUCAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NFDevolucao", Column = "CPP_NF_DEVOLUCAO", TypeType = typeof(int), NotNull = false)]
        public virtual int NFDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoDevolucaoEntrega", Column = "MDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoEntregas.MotivoDevolucaoEntrega MotivoDaDevolucao { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}