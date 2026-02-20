using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_CONTATO", EntityName = "PedidoContato", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoContato", NameType = typeof(PedidoContato))]
    public class PedidoContato : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PDC_CONTATO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Contato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF", Column = "PDC_CPF", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string CPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PDC_EMAIL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PDC_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PDC_TELEFONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposContato", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_CONTATO_TIPO_CONTATO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PDC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoContato", Column = "TCO_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Contatos.TipoContato> TiposContato { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        public virtual string DescricaoTipoContato
        {
            get
            {
                if (TiposContato != null)
                    return string.Join(", ", TiposContato.Select(o => o.Descricao));
                else
                    return string.Empty;
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual string Descricao
        {
            get
            {
                return Contato;
            }
        }
    }
}
