using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LICITACAO", EntityName = "Licitacao", Name = "Dominio.Entidades.Embarcador.Frete.Licitacao", NameType = typeof(Licitacao))]
    public class Licitacao : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade, IEquatable<Licitacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "LIC_DATA_FIM", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "LIC_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "LIC_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarTodosTransportadores", Column = "LIC_LIBERAR_TODOS_TRANSPORTADORES", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LiberarTodosTransportadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LIC_NUMERO", TypeType = typeof(int), UniqueKey = "UK_T_LICITACAO_NUMERO", NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "LIC_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frete.TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoLicitacao", Column = "SLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SolicitacaoLicitacao SolicitacaoLicitacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LICITACAO_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LIC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LicitacaoAnexo", Column = "ANX_CODIGO")]
        public virtual IList<LicitacaoAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LICITACAO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LIC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LicitacaoTransportador", Column = "LTR_CODIGO")]
        public virtual IList<LicitacaoTransportador> Transportadores { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TabelasFreteCliente", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LICITACAO_TABELA_FRETE_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LIC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaFreteCliente", Column = "TFC_CODIGO")]
        public virtual ICollection<TabelaFreteCliente> TabelasFreteCliente { get; set; }

        public virtual bool Equals(Licitacao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
