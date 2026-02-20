using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SIGA_FACIL_CIOT_LOGXML", EntityName = "CIOTSigaFacilLogXML", Name = "Dominio.Entidades.CIOTSigaFacilLogXML", NameType = typeof(CIOTSigaFacilLogXML))]
    public class CIOTSigaFacilLogXML : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CIOTSigaFacil", Column = "SFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CIOTSigaFacil CIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLX_DATAHORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLX_REQUISICAO", Type = "StringClob", NotNull = false)]
        public virtual string Requisicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLX_RESPOSTA", Type = "StringClob", NotNull = false)]
        public virtual string Resposta { get; set; }
    }
}
