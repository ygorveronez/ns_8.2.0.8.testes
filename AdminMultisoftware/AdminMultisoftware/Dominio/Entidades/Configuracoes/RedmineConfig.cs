using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REDMINE_CONFIG", EntityName = "RedmineConfig", Name = "AdminMultisoftware.Dominio.Entidades.Configuracoes.RedmineConfig", NameType = typeof(RedmineConfig))]
    public class RedmineConfig : EntidadeBase, IEquatable<AdminMultisoftware.Dominio.Entidades.Configuracoes.RedmineConfig>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAPI", Column = "RDC_URL_API", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string URLAPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "APIkey", Column = "RDC_API_KEY", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string APIkey { get; set; }

        public virtual bool Equals(RedmineConfig other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
