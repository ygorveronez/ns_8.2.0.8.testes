using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFS_MANUAL_CANCELAMENTO_INTEGRACAO_CTE", EntityName = "NFSManualCancelamentoIntegracaoCTe", Name = "Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe", NameType = typeof(NFSManualCancelamentoIntegracaoCTe))]
    public class NFSManualCancelamentoIntegracaoCTe : Integracao.Integracao, IEquatable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSManualCancelamento", Column = "NMC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento NFSManualCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LancamentoNFSManual", Column = "LNM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual LancamentoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSManualCancelamentoIntegracaoLote", Column = "NIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoLote Lote { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NFS_MANUAL_CANCELAMENTO_INTEGRACAO_CTE_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NFSManualIntegracaoArquivo", Column = "NMA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Integração - " + (this.NFSManualCancelamento?.Descricao ?? string.Empty);
            }
        }

        public virtual bool Equals(NFSManualCancelamentoIntegracaoCTe other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
