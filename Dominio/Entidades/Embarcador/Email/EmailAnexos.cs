namespace Dominio.Entidades.Embarcador.Email
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMAIL_ANEXO", EntityName = "EmailAnexos", Name = "Dominio.Entidades.Email.EmailAnexos", NameType = typeof(EmailAnexos))]
    public class EmailAnexos: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EAN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidNomeArquivo", Column = "EAN_GUID_NOME_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string GuidNomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "EAN_NOME_ARQUIVO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArquivoZipado", Column = "EAN_ARQUIVO_ZIPADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ArquivoZipado { get; set; }

    }
}
