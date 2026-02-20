using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_INTEGRAR_ALTERACAO_INTEGRACAO", EntityName = "TabelaFreteIntegrarAlteracaoIntegracao", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao", NameType = typeof(TabelaFreteIntegrarAlteracaoIntegracao))]
    public class TabelaFreteIntegrarAlteracaoIntegracao : Integracao.Integracao, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>, IEquatable<TabelaFreteIntegrarAlteracaoIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteIntegrarAlteracao", Column = "TIA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteIntegrarAlteracao TabelaFreteIntegrarAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_INTEGRAR_ALTERACAO_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(TabelaFreteIntegrarAlteracaoIntegracao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
