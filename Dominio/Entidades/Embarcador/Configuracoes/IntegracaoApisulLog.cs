namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_APISULLOG", EntityName = "IntegracaoApisulLog", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoApisulLog", NameType = typeof(IntegracaoApisulLog))]
    public class IntegracaoApisulLog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoApisulLog", Column = "CIU_POSSUI_INTEGRACAO_APISULOG", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoApisulLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_APISULLOG", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoApisulLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_URL_INTEGRACAO_APISULLOG_EVENTOS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoApisulLogEventos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_TOKEN_APISULOG", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_CNPJ_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CNPJEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_VALOR_CARGA_ORIGEM", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCargaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIU_TIPO_CARGA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoUtilizarRastreadores", Column = "CIU_NAO_UTILIZAR_RASTREADORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarRastreadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaCarga", Column = "CIU_ETAPA_CARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga? EtapaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarCodigoIntegracaoAbaCodigosIntegracaoTipoDeCarga", Column = "CIU_ENVIAR_CODIGO_INTEGRACAO_ABA_CODIGOS_INTEGRACAO_TIPO_DE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarCodigoIntegracaoAbaCodigosIntegracaoTipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConcatenarCodigoIntegracaoDoClienteCidadeEstado", Column = "CIU_CONCATENAR_CODIGO_INTEGRACAO_DO_CLIENTE_CIDADE_ESTADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConcatenarCodigoIntegracaoDoClienteCidadeEstado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConcatenarCodigoIntegracaoTransporteOridemEDestino", Column = "CIU_CONCATENAR_CODIGO_INTEGRACAO_TRANSPORTE_ORIGEM_E_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConcatenarCodigoIntegracaoTransporteOridemEDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentificadorUnicoViagem", Column = "CIU_IDENTIFICADOR_UNICO_VIAGEM", TypeType = typeof(ObjetosDeValor.Embarcador.Integracao.ApiSulLog.IdentificadorUnicoViagem), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Integracao.ApiSulLog.IdentificadorUnicoViagem? IdentificadorUnicoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemDataInicioViagem", Column = "CIU_ORIGEM_DATA_INICIO_VIAGEM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemDataInicioViagem), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemDataInicioViagem? OrigemDataInicioViagem { get; set; }
    }
}