namespace Dominio.Entidades.Embarcador.TorreControle
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_WIDGET_ACOMPANHAMENTO_CARGA_USUARIO", DynamicUpdate = true, EntityName = "ConfiguracaoWidgetAcompanhamentoCargaUsuario", Name = "Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario", NameType = typeof(ConfiguracaoWidgetAcompanhamentoCargaUsuario))]
    public class ConfiguracaoWidgetAcompanhamentoCargaUsuario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CWA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirVeiculos", Column = "CWA_EXIBIR_VEICULOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirMotorista", Column = "CWA_EXIBIR_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirDescricaoTipoOperacao", Column = "CWA_EXIBIR_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDescricaoTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirAlertas", Column = "CWA_EXIBIR_ALERTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirAlertas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirPedidosEmMaisCargas", Column = "CWA_PEDIDOS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPedidosEmMaisCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirProximoDestino", Column = "CWA_PROXIMO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirProximoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirProximaEntregaPrevisao", Column = "CWA_EXIBIR_DATA_ENTREGA_PREVISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirProximaEntregaPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirPrevisaoReprogramada", Column = "CWA_EXIBIR_DATA_PREVISAO_REPROGRAMADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPrevisaoReprogramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirDataAgendamentoEntrega", Column = "CWA_EXIBIR_DATA_AGENDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDataAgendamentoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirDataInicioViagem", Column = "CWA_EXIBIR_DATA_INICIO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDataInicioViagem { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirVeiculoTracao", Column = "CWA_EXIBIR_VEICULO_TRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirVeiculoTracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirReboques", Column = "CWA_EXIBIR_REBOQUES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirReboques { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirNumeroFrotaReboques", Column = "CWA_EXIBIR_NUMERO_FROTA_REBOQUES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirNumeroFrotaReboques { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirCidadeProximoDestino", Column = "CWA_EXIBIR_CIDADE_PROXIMO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirCidadeProximoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirValorTotalNFe", Column = "CWA_EXIBIR_VALOR_TOTAL_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirValorTotalNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirPesoTotalNFe", Column = "CWA_EXIBIR_PESO_TOTAL_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPesoTotalNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DesativarAtualizacaoNovasCargas", Column = "CWA_DESATIVAR_ATUALIZACAO_NOVAS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesativarAtualizacaoNovasCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirAnotacoes", Column = "CWA_EXIBIR_ANOTACOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirAnotacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirFilial", Column = "CWA_EXIBIR_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirPesoBrutoNFe", Column = "CWA_EXIBIR_PESO_BRUTO_TOTAL_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPesoBrutoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirLeadTimeTransportador", Column = "CWA_EXIBIR_LEAD_TIME_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirLeadTimeTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirDataColeta", Column = "CWA_EXIBIR_DATA_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDataColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirDataAgendamentoPedido", Column = "CWA_EXIBIR_DATA_AGENDAMENTO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDataAgendamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirCanalEntrega", Column = "CWA_EXIBIR_CANAL_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirCanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirModalTransporte", Column = "CWA_EXIBIR_MODAL_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirModalTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BotaoPrimarioDetalheAcompanhamentoCarga", Column = "CWA_BOTAO_PRIMARIO_ACOMPANHAMENTO_CARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.BotoesDetalheAcompanhamentoCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.BotoesDetalheAcompanhamentoCarga BotaoPrimarioDetalheAcompanhamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BotaoSecundarioDetalheAcompanhamentoCarga", Column = "CWA_BOTAO_SECUNDARIO_ACOMPANHAMENTO_CARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.BotoesDetalheAcompanhamentoCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.BotoesDetalheAcompanhamentoCarga BotaoSecundarioDetalheAcompanhamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OpcaoOrdenacaoCardsAcompanhamentoCarga", Column = "CWA_OPCAO_ORDENACAO_CARDS_ACOMPANHAMENTO_CARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.BotoesDetalheAcompanhamentoCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.OpcoesOrdenacaoCardsAcompanhamentoCarga OpcaoOrdenacaoCardsAcompanhamentoCarga { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirCanalVenda", Column = "CWU_EXIBIR_CANAL_VENDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirCanalVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirMesorregiao", Column = "CWU_EXIBIR_MESORREGIAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirMesorregiao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirRegiao", Column = "CWU_EXIBIR_REGIAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirRegiao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirTendenciaAtraso", Column = "CWA_EXIBIR_TENDENCIA_ATRASO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirTendenciaAtraso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirTendenciaAtrasoColeta", Column = "CWA_EXIBIR_TENDENCIA_ATRASO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirTendenciaAtrasoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirTendenciaAtrasoEntrega", Column = "CWA_EXIBIR_TENDENCIA_ATRASO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirTendenciaAtrasoEntrega { get; set; }
    }
}
