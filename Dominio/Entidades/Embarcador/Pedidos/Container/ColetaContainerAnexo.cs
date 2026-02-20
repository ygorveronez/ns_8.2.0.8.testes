namespace Dominio.Entidades.Embarcador.Pedidos
{
    //OBS: Não foi herdado do ANEXO por ter mais de uma entidade subjacente e pode ter mais..

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLETA_CONTAINER_ANEXO", EntityName = "ColetaContainerAnexo", Name = "Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo", NameType = typeof(ColetaContainerAnexo))]
    public class ColetaContainerAnexo : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ColetaContainer", Column = "CCR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainer ColetaContainer { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ColetaContainerAnexoRic", Column = "RIC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexoRic ColetaContainerAnexoRic { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_DESCRICAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_NOME_ARQUIVO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        public virtual string ExtensaoArquivo
        {
            get { return System.IO.Path.GetExtension(NomeArquivo).ToLower().Replace(".", ""); }
        }

    }
}
