using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Usuarios.Colaborador
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLABORADOR_SITUACAO_LANCAMENTO_INTEGRACAO", EntityName = "ColaboradorSituacaoLancamentoIntegracao", Name = "Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao", NameType = typeof(ColaboradorSituacaoLancamentoIntegracao))]
    public class ColaboradorSituacaoLancamentoIntegracao : Integracao.Integracao, IIntegracaoComArquivo<ColaboradorSituacaoLancamentoIntegracaoArquivo>, IEquatable<ColaboradorSituacaoLancamentoIntegracao>
    {
        public ColaboradorSituacaoLancamentoIntegracao() { }
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ColaboradorLancamento", Column = "CLS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento ColaboradorLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "CLI_PROTOCOLO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_MENSAGEM", TypeType = typeof(string), Length = 10000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoTipo", Column = "CLI_DESCRICAO_TIPO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DescricaoTipo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COLABORADOR_SITUACAO_LANCAMENTO_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CLA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(ColaboradorSituacaoLancamentoIntegracao other)
        {
            return (other.Codigo == this.Codigo);
            
        }
    }
}
