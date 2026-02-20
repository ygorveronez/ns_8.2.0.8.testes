namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_PLANEJAMENTO_PEDIDO_CONFIGURACAO", EntityName = "PlanejamentoPedidoConfiguracao", Name = "Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoConfiguracao", NameType = typeof(PlanejamentoPedidoConfiguracao))]
    public class PlanejamentoPedidoConfiguracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "PPC_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = true)]
        public virtual string Email { get; set; }

        

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}





