using NHibernate.Mapping.Attributes;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Auditoria
{
    [Class(0, Table = "T_HISTORICO_OBJETO", EntityName = "HistoricoObjeto", Name = "Dominio.Entidades.Auditoria.HistoricoObjeto", NameType = typeof(HistoricoObjeto))]
    public class HistoricoObjeto : EntidadeBase
    {

        public HistoricoObjeto() { }

        [Id(0, Name = "Codigo", Type = "System.Int64", Column = "HIO_CODIGO")]
        [Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [Property(0, Name = "Objeto", Column = "HIO_OBJETO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Objeto { get; set; }

        [Property(0, Name = "Descricao", Column = "HIO_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Descricao { get; set; }

        [Property(0, Name = "CodigoObjeto", Column = "HIO_CODIGO_OBJETO", TypeType = typeof(long), NotNull = true)]
        public virtual long CodigoObjeto { get; set; }

        [Property(0, Name = "Acao", Column = "HIO_ACAO", TypeType = typeof(ObjetosDeValor.Enumerador.AcaoBancoDados), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.AcaoBancoDados Acao { get; set; }

        [Property(0, Name = "TipoAuditado", Column = "HIO_TIPO_AUDITADO", TypeType = typeof(ObjetosDeValor.Enumerador.TipoAuditado), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.TipoAuditado TipoAuditado { get; set; }

        [Property(0, Name = "OrigemAuditado", Column = "HIO_ORIGEM_AUDITADO", TypeType = typeof(ObjetosDeValor.Enumerador.TipoAuditado), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.OrigemAuditado OrigemAuditado { get; set; }

        [Property(0, Name = "DescricaoAcao", Column = "HIO_DESCRICAO_ACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string DescricaoAcao { get; set; }

        [Property(0, Name = "Data", Column = "HIO_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [Property(0, Name = "UsuarioMultisoftware", Column = "HIO_USER_MULTISOFTWARE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UsuarioMultisoftware { get; set; }

        [ManyToOne(0, Class = "Integradora", Column = "INT_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual Dominio.Entidades.WebService.Integradora Integradora { get; set; }

        [ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [Property(0, Name = "IP", Column = "HIO_IP", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IP { get; set; }

        [Set(0, Name = "Propriedades", Cascade = "all", Lazy = CollectionLazy.True, Table = "T_HISTORICO_OBJETO_PROPRIEDADE")]
        [Key(1, Column = "HIO_CODIGO")]
        [ManyToMany(2, Class = "HistoricoPropriedade", Column = "HIP_CODIGO")]
        public virtual ICollection<HistoricoPropriedade> Propriedades { get; set; }

        public virtual string Auditado
        {
            get
            {
                if (TipoAuditado == ObjetosDeValor.Enumerador.TipoAuditado.Usuario)
                    return Usuario?.Nome ?? "";
                if (TipoAuditado == ObjetosDeValor.Enumerador.TipoAuditado.Integradoras)
                    return Integradora?.Descricao + " (Via Integração -> " + IP + ")" ?? string.Empty;
                if (TipoAuditado == ObjetosDeValor.Enumerador.TipoAuditado.Sistema)
                    return "Sistema";
                else
                    return "";
            }
        }
    }
}
