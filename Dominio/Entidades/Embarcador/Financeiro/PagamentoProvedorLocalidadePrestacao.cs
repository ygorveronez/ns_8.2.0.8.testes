using Dominio.Interfaces.Embarcador.Entidade;
using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_PROVEDOR_LOCALIDADE_PRESTACAO", EntityName = "PagamentoProvedorLocalidadePrestacao", Name = "Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorLocalidadePrestacao", NameType = typeof(PagamentoProvedorLocalidadePrestacao))]
    public class PagamentoProvedorLocalidadePrestacao : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade LocalidadePrestacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoProvedor", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor PagamentoProvedor { get; set; }

        public virtual bool Equals(PagamentoProvedorLocalidadePrestacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao { get { return this.Codigo.ToString(); } }
    }
}
