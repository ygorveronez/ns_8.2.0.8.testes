using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_HISTORICO", EntityName = "CargaPedidoHistorico", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoHistorico", NameType = typeof(CargaPedidoHistorico))]
    public class CargaPedidoHistorico : EntidadeBase, IEquatable<CargaPedidoHistorico>
    {
        public CargaPedidoHistorico()
        {
           DataCriacao = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CPH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPH_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAcao", Column = "CPH_TIPO_ACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CargaPedidoHistoricoTipoAcao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CargaPedidoHistoricoTipoAcao TipoAcao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "CPH_SITUACAO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CargaPedidoHistoricoSituacaoIntegracao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CargaPedidoHistoricoSituacaoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "none")]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "none")]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public virtual bool Equals(CargaPedidoHistorico other)
        {
            return this.Codigo == other.Codigo;
        }
    }
}
