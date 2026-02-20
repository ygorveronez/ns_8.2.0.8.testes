namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_MONTAGEM_CARGA", EntityName = "ConfiguracaoMontagemCarga", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoMontagemCarga", NameType = typeof(ConfiguracaoMontagemCarga))]
    public class ConfiguracaoMontagemCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_EXIGIR_DEFINICAO_TIPO_CARREGAMENTO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirDefinicaoTipoCarregamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_ATUALIZAR_INFORMACOES_PEDIDOS_POR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarInformacoesPedidosPorCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_APRESENTA_OPCAO_REMOVER_E_CANCELAR_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ApresentaOpcaoRemoverCancelarPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_APRESENTA_OPCAO_CANCELAR_RESERVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ApresentaOpcaoCancelarReserva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_FILTRO_PERIODO_VAZIO_INICIAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltroPeriodoVazioAoIniciar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_DATA_ATUAL_NOVO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DataAtualNovoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_OCULTAR_BIPAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarBipagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_UTILIZAR_DATA_PREVISAO_SAIDA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataPrevisaoSaidaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_NAO_PERMITIR_PEDIDOS_TOMADORES_DIFERENTES_MESMO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirPedidosTomadoresDiferentesMesmoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_GERAR_UNICO_BLOCO_POR_RECEBEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarUnicoBlocoPorRecebedor { get; set; }

        /// <summary>
        /// Configuração que permite adicionar os pedidos em carregamentos, mas não deve deixar gerar a carga.
        /// 55993 - FRIMESA
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_PERMITIR_GERAR_CARREGAMENTO_PEDIDO_BLOQUEADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirGerarCarregamentoPedidoBloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_ATIVAR_TRATATIVA_DUPLICIDADE_EMISSAO_CARGAS_FEEDER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarTratativaDuplicidadeEmissaoCargasFeeder { get; set; }

        /// <summary>
        /// Atributo utilizado para descontar os saldo do pedido de acordo com os pesos enviados na NF.
        /// Problema BOTICÁRIO, pedidos com saldo negativo.. multiplas notas...
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_CONSIDERAR_PESO_NF_SALDO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarPesoNFSaldoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoControleSaldoPedido", Column = "CMC_TIPO_CONTROLE_SALDO_PEDIDO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoControleSaldoPedido), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoControleSaldoPedido TipoControleSaldoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_GERAR_CARGA_AO_CONFIRMAR_INTEGRACAO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaAoConfirmarIntegracaoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_NAO_RETORNAR_INTEGRACAO_CARREGAMENTO_SE_SOMENTE_DADOS_TRANSPORTE_FOREM_ALTERADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRetornarIntegracaoCarregamentoSeSomenteDadosTransporteForemAlterados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_ATIVAR_MONTAGEM_CARGA_POR_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarMontagemCargaPorNFe { get; set; }

        /// <summary>
        /// Utilizado no Montagem Carga (Marfrig) para roteirizar automatticamente carregamento após roteirizado quando adicionado ou removido pedidos do carregamento.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_ROTEIRIZAR_AUTOMATICAMENTE_ADICIONAR_REMOVER_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RoteirizarAutomaticamenteAposRoteirizadoAoAdicionarRemoverPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_EXIBIR_ALERTA_RESTRICAO_ENTREGA_CLIENTE_CARD_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirAlertaRestricaoEntregaClienteCardCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_UTILIZAR_FILIAIS_HABILITADAS_TRANSPORTAR_MONTAGEM_CARGA_MAPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarFiliaisHabilitadasTransportarMontagemCargaMapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_FILTRAR_PEDIDOS_ONDE_RECEBEDOR_TRANSPORTADOR_NO_PORTAL_DO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarPedidosOndeRecebedorTransportadorNoPortalDoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_VENCEDOR_SIMULADOR_FRETE_EMPRESA_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VencedorSimuladorFreteEmpresaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_PERMITIR_EDITAR_PEDIDOS_ATRAVES_TELA_MONTAGEM_CARGA_MAPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirEditarPedidosAtravesTelaMontagemCargaMapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_IGNORAR_ROTA_FRETE_PEDIDOS_MONTAGEM_CARGA_MAPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IgnorarRotaFretePedidosMontagemCargaMapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_MANTER_PEDIDOS_COM_MESMO_AGRUPADOR_NA_MESMA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ManterPedidosComMesmoAgrupadorNaMesmaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_EXIBIR_PEDIDOS_FORMATO_GRID", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPedidosFormatoGrid { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_EXIBIR_LISTAGEM_NOTAS_FISCAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirListagemNotasFiscais { get; set; }

        /// <summary>
        /// Configuração criada para atender fluxo de transbordo da Casaredo
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_FILTRAR_PEDIDOS_VINCULADO_OUTRAS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarPedidosVinculadoOutrasCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_CONSIDERAR_SOMENTE_PESO_OU_CUBAGEM_AO_GERAR_BLOCO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarSomentePesoOuCubagemAoGerarBloco { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração para montagem de carga";
            }
        }
    }
}
