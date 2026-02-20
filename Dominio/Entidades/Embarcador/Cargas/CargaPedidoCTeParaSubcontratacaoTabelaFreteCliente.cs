namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_CTE_PARA_SUBCONTRATACAO_TABELA_FRETE_CLIENTE", EntityName = "CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente", NameType = typeof(CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente))]
    public class CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PSX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoCTeParaSubContratacao", Column = "PSC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao PedidoCTeParaSubContratacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente TabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "PSX_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFixo", Column = "PSX_VALOR_FIXO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFixo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualSobreNF", Column = "PSX_PERCENTUAL_SOBRE_NF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualSobreNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PSX_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PSX_OBSERVACAO_TERCEIRO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ObservacaoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TabelaFreteFilialEmissora", Column = "PSX_TABELA_FRETE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TabelaFreteFilialEmissora { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente)this.MemberwiseClone();
        }

        public virtual bool Equals(CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente other)
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
