using System;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SESSAO_ROTEIRIZADOR_PEDIDO", EntityName = "SessaoRoteirizadorPedido", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido", NameType = typeof(SessaoRoteirizadorPedido))]
    public class SessaoRoteirizadorPedido : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SessaoRoteirizador", Column = "SRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SessaoRoteirizador SessaoRoteirizador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido Pedido { get; set; }
                
        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "SRP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido Situacao { get; set; }

        public virtual bool Equals(SessaoRoteirizadorPedido other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
