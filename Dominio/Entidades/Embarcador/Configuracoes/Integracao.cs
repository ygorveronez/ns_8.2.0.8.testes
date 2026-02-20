using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO", EntityName = "Integracao", Name = "Dominio.Entidades.Embarcador.Configuracoes.Integracao", NameType = typeof(Integracao))]
    public class Integracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração";
            }
        }


        #region Avon

        [Obsolete("Utilizar configuração própria por empresa (classe IntegracaoAvon).")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenAvonProducao", Column = "COI_TOKEN_AVON", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TokenAvonProducao { get; set; }

        [Obsolete("Utilizar configuração própria por empresa (classe IntegracaoAvon).")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenAvonHomologacao", Column = "COI_TOKEN_AVON_HOMOLOGACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TokenAvonHomologacao { get; set; }

        [Obsolete("Utilizar configuração própria por empresa (classe IntegracaoAvon).")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "EnterpriseIdAvon", Column = "COI_ENTERPRISE_ID_AVON", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string EnterpriseIdAvon { get; set; }

        #endregion

        #region Boticario

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_BOTICARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoBoticario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_BOTICARIO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoBoticario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_INTEGRACAO_BOTICARIO_CLIENT_ID", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string IntegracaoBoticarioClientId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_INTEGRACAO_BOTICARIO_CLIENT_SECRET", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string IntegracaoBoticarioClientSecret { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_GERAR_TOKEN_BOTICARIO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLGerarTokenBoticario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_ENVIO_SEQUENCIA_BOTICARIO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLEnvioSequenciaBoticario { get; set; }

        #endregion

        #region Natura

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoMatrizNatura", Column = "COI_CODIGO_MATRIZ_NATURA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoMatrizNatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioNatura", Column = "COI_USUARIO_NATURA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioNatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaNatura", Column = "COI_SENHA_NATURA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaNatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ENVIAR_OCORRENCIA_NATURA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarOcorrenciaNaturaAutomaticamente { get; set; }

        /// <summary>
        /// Indica se deve utilizar o valor do frete calculado/informado no TMS e não do DT da Natura
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_UTILIZAR_VALOR_FRETE_TMS_NATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarValorFreteTMSNatura { get; set; }

        #endregion

        #region Opentech

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioOpenTech", Column = "COI_USUARIO_OPENTECH", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaOpenTech", Column = "COI_SENHA_OPENTECH", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DominioOpenTech", Column = "COI_DOMINIO_OPENTECH", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DominioOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoClienteOpenTech", Column = "COI_CODIGO_CLIENTE_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoClienteOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPASOpenTech", Column = "COI_CODIGO_PAS_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoPASOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLOpenTech", Column = "COI_URL_OPENTECH", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CODIGO_PRODUTO_VEICULO_COM_LOCALIZADOR_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoProdutoVeiculoComLocalizadorOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBaseOpenTech", Column = "COI_VALOR_BASE_OPENTECH", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBaseOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_INTEGRAR_VEICULO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarVeiculoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProdutoColetaOpentech", Column = "COI_CODIGO_PRODUTO_COLETA_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoProdutoColetaOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProdutoColetaEmbarcadorOpentech", Column = "COI_CODIGO_PRODUTO_COLETA_EMBARCADOR_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoProdutoColetaEmbarcadorOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProdutoColetaTransportadorOpentech", Column = "COI_CODIGO_PRODUTO_COLETA_TRANSPORTADOR_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoProdutoColetaTransportadorOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProdutoPadraoOpentech", Column = "COI_CODIGO_PRODUTO_PADRAO_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoProdutoPadraoOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_INTEGRAR_COLETA_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarColetaOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ATUALIZAR_VEICULO_MOTORISTAS_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarVeiculoMotoristaOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ENVIAR_CODIGO_EMBARCADOR_PRODUTO_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarCodigoEmbarcadorProdutoOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_INTEGRAR_ROTA_CARGA_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarRotaCargaOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_NOTIFICAR_FALHA_INTEGRACAO_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarFalhaIntegracaoOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailsNotificacaoFalhaIntegracaoOpentech", Column = "COI_EMAILS_NOTIFICACAO_INTEGRACAO_OPENTECH", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EmailsNotificacaoFalhaIntegracaoOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProdutoParaValidarSomenteVeiculoMotoristaSemUsoRastreador", Column = "COI_CODIGO_PRODUTO_SOMENTE_VEICULO_MOTORISTA_SEM_USO_RASTREADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoProdutoParaValidarSomenteVeiculoMotoristaSemUsoRastreador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirTransportadorReenviarIntegracoesComProblemasOpenTech", Column = "COI_PERMITIR_TRANSPORTADOR_REENVIAR_INTEGRACOES_COM_PROBLEMAS_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransportadorReenviarIntegracoesComProblemasOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarDataPrevisaoEntregaDataCarregamentoOpentech", Column = "COI_ENVIAR_DATA_PREVISAO_ENTREGA_DATA_CARREGAMENTO_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDataPrevisaoEntregaDataCarregamentoOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarDataAtualNaDataPrevisaoOpentech", Column = "COI_ENVIAR_DATA_ATUAL_NA_DATA_PREVISAO_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDataAtualNaDataPrevisaoOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarCargaOpenTechV10", Column = "COI_INTEGRAR_CARGA_V10_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarCargaOpenTechV10 { get; set; }

        /// <summary>
        /// Regra mudou: faz apenas envio da data de integração para início e data integração + 1 hora para fim
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularPrevisaoEntregaComBaseDistanciaOpentech", Column = "COI_CALCULAR_PREVISAO_ENTREGA_COM_BASE_DISTANCIA_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularPrevisaoEntregaComBaseDistanciaOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarDataPrevisaoSaidaPedidoOpentech", Column = "COI_ENVIAR_DATA_PREVISAO_SAIDA_PEDIDO_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDataPrevisaoSaidaPedidoOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarInformacoesRastreadorCavaloOpentech", Column = "COI_ENVIAR_INFORMACOES_RASTREADOR_CAVALO_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarInformacoesRastreadorCavaloOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarCodigoIntegracaoRotaCargaOpenTech", Column = "COI_ENVIAR_CODIGO_INTEGRACAO_ROTA_CARGA_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarCodigoIntegracaoRotaCargaOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarNrfonecelBrancoOpenTech", Column = "COI_ENVIAR_NRFONECEL_BRANCO_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarNrfonecelBrancoOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarPlacaVeiculoSeNaoExistirNumeroFrotaOpenTech", Column = "COI_ENVIAR_PLACA_VEICULO_SE_NAO_EXISTIR_NUMERO_FROTA_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarPlacaVeiculoSeNaoExistirNumeroFrotaOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarValorNotasValorDocOpenTech", Column = "COI_ENVIAR_VALOR_NOTAS_VALORDOC_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarValorNotasValorDocOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarCodigoIntegracaoCentroCustoCargaOpenTech", Column = "COI_ENVIAR_CODIGO_INTEGRACAO_CENTRO_CUSTO_CARGA_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarCodigoIntegracaoCentroCustoCargaOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ENVIAR_VALOR_NOTAS_CAMPO_VALORDOC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarValorDasNotasNoCampoValorDoc { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CadastrarRotaCargaOpentech", Column = "COI_CADASTRAR_ROTA_CARGA_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarRotaCargaOpentech { get; set; }

        //Removido e adicionado IntegracaoGeralOpenTech
        //[NHibernate.Mapping.Attributes.Property(0, Name = "EnviarDataNFeNaDataPrevistaOpentech", Column = "COI_ENVIAR_DATA_NFE_DATA_PREVISTA_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool EnviarDataNFeNaDataPrevistaOpentech { get; set; }

        #endregion

        #region CIOT

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        #endregion

        #region DT-e

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLRecepcaoDTe", Column = "COI_URL_RECEPCAO_DTE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLRecepcaoDTe { get; set; }

        #endregion

        #region e-Frete

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegradorEFrete", Column = "COI_CODIGO_INTEGRADOR_EFRETE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoIntegradorEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioEFrete", Column = "COI_USUARIO_EFRETE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UsuarioEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaEFrete", Column = "COI_SENHA_EFRETE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SenhaEFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "COI_EMPRESA_MATRIZ_EFRETE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa MatrizEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ENCERRAR_TODOS_CIOTS_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncerrarTodosCIOTAutomaticamente { get; set; }

        //Removido e adicionado na IntegracaoGeralEFrete
        //[NHibernate.Mapping.Attributes.Property(0, Column = "COI_ENVIAR_IMPOSTOS_INTEGRACAO_CIOT", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool EnviarImpostosNaIntegracaoDoCIOT { get; set; }

        #endregion

        #region BrasilRisk

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_BRASILRISK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_BRASILRISK", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_BRASILRISK", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_BRASILRISK_HOMOLOGACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLHomologacaoBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_BRASILRISK_PRODUCAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLProducaoBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_BRASILRISK_GESTAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLBrasilRiskGestao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_BRASILRISK_VEICULO_MOTORISTA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLBrasilRiskVeiculoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CNPJ_EMBARCADOR_BRASILRISK", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CNPJEmbarcadorBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBaseBrasilRisk", Column = "COI_VALOR_BASE_BRASILRISK_SM", TypeType = typeof(decimal), Scale = 5, Precision = 19, NotNull = false)]
        public virtual decimal ValorBaseBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ENVIAR_TODOS_DESTINOS_BRASIL_RISK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTodosDestinosBrasilRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_INICIO_VIAGEM_FIXO_HORA_ATUAL_MAIS_MINUTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioViagemFixoHoraAtualMaisMinutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MinutosAMaisInicioViagem", Column = "COI_MINUTOS_A_MAIS_INICIO_VIAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int MinutosAMaisInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ENVIAR_DADOS_TRANSPORTADORA_SUBCONTRATADA_NAS_OBSERVACOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDadosTransportadoraSubContratadaNasObservacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_INTEGRAR_ROTA_BRASIL_RISK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarRotaBrasilRisk { get; set; }

        #endregion BrasilRisk

        #region MundialRisk

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_MUNDIALRISK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoMundialRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_MUNDIALRISK", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioMundialRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_MUNDIALRISK", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaMundialRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_MUNDIALRISK_HOMOLOGACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLHomologacaoMundialRisk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_MUNDIALRISK_PRODUCAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLProducaoMundialRisk { get; set; }

        #endregion

        #region Logiun

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_LOGIUN", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoLogiun { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_LOGIUN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioLogiun { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_LOGIUN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaLogiun { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_LOGIUN_HOMOLOGACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLHomologacaoLogiun { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_LOGIUN_PRODUCAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLProducaoLogiun { get; set; }

        #endregion

        #region Buonny

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CNPJ_CLIENTE_BUONNY", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CNPJClienteBuonny { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_BUONNY", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TokenBuonny { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CNPJ_GERENCIADORA_RISCO_BUONNY", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CNPJGerenciadoraDeRiscoBuonny { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_HOMOLOGACAO_BUONNY", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLHomologacaoBuonny { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_PRODUCAO_BUONNY", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLProducaoBuonny { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_REST_HOMOLOGACAO_BUONNY", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLRestHomologacaoBuonny { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_REST_PRODUCAO_BUONNY", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLRestProducaoBuonny { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoHorasConsultasBuonny", Column = "COI_TEMPO_HORAS_CONSULTA_BUONNY", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoHorasConsultasBuonny { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CONSULTAR_SM_BUONNY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarSMBuonny { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CADASTRAR_MOTORISTA_ANTES_CONSULTAR_BUONNY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarMotoristaAntesConsultarBuonny { get; set; }

        #endregion

        #region Avior

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_AVIOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioAvior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_AVIOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaAvior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_AVIOR", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLAvior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CNPJ_AVIOR", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CNPJAvior { get; set; }

        #endregion

        #region NOX

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_NOX", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoNOX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_NOX", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioNOX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_NOX", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaNOX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_NOX", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TokenNOX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_NOX_HOMOLOGACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLHomologacaoNOX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_NOX_PRODUCAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLProducaoNOX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CNPJ_MATRIZ_NOX", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJMatrizNOX { get; set; }

        #endregion

        #region Carrefour

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_CARREFOUR_CANCELAMENTO_CARGA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLCarrefourCancelamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_CARREFOUR_CARGA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLCarrefourCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_CARREFOUR_INDICADOR_INTEGRACAO_CTE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLCarrefourIndicadorIntegracaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_CARREFOUR_OCORRENCIA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLCarrefourOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_CARREFOUR_PROVISAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLCarrefourProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_CARREFOUR_VALIDAR_CANCELAMENTO_CARGA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLCarrefourValidarCancelamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_CARREFOUR", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string TokenCarrefour { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_CARREFOUR_INDICADOR_INTEGRACAO_CTE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string TokenCarrefourIndicadorIntegracaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_CARREFOUR_PROVISAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string TokenCarrefourProvisao { get; set; }

        #endregion

        #region Golden Service

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_GOLDEN_SERVICE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoGoldenService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ID_GOLDEN_SERVICE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IdGoldenService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CODIGO_GOLDEN_SERVICE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoGoldenService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_GOLDEN_SERVICE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaGoldenService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_GOLDEN_SERVICE_HOMOLOGACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLHomologacaoGoldenService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_GOLDEN_SERVICE_PRODUCAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLProducaoGoldenService { get; set; }

        #endregion

        #region GPA

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_GPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoGPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_GPA_HOMOLOGACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLHomologacaoGPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_GPA_PRODUCAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLProducaoGPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_API_KEY_GPA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string APIKeyGPA { get; set; }

        #endregion

        #region Ortec

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_ORTEC", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioOrtec { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_ORTEC", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaOrtec { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_ORTEC", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLOrtec { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_INTEGRAR_ENTREGA_ORTEC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarEntregaOrtec { get; set; }

        #endregion

        #region APIGoogle

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_API_KEY_GOOGLE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string APIKeyGoogle { get; set; }

        #endregion

        #region OpenStreetMap

        #region Geocoding

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_GEO_SERVICE_GEOCODING", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding GeoServiceGeocoding { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SERVER_ROUTE_OSM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ServidorRouteOSM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SERVER_ROUTE_GOOGLE_ORTOOLS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ServidorRouteGoogleOrTools { get; set; }

        /// <summary>
        /// Default: http://20.195.231.113:8080/nominatim/{0}?
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SERVER_NOMINATIM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ServidorNominatim { get; set; }

        #endregion

        #region PamCard

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_EMPRESA_FIXA_PAMCARD", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaFixaPamCard { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_PAMCARD_CORPORATIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLPamcardCorporativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_PAMCARD_CORPORATIVO_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLPamcardCorporativoAutenticacao { get; set; }

        #endregion

        #region MAGALOG

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_MAGALOG", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoMagalog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_MAGLOG_HOMOLOGACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLHomologacaoMagalog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_MAGALOG_PRODUCAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLProducaoMagalog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_MAGALOG_PRODUCAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TokenMagalog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_MAGALOG_ESCRITURACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEscrituracaoMagalog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_MAGALOG_KEYCLOAC", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLKeyCloacMagalog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ID_MAGALOG_KEYCLOAC", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string IDKeyCloacMagalog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SECRET_MAGALOG_KEYCLOAC", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string SecretKeyCloacMagalog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_MAGALOG_CTE_MULTICTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLCTeMultiCTeMagalog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_MAGALOG_PCTE_MULTICTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TokenCTeMultiCTeMagalog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_MAGALOG_MDFE_MULTICTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLMDFeMultiCTeMagalog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_MAGALOG_MDFE_MULTICTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TokenMDFeMultiCTeMagalog { get; set; }

        #endregion

        #region Piracanjuba Obsoleto migrado

        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_CANHOTO_PIRACANJUBA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCanhotoPiracanjuba { get; set; }

        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_CARGA_PIRACANJUBA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCargaPiracanjuba { get; set; }

        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIO_AMBIENTE_PRODUCAO_PIRACANJBA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AmbienteProducaoPiracanjuba { get; set; }

        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_STRING_AMBIENTE_PIRACANJUBA", TypeType = typeof(string), NotNull = false)]
        public virtual string StringAmbientePiracanjuba { get; set; }

        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_API_KEY_PIRACANJUBA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string APIKeyPiracanjuba { get; set; }

        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_MAESTRO_SUBSCRIPTION_KEY_PIRACANJUBA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string APIKeyPiMaestroSubscriptionkeyracanjuba { get; set; }

        #endregion

        #region MARFRIG

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_MARFRIG", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoMarfrig { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_MARFRIG", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLMarfrig { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_IMPRESSAO_DOCUMENTOS_MARFRIG", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoImpressaoDocumentosMarfrig { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_DATA_CORTE_CRIACAO_INTEGRACAO_MARFRIG", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCorteCriacaoIntegracaoTituloMarfrig { get; set; } = null;

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_DATA_CORTE_CONSULTA_INTEGRACAO_MARFRIG", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCorteConsultaIntegracaoTituloMarfrig { get; set; } = null;

        #endregion

        #region MINERVA

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_MINERVA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLMinerva { get; set; }

        #endregion

        #region Serpro

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_SERPRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoSerpro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CONSUMER_KEY_SERPRO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ConsumerKeySerpro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CONSUMER_SECRET_SERPRO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ConsumerSecretSerpro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_SERPRO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string TokenSerpro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_DATA_TOKEN_SERPRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataTokenSerpro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_SERPRO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLSerpro { get; set; }

        #endregion

        #region Raster

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_RASTER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoRaster { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_RASTER", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioRaster { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_RASTER", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaRaster { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_RASTER", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLRaster { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_NOTIFICAR_FALHA_INTEGRACAO_RASTER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarFalhaIntegracaoRaster { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_GERAR_INTEGRACAO_PRESM_CARGA_DADOS_TRANSPORTE_RASTER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarIntegracaoPreSmEtapaCargaDadosTransporteRaster { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_GERAR_INTEGRACAO_EFETIVA_PRESM_ETAPA_CARGA_FRETE_RASTER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarIntegracaoEfetivaPreSmEtapaCargaFreteRaster { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_GERAR_REVISAO_PRESM_ETAPA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarIntegracaoSetRevisaoPreSMnaEtapaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REENVIAR_INTEGRACAO_DADOS_TRANSPORTE_AO_ALTERAR_DADOS_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReenviarIntegracaoDadosTransporteAoAlterarDadosTransporteRaster { get; set; }
        #endregion

        #region Unilever Four Kites

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_HOMOLOGACAO_UNILEVER_FOUR_KITES", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLHomologacaoUnileverFourKites { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_PRODUCAO_UNILEVER_FOUR_KITES", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLProducaoUnileverFourKites { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_UNILEVER_FOUR_KITES", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioUnileverFourKites { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_UNILEVER_FOUR_KITES", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaUnileverFourKites { get; set; }

        #endregion

        #region Digibee

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_DIGIBEE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoDigibee { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_DADOS_CARGA_DIGIBEE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoDadosCargaDigibee { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_CANCELAMENTO_DIGIBEE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoCancelamentoDigibee { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_AUTENTICACAO_DIGIBEE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLAutenticacaoDigibee { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_AUTENTICACAO_DIGIBEE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioAutenticacaoDigibee { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_AUTENTICACAO_DIGIBEE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string SenhaAutenticacaoDigibee { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_API_KEY_DIGIBEE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string APIKeyDigibee { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_DIGIBEE_PADRAO_CONSINCO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoDigibeePadraoConsinco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_DADOS_CONTABEIS_CTE_DIGIBEE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoDadosContabeisCTeDigibee { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_ESCRITURACAO_CTE_DIGIBEE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoEscrituracaoCTeDigibee { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_API_KEY_DIGIBEE_GERAL", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string APIKeyDigibeeGeral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_AJUSTAR_DATA_PARA_CORRESPONDER_QUINZENA_DIGIBEE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AjustarDataParaCorresponderQuinzenaDigibee { get; set; }

        #endregion

        #region Telerisco

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_TELERISCO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoTelerisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CNPJ_EMBARCADOR_TELERISCO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CNPJEmbarcadorTelerisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CAMINHO_CERTIFICADO_TELERISCO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CaminhoCertificadoTelerisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_CERTIFICADO_TELERISCO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string SenhaCertificadoTelerisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_INTEGRACAO_POST_TELERISCO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoViaPOSTTelerisco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_EMPRESA_FIXA_TELERISCO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaFixaTelerisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_NAO_ENVIAR_DATA_EMBARQUE_GR_MOTORISTA_TELERISCO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarDataEmbarqueGrMotoristaTelerisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ENVIAR_EMPRESA_FIXA_COMO_CNPJ_EMBARCADOR_NA_INTEGRACAO_TELERISCO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmpresaFixaComoCNPJEmbarcadorNaIntegracaoTelerisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CODIGOS_ACEITOS_RETORNO_TELERISCO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string CodigosAceitosRetornoTelerisco { get; set; }

        #endregion

        #region CargoX

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_CARGOX", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCargoX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_TELERISCO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string TokenCargoX { get; set; }

        #endregion

        #region Krona

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_KRONA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoKrona { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_KRONA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoKrona { get; set; }

        #endregion

        #region PH

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_PH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoPH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_PH", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioPH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_PH", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaPH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_PH_HOMOLOGACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLHomologacaoPH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_PH_PRODUCAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLProducaoPH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CNPJ_CONTADOR_PH", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CNPJContadorPH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SOFTWARE_PH", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string SoftwarePH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_PORTA_PH", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string PortaPH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_IP_SOCKET_PH", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string IPSocketPH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_PORTA_SOCKET_PH", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string PortaSocketPH { get; set; }

        #endregion

        #region Infolog

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_INFOLOG", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoInfolog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_INFOLOG", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoInfolog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_INFOLOG", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioInfolog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_INFOLOG", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaInfolog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CODIGO_OPERACAO_INFOLOG", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoOperacaoInfolog { get; set; }

        #endregion

        #region Riachuelo

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_RIACHUELO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoRiachuelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_TOKEN_RIACHUELO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLTokenRiachuelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_APIKEY_INTEGRACAO_RIACHUELO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ApiKeyRiachuelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_APPID_INTEGRACAO_RIACHUELO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string AppIDRiachuelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_LOGIN_CRIPT_RIACHUELO", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string LoginCriptRiachuelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_ENTREGA_NFE_RIACHUELO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoEntregaNFeRiachuelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_HABILITAR_DATA_SAIDA_CD_LOJA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarDataSaidaCDLoja { get; set; }

        #endregion

        #region Saint-Gobain

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_SAINTGOBAIN", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoSaintGobain { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_SAINTGOBAIN", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URLIntegracaoSaintGobain { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USER_NAME_SAINTGOBAIN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UserNameSaintGobain { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_PASSWORD_SAINTGOBAIN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PasswordSaintGobain { get; set; }

        #endregion

        #region Toledo

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_TOLEDO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLToledo { get; set; }

        #endregion

        #region Adagio

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_EMAIL_ADAGIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string EmailAdagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_ADAGIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaAdagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_ADAGIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAdagio { get; set; }

        #endregion

        #region Trizy

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_TRIZY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_TRIZY", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string TokenTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_TRIZY", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string URLTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_AGENCIA_TRIZY", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string AgenciaTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_NAO_REALIZAR_INTEGRACAO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRealizarIntegracaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEixosPadrao", Column = "COI_QUANTIDADE_EIXO_PADRAO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEixosPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CNPJ_COMPANY_TRIZY", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CNPJCompanyTrizy { get; set; }

        [Obsolete("Migrado para a entidade IntegracaoTrizy.")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_ENVIO_CARGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEnvioCarga { get; set; }

        [Obsolete("Migrado para a entidade IntegracaoTrizy.")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ENVIO_CANCELAMENTO_CARGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEnvioCancelamentoCarga { get; set; }

        [Obsolete("Migrado para a entidade IntegracaoTrizy.")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ENVIO_EVENTOS_PATIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEnvioEventosPatio { get; set; }

        [Obsolete("Migrado para a entidade IntegracaoTrizy.")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_ENVIO_MS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TokenEnvioMS { get; set; }

        #endregion

        #region AX

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_AX", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoAX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_AX", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string URLAX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_AX_CONTRATO_FRETE", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string URLAXContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_AX_ORDEM_VENDA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string URLAXOrdemVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_AX_COMPENSACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string URLAXCompansacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_AX_PEDIDO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string URLAXPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_AX_COMPLEMENTO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string URLAXComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_AX_CANCELAMENTO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string URLAXCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_AX", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UsuarioAX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_AX", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string SenhaAX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CNPJ_AX", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CNPJAX { get; set; }

        #endregion

        #region Qbit

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_QBIT", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLQbit { get; set; }

        #endregion

        #region Ultragaz

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_ULTRAGAZ", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoUltragaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_AUTENTICACAO_ULTRAGAZ", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URLAutenticacaoUltragaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_ULTRAGAZ", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URLIntegracaoUltragaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CLIENT_SECRET_ULTRAGAZ", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ClientSecretUltragaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CLIENT_ID_ULTRAGAZ", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ClientIdUltragaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_CONTABILIZACAO_ULTRAGAZ", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URLContabilizacaoUltragaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_VEICULO_ULTRAGAZ", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URLIntegracaoVeiculoUltragaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_NAO_PERMITIR_REENVIAR_INTEGRACAO_PAGAMENTO_AG_RETORNO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirReenviarIntegracaoPagamentoAgRetorno { get; set; }

        #endregion

        #region SAD

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_SAD", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoSAD { get; set; }

        [Obsolete("Mudado para lista")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_SAD", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URLIntegracaoSADBuscarSenha { get; set; }

        [Obsolete("Mudado para lista")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_SAD_FINALIZAR_AGENDA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string URLIntegracaoSADFinalizarAgenda { get; set; }

        [Obsolete("Mudado para lista")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_SAD", Type = "StringClob", NotNull = false)]
        public virtual string TokenSAD { get; set; }

        #endregion

        #region Gadle

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_GADLE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoGadle { get; set; }

        #endregion

        #region Correios

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_CORREIOS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLCorreios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_CORREIOS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string UsuarioCorreios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_CORREIOS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaCorreios { get; set; }

        #endregion

        #region E-Millenium

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_EMILLENIUM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEmillenium { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_EMILLENIUM_CONFIRMAR_ENTREGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEmilleniumConfirmarEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_FRONT_DOOR", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string SenhaFrontDoor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_EMILLENIUM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string UsuarioEmillenium { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_EMILLENIUM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaEmillenium { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaIntegracaoEmillenium", Column = "COI_DATA_ULTIMA_INTEGRACAO_EMILLENIUM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaIntegracaoEmillenium { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TRANSID_EMILLENIUM", TypeType = typeof(int), NotNull = false)]
        public virtual int TransIdAtualEmillenium { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailsNotificacaoEmillenium", Column = "COI_EMAILS_NOTIFICACAO_EMILLENIUM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailsNotificacaoEmillenium { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeNotificacaoEmillenium", Column = "COI_QUANTIDADE_NOTIFICACAO_EMILLENIUM", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeNotificacaoEmillenium { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "COI_TRANSID_MASSIVO_EMILLENIUM", TypeType = typeof(int), NotNull = false)]
        //public virtual int TransIdInicioBuscaMassiva { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "COI_TRANSID_MASSIVO_FIM_EMILLENIUM", TypeType = typeof(int), NotNull = false)]
        //public virtual int TransIdFimBuscaMassiva { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalizacaoBuscaMassiva", Column = "COI_DATA_FINALIZACAO_INTEGRACAO_MASSIVA_EMILLENIUM", TypeType = typeof(DateTime), NotNull = false)]
        //public virtual DateTime? DataFinalizacaoBuscaMassiva { get; set; }

        #endregion

        #region Michelin

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_MICHELIN", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoMichelin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_MICHELIN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioMichelin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_MICHELIN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaMichelin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_MICHELIN_HOMOLOGACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLHomologacaoMichelin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_MICHELIN_PRODUCAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLProducaoMichelin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CODIGO_TRANSPORTADORA_MICHELIN", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CodigoTransportadoraMichelin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CNPJ_MICHELIN", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CnpjTransportadoraMichelin { get; set; }

        #endregion

        #region Telha Norte

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_TELHA_NORTE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLTelhaNorte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_PEDIDO_TELHA_NORTE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLPedidoTelhaNorte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_OBTER_TOKEN_TELHA_NORTE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLObterTokenTelhaNorte { get; set; }

        [Obsolete("Token gerado via requisição")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_OAUTH_CREDENTIAL_TELHA_NORTE", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string OAuthCredentialTelhaNorte { get; set; }

        #endregion

        #region Cadastros Multi

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_CADASTROS_MULTI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoDeCadastrosMulti { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_CADASTROS_MULTI", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string URLIntegracaoCadastrosMulti { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_INTEGRACAO_CADASTROS_MULTI", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string TokenIntegracaoCadastrosMulti { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_CADASTROS_MULTI_SECUNDARIO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string URLIntegracaoCadastrosMultiSecundario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_INTEGRACAO_CADASTROS_MULTI_SECUNDARIO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string TokenIntegracaoCadastrosMultiSecundario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZAR_INTEGRACAO_DE_PESSOA_PARA_PESSOA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoDePessoaParaPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZOU_INTEGRACAO_COMPLETA_DE_PESSOA_PARA_PESSOA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizouIntegracaoCompletaDePessoaParaPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZAR_INTEGRACAO_DE_TRANSPORTADOR_PARA_EMPRESA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoDeTransportadorParaEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZOU_INTEGRACAO_COMPLETA_DE_TRANSPORTADOR_PARA_EMPRESA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizouIntegracaoCompletaDeTransportadorParaEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZAR_INTEGRACAO_DE_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoDeContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZOU_INTEGRACAO_COMPLETA_DE_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizouIntegracaoCompletaDeContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZAR_INTEGRACAO_DE_NAVIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoDeNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZOU_INTEGRACAO_COMPLETA_DE_NAVIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizouIntegracaoCompletaDeNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZAR_INTEGRACAO_DE_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoDeViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZOU_INTEGRACAO_COMPLETA_DE_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizouIntegracaoCompletaDeViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZAR_INTEGRACAO_DE_CTE_ANTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoDeCTeAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZAR_INTEGRACAO_DE_CTE_PARA_COMPLEMENTO_OS_MAE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoDeCTeParaComplementoOSMae { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZAR_INTEGRACAO_DE_PORTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoDePorto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZAR_INTEGRACAO_DE_TIPO_DE_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoDeTipoDeContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZAR_INTEGRACAO_DE_TERMINAL_PORTUARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoDeTerminalPortuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_REALIZAR_INTEGRACAO_DE_PRODUTO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoDeProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_BUSCAR_CARGAS_SEM_INTEGRACOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarCargasSemIntegracoes { get; set; }

        #endregion

        #region Cadastros Totvs

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_TOTVS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoDeTotvs { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_TOVS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string URLIntegracaoTotvs { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_INTEGRACAO_TOTVS_PROCESS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string URLIntegracaoTotvsProcess { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_USUARIO_INTEGRACAO_TOVS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string UsuarioTotvs { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_SENHA_INTEGRACAO_TOVS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string SenhaTotvs { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_CONTEXTO_INTEGRACAO_TOVS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ContextoTotvs { get; set; }

        #endregion

        #region Cobasi

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_COBASI", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLCobasi { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_APIKEY_COBASI", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string APIKeyCobasi { get; set; }

        #endregion

        #region Onetrust

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_ONETRUST", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string URLOnetrust { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_ONETRUST_OBTER_TOKEN", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLObterTokenOnetrust { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_ONETRUST_REGULARIZACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UrlRegularizacaoOneTrust { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ONETRUST_PURPOSE_ID", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string PurposeIdOneTrust { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ONETRUST_CLIENT_ID", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ClientIdOneTrust { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_ONETRUST_CLIENT_SECRET", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ClientSecretOneTrust { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_APIKEY_ONETRUST", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string APIKeyOnetrust { get; set; }

        #endregion

        #region Sintegra

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_URL_SINTEGRA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLSintegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_TOKEN_SINTEGRA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TokenSintegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_INTERVALO_CONSULTA_SINTEGRA", TypeType = typeof(int), NotNull = false)]
        public virtual int IntervaloConsultaSintegra { get; set; }
        #endregion

        #region CTASmart

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_POSSUI_INTEGRACAO_CTASMART", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoCTASmart { get; set; }

        #endregion

    }
}
