using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_INTEGRACAO_RETORNO_LOG", EntityName = "CTeIntegracaoRetornoLog", Name = "Dominio.Entidades.CTeIntegracaoRetornoLog", NameType = typeof(CTeIntegracaoRetornoLog))]
    public class CTeIntegracaoRetornoLog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeIntegracaoRetorno", Column = "CIR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CTeIntegracaoRetorno CTeIntegracaoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CIL_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "CIL_MENSAGEM", Type = "StringClob", NotNull = true)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Request", Column = "CIL_REQUEST", Type = "StringClob", NotNull = true)]
        public virtual string Request { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Response", Column = "CIL_RESPONSE", Type = "StringClob", NotNull = true)]
        public virtual string Response { get; set; }


    }
}
