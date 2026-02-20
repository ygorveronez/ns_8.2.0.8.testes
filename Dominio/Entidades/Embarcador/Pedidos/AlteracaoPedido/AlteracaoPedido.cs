using System;

namespace Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALTERACAO_PEDIDO", EntityName = "AlteracaoPedido", Name = "Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido", NameType = typeof(AlteracaoPedido))]
    public class AlteracaoPedido : EntidadeBase, IEquatable<AlteracaoPedido>, Interfaces.Embarcador.Entidade.IEntidade
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ALP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALP_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ALP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoPedido), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoPedido Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALP_SITUACAO_CONSULTADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SituacaoConsultada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoRejeicaoAlteracaoPedido", Column = "RAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoRejeicaoAlteracaoPedido MotivoRejeicao { get; set; }

        public virtual string Descricao
        {
            get { return $"Alteração do pedido {Pedido.NumeroPedidoEmbarcador}"; }
        }

        #endregion

        #region Propriedades para Alteração do Pedido

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALP_COMPANHIA", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string Companhia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataETA", Column = "ALP_DATA_ETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataETA { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AlteracaoPedidoCliente", Column = "APC_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AlteracaoPedidoCliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNavio", Column = "ALP_NUMERO_NAVIO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string NumeroNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "ALP_ORDEM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotal", Column = "ALP_PESO_TOTAL_CARGA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALP_PORTO_CHEGADA", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string PortoChegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALP_PORTO_SAIDA", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string PortoSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoEntrega", Column = "ALP_PREVISAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AlteracaoPedidoCliente", Column = "APC_CODIGO_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AlteracaoPedidoCliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AlteracaoPedidoCliente", Column = "APC_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AlteracaoPedidoCliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALP_RESERVA", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string Reserva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALP_RESUMO", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string Resumo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Temperatura", Column = "ALP_TEMPERATURA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Temperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALP_TIPO_EMBARQUE", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string TipoEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALP_VENDEDOR", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string Vendedor { get; set; }

        #endregion

        #region Métodos Públicos

        public virtual bool Equals(AlteracaoPedido other)
        {
            return (this.Codigo == other.Codigo);
        }

        #endregion
    }
}
