using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_MONITORAMENTO_EVENTO", EntityName = "MonitoramentoEvento", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento", NameType = typeof(MonitoramentoEvento))]
    public class MonitoramentoEvento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MEV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MEV_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMonitoramentoEvento", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento), Column = "MEV_TIPO_MONITORAMENTO_EVENTO", NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento TipoMonitoramentoEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MEV_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAlerta", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta), Column = "MEV_TIPO_ALERTA", NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Gatilhos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MONITORAMENTO_EVENTO_GATILHO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MEV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MonitoramentoEventoGatilho", Column = "MEG_CODIGO")]
        public virtual ICollection<MonitoramentoEventoGatilho> Gatilhos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Horarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MONITORAMENTO_EVENTO_HORARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MEV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MonitoramentoEventoHorario", Column = "MEH_CODIGO")]
        public virtual ICollection<MonitoramentoEventoHorario> Horarios { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "StatusViagem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MONITORAMENTO_EVENTO_STATUS_VIAGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MEV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MonitoramentoEventoStatusViagem", Column = "MES_CODIGO")]
        public virtual ICollection<MonitoramentoEventoStatusViagem> StatusViagem { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TipoDeCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MONITORAMENTO_EVENTO_TIPO_DE_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MEV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MonitoramentoEventoTipoDeCarga", Column = "MET_CODIGO")]
        public virtual ICollection<MonitoramentoEventoTipoDeCarga> TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TipoDeOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MONITORAMENTO_EVENTO_TIPO_DE_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MEV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MonitoramentoEventoTipoDeOperacao", Column = "MTO_CODIGO")]
        public virtual ICollection<MonitoramentoEventoTipoDeOperacao> TipoDeOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Tratativas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MONITORAMENTO_EVENTO_TRATATIVA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MEV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MonitoramentoEventoTratativa", Column = "MET_CODIGO")]
        public virtual ICollection<MonitoramentoEventoTratativa> Tratativas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TratativaAutomatica", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MONITORAMENTO_EVENTO_TRATATIVA_AUTOMATICA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MEV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MonitoramentoEventoTratativaAutomatica", Column = "MTA_CODIGO")]
        public virtual ICollection<MonitoramentoEventoTratativaAutomatica> TratativaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prioridade", Column = "MEV_PRIORIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Prioridade { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "Cor", Column = "MEV_COR", TypeType = typeof(string), Length = 7, NotNull = false)]
        public virtual string Cor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirControleEntrega", Column = "MEV_EXIBIR_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExibirControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirApp", Column = "MEV_EXIBIR_APP", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExibirApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarParaPreCarga", Column = "MEV_NAO_GERAR_PRE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarParaPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAlertaAcompanhamentoCarga", Column = "MEV_GERAR_ALERTA_ACOMPANHAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAlertaAcompanhamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirDescricaoAlerta", Column = "MEV_EXIBIR_DESCRICAO_ALERTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDescricaoAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirDataeHoraGeracaoAlerta", Column = "MEV_EXIBIR_DATA_E_HORA_GERACAO_ALERTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDataeHoraGeracaoAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarParaSemaforo", Column = "MEV_CONSIDERAR_SEMAFORO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarParaSemaforo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuandoProcessar", Column = "MEV_QUANDO_PROCESSAR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento QuandoProcessar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VerificarStatusViagem", Column = "MEV_VERIFICAR_STATUS_VIAGEM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem VerificarStatusViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VerificarTipoDeCarga", Column = "MEV_VERIFICAR_TIPO_DE_CARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeCarga VerificarTipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VerificarTipoDeOperacao", Column = "MEV_VERIFICAR_TIPO_DE_OPERACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeOperacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeOperacao VerificarTipoDeOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAtendimento", Column = "MEV_GERAR_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "MCH_CODIGO ", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.MotivoChamado MotivoChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarEvento", Column = "MEV_INTEGRAR_EVENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarEvento { get; set; }

        public virtual MonitoramentoEventoGatilho Gatilho
        {
            get { return Gatilhos.FirstOrDefault(); }
        }

        public virtual MonitoramentoEventoHorario Horario
        {
            get { return Horarios.FirstOrDefault(); }
        }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
