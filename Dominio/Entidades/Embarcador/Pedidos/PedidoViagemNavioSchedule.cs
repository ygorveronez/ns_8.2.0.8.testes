using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_VIAGEM_NAVIO_SCHEDULE", EntityName = "PedidoViagemNavioSchedule", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule", NameType = typeof(PedidoViagemNavioSchedule))]
    public class PedidoViagemNavioSchedule : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PVS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString() + " " + (this.PedidoViagemNavio?.Descricao ?? "") + " ETA " + (this.DataPrevisaoChegadaNavio.HasValue ? this.DataPrevisaoChegadaNavio.Value.ToString("dd/MM/yyyy HH:mm") : "") + " ETS " + (this.DataPrevisaoSaidaNavio.HasValue ? this.DataPrevisaoSaidaNavio.Value.ToString("dd/MM/yyyy HH:mm") : "");
            }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO_ATRACACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoTerminalImportacao TerminalAtracacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ATRACACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Porto PortoAtracacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoChegadaNavio", Column = "PVS_DATA_PREVISAO_CHEGADA_NAVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoChegadaNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoSaidaNavio", Column = "PVS_DATA_PREVISAO_SAIDA_NAVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoSaidaNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDeadLine", Column = "PVS_DATA_DEAD_LINE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDeadLine { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ETAConfirmado", Column = "PVS_ETA_CONFIRMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ETAConfirmado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ETSConfirmado", Column = "PVS_ETS_CONFIRMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ETSConfirmado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "PVS_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerouFaturamentoAutomatico", Column = "PVS_GEROU_FATURAMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouFaturamentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviouDocumentacaoAutomatica", Column = "PVS_ENVIOU_DOCUMENTACAO_AUTOMATICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviouDocumentacaoAutomatica { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case true:
                        return "Ativo";
                    case false:
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(PedidoViagemNavioSchedule other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
