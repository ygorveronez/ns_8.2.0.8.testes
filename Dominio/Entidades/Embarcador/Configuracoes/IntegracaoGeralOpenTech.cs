namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_GERAL_OPENTECH", EntityName = "IntegracaoGeralOpenTech", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralOpenTech", NameType = typeof(IntegracaoGeralOpenTech))]
    public class IntegracaoGeralOpenTech : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarDataNFeNaDataPrevistaOpentech", Column = "CGO_ENVIAR_DATA_NFE_DATA_PREVISTA_OPENTECH", TypeType = typeof(bool), Length = 200, NotNull = false)]
        public virtual bool EnviarDataNFeNaDataPrevistaOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarLocalidadeProdutoIntegracaoEntrega", Column = "CGO_CONSIDERAR_LOCALIDADE_PRODUTO_INTEGRACAO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarLocalidadeProdutoIntegracaoEntrega { get; set; }
    }
}
