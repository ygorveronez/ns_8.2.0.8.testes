namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ZONA_TRANSPORTE", EntityName = "CargaZonaTransporte", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte", NameType = typeof(CargaZonaTransporte))]
    public class CargaZonaTransporte : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CZT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDetalhe", Column = "TDE_CODIGO_ZONA_TRANSPORTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe ZonaTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Column = "CZT_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotalPedido", Column = "CZT_PESO_TOTAL_PEDIDO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotalPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CubagemTotalPedido", Column = "CZT_CUBAGEM_TOTAL_PEDIDO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal CubagemTotalPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMercadoriaPedido", Column = "CZT_VALOR_MERCADORIA_PEDIDO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorMercadoriaPedido { get; set; }

        public virtual string Descricao
        {
            get { return ZonaTransporte?.Descricao ?? string.Empty; }
        }
    }
}