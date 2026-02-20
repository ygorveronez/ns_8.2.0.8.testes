namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_P44", EntityName = "IntegracaoP44", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoP44", NameType = typeof(IntegracaoP44))]
	public class IntegracaoP44 : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIP_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIP_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool PossuiIntegracao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacao", Column = "CIP_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
		public virtual string URLAutenticacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "URLAplicacao", Column = "CIP_URL_APLICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
		public virtual string URLAplicacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoPatio", Column = "CIP_URL_INTEGRACAO_PATIO", TypeType = typeof(string), Length = 500, NotNull = false)]
		public virtual string URLIntegracaoPatio { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ClientId", Column = "CIP_CLIENT_ID", TypeType = typeof(string), Length = 500, NotNull = false)]
		public virtual string ClientId { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "CIP_CLIENT_SECRET", TypeType = typeof(string), Length = 500, NotNull = false)]
		public virtual string ClientSecret { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIP_USUARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
		public virtual string Usuario { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIP_SENHA", TypeType = typeof(string), Length = 150, NotNull = false)]
		public virtual string Senha { get; set; }

	}
}
