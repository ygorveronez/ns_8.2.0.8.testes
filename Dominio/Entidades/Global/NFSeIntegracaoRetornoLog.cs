using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFSE_INTEGRACAO_RETORNO_LOG", EntityName = "NFSeIntegracaoRetornoLog", Name = "Dominio.Entidades.NFSeIntegracaoRetornoLog", NameType = typeof(NFSeIntegracaoRetornoLog))]
    public class NFSeIntegracaoRetornoLog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSeIntegracaoRetorno", Column = "NIR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFSeIntegracaoRetorno NFSeIntegracaoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "NIL_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "NIL_MENSAGEM", Type = "StringClob", NotNull = true)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Request", Column = "NIL_REQUEST", Type = "StringClob", NotNull = true)]
        public virtual string Request { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Response", Column = "NIL_RESPONSE", Type = "StringClob", NotNull = true)]
        public virtual string Response { get; set; }


    }
}
