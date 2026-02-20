using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOVIMENTO_FINANCEIRO_ENTIDADE", EntityName = "MovimentoFinanceiroEntidade", Name = "Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade", NameType = typeof(MovimentoFinanceiroEntidade))]
    public class MovimentoFinanceiroEntidade : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimentoEntidade", Column = "MOE_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade TipoMovimentoEntidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MovimentoFinanceiro", Column = "MOV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro MovimentoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Produto Produto { get; set; }

        public virtual bool Equals(MovimentoFinanceiroEntidade other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
