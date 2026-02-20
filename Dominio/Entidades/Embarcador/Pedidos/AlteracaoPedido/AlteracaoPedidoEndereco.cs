namespace Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALTERACAO_PEDIDO_ENDERECO", EntityName = "AlteracaoPedidoEndereco", Name = "Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoEndereco", NameType = typeof(AlteracaoPedidoEndereco))]
    public class AlteracaoPedidoEndereco : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "APE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bairro", Column = "APE_BAIRRO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string Bairro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cep", Column = "APE_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Cep { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Logradouro", Column = "APE_LOGRADOURO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Logradouro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "APE_NUMERO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Numero { get; set; }
    }
}
