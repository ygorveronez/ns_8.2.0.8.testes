namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_BRADO", EntityName = "IntegracaoBrado", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado", NameType = typeof(IntegracaoBrado))]
    public class IntegracaoBrado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIB_POSSUI_INTEGRACAO", TypeType = typeof(bool), Length = 500, NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacao", Column = "CIB_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIB_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIB_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoGestao", Column = "CIB_CODIGO_GESTAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string CodigoGestao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLEnvioDadosTransporte", Column = "CIB_URL_ENVIO_DADOS_TRANSPORTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEnvioDadosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLEnvioDocumentosEmitidos", Column = "CIB_URL_ENVIO_DOCUMENTOS_EMITIDOS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEnvioDocumentosEmitidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLCancelamentoBrado", Column = "CIB_URL_ENVIO_CANCELAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLCancelamentoBrado { get; set; }
    }
}
