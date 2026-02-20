using Dominio.Entidades.Embarcador.Compras;
using Dominio.Entidades.Embarcador.Patrimonio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.PedidoVenda
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_VENDA", EntityName = "PedidoVenda", Name = "Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda", NameType = typeof(PedidoVenda))]
    public class PedidoVenda : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PEV_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroInterno", Column = "PEV_NUMERO_INTERNO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroInterno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PessoaSolicitante", Column = "PEV_PESSOA_SOLICITANTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string PessoaSolicitante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "PEV_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrega", Column = "PEV_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Referencia", Column = "PEV_REFERENCIA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Referencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PEV_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PEV_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVenda), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVenda Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "PEV_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVenda), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVenda Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "PEV_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProdutos", Column = "PEV_VALOR_PRODUTOS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorServicos", Column = "PEV_VALOR_SERVICOS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorServicos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoRelatorio", Column = "PEV_CAMINHO_RELATORIO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CaminhoRelatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KM", Column = "PEV_KM", TypeType = typeof(int), NotNull = false)]
        public virtual int KM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaPagamento", Column = "PEV_FORMA_PAGAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string FormaPagamento { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CondicaoPagamento", Column = "COP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual CondicaoPagamento CondicaoPagamentoPadrao { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_SOLICITANTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario FuncionarioSolicitante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDesconto", Column = "PEV_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualDesconto", Column = "PEV_PERCENTUAL_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal PercentualDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Itens", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_VENDA_ITENS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PEV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoVendaItens", Column = "PVI_CODIGO")]
        public virtual IList<Embarcador.PedidoVenda.PedidoVendaItens> Itens { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Parcelas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_VENDA_PARCELA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PEV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoVendaParcela", Column = "PVP_CODIGO")]
        public virtual IList<Embarcador.PedidoVenda.PedidoVendaParcela> Parcelas { get; set; }

        public virtual string Descricao
        {
            get { return this.Cliente?.Descricao ?? string.Empty; }
        }

        public virtual string DescricaoTipo
        {
            get { return Tipo.ObterDescricao(); }
        }

        public virtual string DescricaoStatus
        {
            get { return Status.ObterDescricao(); }
        }

        public virtual bool Equals(PedidoVenda other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pet", Column = "PET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pet Pet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "PEV_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusOrdemServicoPet", Column = "PEV_STATUS_ORDEM_SERVICO_PET", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusOrdemServicoPet), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusOrdemServicoPet StatusOrdemServicoPet { get; set; }
    }
}
