using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Email
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMAIL_DOCUMENTACAO_CARGA", EntityName = "EmailDocumentacaoCarga", Name = "Dominio.Entidades.Email.EmailDocumentacaoCarga", NameType = typeof(EmailDocumentacaoCarga))]
    public class EmailDocumentacaoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_PESSOA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Emails", Column = "EDC_EMAILS", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Emails { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarCTe", Column = "EDC_ENVIAR_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarCTeXML", Column = "EDC_ENVIAR_CTE_XML", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarCTeXML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarCTePDF", Column = "EDC_ENVIAR_CTE_PDF", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarCTePDF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarMDFe", Column = "EDC_ENVIAR_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarContratoFrete", Column = "EDC_ENVIAR_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarCIOT", Column = "EDC_ENVIAR_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EnviarCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgruparEnvioEmUmUnicoEmail", Column = "EDC_AGRUPAR_ENVIO_EM_UM_UNICO_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparEnvioEmUmUnicoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMAIL_DOCUMENTACAO_CARGA_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EDC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Pedidos.TipoOperacao> TiposOperacao { get; set; }

        public virtual string Descricao
        {
            get { return Pessoa.NomeCNPJ; }
        }
    }
}
