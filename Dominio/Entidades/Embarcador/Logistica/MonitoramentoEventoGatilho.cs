using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_MONITORAMENTO_EVENTO_GATILHO", EntityName = "MonitoramentoEventoGatilho", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho", NameType = typeof(MonitoramentoEventoGatilho))]
    public class MonitoramentoEventoGatilho : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MEG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "Raio", Type = "System.Int32", Column = "MEG_RAIO")]
        public virtual int Raio { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "Tempo", Type = "System.Int32", Column = "MEG_TEMPO")]
        public virtual int Tempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "Posicao", Type = "System.Int64", Column = "POS_CODIGO")]
        public virtual Int64 Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "Velocidade", Type = "System.Int32", Column = "MEG_VELOCIDADE")]
        public virtual int Velocidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "Velocidade2", Type = "System.Int32", Column = "MEG_VELOCIDADE2")]
        public virtual int Velocidade2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "TempoEvento", Type = "System.Int32", Column = "MEG_TEMPO_EVENTO")]
        public virtual int TempoEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "TempoEvento2", Type = "System.Int32", Column = "MEG_TEMPO_EVENTO2")]
        public virtual int TempoEvento2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "MaximoDias", Type = "System.Int32", Column = "MEG_MAXIMO_DIAS")]
        public virtual int MaximoDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "Quantidade", Type = "System.Int32", Column = "MEG_QUANTIDADE")]
        public virtual int Quantidade { get; set; }

        [Obsolete("Transferido para lista PontosDeApoio")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Locais", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Locais PontoApoio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RaioProximidade", Column = "RAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.RaioProximidade RaioProximidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "DataReferencia", Column = "MEG_DATA_REFERENCIA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData DataReferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "ConsiderarApenasDataNaReferencia", TypeType = typeof(bool), NotNull = false, Column = "MEG_APENAS_DATA_REFERENCIA")]
        public virtual bool ConsiderarApenasDataNaReferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "DataBase", Column = "MEG_DATA_BASE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData DataBase { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MonitoramentoEvento", Column = "MEV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento MonitoramentoEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TratativaAutomatica", Column = "MEG_TRATATIVA_AUTOMATICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TratativaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EventoContinuo", Column = "MEG_EVENTO_CONTINUO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EventoContinuo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoReferenteaDataCarregamentoCarga", Column = "MEG_TEMPO_E_REFERENTE_A_DATA_DE_CARREGAMENTO_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TempoReferenteaDataCarregamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarApenasCargasNaoIniciadas", Column = "MEG_VALIDAR_APENAS_CARGAS_NAO_INICIADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarApenasCargasNaoIniciadas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PontosDeApoio", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MONITORAMENTO_EVENTO_GATILHO_PONTOS_DE_APOIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MEG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Locais", Column = "LOC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.Locais> PontosDeApoio { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } } 

    }
}


