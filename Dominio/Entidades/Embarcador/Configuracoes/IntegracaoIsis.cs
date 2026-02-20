namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_ISIS", EntityName = "IntegracaoIsis", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIsis", NameType = typeof(IntegracaoIsis))]
    public class IntegracaoIsis : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CII_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CII_POSSUI_INTEGRACAO_FTP", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiIntegracaoFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoFTP", Column = "CII_ENDERECO_FTP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string EnderecoFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CII_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CII_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "CII_PORTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Diretorio", Column = "CII_DIRETORIO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Diretorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CII_PASSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Passivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CII_UTILIZAR_SFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarSFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CII_UTILIZAR_SSL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SSL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CII_NOMENCLATURA_ARQUIVO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NomenclaturaArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CII_NOMENCLATURA_ARQUIVO_CARREGAMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NomenclaturaArquivoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiretorioCarregamento", Column = "CII_DIRETORIO_CARREGAMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DiretorioCarregamento { get; set; }
    }
}
