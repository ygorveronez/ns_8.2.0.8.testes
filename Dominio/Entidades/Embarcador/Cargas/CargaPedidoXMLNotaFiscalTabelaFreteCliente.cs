namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_XML_NOTA_FISCAL_TABELA_FRETE_CLIENTE", EntityName = "CargaPedidoXMLNotaFiscalTabelaFreteCliente", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente", NameType = typeof(CargaPedidoXMLNotaFiscalTabelaFreteCliente))]
    public class CargaPedidoXMLNotaFiscalTabelaFreteCliente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente TabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "PNX_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFixo", Column = "PNX_VALOR_FIXO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFixo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualSobreNF", Column = "PNX_PERCENTUAL_SOBRE_NF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualSobreNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PNX_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNX_OBSERVACAO_TERCEIRO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ObservacaoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TabelaFreteFilialEmissora", Column = "PNX_TABELA_FRETE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TabelaFreteFilialEmissora { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTabelaFreteCliente)this.MemberwiseClone();
        }

        public virtual bool Equals(CargaPedidoXMLNotaFiscalTabelaFreteCliente other)
        {
            if (other.Codigo == this.Codigo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
