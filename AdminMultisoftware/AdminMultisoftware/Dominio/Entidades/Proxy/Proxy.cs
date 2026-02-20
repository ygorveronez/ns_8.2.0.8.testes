using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Proxy
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROXY", EntityName = "Proxy", Name = "AdminMultisoftware.Dominio.Entidades.Proxy.Proxy", NameType = typeof(Proxy))]
    public class Proxy : EntidadeBase, IEquatable<AdminMultisoftware.Dominio.Entidades.Proxy.Proxy>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IP", Column = "PRO_IP", TypeType = typeof(string), Length = 15, NotNull = true)]
        public virtual string IP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveEmConsulta", Column = "PRO_CHAVE_EM_CONSULTA", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string ChaveEmConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "PRO_PORTA", TypeType = typeof(int), NotNull = true)]
        public virtual int Porta { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaRequisicaoValida", Column = "PRO_DATA_ULTIMA_REQUISICAO_VALIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataUltimaRequisicaoValida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Databloqueio", Column = "PRO_DATA_BLOQUEIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Databloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Uso", Column = "PRO_USO", TypeType = typeof(int), NotNull = true)]
        public virtual int Uso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmBloqueio", Column = "PRO_EMBLOQUEIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EmBloqueio { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Pais", Column = "PRO_PAIS", TypeType = typeof(string), Length = 30, NotNull = true)]
        public virtual string Pais { get; set; }
        
        public virtual bool Equals(Proxy other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
