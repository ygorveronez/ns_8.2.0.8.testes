using System;

namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFS_MANUAL_CANCELAMENTO_INTEGRACAO", EntityName = "NFSManualCancelamentoIntegracao", Name = "Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracao", NameType = typeof(NFSManualCancelamentoIntegracao))]
    public class NFSManualCancelamentoIntegracao : Integracao.Integracao, IEquatable<NFSManualCancelamentoIntegracao>
    {
        public NFSManualCancelamentoIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ILN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSManualCancelamento", Column = "NMC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFSManualCancelamento NFSManualCancelamento { get; set; }

        public virtual bool Equals(NFSManualCancelamentoIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
