namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_NSTECH", EntityName = "IntegracaoNSTech", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech", NameType = typeof(IntegracaoNSTech))]
    public class IntegracaoNSTech : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INS_URL_AUTENTICACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UrlAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INS_SENHA_AUTENTICACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INS_ID_AUTENTICACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IDAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INS_URL_INTEGRACAO_SM", TypeType = typeof(string), Length = 850, NotNull = false)]
        public virtual string UrlIntegracaoSM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INS_URL_INTEGRACAO_SOLICITACAO_CADASTRO", TypeType = typeof(string), Length = 850, NotNull = false)]
        public virtual string UrlIntegracaoSolicitacaoCadastral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INS_URL_INTEGRACAO_VERIFICACAO_CADASTRO", TypeType = typeof(string), Length = 850, NotNull = false)]
        public virtual string UrlIntegracaoVerificacaoCadastral { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração NSTech";
            }
        }

    }
}
