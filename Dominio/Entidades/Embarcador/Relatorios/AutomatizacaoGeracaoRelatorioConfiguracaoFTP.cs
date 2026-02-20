namespace Dominio.Entidades.Embarcador.Relatorios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTOMATIZACAO_GERACAO_RELATORIO_CONFIGURACAO_FTP", EntityName = "AutomatizacaoGeracaoRelatorioConfiguracaoFTP", Name = "Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP", NameType = typeof(AutomatizacaoGeracaoRelatorioConfiguracaoFTP))]
    public class AutomatizacaoGeracaoRelatorioConfiguracaoFTP : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AutomatizacaoGeracaoRelatorio", Column = "AGR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AutomatizacaoGeracaoRelatorio AutomatizacaoGeracaoRelatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoFTP", Column = "ACF_ENDERECO_FTP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string EnderecoFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "ACF_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "ACF_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "ACF_PORTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Diretorio", Column = "ACF_DIRETORIO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Diretorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nomenclatura", Column = "ACF_NOMENCLATURA", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Nomenclatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACF_PASSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Passivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACF_UTILIZAR_SFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarSFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACF_UTILIZAR_SSL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SSL { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Configuração FTP - {AutomatizacaoGeracaoRelatorio.Descricao}";
            }
        }
    }
}
