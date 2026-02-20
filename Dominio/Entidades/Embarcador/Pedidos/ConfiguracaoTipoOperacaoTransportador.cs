namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_TRANSPORTADOR", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoTransportador", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTransportador", NameType = typeof(ConfiguracaoTipoOperacaoTransportador))]
    public class ConfiguracaoTipoOperacaoTransportador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_PERMITIR_TRANSPORTADOR_SOLICITAR_NOTAS_FISCAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransportadorSolicitarNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirEnvioImagemMultiplosCanhotos", Column = "CTR_PERMITIR_ENVIO_IMAGEM_MULTIPLOS_CANHOTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirEnvioImagemMultiplosCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacaoModalFerroviario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirTransportadorAjusteCargaSegundoTrecho", Column = "CTR_PERMITIR_TRANSPORTADOR_AJUSTE_CARGA_SEGUNDO_TRECHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransportadorAjusteCargaSegundoTrecho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirRetornarEtapa", Column = "CTR_PERMITIR_RETORNAR_ETAPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRetornarEtapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearTransportadorNaoIMOAptoCargasPerigosas", Column = "CTR_BLOQUEAR_TRANSPORTADOR_NAO_IMO_APTO_CARGAS_PERIGOSAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearTransportadorNaoIMOAptoCargasPerigosas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertarTransportadorNaoIMOCargasPerigosas", Column = "CTR_ALERTAR_TRANSPORTADOR_NAO_IMO_APTO_CARGAS_PERIGOSAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlertarTransportadorNaoIMOCargasPerigosas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearVeiculoSemEspelhamento", Column = "TOP_BLOQUEAR_VEICULO_SEM_ESPELHAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearVeiculoSemEspelhamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearVeiculoSemEspelhamentoJanela", Column = "CTR_BLOQUEAR_VEICULO_SEM_ESPELHAMENTO_JANELA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearVeiculoSemEspelhamentoJanela { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações de Transportador"; }
        }
    }
}
