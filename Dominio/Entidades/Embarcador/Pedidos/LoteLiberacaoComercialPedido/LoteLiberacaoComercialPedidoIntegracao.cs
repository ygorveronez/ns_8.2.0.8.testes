using Dominio.Interfaces.Embarcador.Integracao;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_LIBERACAO_COMERCIAL_PEDIDO_INTEGRACAO", EntityName = "LoteLiberacaoComercialPedidoIntegracao", Name = "Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao", NameType = typeof(LoteLiberacaoComercialPedidoIntegracao))]
    public class LoteLiberacaoComercialPedidoIntegracao : Integracao.Integracao, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteLiberacaoComercialPedido", Column = "LLC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LoteLiberacaoComercialPedido LoteLiberacaoComercialPedido { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LOTE_LIBERACAO_COMERCIAL_PEDIDO_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(LoteLiberacaoComercialPedidoIntegracao other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}