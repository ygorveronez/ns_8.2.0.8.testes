namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_TMS_CUSTO_ADICIONAL", EntityName = "ChamadoTMSCustoAdicional", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCustoAdicional", NameType = typeof(ChamadoTMSCustoAdicional))]
    public class ChamadoTMSCustoAdicional : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CHU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChamadoTMS", Column = "CHT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ChamadoTMS Chamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoTipoPagamento", Column = "PTP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoTipoPagamento PedidoTipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCustoExtra", Column = "CHU_QUANTIDADE_CUSTO_EXTRA", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 15)]
        public virtual decimal QuantidadeCustoExtra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "CHU_VALOR_UNITARIO", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 15)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "CHU_VALOR_TOTAL", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 15)]
        public virtual decimal ValorTotal { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.PedidoTipoPagamento?.Descricao ?? string.Empty) + " - " + (this.Chamado?.Descricao ?? string.Empty);
            }
        }
    }
}
