using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BOLETO_REMESSA_CANCELAMENTO_TITULO", EntityName = "BoletoRemessaCancelamentoTitulo", Name = "Dominio.Entidades.Embarcador.Financeiro.BoletoRemessaCancelamentoTitulo", NameType = typeof(BoletoRemessaCancelamentoTitulo))]
    public class BoletoRemessaCancelamentoTitulo : EntidadeBase, IEquatable<BoletoRemessaCancelamentoTitulo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BRT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NossoNumeroAnterior", Column = "BRT_NOSSO_NUMERO_ANTERIOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NossoNumeroAnterior { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoRemessa", Column = "BRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BoletoRemessa BoletoRemessa { get; set; }

        public virtual string Descricao
        {
            get { return NossoNumeroAnterior; }
        }

        public virtual bool Equals(BoletoRemessaCancelamentoTitulo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
