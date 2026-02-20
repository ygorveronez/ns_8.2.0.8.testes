namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE_PEDIDO", EntityName = "MontagemCarregamentoBlocoSimuladorFretePedido", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido", NameType = typeof(MontagemCarregamentoBlocoSimuladorFretePedido))]
    public class MontagemCarregamentoBlocoSimuladorFretePedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MSP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MontagemCarregamentoBlocoSimuladorFrete", Column = "MSF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete SimuladorFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido Pedido { get; set; }

        /// <summary>
        /// Contem o peso do pedido no frete simulado.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "MSP_PESO_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Peso { get; set; }

        /// <summary>
        /// Contem a quantidade de pallet do pedido no frete simulado.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePallet", Column = "MSP_QUANTIDADE_PALLET", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal QuantidadePallet { get; set; }

        /// <summary>
        /// Contem a cubagem do pedido no frete simulado.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "MetroCubico", Column = "MSP_METRO_CUBICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal MetroCubico { get; set; }

        /// <summary>
        /// Contem a quantidade de volumes do pedido no frete simulado.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Volumes", Column = "MSP_VOLUMES", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Volumes { get; set; }
    }
}
