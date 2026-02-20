namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_FTP_AMAZON", EntityName = "ConfiguracaoFTPAmazon", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFTPAmazon", NameType = typeof(ConfiguracaoFTPAmazon))]
    public class ConfiguracaoFTPAmazon : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_ENDERECO_FTP", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string EnderecoFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CFA_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CFA_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "CFA_PORTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Diretorio", Column = "CFA_DIRETORIO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Diretorio { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DiretorioDownload", Column = "CFA_DIRETORIO_DOWNLOAD", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string DiretorioDownload { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_PASSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Passivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_UTILIZAR_SFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarSFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_UTILIZAR_SSL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SSL { get; set; }
    }
}
