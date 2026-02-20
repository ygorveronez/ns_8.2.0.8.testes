using System;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARREGAMENTO_PEDIDO", EntityName = "CarregamentoPedido", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido", NameType = typeof(CarregamentoPedido))]
    public class CarregamentoPedido : EntidadeBase, IEquatable<CarregamentoPedido>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carregamento", Column = "CRG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carregamento Carregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroReboque", Column = "CRP_NUMERO_REBOQUE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque NumeroReboque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCarregamentoPedido", Column = "CRP_TIPO_CARREGAMENTO_PEDIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCarregamentoPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCarregamentoPedido TipoCarregamentoPedido { get; set; }

        /// <summary>
        /// Comtem a ordem de carregamento do pedido..
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "CRP_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }

        /// <summary>
        /// Contem o peso parcial/total do pedido no carregamento.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "CRP_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        /// <summary>
        /// Contem o pallet parcial/total do pedido no carregamento.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Pallet", Column = "CRP_PALLET", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Pallet { get; set; }

        /// <summary>
        /// VOLUME BIPADO NO CARREGAMENTO
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "VolumeBipado", Column = "CRP_VOLUME_BIPADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal VolumeBipado { get; set; }

        /// <summary>
        /// VOLUME TOTAL DO PEDIDO
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "VolumeTotal", Column = "CRP_VOLUME_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal VolumeTotal { get; set; }

        /// <summary>
        /// Contem o peso dos pallets (cadastro em TipoDetalhe.Valor * qtdePallet).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoPallet", Column = "CRP_PESO_PALLET", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoPallet { get; set; }

        public virtual string Descricao
        {
            get { return this.Pedido.NumeroPedidoEmbarcador; }
        }

        public virtual bool Equals(CarregamentoPedido other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
