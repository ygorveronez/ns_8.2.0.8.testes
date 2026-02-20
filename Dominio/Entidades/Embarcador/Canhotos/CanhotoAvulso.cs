using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANHOTO_AVULSO", EntityName = "CanhotoAvulso", Name = "Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso", NameType = typeof(CanhotoAvulso))]

    public class CanhotoAvulso : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CAV_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CAV_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QRCode", Column = "CAV_QR_CODE", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string QRCode { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PedidosXMLNotasFiscais", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CANHOTO_AVULSO_PEDIDO_XML_NOTA_FISCAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> PedidosXMLNotasFiscais { get; set; }

        public virtual bool Equals(CanhotoAvulso other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
