namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_CANCELAMENTO_PEDIDO", EntityName = "MotivoCancelamentoPedido", Name = "Dominio.Entidades.Embarcador.Pedidos.MotivoCancelamentoPedido", NameType = typeof(MotivoCancelamentoPedido))]
    public class MotivoCancelamentoPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MCP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MCP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "MCP_MOTIVO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Motivo { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return this.Ativo ? "Ativo" : "Inativo";
            }
        }


        public virtual bool Equals(MotivoPedido other)
        {
            return other.Codigo == this.Codigo;
        }

    }
}
