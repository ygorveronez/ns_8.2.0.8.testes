namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_DOCUMENTACAO_AFRMM", EntityName = "ConfiguracaoDocumentacaoAFRMM", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM", NameType = typeof(ConfiguracaoDocumentacaoAFRMM))]
    public class ConfiguracaoDocumentacaoAFRMM : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDM_QUANTIDADE_DIAS_APOS_DESCARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasAposDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDM_ENDERECO_FTP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string EnderecoFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDM_PORTA_FTP", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string PortaFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDM_DIRETORIO_FTP", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string DiretorioFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDM_DIRETORIO_FTP_SUBCONTRATACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string DiretorioFTPSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDM_DIRETORIO_FTP_REDESPACHO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string DiretorioFTPRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDM_USUARIO_FTP", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDM_SENHA_FTP", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDM_FTP_PASSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FTPPassivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDM_SFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDM_SSL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SSL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDM_EMAIL_FALHA_ENVIO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string EmailFalhaEnvio { get; set; }

    }
}
