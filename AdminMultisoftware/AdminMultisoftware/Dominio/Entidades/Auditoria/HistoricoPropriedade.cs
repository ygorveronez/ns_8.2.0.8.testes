using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Auditoria
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_PROPRIEDADE", EntityName = "HistoricoPropriedade", Name = "AdminMultisoftware.Dominio.Entidades.Auditoria.HistoricoPropriedade", NameType = typeof(HistoricoPropriedade))]
    public class HistoricoPropriedade : EntidadeBase
    {
        protected HistoricoPropriedade() { }

        public HistoricoPropriedade(string propriedade, string de, string para)
        {
            this.Propriedade = propriedade;
            this.De = de;
            this.Para = para;
        }

        public HistoricoPropriedade(string propriedade, string de, string para, HistoricoObjeto historicoPai)
        {
            this.Propriedade = propriedade;
            this.De = de;
            this.Para = para;
            this.HistoricoObjetoPai = historicoPai;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "HIP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Propriedade", Column = "HIP_PROPRIEDADE", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Propriedade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "De", Column = "HIP_DE", Type = "StringClob", NotNull = false)]
        public virtual string De { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Para", Column = "HIP_PARA", Type = "StringClob", NotNull = false)]
        public virtual string Para { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "HistoricoObjeto", Column = "HIO_CODIGO_PAI", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual HistoricoObjeto HistoricoObjetoPai { get; set; }
    }
}
