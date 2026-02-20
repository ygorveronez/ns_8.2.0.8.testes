namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_IMPORTACAO_PLANILHA_LINHA_COLUNA", EntityName = "PedidoImportacaoPlanilhaLinhaColuna", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoImportacaoPlanilhaLinhaColuna", NameType = typeof(PedidoImportacaoPlanilhaLinhaColuna))]
    public class PedidoImportacaoPlanilhaLinhaColuna : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PLC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoImportacaoPlanilhaLinha Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PLC_CAMPO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Campo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PLC_VALOR", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Valor { get; set; }
    }
}
