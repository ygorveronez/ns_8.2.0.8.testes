using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;
namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_CTE_CANCELAMENTO_INTEGRACAO", EntityName = "OcorrenciaCTeCancelamentoIntegracao", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao", NameType = typeof(OcorrenciaCTeCancelamentoIntegracao))]
    public class OcorrenciaCTeCancelamentoIntegracao : Integracao.Integracao, IIntegracaoComArquivo<OcorrenciaCTeCancelamentoIntegracaoArquivo>, IEquatable<OcorrenciaCTeCancelamentoIntegracao>
    {
        public OcorrenciaCTeCancelamentoIntegracao()
        {
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaCTeIntegracao", Column = "OCI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OcorrenciaCTeIntegracao OcorrenciaCTeIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaCancelamento", Column = "CAO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OcorrenciaCancelamento OcorrenciaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCORRENCIA_CTE_CANCELAMENTO_INTEGRACAO_ARQUIVO_TRANSACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OCC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OcorrenciaCTeCancelamentoIntegracaoArquivo", Column = "ACC_CODIGO")]
        public virtual ICollection<OcorrenciaCTeCancelamentoIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(OcorrenciaCTeCancelamentoIntegracao other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual string Descricao
        {
            get
            {
                return this.OcorrenciaCTeIntegracao.Descricao ?? string.Empty;
            }
        }
    }
}
