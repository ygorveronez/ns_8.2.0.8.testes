using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_EMPRESA_RESPONSAVEL", EntityName = "PedidoEmpresaResponsavel", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel", NameType = typeof(PedidoEmpresaResponsavel))]
    public class PedidoEmpresaResponsavel : EntidadeBase, IEquatable<PedidoEmpresaResponsavel>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PER_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PER_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "PER_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        public virtual bool Equals(PedidoEmpresaResponsavel other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}