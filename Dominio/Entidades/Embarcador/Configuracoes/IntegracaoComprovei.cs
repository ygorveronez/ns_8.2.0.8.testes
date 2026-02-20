namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_COMPROVEI", EntityName = "IntegracaoComprovei", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei", NameType = typeof(IntegracaoComprovei))]
    public class IntegracaoComprovei : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "COM_POSSUI_INTEGRACAO", TypeType = typeof(bool), Length = 500, NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "COM_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLBaseRest", Column = "COM_URL_BASE_REST", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLBaseRest { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "COM_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "COM_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIACanhoto", Column = "COM_URL_IA_CANHOTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIACanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioIACanhoto", Column = "COM_USUARIO_IA_CANHOTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string UsuarioIACanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaIACanhoto", Column = "COM_SENHA_IA_CANHOTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaIACanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoIACanhoto", Column = "COM_POSSUI_INTEGRACAO_IA_CANHOTO", TypeType = typeof(bool), Length = 500, NotNull = false)]
        public virtual bool PossuiIntegracaoIACanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoRetornoGerarCarregamento", Column = "COM_URL_INTEGRACAO_RETORNO_GERACAO_CARREGAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoRetornoGerarCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoRetornoConfirmacaoPedidos", Column = "COM_URL_INTEGRACAO_RETORNO_CONFIRMACAO_PEDIDO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoRetornoConfirmacaoPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoRetornoEnviarDigitalizacaoCanhoto", Column = "COM_URL_INTEGRACAO_RETORNO_ENVIAR_DIGITALIZACAO_CANHOTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoRetornoEnviarDigitalizacaoCanhoto { get; set; }
    }
}