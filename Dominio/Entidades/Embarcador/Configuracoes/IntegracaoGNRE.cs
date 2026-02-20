namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_GNRE", EntityName = "IntegracaoGNRE", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGNRE", NameType = typeof(IntegracaoGNRE))]
    public class IntegracaoGNRE: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoGNRE", Column = "CIG_POSSUI_INTEGRACAO_GNRE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoGNRE { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoGNRE", Column = "CIG_URL_INTEGRACAO_GNRE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string URLIntegracaoGNRE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioIntegracaoGNRE", Column = "CIG_USUARIO_INTEGRACAO_GNRE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioIntegracaoGNRE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaIntegracaoGNRE", Column = "CIG_SENHA_INTEGRACAO_GNRE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaIntegracaoGNRE { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração GNRE";
            }
        }
    }
}