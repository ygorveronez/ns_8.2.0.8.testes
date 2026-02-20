using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO_BAIXA_CHEQUE", EntityName = "TituloBaixaCheque", Name = "Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque", NameType = typeof(TituloBaixaCheque))]
    public class TituloBaixaCheque : EntidadeBase, IEquatable<TituloBaixaCheque>, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TBC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cheque", Column = "CHQ_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cheque Cheque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloBaixa", Column = "TIB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TituloBaixa TituloBaixa { get; set; }

        public virtual bool Equals(TituloBaixaCheque other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
