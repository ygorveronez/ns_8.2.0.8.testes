using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Localidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "cepbr_geo", EntityName = "Geo", Name = "AdminMultisoftware.Dominio.Entidades.Localidades.Estado", NameType = typeof(Geo))]
    public class Geo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "cep", Type = "System.Int32", Column = "cep")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int cep { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "latitude", Column = "latitude", TypeType = typeof(decimal), Scale = 10, Precision = 20, NotNull = false)]
        public virtual decimal latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "longitude", Column = "longitude", TypeType = typeof(decimal), Scale = 10, Precision = 20, NotNull = false)]
        public virtual decimal longitude { get; set; }

    }
}
