using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Localidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAIS", EntityName = "Pais", Name = "AdminMultisoftware.Dominio.Entidades.Localidades.Pais", NameType = typeof(Pais))]
    public class Pais : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PAI_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPais", Column = "PAI_CODIGO_PAIS", TypeType = typeof(int),  NotNull = true)]
        public virtual int CodigoPais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sigla", Column = "PAI_SIGLA", TypeType = typeof(string), Length = 2, NotNull = true)]
        public virtual string Sigla { get; set; }

    }
}
