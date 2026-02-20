using Dominio.Entidades.Embarcador.Pedidos;
using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AGENDAMENTO_ENTREGA_PEDIDO_CONSULTA_AUDITORIA", EntityName = "AgendamentoEntregaPedidoConsultaAuditoria", Name = "Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria", NameType = typeof(AgendamentoEntregaPedidoConsultaAuditoria))]
    public class AgendamentoEntregaPedidoConsultaAuditoria : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "APA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "APA_DATA", TypeType = typeof(DateTime) , NotNull = true)]
        public virtual DateTime Data { get; set; }
    }
}
