using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_CLIENTE_INTEGRACAO", EntityName = "TabelaFreteClienteIntegracao", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao", NameType = typeof(TabelaFreteClienteIntegracao))]
    public class TabelaFreteClienteIntegracao : Integracao.Integracao, IIntegracaoComArquivo<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo>, IEquatable<TabelaFreteClienteIntegracao>
    {
        public TabelaFreteClienteIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente TabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TCI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaFreteClienteArquivo", Column = "TCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(TabelaFreteClienteIntegracao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}