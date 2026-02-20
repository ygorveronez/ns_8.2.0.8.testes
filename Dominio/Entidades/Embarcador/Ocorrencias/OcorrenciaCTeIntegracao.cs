using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_CTE_INTEGRACAO", EntityName = "OcorrenciaCTeIntegracao", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao", NameType = typeof(OcorrenciaCTeIntegracao))]
    public class OcorrenciaCTeIntegracao : Integracao.Integracao, IIntegracaoComArquivo<OcorrenciaCTeIntegracaoArquivo>, IEquatable<OcorrenciaCTeIntegracao>
    {
        public OcorrenciaCTeIntegracao()
        {
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaCTeIntegracaoLote", Column = "OCL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OcorrenciaCTeIntegracaoLote Lote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "OCI_PROTOCOLO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenteRetorno", Column = "OCI_PENDENTE_RETORNO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCORRENCIA_CTE_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OCI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OcorrenciaCTeIntegracaoArquivo", Column = "OIA_CODIGO")]
        public virtual ICollection<OcorrenciaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoExternoRetornoIntegracao", Column = "OCI_CODIGO_EXTERNO_RETORNO_INTEGRACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string CodigoExternoRetornoIntegracao { get; set; }

        public virtual bool Equals(OcorrenciaCTeIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get
            {
                return this.CargaCTe?.CTe?.Descricao ?? string.Empty;
            }
        }
    }
}
