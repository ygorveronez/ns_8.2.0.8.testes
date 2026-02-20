namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_ARQUIVO", EntityName = "ConfiguracaoArquivo", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoArquivo", NameType = typeof(ConfiguracaoArquivo))]
    public class ConfiguracaoArquivo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_RELATORIOS", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoRelatorios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_TEMPORARIO_ARQUIVOS_IMPORTACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoTempArquivosImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_CANHOTOS", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_CANHOTOS_AVULSOS", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoCanhotosAvulsos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_XML_NOTA_FISCAL_COMPROVANTE_ENTREGA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoXMLNotaFiscalComprovanteEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_ARQUIVOS_INTEGRACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoArquivosIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_RELATORIOS_EMBARCADOR", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoRelatoriosEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_LOGO_EMBARCADOR", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoLogoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_DOCUMENTOS_FISCAIS_EMBARCADOR", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoDocumentosFiscaisEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_ANEXOS", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_GERADOR_RELATORIOS", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoGeradorRelatorios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_ARQUIVOS_EMPRESAS", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoArquivosEmpresas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_RELATORIOS_CRYSTAL", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoRelatoriosCrystal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_RETORNO_XML_INTEGRADOR", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoRetornoXMLIntegrador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_ARQUIVOS", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoArquivos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_ARQUIVOS_INTEGRACAO_EDI", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoArquivosIntegracaoEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_ARQUIVOS_IMPORTACAO_BOLETO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoArquivosImportacaoBoleto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_OCORRENCIAS", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoOcorrencias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_OCORRENCIAS_MOBILE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoOcorrenciasMobiles { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_ARQUIVOS_IMPORTACAO_XML_NOTA_FISCAL", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoArquivosImportacaoXMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_DESTINO_XML", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoDestinoXML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_CANHOTOS_ANTIGOS", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoCanhotosAntigos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_RAIZ", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoRaiz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_GUIA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoGuia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_DANFE_SMS", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoDanfeSMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_RAIZ_FTP", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoRaizFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_DOCUMENTOS_INPUT", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoDocumentosINPUT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAMINHO_DOCUMENTOS_OUTPUT", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoDocumentosOUTPUT { get; set; }  
        
        
    }

}
