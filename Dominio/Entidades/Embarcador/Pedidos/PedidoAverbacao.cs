using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_AVERBACAO", EntityName = "PedidoAverbacao", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoAverbacao", NameType = typeof(PedidoAverbacao))]
    public class PedidoAverbacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoAverbacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJResponsavel", Column = "PEA_CNPJ_RESPONSAVEL", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJSeguradora", Column = "PEA_CNPJ_SEGURADORA", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeSeguradora", Column = "PEA_NOME_SEGURADORA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NomeSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroApolice", Column = "PEA_NUMERO_APOLICE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroApolice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAverbacao", Column = "PEA_NUMERO_AVERBACAO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string NumeroAverbacao { get; set; }

        public virtual bool Equals(PedidoAverbacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
