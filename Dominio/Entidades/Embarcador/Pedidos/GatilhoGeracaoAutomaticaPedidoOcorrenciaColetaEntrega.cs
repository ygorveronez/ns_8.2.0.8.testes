using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GATILHO_GERACAO_AUTOMATICA_PEDIDO_OCORRENCIA_COLETA_ENTREGA", EntityName = "GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega", Name = "Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega", NameType = typeof(GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega))]
    public class GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GPO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GPO_GATILHO", TypeType = typeof(TipoGatilhoPedidoOcorrenciaColetaEntrega), NotNull = true)]
        public virtual TipoGatilhoPedidoOcorrenciaColetaEntrega Gatilho { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "GPO_OBSERVACAO", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string Descricao
        {
            get { return $"Configuração de Ocorrência Automática para o tipo de Ocorrência {TipoOcorrencia.Descricao} e gatilho {Gatilho.ObterDescricao()}"; }
        }
    }
}
