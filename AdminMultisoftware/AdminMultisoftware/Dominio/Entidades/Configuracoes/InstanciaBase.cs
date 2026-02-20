using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INSTANCIA_BASE", EntityName = "InstanciaBase", Name = "AdminMultisoftware.Dominio.Entidades.Configuracoes.InstanciaBase", NameType = typeof(InstanciaBase))]
    public class InstanciaBase : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Servidor", Column = "INB_SERVIDOR", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Servidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "INB_USUARIO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "INB_SENHA", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "INB_PORTA", TypeType = typeof(int), NotNull = true)]
        public virtual int Porta { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Servidor;
            }
        }
    }
}