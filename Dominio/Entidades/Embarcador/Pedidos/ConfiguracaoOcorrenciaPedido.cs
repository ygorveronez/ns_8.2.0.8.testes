namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_OCORRENCIA_PEDIDO", EntityName = "ConfiguracaoOcorrenciaPedido", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido", NameType = typeof(ConfiguracaoOcorrenciaPedido))]
    public class ConfiguracaoOcorrenciaPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoDeOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EventoColetaEntrega", Column = "COP_EVENTO_COLETA_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega EventoColetaEntrega { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}