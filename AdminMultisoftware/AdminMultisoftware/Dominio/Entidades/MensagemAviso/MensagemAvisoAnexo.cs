namespace AdminMultisoftware.Dominio.Entidades.MensagemAviso
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MENSAGEM_AVISO_ANEXO", EntityName = "MensagemAvisoAnexo", Name = "AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAvisoAnexo", NameType = typeof(MensagemAvisoAnexo))]
    public class MensagemAvisoAnexo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ANX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MensagemAviso", Column = "MAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MensagemAviso MensagemAviso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANX_DESCRICAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANX_NOME_ARQUIVO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANX_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(Column = "ANX_ARQUIVO", Type = "BinaryBlob", NotNull = false)]
        public virtual byte[] Arquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANX_EXTENSAO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Extensao { get; set; }


        public virtual string ExtensaoArquivo
        {
            get { return System.IO.Path.GetExtension(NomeArquivo).ToLower().Replace(".", ""); }
        }
    }
}
