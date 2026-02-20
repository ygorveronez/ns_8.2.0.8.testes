using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFS_MANUAL_CTE_INTEGRACAO", EntityName = "NFsManualCTeIntegracao", Name = "Dominio.Entidades.Embarcador.NFS.NFsManualCTeIntegracao", NameType = typeof(NFSManualCTeIntegracao))]
    public class NFSManualCTeIntegracao : Integracao.Integracao, IIntegracaoComArquivo<NFSManualIntegracaoArquivo>, IEquatable<NFSManualCTeIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LancamentoNFSManual", Column = "LNM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LancamentoNFSManual LancamentoNFSManual { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSManualIntegracaoLote", Column = "NIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFSManualIntegracaoLote Lote { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NFS_MANUAL_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NFSManualIntegracaoArquivo", Column = "NMA_CODIGO")]
        public virtual ICollection<NFSManualIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoExternoRetornoIntegracao", Column = "CCI_CODIGO_EXTERNO_RETORNO_INTEGRACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string CodigoExternoRetornoIntegracao { get; set; }


        public virtual string Descricao
        {
            get
            {
                return "Integração - " + (this.LancamentoNFSManual?.Descricao ?? string.Empty);
            }
        }

        public virtual bool Equals(NFSManualCTeIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
