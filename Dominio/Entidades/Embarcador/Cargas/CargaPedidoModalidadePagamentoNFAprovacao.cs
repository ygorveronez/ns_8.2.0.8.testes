using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_MODALIDADE_PAGAMENTO_NF_APROVACAO", EntityName = "CargaPedidoModalidadePagamentoNFAprovacao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao", NameType = typeof(CargaPedidoModalidadePagamentoNFAprovacao))]
    public class CargaPedidoModalidadePagamentoNFAprovacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAutorizacaoModalidadePagamento", Column = "CMA_SITUACAO_LIBERACAO_MODALIDADE_PAGAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoModalidadePagamento), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoModalidadePagamento SituacaoAutorizacaoModalidadePagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHora", Column = "CMA_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoExtornoAutorizacao", Column = "CMA_MOTIVO_EXTORNO_AUTORIZACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string MotivoExtornoAutorizacao { get; set; }
    }
}
