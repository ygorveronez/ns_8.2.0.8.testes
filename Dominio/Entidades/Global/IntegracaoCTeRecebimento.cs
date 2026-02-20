using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_CTE_RECEBIMENTO", EntityName = "IntegracaoCTeRecebimento", Name = "Dominio.Entidades.IntegracaoCTeRecebimento", NameType = typeof(IntegracaoCTeRecebimento))]
    public class IntegracaoCTeRecebimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ICR_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataStatus", Column = "ICR_DATA_STATUS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ICR_STATUS", TypeType = typeof(Enumeradores.StatusIntegracaoRecebimentoCTe), NotNull = true)]
        public virtual Dominio.Enumeradores.StatusIntegracaoRecebimentoCTe Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "ICR_TIPO", TypeType = typeof(Enumeradores.TipoIntegracaoEnvioCTe), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoIntegracaoEnvioCTe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Log", Column = "ICR_LOG", Type = "StringClob", NotNull = false)]
        public virtual string Log { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "ICR_USUARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoVeiculo", Column = "ICR_TIPO_VEICULO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TipoVeiculo { get; set; }
    }
}
