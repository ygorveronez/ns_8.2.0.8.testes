using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_ELETRONICO_TITULO", EntityName = "PagamentoEletronicoTitulo", Name = "Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo", NameType = typeof(PagamentoEletronicoTitulo))]
    public class PagamentoEletronicoTitulo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PET_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoEletronico", Column = "PAE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoEletronico PagamentoEletronico { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual bool Equals(PagamentoEletronicoTitulo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
