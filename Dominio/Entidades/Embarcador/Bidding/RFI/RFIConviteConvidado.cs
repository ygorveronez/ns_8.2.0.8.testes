using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Bidding.RFI
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RFI_CONVITE_CONVIDADO", EntityName = "RFIConviteConvidado", Name = "Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteConvidado", NameType = typeof(RFIConviteConvidado))]
    public class RFIConviteConvidado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RFIConvite", Column = "TRC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RFIConvite RFIConvite { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "CON_CODIGO", NotNull = true)]
        public virtual Empresa Convidado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCC_STATUS", TypeType = typeof(StatusRFIConviteConvidado), NotNull = true)]
        public virtual StatusRFIConviteConvidado Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCC_STATUS_RFI", TypeType = typeof(StatusRFIConvite), NotNull = true)]
        public virtual StatusRFIConvite StatusRFI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCC_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCC_EMAIL_AVISO_CONVIDADO_ENVIADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailAvisoConvidadoEnviado { get; set; }
    }
}
