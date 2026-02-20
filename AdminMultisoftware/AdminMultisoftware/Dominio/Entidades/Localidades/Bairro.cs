using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Localidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "cepbr_bairro", EntityName = "Bairro", Name = "AdminMultisoftware.Dominio.Entidades.Localidades.Bairro", NameType = typeof(Bairro))]
    public class Bairro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "id_bairro")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "bairro", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "id_cidade", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Localidades.Localidade Localidade { get; set; }
    }
}
