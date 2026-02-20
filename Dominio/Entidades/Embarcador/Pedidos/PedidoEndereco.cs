using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_ENDERECO", EntityName = "PedidoEndereco", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco", NameType = typeof(PedidoEndereco))]
    public class PedidoEndereco : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IE_RG", Column = "PEN_IERG", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string IE_RG { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Endereco", Column = "PEN_ENDERECO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Endereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PEN_NUMERO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Complemento", Column = "PEN_COMPLEMENTO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Complemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEP", Column = "PEN_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bairro", Column = "PEN_BAIRRO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string Bairro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone", Column = "PEN_FONE", TypeType = typeof(string), Length = 20, NotNull = false)]
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

        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco Clonar()
        {
            return (Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco)this.MemberwiseClone();
        }

        public virtual bool Equals(PedidoEndereco other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
