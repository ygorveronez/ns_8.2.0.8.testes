namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_ROUTEASY", EntityName = "IntegracaoRouteasy", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy", NameType = typeof(IntegracaoRouteasy))]
    public class IntegracaoRouteasy : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIR_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIR_URL_CONTABILIZACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "APIKey", Column = "CIR_API_KEY", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string APIKey { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConfiguracaoLoads", Column = "CIR_CONFIGURACAO_LOADS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ConfiguracaoLoads { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarDataCriacaoVendaPedidoAbaAdicionaisIntegracao", Column = "CIR_ENVIAR_DATA_CRIACAO_VENDA_PEDIDO_ABA_ADICIONAIS_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDataCriacaoVendaPedidoAbaAdicionaisIntegracao { get; set; }

    }
}
