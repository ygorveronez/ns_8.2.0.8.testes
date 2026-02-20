using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_CTE_IMPRESSAO", EntityName = "ImpressaoIntegracaoCTe", Name = "Dominio.Entidades.ImpressaoIntegracaoCTe", NameType = typeof(ImpressaoIntegracaoCTe))]
    public class ImpressaoIntegracaoCTe:EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSolicitacao", Column = "ICI_DATA_SOLICITACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataSolicitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ICI_STATUS", TypeType = typeof(Dominio.Enumeradores.StatusImpressaoCTe), NotNull = true)]
        public virtual Dominio.Enumeradores.StatusImpressaoCTe Status { get; set; }
    }
}
