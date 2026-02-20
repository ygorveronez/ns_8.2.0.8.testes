namespace Dominio.Entidades.Embarcador.TorreControle
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_MONITORAMENTO_NOTIFICACOES_APP", EntityName = "MonitoramentoNotificacoesApp", Name = "Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp", NameType = typeof(MonitoramentoNotificacoesApp))]
    public class MonitoramentoNotificacoesApp : Integracao.Integracao
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MNA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotificacaoApp", Column = "NOT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp NotificacaoApp { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDadosColetaDadosNFe", Column = "GDN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe GestaoDadosColetaDadosNFe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Motivo", Column = "MOT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Configuracoes.Motivo Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoNotificacaoApp", Column = "MNA_TIPO_NOTIFICACAO_APP", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacaoApp), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacaoApp TipoNotificacaoApp { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "MNA_ARQUIVO_REQUISICAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "MNA_ARQUIVO_RESPOSTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.Chamado Chamado { get; set; }

    }
}
