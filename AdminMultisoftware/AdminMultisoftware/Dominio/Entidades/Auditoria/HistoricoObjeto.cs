using System;
using System.Collections.Generic;

namespace AdminMultisoftware.Dominio.Entidades.Auditoria
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_OBJETO", EntityName = "HistoricoObjeto", Name = "AdminMultisoftware.Dominio.Entidades.Auditoria.HistoricoObjeto", NameType = typeof(HistoricoObjeto))]
    public class HistoricoObjeto : EntidadeBase
    {

        public HistoricoObjeto() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "HIO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Objeto", Column = "HIO_OBJETO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Objeto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "HIO_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoObjeto", Column = "HIO_CODIGO_OBJETO", TypeType = typeof(long), NotNull = true)]
        public virtual long CodigoObjeto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Acao", Column = "HIO_ACAO", TypeType = typeof(Enumeradores.AcaoBancoDados), NotNull = true)]
        public virtual Enumeradores.AcaoBancoDados Acao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAuditado", Column = "HIO_TIPO_AUDITADO", TypeType = typeof(Enumeradores.TipoAuditadoAdmin), NotNull = true)]
        public virtual Enumeradores.TipoAuditadoAdmin TipoAuditado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemAuditado", Column = "HIO_ORIGEM_AUDITADO", TypeType = typeof(Enumeradores.OrigemAuditadoAdmin), NotNull = true)]
        public virtual Enumeradores.OrigemAuditadoAdmin OrigemAuditado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoAcao", Column = "HIO_DESCRICAO_ACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string DescricaoAcao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "HIO_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "USU_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioMultisoftware", Column = "HIO_USER_MULTISOFTWARE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioMultisoftware { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IP", Column = "HIO_IP", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IP { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Propriedades", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_HISTORICO_OBJETO_PROPRIEDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "HIO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "HistoricoPropriedade", Column = "HIP_CODIGO")]
        public virtual ICollection<HistoricoPropriedade> Propriedades { get; set; }

        public virtual string Auditado
        {
            get
            {
                if (TipoAuditado == Enumeradores.TipoAuditadoAdmin.Usuario)
                    return Usuario?.Nome ?? "";
                if (TipoAuditado == Enumeradores.TipoAuditadoAdmin.Sistema)
                    return "Sistema";
                else
                    return "";
            }
        }
    }
}
