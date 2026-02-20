using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_CANCELAMENTO_INTEGRACAO", EntityName = "OcorrenciaCancelamentoIntegracao", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao", NameType = typeof(OcorrenciaCancelamentoIntegracao))]
    public class OcorrenciaCancelamentoIntegracao : Integracao.Integracao, IIntegracaoComArquivo<OcorrenciaCancelamentoIntegracaoArquivo>, IEquatable<OcorrenciaCancelamentoIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaCancelamento", Column = "CAO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OcorrenciaCancelamento OcorrenciaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCORRENCIA_CANCELAMENTO_INTEGRACAO_ARQUIVO_TRANSACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OcorrenciaCancelamentoIntegracaoArquivo", Column = "OCA_CODIGO")]
        public virtual ICollection<OcorrenciaCancelamentoIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get { return $"{OcorrenciaCancelamento.DescricaoTipo} da Ocorrência {OcorrenciaCancelamento.Ocorrencia.Descricao}"; }
        }

        public virtual bool Equals(OcorrenciaCancelamentoIntegracao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
