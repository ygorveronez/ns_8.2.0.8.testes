using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFe_INTEGRACAO_RETORNO_LOG", EntityName = "MDFeIntegracaoRetornoLog", Name = "Dominio.Entidades.MDFeIntegracaoRetornoLog", NameType = typeof(MDFeIntegracaoRetornoLog))]
    public class MDFeIntegracaoRetornoLog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MDFeIntegracaoRetorno", Column = "MIR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MDFeIntegracaoRetorno MDFeIntegracaoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "MIL_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "MIL_MENSAGEM", Type = "StringClob", NotNull = true)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Request", Column = "MIL_REQUEST", Type = "StringClob", NotNull = true)]
        public virtual string Request { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Response", Column = "MIL_RESPONSE", Type = "StringClob", NotNull = true)]
        public virtual string Response { get; set; }


    }
}
