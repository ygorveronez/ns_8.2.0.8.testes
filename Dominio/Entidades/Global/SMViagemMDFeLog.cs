using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_SM_VIAGEM_LOG", EntityName = "SMViagemMDFeLog", Name = "Dominio.Entidades.SMViagemMDFeLog", NameType = typeof(SMViagemMDFeLog))]
    public class SMViagemMDFeLog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MVX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SMViagemMDFe", Column = "MSV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SMViagemMDFe SMViagemMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVX_DATAHORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVX_REQUISICAO", Type = "StringClob", NotNull = false)]
        public virtual string Requisicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVX_RESPOSTA", Type = "StringClob", NotNull = false)]
        public virtual string Resposta { get; set; }
        
    }
}
