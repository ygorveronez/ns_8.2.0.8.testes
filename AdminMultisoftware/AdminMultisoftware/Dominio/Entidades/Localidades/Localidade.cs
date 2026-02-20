using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Localidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "cepbr_cidade", EntityName = "Localidade", Name = "AdminMultisoftware.Dominio.Entidades.Localidades.Localidade", NameType = typeof(Localidade))]
    public class Localidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "id_cidade")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "cidade", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "uf", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidades.Estado Estado  { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIBGE", Column = "cod_ibge", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string CodigoIBGE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Area", Column = "area", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Area { get; set; }

        public virtual string DescricaoCidadeEstado
        {
            get
            {
                return this.Descricao.ToUpper() + " - " + this.Estado.UF;
            }
        }
    }
}
