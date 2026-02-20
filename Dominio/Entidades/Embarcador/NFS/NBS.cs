using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NBS", EntityName = "NBS", Name = "Dominio.Entidades.Embarcador.NFS.NBS", NameType = typeof(NBS))]
    public class NBS : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NBS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "NBS_NUMERO", TypeType = typeof(string), Length = 8, NotNull = true)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "NBS_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }
    }
}
