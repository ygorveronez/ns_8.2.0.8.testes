using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GUARITA_CHECK_LIST_PERGUNTA", EntityName = "GuaritaCheckListPerguntas", Name = "Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas", NameType = typeof(GuaritaCheckListPerguntas))]
    public class GuaritaCheckListPerguntas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GuaritaCheckList", Column = "GCL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GuaritaCheckList GuaritaCheckList { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GPE_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GPE_CATEGORIA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList Categoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GPE_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Alternativas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GUARITA_CHECK_LIST_PERGUNTA_ALTERNATIVA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GuaritaCheckListPerguntasAlternativa", Column = "GAL_CODIGO")]
        public virtual IList<GuaritaCheckListPerguntasAlternativa> Alternativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GPE_REPSOSTA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Resposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GPE_OPCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Opcao { get; set; }        
    }
}