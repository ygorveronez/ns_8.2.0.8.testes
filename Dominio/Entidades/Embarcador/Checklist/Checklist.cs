using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Checklist
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECKLIST", EntityName = "Checklist", Name = "Dominio.Entidades.Embarcador.Checklist.Checklist", NameType = typeof(Checklist))]
    public class Checklist : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CKL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CKL_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CKL_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CKL_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CKL_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Perguntas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHECKLIST_PERGUNTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CKL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ChecklistPergunta", Column = "CLP_CODIGO")]
        public virtual ICollection<ChecklistPergunta> Perguntas { get; set; }

    }
}
