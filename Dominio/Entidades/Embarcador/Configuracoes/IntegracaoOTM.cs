namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_OTM", EntityName = "IntegracaoOTM", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoOTM", NameType = typeof(IntegracaoOTM))]
	public class IntegracaoOTM : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIO_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoOTM", Column = "CIO_POSSUI_INTEGRACAO_OTM", TypeType = typeof(bool), NotNull = false)]
		public virtual bool PossuiIntegracaoOTM { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ClientIDOTM", Column = "CIO_CLIENT_ID_OTM", TypeType = typeof(string), Length = 200, NotNull = false)]
		public virtual string ClientIDOTM { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecretOTM", Column = "CIO_CLIENT_SECRET_OTM", TypeType = typeof(string), Length = 200, NotNull = false)]
		public virtual string ClientSecretOTM { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoLeilaoOTM", Column = "CIO_URL_INTEGRACAO_LEILAO_OTM", TypeType = typeof(string), Length = 500, NotNull = false)]
		public virtual string URLIntegracaoLeilaoOTM { get; set; }
	}
}
