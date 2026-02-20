using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Localidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "cepbr_estado", EntityName = "Estado", Name = "AdminMultisoftware.Dominio.Entidades.Localidades.Estado", NameType = typeof(Estado))]
    public class Estado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "UF", Type = "System.String", Column = "uf", Length = 255)]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "assigned")]
        public virtual string UF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "estado", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIBGE", Column = "cod_ibge", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string CodigoIBGE { get; set; }

    }
}
