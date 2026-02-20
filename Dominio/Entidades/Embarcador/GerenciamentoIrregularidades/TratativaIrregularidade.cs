using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GerenciamentoIrregularidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IRREGULARIDADE_TRATATIVA", EntityName = "TratativaIrregularidade", Name = "Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade", NameType = typeof(TratativaIrregularidade))]
    public class TratativaIrregularidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IRT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DefinicaoTratativasIrregularidade", Column = "IDT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade DefinicaoTratativasIrregularidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IRT_SEQUENCIA", TypeType = typeof(int), NotNull = true)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Setor Setor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IRT_PROXIMA_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int ProximaSequencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoTipoOperacao", Column = "GTO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao GrupoTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IRT_INFORMAR_MOTIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarMotivo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Motivos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_IRREGULARIDADE_TRATATIVA_MOTIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IRT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MotivoIrregularidade", Column = "MTI_CODIGO")]
        public virtual ICollection<MotivoIrregularidade> Motivos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Acoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_IRREGULARIDADE_TRATATIVA_ACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IRT_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "IRT_ACAO", TypeType = typeof(AcaoTratativaIrregularidade), NotNull = true)]
        public virtual ICollection <AcaoTratativaIrregularidade> Acoes { get; set; }

        #region Métodos privados
        public virtual string Descricao
        {
            get { return "Tratativa da Irregularidade: " + this.DefinicaoTratativasIrregularidade.Irregularidade.Descricao; }
        }
        #endregion

    }
}
