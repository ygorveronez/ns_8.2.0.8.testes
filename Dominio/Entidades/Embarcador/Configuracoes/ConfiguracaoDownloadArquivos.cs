namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_DOWNLOAD_ARQUIVOS", EntityName = "ConfiguracaoDownloadArquivos", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos", NameType = typeof(ConfiguracaoDownloadArquivos))]
    public  class ConfiguracaoDownloadArquivos: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirBaixarArquivosConembOcorenManualmente", Column = "CDA_PERMITIR_BAIXAR_ARQUIVOS_CONEMB_E_OCOREN_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirBaixarArquivosConembOcorenManualmente { get; set; }
    }
}
