
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Localidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DISTRIBUIDOR_REGIAO", EntityName = "DistribuidorRegiao", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao", NameType = typeof(DistribuidorRegiao))]
    public class DistribuidorRegiao: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Regiao", Column = "REG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidades.Regiao Regiao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_DISTRIBUIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Distribuidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ClienteDistribuidor { get; set; }

        [Obsolete("Migrado para uma lista")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposDeCargas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_DE_CARGA_DISTRIBUIDOR_POR_REGIAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> TiposDeCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DR_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }
    }
}
