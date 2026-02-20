using Dominio.Entidades.Embarcador.Pedidos;

namespace Dominio.Entidades.Embarcador.Roteirizador
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTEIRIZADOR_INTEGRACAO_PEDIDO", EntityName = "RoteirizadorIntegracaoPedido", Name = "Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido", NameType = typeof(RoteirizadorIntegracaoPedido))]
    public class RoteirizadorIntegracaoPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RIP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RoteirizadorIntegracao", Column = "RIN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RoteirizadorIntegracao RoteirizadorIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }
    }
}
