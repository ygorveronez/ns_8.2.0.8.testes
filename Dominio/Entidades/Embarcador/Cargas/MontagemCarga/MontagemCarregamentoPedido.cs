
namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONTAGEM_CARREGAMENTO_PEDIDO", EntityName = "MontagemCarregamentoPedido", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido", NameType = typeof(MontagemCarregamentoPedido))]
    public class MontagemCarregamentoPedido : EntidadeBase
    { 
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SessaoRoteirizador", Column = "SRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador SessaoRoteirizador { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

    }
}
