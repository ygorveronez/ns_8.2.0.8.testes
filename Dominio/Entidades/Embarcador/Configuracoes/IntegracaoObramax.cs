namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_OBRAMAX", EntityName = "IntegracaoObramax", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramax", NameType = typeof(IntegracaoObramax))]
    public class IntegracaoObramax : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIO_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Endpoint", Column = "CIO_ENDPOINT", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Endpoint { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "CIO_TOKEN", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EndpointPedidoOcorrencia", Column = "CIO_ENDPOINT_PEDIDO_OCORRENCIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EndpointPedidoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEventoCanhoto", Column = "CIO_CODIGO_EVENTO_CANHOTO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoEventoCanhoto { get; set; }
    }
}
