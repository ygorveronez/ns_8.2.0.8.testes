using System;

namespace Dominio.Entidades.Embarcador.CotacaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COTACAO_PEDIDO_ENDERECO", EntityName = "CotacaoPedidoEndereco", Name = "Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco", NameType = typeof(CotacaoPedidoEndereco))]
    public class CotacaoPedidoEndereco : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IE_RG", Column = "CPE_IERG", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string IE_RG { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Endereco", Column = "CPE_ENDERECO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Endereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CPE_NUMERO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Complemento", Column = "CPE_COMPLEMENTO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Complemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEP", Column = "CPE_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bairro", Column = "CPE_BAIRRO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string Bairro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone", Column = "CPE_FONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteOutroEndereco", Column = "COE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco ClienteOutroEndereco { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Endereco + " - " + Numero + ", " + Complemento;
            }
        }

        public virtual Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco Clonar()
        {
            return (Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco)this.MemberwiseClone();
        }

        public virtual bool Equals(CotacaoPedidoEndereco other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
