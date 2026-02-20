namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_SENIOR", EntityName = "IntegracaoSenior", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSenior", NameType = typeof(IntegracaoSenior))]
    public class IntegracaoSenior : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIS_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacao", Column = "CIS_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracao", Column = "CIS_URL_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIS_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIS_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }
    }
}