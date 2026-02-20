using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SETOR_FILIAL", EntityName = "SetorFilial", Name = "Dominio.Entidades.Embarcador.Filiais.SetorFilial", NameType = typeof(SetorFilial))]
    public class SetorFilial : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SEF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Setor Setor { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Turnos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SETOR_FILIAL_TURNO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "SEF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Turno", Column = "TUR_CODIGO")]
        public virtual ICollection<Turno> Turnos { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }

        public virtual bool Equals(SetorFilial other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
