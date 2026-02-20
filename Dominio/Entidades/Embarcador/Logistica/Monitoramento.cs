using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_MONITORAMENTO", EntityName = "Monitoramento", Name = "Dominio.Entidades.Embarcador.Logistica.Monitoramento", NameType = typeof(Monitoramento))]
    public class Monitoramento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MON_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MON_POLINHA_PREVISTA", Type = "StringClob", NotNull = false, Lazy = true)]
        public virtual string PolilinhaPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MON_POLINHA_REALIZADA", Type = "StringClob", NotNull = false, Lazy = true)]
        public virtual string PolilinhaRealizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MON_DISTANCIA_PREVISTA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? DistanciaPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MON_DISTANCIA_REALIZADA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal DistanciaRealizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MON_PONTOS_PREVISTO", Type = "StringClob", NotNull = false)]
        public virtual string PontosPrevistos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "MON_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "MON_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "MON_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimDescanso", Column = "MON_DATA_FIM_DESCANSO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimDescanso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "MON_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NoRaioDaOrigem", Column = "MON_NO_RAIO_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NoRaioDaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualViagem", Column = "MON_PERCENTUAL_VIAGEM", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MonitoramentoStatusViagem", Column = "MSV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem StatusViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Processar", Column = "MON_PROCESSAR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao Processar { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Posicao", Column = "POS_ULTIMA_POSICAO ", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Posicao UltimaPosicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Critico", Column = "MON_CRITICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Critico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MON_POLILINHA_ATE_DESTINO", Type = "StringClob", NotNull = false)]
        public virtual string PolilinhaAteDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MON_DISTANCIA_ATE_DESTINO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? DistanciaAteDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MON_POLILINHA_ATE_ORIGEM", Type = "StringClob", NotNull = false)]
        public virtual string PolilinhaAteOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MON_DISTANCIA_ATE_ORIGEM", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? DistanciaAteOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MON_NUMERO_TEMPERATURA_RECEBIDA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTemperaturaRecebidas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MON_TEMPERATURA_NA_FAIXA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTemperaturasNaFaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MON_OBSERVACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoFinalizacao", Column = "MON_MOTIVO_FINALIZACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoFinalizacaoMonitoramento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoFinalizacaoMonitoramento MotivoFinalizacao { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }

    }
}
