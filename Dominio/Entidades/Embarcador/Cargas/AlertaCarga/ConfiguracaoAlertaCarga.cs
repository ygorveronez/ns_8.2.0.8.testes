using System;

namespace Dominio.Entidades.Embarcador.Cargas.AlertaCarga
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_ALERTA_CARGA", EntityName = "ConfiguracaoAlertaCarga", Name = "Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga", NameType = typeof(ConfiguracaoAlertaCarga))]
    public class ConfiguracaoAlertaCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CAC_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CAC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCargaAlerta", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga), Column = "CAC_TIPO_ALERTA", NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga TipoCargaAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cor", Column = "CAC_COR", TypeType = typeof(string), Length = 7, NotNull = false)]
        public virtual string Cor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarParaPreCarga", Column = "CAC_NAO_GERAR_PRE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarParaPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAlertaAcompanhamentoCarga", Column = "CAC_GERAR_ALERTA_ACOMPANHAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAlertaAcompanhamentoCarga { get; set; }

        //GATILHO
        [NHibernate.Mapping.Attributes.Property(Name = "Tempo", Type = "System.Int32", Column = "CAC_TEMPO")]
        public virtual int Tempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "TempoEvento", Type = "System.Int32", Column = "CAC_TEMPO_EVENTO")]
        public virtual int TempoEvento { get; set; }

        //TRATATIVA
        [NHibernate.Mapping.Attributes.Property(0, Name = "EnvioEmailTransportador", Column = "CAC_ENVIO_EMAIL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnvioEmailTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnvioEmailCliente", Column = "CAC_ENVIO_EMAIL_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnvioEmailCliente { get; set; }


        [Obsolete("Coluna removida")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoLimiteTratativaAutomatica", Column = "CAC_TEMPO_TRATATIVA_AUTOMATICA", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoLimiteTratativaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoLimiteTratativaAutomaticaTime", Column = "CAC_TEMPO_TRATATIVA_AUTOMATICA_TIME", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan TempoLimiteTratativaAutomaticaTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseAlerta", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DataBaseAlerta), Column = "CAC_DATA_BASE_ALERTA", NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DataBaseAlerta? DataBaseAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarDataDeEntradaNoDestinoComoDataBase", Column = "CAC_CONSIDERAR_DATA_DE_ENTRADA_NO_DESTINO_COMO_DATA_BASE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarDataDeEntradaNoDestinoComoDataBase { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
