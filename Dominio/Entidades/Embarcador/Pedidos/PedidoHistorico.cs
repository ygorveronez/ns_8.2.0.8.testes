using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_HISTORICO", EntityName = "PedidoHistorico", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoHistorico", NameType = typeof(PedidoHistorico))]
    public class PedidoHistorico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PHI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Historico", Column = "PHI_HISTORICO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Historico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PHI_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }

    }
}
