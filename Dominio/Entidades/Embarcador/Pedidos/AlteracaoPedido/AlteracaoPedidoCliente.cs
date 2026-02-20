namespace Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALTERACAO_PEDIDO_CLIENTE", EntityName = "AlteracaoPedidoCliente", Name = "Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoCliente", NameType = typeof(AlteracaoPedidoCliente))]
    public class AlteracaoPedidoCliente : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "APC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClienteExterior", Column = "APC_CLIENTE_EXTERIOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ClienteExterior { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade AtividadeClienteExterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "APC_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CpfCnpj", Column = "APC_CPF_CNPJ", TypeType = typeof(double), NotNull = true)]
        public virtual double CpfCnpj { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IeRg", Column = "APC_IE_RG", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string IeRg { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AlteracaoPedidoEndereco", Column = "APE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AlteracaoPedidoEndereco Endereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "APC_NOME", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Nome { get; set; }
    }
}
