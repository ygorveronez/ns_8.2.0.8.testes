using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_FROTA_162", EntityName = "IntegracaoFrota162", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrota162", NameType = typeof(IntegracaoFrota162))]
    public class IntegracaoFrota162 : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_POSSUI_INTEGRACAO_FROTA_162", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoFrota162 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_USUARIO_FROTA_162", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_SENHA_FROTA_162", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [Obsolete("Desnecessário, só precisava um campo de URL")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_URL_FROTA_162_HOMOLOGACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLHomologacaoFrota162 { get; set; }

        [Obsolete("Desnecessário, só precisava um campo de URL")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_URL_FROTA_162_PRODUCAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLProducaoFrota162 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_URL_FROTA_162", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_TOKEN_FROTA_162", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_SECRET_KEY_FROTA_162", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SecretKey { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_COMPANY_ID_FROTA_162", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CompanyId { get; set; }
    }
}
