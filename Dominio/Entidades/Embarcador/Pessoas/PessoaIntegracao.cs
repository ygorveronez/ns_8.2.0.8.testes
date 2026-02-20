using Dominio.Interfaces.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_INTEGRACAO", EntityName = "PessoaIntegracao", Name = "Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao", NameType = typeof(PessoaIntegracao))]
    public class PessoaIntegracao : Integracao.Integracao, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>, IEquatable<PessoaIntegracao>
    {
        public PessoaIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusIntegracao", Column = "INT_STATUS_INTEGRACAO", TypeType = typeof(StatusIntegracaoSIC), NotNull = true)]
        public virtual StatusIntegracaoSIC StatusIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "INT_PROTOCOLO", Type = "StringClob", NotNull = true)]
        public virtual string Protocolo { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(PessoaIntegracao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
