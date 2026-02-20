using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_PEDIDO_CARREGAMENTO_SITUACAO", EntityName = "PedidoCarregamentoSituacao", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoCarregamentoSituacao", NameType = typeof(PedidoCarregamentoSituacao))]
    public class PedidoCarregamentoSituacao : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "STP_PEDIDO_CARREGAMENTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAtualPedidoRetirada", Column = "STP_SITUACAO_ATUAL_PEDIDO_RETIRADA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada SituacaoAtualPedidoRetirada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacaoPedido", Column = "STP_DATA_CRIACAO_PEDIDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiberacaoComercial", Column = "STP_DATA_LIBERACAO_COMERCIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacaoComercial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiberacaoFinanceira", Column = "STP_DATA_LIBERACAO_FINANCEIRA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacaoFinanceira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAgendamento", Column = "STP_DATA_AGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRemessaConcluida", Column = "STP_DATA_REMESSA_CONCLUIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRemessaConcluida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarregamentoConcluido", Column = "STP_DATA_CARREGAMENTO_CONCLUIDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarregamentoConcluido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFaturamentoConcluido", Column = "STP_DATA_FATURAMENTO_CONCLUIDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFaturamentoConcluido { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

    }
}
