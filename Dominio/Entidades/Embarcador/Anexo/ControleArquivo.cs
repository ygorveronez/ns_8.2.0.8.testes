using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Anexo
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_ARQUIVO", EntityName = "ControleArquivo", Name = "Dominio.Entidades.Embarcador.Anexo.ControleArquivo", NameType = typeof(ControleArquivo))]
    public class ControleArquivo : EntidadeBase, IEquatable<ControleArquivo>, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "COA_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "COA_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "COA_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_GEROU_ALERTA_EMPRESA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouAlertaEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_GEROU_ALERTA_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouAlertaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTROLE_ARQUIVO_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ControleArquivoAnexo", Column = "ANX_CODIGO")]
        public virtual IList<ControleArquivoAnexo> Anexos { get; set; }

        public virtual bool Equals(ControleArquivo other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
