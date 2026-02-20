using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_OBSERVACAO", EntityName = "PedidoObservacao", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoObservacao", NameType = typeof(PedidoObservacao))]
    public class PedidoObservacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]  
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEO_DATA_HORA_INCLUSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataHoraInclusao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEO_OBSERVACAO", Type = "StringClob", NotNull = true)]
        public virtual string Observacao { get; set; }

    }
}
