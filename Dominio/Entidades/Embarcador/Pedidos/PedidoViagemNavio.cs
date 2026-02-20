using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_VIAGEM_NAVIO", EntityName = "PedidoViagemNavio", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio", NameType = typeof(PedidoViagemNavio))]
    public class PedidoViagemNavio : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PVN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "PVN_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PVN_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroViagem", Column = "PVN_NUMERO_VIAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PVN_DIRECAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal DirecaoViagemMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Navio Navio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO_ATRACACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoTerminalImportacao TerminalAtracacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ATRACACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Porto PortoAtracacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoChegadaNavio", Column = "PED_DATA_PREVISAO_CHEGADA_NAVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoChegadaNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoSaidaNavio", Column = "PED_DATA_PREVISAO_SAIDA_NAVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoSaidaNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDeadLine", Column = "PED_DATA_DEAD_LINE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDeadLine { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ETAConfirmado", Column = "PVN_ETA_CONFIRMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ETAConfirmado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ETSConfirmado", Column = "PVN_ETS_CONFIRMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ETSConfirmado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "PVN_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "PVN_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsumoPlugs", Column = "PVN_CONSUMO_PLUGS", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal ConsumoPlugs { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsumoTeus", Column = "PVN_CONSUMO_TEUS", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal ConsumoTeus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsumoTons", Column = "PVN_CONSUMO_TONS", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal ConsumoTons { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Schedules", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_VIAGEM_NAVIO_SCHEDULE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PVN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoViagemNavioSchedule", Column = "PVS_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule> Schedules { get; set; }

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

        public virtual bool Equals(PedidoViagemNavio other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
