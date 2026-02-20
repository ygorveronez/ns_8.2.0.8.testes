namespace Dominio.Entidades.Embarcador.PagamentoMotorista
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PENDENCIA_MOTORISTA_ANEXOS", EntityName = "PendenciaMotoristaAnexo", Name = "Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo", NameType = typeof(PendenciaMotoristaAnexo))]

    public class PendenciaMotoristaAnexo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "APE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PendenciaMotorista", Column = "PEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PendenciaMotorista PendenciaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APE_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APE_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APE_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }

        public virtual string ExtensaoArquivo
        {
            get { return System.IO.Path.GetExtension(NomeArquivo).ToLower().Replace(".", ""); }
        }

        public virtual string NomeArquivoSemExtensao
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(NomeArquivo); }
        }

    }
}
