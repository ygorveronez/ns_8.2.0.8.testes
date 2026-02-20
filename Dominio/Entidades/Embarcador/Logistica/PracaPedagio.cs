using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRACA_PEDAGIO", EntityName = "PracaPedagio", Name = "Dominio.Entidades.Embarcador.Logistica.PracaPedagio", NameType = typeof(PracaPedagio))]
    public class PracaPedagio : EntidadeBase, IEquatable<PracaPedagio>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PRP_DESCRICAO", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rodovia", Column = "PRP_RODOVIA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Rodovia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "PRP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Concessionaria", Column = "PRP_CONCESSIONARIA", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Concessionaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KM", Column = "PRP_KM", TypeType = typeof(decimal), NotNull = true, Scale = 2, Precision = 18)]
        public virtual decimal KM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PRP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PRP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "CLI_LATIDUDE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "CLI_LONGITUDE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PracaPedagioTarifa", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRACA_PEDAGIO_TARIFA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PracaPedagioTarifa", Column = "PPT_CODIGO")]
        public virtual ICollection<PracaPedagioTarifa> PracaPedagioTarifa { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(PracaPedagio other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
