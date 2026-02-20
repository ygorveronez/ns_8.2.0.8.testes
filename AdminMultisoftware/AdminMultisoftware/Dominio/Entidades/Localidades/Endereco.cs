using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Localidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "cepbr_endereco", EntityName = "Endereco", Name = "AdminMultisoftware.Dominio.Entidades.Localidades.Endereco", NameType = typeof(Endereco))]
    public class Endereco : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "CEP", Type = "System.String", Column = "cep", Length = 255)]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "assigned")]
        public virtual string CEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Logradouro", Column = "logradouro", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Logradouro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLogradouro", Column = "tipo_logradouro", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string TipoLogradouro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Complemento", Column = "complemento", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Complemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Local", Column = "local", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Local { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "id_cidade", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Localidades.Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Bairro", Column = "id_bairro", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Localidades.Bairro Bairro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Geo", Column = "geo", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Localidades.Geo Geo { get; set; }

        public virtual string CEP_Formatado
        {
            get
            {

                return String.Format(@"{0:00.000-000}", this.CEP);
            }
        }

    }
}
