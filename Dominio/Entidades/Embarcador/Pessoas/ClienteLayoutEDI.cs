namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_LAYOUT_EDI", EntityName = "ClienteLayoutEDI", Name = "Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI", NameType = typeof(ClienteLayoutEDI))]
    public class ClienteLayoutEDI : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLY_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoIntegracao TipoIntegracao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Emails", Column = "CLY_EMAILS", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Emails { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoFTP", Column = "CLY_ENDERECO_FTP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string EnderecoFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CLY_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CLY_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "CLY_PORTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Diretorio", Column = "CLY_DIRETORIO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Diretorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Passivo", Column = "CLY_PASSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Passivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLY_UTILIZAR_SFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarSFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLY_UTILIZAR_SSL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SSL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLY_UTILIZAR_LEITURA_ARQUIVOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarLeituraArquivos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLY_ADICIONAR_EDI_FILA_PROCESSAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarEDIFilaProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailsAlertaLeituraEDI", Column = "CLY_EMAILS_ALERTA_LEITURA_EDI", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string EmailsAlertaLeituraEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLY_CRIAR_COM_NOME_TEMPORARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CriarComNomeTemporaraio { get; set; }

        /// <summary>
        /// Se tiver o tempo informado faz o reenvio automaticamente da integração após o tempo determinado (somente quando a tentativa anterior estiver integrada).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ReenviarAutomaticamenteOutraVezAposMinutos", Column = "CLY_REENVIAR_AUTOMATICAMENTE_APOS_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int ReenviarAutomaticamenteOutraVezAposMinutos { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.LayoutEDI?.Descricao ?? string.Empty;
            }
        }
    }
}
