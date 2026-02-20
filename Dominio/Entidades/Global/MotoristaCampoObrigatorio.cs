using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTORISTA_CAMPO_OBRIGATORIO", EntityName = "MotoristaCampoObrigatorio ", Name = "Dominio.Entidades.MotoristaCampoObrigatorio ", NameType = typeof(MotoristaCampoObrigatorio))]
    public class MotoristaCampoObrigatorio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Campos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MOTORISTA_CAMPO_OBRIGATORIO_CAMPO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MotoristaCampo", Column = "MCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.MotoristaCampo> Campos { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString() ?? string.Empty;
            }
        }

    }
}
