using System;

namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFS_MANUAL_INTEGRACAO", EntityName = "LancamentoNFSIntegracao", Name = "Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao", NameType = typeof(LancamentoNFSIntegracao))]
    public class LancamentoNFSIntegracao : Integracao.Integracao, IEquatable<LancamentoNFSIntegracao>
    {
        public LancamentoNFSIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ILN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LancamentoNFSManual", Column = "LNM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LancamentoNFSManual LancamentoNFSManual { get; set; }

        public virtual bool Equals(LancamentoNFSIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
