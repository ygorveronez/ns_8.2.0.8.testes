using System;

namespace Dominio.Entidades.Embarcador.Patrimonio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BEM_DEPRECIACAO", EntityName = "BemDepreciacao", Name = "Dominio.Entidades.Embarcador.Patrimonio.BemDepreciacao", NameType = typeof(BemDepreciacao))]
    public class BemDepreciacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Patrimonio.BemBaixa>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "BDE_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "BDE_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Percentual", Column = "BDE_PERCENTUAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mes", Column = "BDE_MES", TypeType = typeof(int), NotNull = false)]
        public virtual int Mes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ano", Column = "BDE_ANO", TypeType = typeof(int), NotNull = false)]
        public virtual int Ano { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Bem", Column = "BEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Bem Bem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string Descricao
        {
            get { return Bem.Descricao; }
        }

        public virtual bool Equals(BemBaixa other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
