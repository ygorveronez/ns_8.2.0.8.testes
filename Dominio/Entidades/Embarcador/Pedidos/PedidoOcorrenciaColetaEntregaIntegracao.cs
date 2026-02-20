using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_OCORRENCIA_COLETA_ENTREGA_INTEGRACAO", EntityName = "PedidoOcorrenciaColetaEntregaIntegracao", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao", NameType = typeof(PedidoOcorrenciaColetaEntregaIntegracao))]
    public class PedidoOcorrenciaColetaEntregaIntegracao : Integracao.Integracao
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoOcorrenciaColetaEntrega", Column = "POC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PedidoOcorrenciaColetaEntrega PedidoOcorrenciaColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.LayoutEDI LayoutEDI { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_OCORRENCIA_COLETA_ENTREGA_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get { return $"Integração {Codigo}"; }
        }
    }
}
