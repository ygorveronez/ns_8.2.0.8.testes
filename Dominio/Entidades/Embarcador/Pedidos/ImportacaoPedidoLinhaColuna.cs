namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_PEDIDO_LINHA_COLUNA", EntityName = "ImportacaoPedidoLinhaColuna", Name = "Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna", NameType = typeof(ImportacaoPedidoLinhaColuna))]
    public class ImportacaoPedidoLinhaColuna : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoPedidoLinha", Column = "IML_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha Linha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IMC_NOME_CAMPO", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string NomeCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IMC_VALOR", Type = "StringClob", NotNull = true)]
        public virtual string Valor { get; set; }
    }
}
