using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_AGENDAMENTO_ENTREGA_PEDIDO", EntityName = "HistoricoAgendamentoEntregaPedido", Name = "Dominio.Entidades.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido", NameType = typeof(HistoricoAgendamentoEntregaPedido))]
    public class HistoricoAgendamentoEntregaPedido : EntidadeBase
    {
        public HistoricoAgendamentoEntregaPedido()
        {
            DataHoraRegistro = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HAE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HAE_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoHistoricoAgendamento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoHistoricoAgendamento Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HAE_DATA_HORA_REGISTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataHoraRegistro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HAE_DATA_HORA_AGENDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraAgenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HAE_OBSERVACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoReagendamento", Column = "MRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento MotivoReagendamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoResponsavelAtrasoEntrega", Column = "TRA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega ResponsavelMotivoReagendamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HAE_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoEntregaPedido), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoEntregaPedido? SituacaoAgendamentoEntregaPedido { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Pedido?.Descricao ?? string.Empty;
            }
        }
    }
}
