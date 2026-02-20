using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Usuarios.Comissao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FUNCIONARIO_META", EntityName = "FuncionarioMeta", Name = "Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta", NameType = typeof(FuncionarioMeta))]
    public class FuncionarioMeta : EntidadeBase, IEquatable<FuncionarioMeta>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FME_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVigencia", Column = "FME_DATA_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataVigencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FUNCIONARIO_META_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FME_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FuncionarioMetaValor", Column = "FMV_CODIGO")]
        public virtual IList<FuncionarioMetaValor> Metas { get; set; }

        public virtual string Descricao
        {
            get { return DataVigencia.ToString("dd/MM/yyyy"); }
        }

        public virtual bool Equals(FuncionarioMeta other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
