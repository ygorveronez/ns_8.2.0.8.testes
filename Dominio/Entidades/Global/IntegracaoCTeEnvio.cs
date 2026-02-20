using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_CTE_ENVIO", EntityName = "IntegracaoCTeEnvio", Name = "Dominio.Entidades.IntegracaoCTeEnvio", NameType = typeof(IntegracaoCTeEnvio))]
    public class IntegracaoCTeEnvio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ICE_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvio", Column = "ICE_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ICE_STATUS", TypeType = typeof(Enumeradores.StatusIntegracaoEnvioCTe), NotNull = true)]
        public virtual Dominio.Enumeradores.StatusIntegracaoEnvioCTe Status { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "ICE_TIPO", TypeType = typeof(Enumeradores.TipoIntegracaoEnvioCTe), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoIntegracaoEnvioCTe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Log", Column = "ICE_LOG", Type = "StringClob", NotNull = false)]
        public virtual string Log { get; set; }
    }
}
