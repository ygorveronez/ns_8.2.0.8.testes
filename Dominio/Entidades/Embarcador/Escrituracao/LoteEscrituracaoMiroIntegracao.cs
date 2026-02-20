using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [Obsolete("Classe nao deve ser usada. Informações da MIRO estao em DocumentoFaturamento.cs")]
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_ESCRITURACAO_MIRO_INTEGRACAO", EntityName = "LoteEscrituacaoMiroIntegracao", Name = "Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituacaoMiroIntegracao", NameType = typeof(LoteEscrituacaoMiroIntegracao))]
    public class LoteEscrituacaoMiroIntegracao : Integracao.Integracao, IEquatable<CancelamentoProvisaoIntegracao>,IIntegracaoComArquivo<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LEI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteEscrituracaoMiroDocumento", Column = "LEM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LoteEscrituracaoMiroDocumento LoteEscrituracaoMiroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LOTE_ESCRITURACAO_MIRO_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(CancelamentoProvisaoIntegracao other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
