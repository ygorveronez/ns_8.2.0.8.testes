using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IMPORTACAO_FTP", EntityName = "ImportacaoFTP", Name = "Dominio.Entidades.ImportacaoFTP", NameType = typeof(ImportacaoFTP))]
    public class ImportacaoFTP : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IFT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacao", Column = "IFT_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProcessamento", Column = "IFT_DATA_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArquivoOriginal", Column = "IFT_ARQUIVO_ORIGINAL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ArquivoOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArquivoSalvo", Column = "IFT_ARQUIVO_SALVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ArquivoSalvo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExtencaoArquivo", Column = "IFT_EXTENCAO_ARQUIVO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ExtencaoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemProcessamento", Column = "IFT_MENSAGEM_PROCESSAMENTO", Type = "StringClob", NotNull = false)]
        public virtual string MensagemProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarContratanteComoTomador", Column = "IFT_UTILIZAR_CONTRATANTE_COMO_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarContratanteComoTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "IFT_STATUS", TypeType = typeof(Dominio.Enumeradores.StatusImportacaoFTP), NotNull = false)]
        public virtual Dominio.Enumeradores.StatusImportacaoFTP Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IFT_TIPO_ARQUIVO", TypeType = typeof(Dominio.Enumeradores.TipoProcessamentoArquivoFTP), NotNull = false)]
        public virtual Enumeradores.TipoProcessamentoArquivoFTP TipoArquivo { get; set; }
    }
}
