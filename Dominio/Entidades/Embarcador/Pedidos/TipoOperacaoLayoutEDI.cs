namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_LAYOUT_EDI", EntityName = "TipoOperacaoLayoutEDI", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI", NameType = typeof(TipoOperacaoLayoutEDI))]
    public class TipoOperacaoLayoutEDI : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TLY_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LayoutEDI LayoutEDI { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Emails", Column = "TLY_EMAILS", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Emails { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoFTP", Column = "TLY_ENDERECO_FTP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string EnderecoFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "TLY_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "TLY_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "TLY_PORTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Diretorio", Column = "TLY_DIRETORIO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Diretorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Passivo", Column = "TLY_PASSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Passivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TLY_UTILIZAR_SFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarSFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TLY_UTILIZAR_SSL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SSL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TLY_CRIAR_COM_NOME_TEMPORARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CriarComNomeTemporaraio { get; set; }

        /// <summary>
        /// Se tiver o tempo informado faz o reenvio automaticamente da integração após o tempo determinado (somente quando a tentativa anterior estiver integrada).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ReenviarAutomaticamenteOutraVezAposMinutos", Column = "TLY_REENVIAR_AUTOMATICAMENTE_APOS_MINUTOS", TypeType = typeof(int), NotNull = false)]
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

