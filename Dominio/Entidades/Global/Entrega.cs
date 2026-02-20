using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ENTREGA", EntityName = "Entrega", Name = "Dominio.Entidades.Entrega", NameType = typeof(Entrega))]
    public class Entrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ENT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "ENT_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ENT_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ENT_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTes", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_ENTREGA_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ENT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", Unique = true)]
        public virtual IList<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes { get; set; }
    }
}
