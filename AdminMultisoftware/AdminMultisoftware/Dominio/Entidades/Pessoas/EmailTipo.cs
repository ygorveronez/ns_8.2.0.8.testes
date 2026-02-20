using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMAIL_TIPO", EntityName = "EmailTipo", Name = "AdminMultisoftware.Dominio.Entidades.Pessoas.EmailTipo", NameType = typeof(EmailTipo))]
    public class EmailTipo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ETP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ETP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }
    }
}
