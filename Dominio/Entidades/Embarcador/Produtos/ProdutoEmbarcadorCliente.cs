using System;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_EMBARCADOR_CLIENTE", EntityName = "ProdutoEmbarcadorCliente", Name = "Dominio.Entidades.Embarcador.Embarcador.ProdutoEmbarcadorCliente", NameType = typeof(ProdutoEmbarcadorCliente))]
    public class ProdutoEmbarcadorCliente : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorCliente>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBarras", Column = "PEC_CODIGO_BARRAS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoBarras { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }

        public virtual bool Equals(ProdutoEmbarcadorCliente other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
