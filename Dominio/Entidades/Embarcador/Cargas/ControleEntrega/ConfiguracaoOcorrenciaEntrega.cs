namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_OCORRENCIA_ENTREGA", EntityName = "ConfiguracaoOcorrenciaEntrega", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega", NameType = typeof(ConfiguracaoOcorrenciaEntrega))]
    public class ConfiguracaoOcorrenciaEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoDeOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAplicacaoColetaEntrega", Column = "COE_TIPO_APLICACAO_COLETA_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega TipoAplicacaoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EventoColetaEntrega", Column = "COE_EVENTO_COLETA_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega EventoColetaEntrega { get; set; }

        /// <summary>
        /// Essa flag indica que o evento está ocorrendo no alvo do pedido ou em alvos intermediarios, exemplo: quando a coleta é realizada no remetente do pedido pode gerar uma ocorrencia diferente de uma coleta realizada em um expedidor por exemplo (alvo = remetente, expedidor, destinatário ou recebedor).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "AlvoDoPedido", Column = "COE_ALVO_DO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlvoDoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Reentrega", Column = "COE_REENTREGA_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Reentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "TempoRecalculo", Type = "System.Int32", Column = "COE_TEMPO_RECALCULO")]
        public virtual int TempoRecalculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}