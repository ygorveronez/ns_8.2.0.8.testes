using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Mobile
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_USUARIO_AVALIACAO", EntityName = "UsuarioAvaliacao", Name = "AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioAvaliacao", NameType = typeof(UsuarioAvaliacao))]
    public class UsuarioAvaliacao : EntidadeBase, IEquatable<AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioAvaliacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "UAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UsuarioMobile", Column = "UMB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Mobile.UsuarioMobile UsuarioMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoAPP", Column = "UAV_VERSAO_APP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string VersaoAPP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAvaliacao", Column = "UAV_DATA_AVALIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataAvaliacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotaExperienciaUsoAplicativo", Column = "UAV_NOTA_AVALIACAO_EXPERIENCIA_USO_APLICATIVO", TypeType = typeof(int), NotNull = true)]
        public virtual int NotaExperienciaUsoAplicativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoExperienciaUsoAplicativo", Column = "UAV_OBSERVACAO_EXPERIENCIA_USO_APLICATIVO", TypeType = typeof(string), Length = 350, NotNull = false)]
        public virtual string ObservacaoExperienciaUsoAplicativo { get; set; }

        public virtual bool Equals(UsuarioAvaliacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
