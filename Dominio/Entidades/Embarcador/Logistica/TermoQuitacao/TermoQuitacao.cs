using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica.TermoQuitacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TERMO_QUITACAO", EntityName = "TermoQuitacao", Name = "Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao", NameType = typeof(TermoQuitacao))]
    public class TermoQuitacao : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TEQ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TEQ_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBase", Column = "TEQ_DATA_BASE", TypeType = typeof(System.DateTime), NotNull = true)]
        public virtual System.DateTime DataBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "TEQ_DATA_CRIACAO", TypeType = typeof(System.DateTime), NotNull = true)]
        public virtual System.DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TEQ_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoTransportador", Column = "TEQ_OBSERVACAO_TRANSPORTADOR", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TEQ_SITUACAO", TypeType = typeof(SituacaoTermoQuitacao), NotNull = true)]
        public virtual SituacaoTermoQuitacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TERMO_QUITACAO_ANEXOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TEQ_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TermoQuitacaoAnexo", Column = "ANX_CODIGO")]
        public virtual IList<TermoQuitacaoAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AnexosTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TERMO_QUITACAO_ANEXOS_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TEQ_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TermoQuitacaoAnexoTransportador", Column = "ANX_CODIGO")]
        public virtual IList<TermoQuitacaoAnexoTransportador> AnexosTransportador { get; set; }

        public virtual string Descricao
        {
            get { return $"Termo de Quitação n° {Numero}"; }
        }
    }
}
