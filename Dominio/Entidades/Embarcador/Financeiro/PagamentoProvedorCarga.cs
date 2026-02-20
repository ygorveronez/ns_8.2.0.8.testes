using Dominio.Interfaces.Embarcador.Entidade;
using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_PROVEDOR_CARGA", EntityName = "PagamentoProvedorCarga", Name = "Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga", NameType = typeof(PagamentoProvedorCarga))]
    public class PagamentoProvedorCarga : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoProvedor", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor PagamentoProvedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRateado", Column = "PRC_VALOR_RATEADO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorRateado { get; set; }

        public virtual bool Equals(PagamentoProvedorCarga other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao { get { return this.Codigo.ToString(); } }

    }
}
