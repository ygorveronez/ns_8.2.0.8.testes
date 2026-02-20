using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RECADO", EntityName = "Recado", Name = "Dominio.Entidades.Recado", NameType = typeof(Recado))]
    public class Recado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "REC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Titulo", Column = "REC_TITULO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "REC_DESCRICAO", TypeType = typeof(string), Length = 10000, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLancamento", Column = "REC_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "REC_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RecadoUsuarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_RECADO_USUARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "REC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RecadoUsuarios", Column = "REU_CODIGO")]
        public virtual IList<Dominio.Entidades.RecadoUsuarios> RecadoUsuarios { get; set; }

    }
}
