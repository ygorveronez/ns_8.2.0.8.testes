
namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_HUB", EntityName = "IntegracaoHUB", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHUB", NameType = typeof(IntegracaoHUB))]
    public class IntegracaoHUB : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlAutenticacaoToken", Column = "CIH_URL_AUTENTICACAO_TOKEN", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string UrlAutenticacaoToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlIntegracao", Column = "CIH_URL_INTEGRACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string UrlIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdOrganizacao", Column = "CIH_ID_ORGANIZACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IdOrganizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdCliente", Column = "CIH_ID_CLIENTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IdCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConexaoServiceBUS", Column = "CIH_CONEXAO_SERVICE_BUS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ConexaoServiceBUS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveSecreta", Column = "CIH_CHAVE_SECRETA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveSecreta { get; set; }

    }
}
