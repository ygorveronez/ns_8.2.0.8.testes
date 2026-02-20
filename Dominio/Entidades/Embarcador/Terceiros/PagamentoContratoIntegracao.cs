using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_CONTRATO_INTEGRACAO", EntityName = "PagamentoContratoIntegracao", Name = "Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao", NameType = typeof(PagamentoContratoIntegracao))]
    public class PagamentoContratoIntegracao : Integracao.Integracao, IEquatable<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>, IIntegracaoComArquivo<Terceiros.PagamentoContratoIntegracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFrete", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Terceiros.ContratoFrete ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AutorizacaoPagamentoContratoFrete", Column = "APC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete AutorizacaoPagamentoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PAGAMENTO_CONTRATO_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PCI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PagamentoContratoIntegracaoArquivo", Column = "PCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(PagamentoContratoIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
