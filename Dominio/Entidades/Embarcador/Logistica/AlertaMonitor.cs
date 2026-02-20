using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_ALERTA_MONITOR", EntityName = "AlertaMonitor", Name = "Dominio.Entidades.Embarcador.Logistica.AlertaMonitor", NameType = typeof(AlertaMonitor))]
    public class AlertaMonitor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ALE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAlerta", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta), Column = "ALE_TIPO", NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MonitoramentoEvento", Column = "MEV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento MonitoramentoEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ALE_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "ALE_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "ALE_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "ALE_LATITUDE", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "ALE_LONGITUDE", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus), Column = "ALE_STATUS", NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALE_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertaDescricao", Column = "ALE_DESCRICAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string AlertaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertaManual", Column = "ALE_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlertaManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertaTrativaAutomatica", Column = "ALE_TRATATIVA_AUTOMATICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlertaTrativaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertaPossuiPosicaoRetroativa", Column = "ALE_POSICAO_RETROATIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlertaPossuiPosicaoRetroativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntregaEvento", Column = "CEE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento CargaEntregaEvento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertaReprogramado", Column = "ALE_ALERTA_REPROGRAMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlertaReprogramado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoReprogramado", Column = "ALE_TEMPO_REPROGRAMADO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoReprogramado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        public virtual string Descricao
        {
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaHelper.ObterDescricao(TipoAlerta); }
        }
    }
}





