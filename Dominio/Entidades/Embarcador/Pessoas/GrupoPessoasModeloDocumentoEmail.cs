namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_MODELO_DOCUMENTO_EMAIL", EntityName = "GrupoPessoasModeloDocumentoEmail", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail", NameType = typeof(GrupoPessoasModeloDocumentoEmail))]
    public class GrupoPessoasModeloDocumentoEmail : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GPD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GPD_GRUPO_PESSOA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "GPD_MODELO_DOCUMENTO_FISCAL", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Emails", Column = "GRP_EMAILS", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Emails { get; set; }
    }
}
