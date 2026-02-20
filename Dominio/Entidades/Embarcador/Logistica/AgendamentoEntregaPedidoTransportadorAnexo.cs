using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AGENDAMENTO_ENTREGA_PEDIDO_TRANSPORTADOR_ANEXOS", EntityName = "AgendamentoEntregaPedidoTransportadorAnexo", Name = "Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoTransportadorAnexo", NameType = typeof(AgendamentoEntregaPedidoTransportadorAnexo))]
    public class AgendamentoEntregaPedidoTransportadorAnexo : Anexo.Anexo<Pedidos.Pedido>
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANX_DATA_CADASTRO_ARQUIVO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANX_USUARIO_CADASTRO_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string UsuarioCadastro { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Pedidos.Pedido EntidadeAnexo { get; set; }

        #endregion
    }
}
