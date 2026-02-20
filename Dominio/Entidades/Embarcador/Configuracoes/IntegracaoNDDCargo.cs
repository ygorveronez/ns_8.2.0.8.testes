using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_NDDCARGO", EntityName = "IntegracaoNDDCargo", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNDDCargo", NameType = typeof(IntegracaoNDDCargo))]
    public class IntegracaoNDDCargo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIN_URL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnterpriseId", Column = "CIN_ENTERPRISE_ID", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string EnterpriseId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "CIN_TOKEN", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Versao", Column = "CIN_VERSAO", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string Versao { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração Integração NDD Cargo"; }
        }
    }
}
