namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_VEICULO", EntityName = "ConfiguracaoVeiculo", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo", NameType = typeof(ConfiguracaoVeiculo))]
    public class ConfiguracaoVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CVE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_OBRIGATORIO_SEGMENTO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioSegmentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_VISUALIZAR_APENAS_VEICULOS_ATIVOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarApenasVeiculosAtivos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_OBRIGATORIO_INFORMAR_ANO_FABRICACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarAnoFabricacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_OBRIGATORIO_INFORMAR_REBOQUE_PARA_VEICULOS_VINCULADOS_TIPO_RODADO_CAVALO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarReboqueParaVeiculosDoTipoRodadoCavalo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_MANTER_VINCULO_MOTORISTA_EM_FOLGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ManterVinculoMotoristaEmFolga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_NAO_PERMITIR_ALTERAR_KM_VEICULO_EQUIPAMENTO_PNEU_PELA_ORDEM_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAlterarKMVeiculoEquipamentoPneuPelaOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMLimiteAberturaOrdemServico", Column = "CEM_KM_LIMITE_ABERTURA_ORDEM_SERVICO", TypeType = typeof(int), NotNull = false)]
        public virtual int KMLimiteAberturaOrdemServico { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_BLOQUEAR_ALTERACAO_CENTRO_RESULTADO_NA_MOVIMENTACAO_PLACA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearAlteracaoCentroResultadoNaMovimentacaoPlaca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_NAO_PERMITIR_QUE_TRANSPORTADOR_INATIVE_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirQueTransportadorInativeVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_NAO_PERMITIR_REALIZAR_CARDASTRO_NUMA_PLACA_BLOQUEADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirRealizarCadastroPlacaBloqueada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_NAO_PERMITIR_UTILIZAR_VEICULO_EM_MANUTENCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirUtilizarVeiculoEmManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_LANCAMENTO_SERVICO_MANUAL_OS_MARCADO_POR_DEFAULT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancamentoServicoManualNaOSMarcadadoPorDefault { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_NAO_PERMITIR_VINCULAR_PNEU_AO_VEICULO_COM_ABASTECIMENTO_ABERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirVincularPneuVeiculoAbastecimentoAberto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_NAO_PERMITIR_CADASTRAR_VEICULO_SEM_RASTREADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirCadastrarVeiculoSemRastreador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_CRIAR_VINCULO_FROTA_CARGA_FORA_PLANEJAMENTO_FROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CriarVinculoFrotaCargaForaDoPlanejamentoFrota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_NAO_ALTERAR_CENTRO_RESULTADO_VEICULOS_EMISSAO_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAlterarCentroResultadoVeiculosEmissaoCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_NAO_PERMITIR_REALIZAR_FECHAMENTO_ORDEM_SERVICO_CUSTO_ZERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirRealizarFechamentoOrdemServicoCustoZerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_VALIDAR_EXISTE_CARGA_MESMO_NUMERO_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarExisteCargaMesmoNumeroCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_REMOVER_NUMERO_CIOT_ENCERRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemoverNumeroCIOTEncerrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_OBRIGATORIO_INFORMAR_MODELO_VEICULAR_CARGA_NO_WEB_SERVICE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarModeloVeicularCargaNoWebService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_SALVAR_TRANSPORTADOR_TERCEIRO_COMO_GERAR_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SalvarTransportadorTerceiroComoGerarCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_EXIBIR_ABA_DE_EIXOS_NO_MODELO_VEICULAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirAbaDeEixosNoModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_NAO_MOSTRAR_MOTIVO_BLOQUEIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoMostrarMotivoBloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_OBRIGAR_ANTT_VEICULO_VALIDAR_SALVAR_DADOS_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarANTTVeiculoValidarSalvarDadosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_UTILIZAR_MESMO_GESTOR_PARA_TODA_COMPOSICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarMesmoGestorParaTodaComposicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_CADASTRAR_VEICULO_MOTORISTA_BRK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarVeiculoMotoristaBRK { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_VALIDAR_TAG_DIGITALCOM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarTAGDigitalCom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformarKmMovimentacaoPlaca", Column = "CVE_INFORMAR_KM_MOVIMENTAO_PLACA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarKmMovimentacaoPlaca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVE_ATUALIZAR_HISTORICO_SITUACAO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarHistoricoSituacaoVeiculo { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para veículo"; }
        }
    }
}
