using System;

namespace Dominio.Entidades.Embarcador.PedidoVenda
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_VENDA_ITENS", EntityName = "PedidoVendaItens", Name = "Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens", NameType = typeof(PedidoVendaItens))]
    public class PedidoVendaItens : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PVI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoItem", Column = "PVI_CODIGO_ITEM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoItem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoItem", Column = "PVI_DESCRICAO_ITEM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string DescricaoItem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PVI_QUANTIDADE", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "PVI_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "PVI_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServico", Column = "PVI_TIPO_SERVICO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoServico), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoServico TipoServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoVenda", Column = "PEV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda PedidoVenda { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Servico", Column = "SER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NotaFiscal.Servico Servico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_AUXILIAR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario FuncionarioAuxiliar { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMInicial", Column = "PVI_KM_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KMInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMFinal", Column = "PVI_KM_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KMFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMTotal", Column = "PVI_KM_TOTAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KMTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorKM", Column = "PVI_VALOR_KM", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalKM", Column = "PVI_VALOR_TOTAL_KM", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorTotalKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicial", Column = "PVI_HORA_INICIAL", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraFinal", Column = "PVI_HORA_FINAL", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraTotal", Column = "PVI_HORA_TOTAL", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorHora", Column = "PVI_VALOR_HORA", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalHora", Column = "PVI_VALOR_TOTAL_HORA", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorTotalHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDesconto", Column = "PVI_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMInicial2", Column = "PVI_KM_INICIAL_SECUNDARIO", TypeType = typeof(int), NotNull = false)]
        public virtual int KMInicial2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMFinal2", Column = "PVI_KM_FINAL_SECUNDARIO", TypeType = typeof(int), NotNull = false)]
        public virtual int KMFinal2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMTotal2", Column = "PVI_KM_TOTAL_SECUNDARIO", TypeType = typeof(int), NotNull = false)]
        public virtual int KMTotal2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicial2", Column = "PVI_HORA_INICIAL_SECUNDARIA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraInicial2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraFinal2", Column = "PVI_HORA_FINAL_SECUNDARIA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraFinal2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraTotal2", Column = "PVI_HORA_TOTAL_SECUNDARIA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraTotal2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOrdemServicoVenda", Column = "PVI_TIPO_ORDEM_SERVICO_VENDA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOrdemServicoVenda), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOrdemServicoVenda? TipoOrdemServicoVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOrdemCompra", Column = "PVI_NUMERO_ORDEM_COMPRA", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string NumeroOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroItemOrdemCompra", Column = "PVI_NUMERO_ITEM_ORDEM_COMPRA", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string NumeroItemOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PVI_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual bool Equals(PedidoVendaItens other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
