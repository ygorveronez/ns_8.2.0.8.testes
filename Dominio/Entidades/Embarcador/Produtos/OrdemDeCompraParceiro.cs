namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_DE_COMPRA_PARCEIRO", DynamicUpdate = true, EntityName = "OrdemDeCompraParceiro", Name = "Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraParceiro", NameType = typeof(OrdemDeCompraParceiro))]
    public class OrdemDeCompraParceiro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FuncaoParceiro", Column = "OPA_FUNCAO_PARCEIRO", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string FuncaoParceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemDeCompraDocumento", Column = "OCP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemDeCompraDocumento OrdemDeCompraDocumento { get; set; }
    }
}
