namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FTP_IMPORTACAO_NOTA_FISCAL", EntityName = "ConfiguracaoFTPImportacaoNotaFiscal", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ConfiguracaoFTPImportacaoNotaFiscal", NameType = typeof(ConfiguracaoFTPImportacaoNotaFiscal))]
    public class ConfiguracaoFTPImportacaoNotaFiscal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoFTP", Column = "CIX_ENDERECO_FTP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string EnderecoFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIX_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIX_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "CIX_PORTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Diretorio", Column = "CIX_DIRETORIO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Diretorio { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Prefixo", Column = "CIX_PREFIXO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Prefixo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Passivo", Column = "CIX_PASSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Passivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIX_UTILIZAR_SFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarSFTP { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIX_UTILIZAR_SSL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SSL { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIX_APENAS_ATUALIZAR_DADOS_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ApenasAtualizarDadosPedido { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoImportacao", Column = "CIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Importacoes.ConfiguracaoImportacao ConfiguracaoImportacao { get; set; }
    }
}
