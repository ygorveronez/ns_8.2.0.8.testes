namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIG_FTP", EntityName = "ConfiguracaoFTP", Name = "Dominio.Entidades.ConfiguracaoFTP", NameType = typeof(ConfiguracaoFTP))]

    public class ConfiguracaoFTP : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoEmpresa", Column = "COF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "save-update")]
        public virtual ConfiguracaoEmpresa Configuracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "FTP_TIPO", TypeType = typeof(Enumeradores.TipoArquivoFTP), NotNull = false)]
        public virtual Enumeradores.TipoArquivoFTP Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FTP_TIPO_ARQUIVO", TypeType = typeof(Enumeradores.TipoProcessamentoArquivoFTP), NotNull = false)]
        public virtual Enumeradores.TipoProcessamentoArquivoFTP TipoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Host", Column = "FTP_HOST", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Host { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "FTP_PORTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "FTP_USUARIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "FTP_SENHA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Diretorio", Column = "FTP_DIRETORIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Diretorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Passivo", Column = "FTP_PASSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Passivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Seguro", Column = "FTP_SEGURO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Seguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SSL", Column = "FTP_SSL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SSL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FTP_EMITIR_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FTP_GERAR_NFSE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FTP_RATEIO", TypeType = typeof(Enumeradores.TipoRateioFTP), NotNull = false)]
        public virtual Enumeradores.TipoRateioFTP Rateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarContratanteComoTomador", Column = "FTP_UTILIZAR_CONTRATANTE_COMO_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarContratanteComoTomador { get; set; }

        public virtual string DescricaoTipoArquivo 
        {
            get
            {
                switch (this.TipoArquivo)
                {
                    case Enumeradores.TipoProcessamentoArquivoFTP.PorExtensao:
                        return "Obter pela extensão";
                    case Enumeradores.TipoProcessamentoArquivoFTP.Texto:
                        return "Texto";
                    case Enumeradores.TipoProcessamentoArquivoFTP.XML:
                        return "XML";
                    case Enumeradores.TipoProcessamentoArquivoFTP.CSV:
                        return "CSV";
                    case Enumeradores.TipoProcessamentoArquivoFTP.XLSX:
                        return "Excel";
                    case Enumeradores.TipoProcessamentoArquivoFTP.XLSXRiachuelo:
                        return "Excel (Riachuelo)";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoRateio {
            get {
                switch(this.Rateio)
                {
                    case Enumeradores.TipoRateioFTP.Destinatario:
                        return "Remetente";
                    case Enumeradores.TipoRateioFTP.Remetente:
                        return "Destinatário";
                    case Enumeradores.TipoRateioFTP.RemetenteDestinatario:
                        return "Remetente e Destinatário";
                    case Enumeradores.TipoRateioFTP.PorNFe:
                        return "CTe por NF-e";
                    default:
                        return "";
                }
            }
        }
    }
}
