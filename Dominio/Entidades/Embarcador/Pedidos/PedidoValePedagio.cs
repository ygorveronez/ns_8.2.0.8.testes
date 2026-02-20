using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_VALE_PEDAGIO", EntityName = "PedidoValePedagio", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoValePedagio", NameType = typeof(PedidoValePedagio))]
    public class PedidoValePedagio : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoValePedagio>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJResponsavel", Column = "PEV_CNPJ_RESPONSAVEL", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJFornecedor", Column = "PEV_CNPJ_FORNECEDOR", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroComprovante", Column = "PEV_NUMERO_COMPROVANTE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PEV_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        public virtual bool Equals(PedidoValePedagio other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
