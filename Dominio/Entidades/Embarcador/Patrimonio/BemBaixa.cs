using System;

namespace Dominio.Entidades.Embarcador.Patrimonio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BEM_BAIXA", EntityName = "BemBaixa", Name = "Dominio.Entidades.Embarcador.Patrimonio.BemBaixa", NameType = typeof(BemBaixa))]
    public class BemBaixa : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Patrimonio.BemBaixa>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BBA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "BBA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorVenda", Column = "BBA_VALOR_VENDA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "BBA_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusBem), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusBem Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Bem", Column = "BEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Bem Bem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal.NotaFiscal NotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

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
