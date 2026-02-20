using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DICA", EntityName = "Dica", Name = "Dominio.Entidades.Dica", NameType = typeof(Dica))]
    public class Dica : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAjuda", Column = "DIC_CODIGO_AJUDA", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoAjuda { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "DIC_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Titulo", Column = "DIC_TITULO", TypeType = typeof(string), NotNull = false)]
        public virtual string Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "DIC_DESCRICAO", Type = "StringClob", NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LinkVideo", Column = "DIC_LINK_VIDEO", TypeType = typeof(string), NotNull = false)]
        public virtual string LinkVideo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DICA_ANEXOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DIC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DicaAnexo", Column = "ANX_CODIGO")]
        public virtual IList<DicaAnexo> Anexos { get; set; }

    }
}
