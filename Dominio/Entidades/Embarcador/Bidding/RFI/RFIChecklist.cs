using System;

namespace Dominio.Entidades.Embarcador.Bidding.RFI
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RFI_CHECKLIST", EntityName = "RFIChecklist", Name = "Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklist", NameType = typeof(RFIChecklist))]
    public class RFIChecklist : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCL_PRAZO_CHECKLIST", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataPrazo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RFIConvite", Column = "TRC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RFIConvite RFIConvite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCL_DATA_LIMITE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLimite { get; set; }
    }
}
