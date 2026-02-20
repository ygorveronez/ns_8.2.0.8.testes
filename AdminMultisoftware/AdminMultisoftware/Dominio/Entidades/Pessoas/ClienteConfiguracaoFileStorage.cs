namespace AdminMultisoftware.Dominio.Entidades.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_CONFIGURACAO_FILE_STORAGE", EntityName = "ClienteConfiguracaoFileStorage", Name = "AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracaoFileStorage", NameType = typeof(ClienteConfiguracaoFileStorage))]
    public class ClienteConfiguracaoFileStorage : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pessoas.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFS_STORAGE_HOMOLOGACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Homologacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFS_FILE_STORAGE_TYPE", TypeType = typeof(Dominio.Enumeradores.FileStorageType), NotNull = true)]
        public virtual Dominio.Enumeradores.FileStorageType TipoFileStorage { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFS_AZURE_CONNECTION_STRING", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string AzureConnectionString { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFS_AZURE_CONTAINER_NAME", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string AzureContainerName { get; set; }
    }
}
