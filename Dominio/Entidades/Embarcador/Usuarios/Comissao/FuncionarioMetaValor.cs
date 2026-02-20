using System;

namespace Dominio.Entidades.Embarcador.Usuarios.Comissao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FUNCIONARIO_META_VALOR", EntityName = "FuncionarioMetaValor", Name = "Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaValor", NameType = typeof(FuncionarioMetaValor))]
    public class FuncionarioMetaValor : EntidadeBase, IEquatable<FuncionarioMetaValor>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FMV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mes", Column = "FMV_MES", TypeType = typeof(int), NotNull = false)]
        public virtual int Mes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ano", Column = "FMV_ANO", TypeType = typeof(int), NotNull = false)]
        public virtual int Ano { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "FMV_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Percentual", Column = "FMV_PERCENTUAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FuncionarioMeta", Column = "FME_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FuncionarioMeta FuncionarioMeta { get; set; }

        public virtual string Descricao
        {
            get { return Mes.ToString() + "/" + Ano.ToString(); }
        }

        public virtual bool Equals(FuncionarioMetaValor other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
