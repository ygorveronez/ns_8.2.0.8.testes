using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracao
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_EMBARCADOR", EntityName = "ConfiguracaoEmbarcador", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoEmbarcador", NameType = typeof(ConfiguracaoTMS))]
    public class ConfiguracaoTMS : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração do Embarcador"; }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImportarValePedagioMDFECarga", Column = "CEM_IMPORTAR_VALE_PEDAGIO_MDFE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarValePedagioMDFECarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarSituacaoEnvioProgramadoIntegracaoCanhoto", Column = "CEM_VALIDAR_SITUACAO_ENVIO_PROGRAMADO_INTEGRACAO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarSituacaoEnvioProgramadoIntegracaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermiteEmitirCargaSemAverbacao", Column = "CEM_NAO_PERMITE_EMITIR_CARGA_SEM_AVERBACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermiteEmitirCargaSemAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermiteInformarValorMaiorTerceiroTabelaFrete", Column = "CEM_IMPEDIR_VALOR_SUPERIOR_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermiteInformarValorMaiorTerceiroTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarFuncionalidadeProjetoNFTP", Column = "CEM_HABILITAR_FUNCIONALIDADE_PROJETO_NFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarFuncionalidadeProjetoNFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularFreteInicioCarga", Column = "CEM_CALCULAR_FRETE_INICIO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularFreteInicioCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirConfirmacaoImpressaoME", Column = "CEM_PERMITIR_CONFIRMACAO_IMPRESSAO_ME", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirConfirmacaoImpressaoME { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControleComissaoPorTipoOperacao", Column = "CEM_CONTROLE_COMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControleComissaoPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCargaAposConfirmacaoImpressao", Column = "CEM_SITUACAO_CARGA_APOS_CONFIRMACAO_IMPRESSAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCargaAposConfirmacaoImpressao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCargaAposEmissaoDocumentos", Column = "CEM_SITUACAO_CARGA_APOS_APOS_EMISSAO_DOCUMENTOS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCargaAposEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCargaAposIntegracao", Column = "CEM_SITUACAO_CARGA_APOS_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCargaAposIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCargaAposFinalizacaoDaCarga", Column = "CEM_SITUACAO_CARGA_APOS_FINALIZACAO_CARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCargaAposFinalizacaoDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FiltrarBuscaVeiculosPorEmpresa", Column = "CEM_FILTRAR_BUSCA_VEICULOS_EMPRESA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarBuscaVeiculosPorEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PreencherMotoristaAutomaticamenteAoInformarVeiculo", Column = "CEM_PREENCHER_MOTORISTA_AUTOMATICAMENTE_AO_INFORMAR_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PreencherMotoristaAutomaticamenteAoInformarVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BLOQUEAR_CANCELAMENTO_CARGAS_COM_DATA_CARREGAMENTO_EDADOS_TRANSPORTE_INFORMADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearCancelamentoCargasComDataCarregamentoEDadosTransporteInformados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BuscarProdutoPredominanteNoPedido", Column = "CEM_BUSCAR_PRODUTO_PREDOMINANTE_NO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarProdutoPredominanteNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControlarCanhotosDasNFEs", Column = "CEM_CONTROLAR_CANHOTOS_NFES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlarCanhotosDasNFEs { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraMontarNumeroPedidoEmbarcadorWebService", Column = "CEM_REGRA_MONTAR_NUMERO_PEDIDO_EMBARCADOR_WEB_SERVICE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string RegraMontarNumeroPedidoEmbarcadorWebService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoProdutoPredominatePadrao", Column = "CEM_DESCRICAO_PRODUTO_PREDOMINANTE_PADRAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DescricaoProdutoPredominatePadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemPadraoInformarDadosTransporteJanelaCarregamentoTransportador", Column = "CEM_MENSAGEM_PADRAO_INFORMAR_DADOS_TRANSPORTE_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string MensagemPadraoInformarDadosTransporteJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirDisponibilizarCargaParaTransportador", Column = "CEM_PERMITIR_DISPONIBILIZAR_CARGA_PARA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirDisponibilizarCargaParaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirLiberarCargaSemNFe", Column = "CEM_PERMITIR_LIBERAR_CARGA_SEM_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirLiberarCargaSemNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlterarDataCarregamentoEDescarregamentoPorPeriodo", Column = "CEM_ALTERAR_DATA_CARREGAMENTO_E_DESCARREGAMENTO_POR_PERIODO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlterarDataCarregamentoEDescarregamentoPorPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirTrocarPedidoCarga", Column = "CEM_PERMITIR_TROCAR_PEDIDO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTrocarPedidoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirAdicionarPedidoOutraFilialCarga", Column = "CEM_PERMITIR_ADICIONAR_PEDIDO_OUTRA_FILIAL_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAdicionarPedidoOutraFilialCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirRemoverPedidoCargaComPendenciaDocumentos", Column = "CEM_PERMITIR_REMOVER_PEDIDO_CARGA_COM_PENDENCIA_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRemoverPedidoCargaComPendenciaDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirAtualizarInicioViagem", Column = "CEM_PERMITIR_ATUALIZAR_INICIO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAtualizarInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarSituacaoNaJanelaDescarregamento", Column = "CEM_UTILIZAR_SITUACAO_NA_JANELA_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarSituacaoNaJanelaDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirCargaSemValorFreteJanelaCarregamentoTransportador", Column = "CEM_EXIBIR_CARGA_SEM_VALOR_FRETE_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirCargaSemValorFreteJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirInformacoesAdicionaisChamado", Column = "CEM_EXIBIR_INFORMACOES_ADICIONAIS_CHAMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirInformacoesAdicionaisChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirNumeroCargaQuandoExistirCarregamento", Column = "CEM_EXIBIR_NUMERO_CARGA_QUANDO_EXISTIR_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirNumeroCargaQuandoExistirCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarNFeEmHomologacao", Column = "CEM_UTILIZAR_NFE_EM_HOMOLOGACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarNFeEmHomologacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarTempoCarregamentoPorPeriodo", Column = "CEM_UTILIZAR_TEMPO_CARREGAMENTO_POR_PERIODO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarTempoCarregamentoPorPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirInformarDadosTransportadorCargaEtapaNFe", Column = "CEM_PERMITIR_INFORMAR_DADOS_TRANSPORTADOR_CARGA_ETAPA_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarDadosTransportadorCargaEtapaNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirOperadorInformarValorFreteMaiorQueTabela", Column = "CEM_PERMITIR_OPERADOR_INFORMAR_VALOR_MAIOR_TABELA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirOperadorInformarValorFreteMaiorQueTabela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirRetornoAgNotasFiscais", Column = "CEM_PERMITIR_RETORNO_AG_NOTAS_FISCAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRetornoAgNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigatorioInformarDadosContratoFrete", Column = "CEM_OBRIGATORIO_INFORMAR_DADOS_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarDadosContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirKmUtilizadoContratoFretePorPeriodoVigenciaContrato", Column = "CEM_EXIBIR_KM_UTILIZADO_CONTRATO_FRETE_POR_PERIODO_VIGENCIA_CONTRATO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirKmUtilizadoContratoFretePorPeriodoVigenciaContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TIPO_FECHAMENTO_FRETE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFechamentoFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFechamentoFrete TipoFechamentoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirCancelamentoTotalCarga", Column = "CEM_PERMITIR_CANCELAMENTO_TOTAL_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirCancelamentoTotalCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirCancelamentoTotalCargaViaWebService", Column = "CEM_PERMITIR_CANCELAMENTO_TOTAL_CARGA_VIA_WEB_SERVICE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirCancelamentoTotalCargaViaWebService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigatorioInformarDataEnvioCanhoto", Column = "CEM_OBRIGATORIO_INFORMAR_DATA_ENVIO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarDataEnvioCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornarCanhotoParaPendenteAoReceberUmaNotaJaDigitalizada", Column = "CEM_RETORNAR_CANHOTO_PARA_PENDENTE_AO_RECEBER_UMA_NOTA_JA_DIGITALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarCanhotoParaPendenteAoReceberUmaNotaJaDigitalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoSegundosParaInicioEmissaoDocumentos", Column = "CEM_TEMPO_SEGUNDOS_PARA_INICIO_EMISSAO_DOCUMENTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoSegundosParaInicioEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativasConsultarCargasErroRoteirizacao", Column = "CEM_NUMERO_TENTATIVAS_CONSULTAR_CARGAS_ERRO_ROTEIRIZACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativasConsultarCargasErroRoteirizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoMinutosAguardarReconsultarCargasErroRoteirizacao", Column = "CEM_TEMPO_MINUTOS_AGUARDAR_RECONSULTAR_CARGAS_ERRO_ROTEIRIZACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoMinutosAguardarReconsultarCargasErroRoteirizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RaioMaximoGeoLocalidadeGeoCliente", Column = "CEM_RAIO_MAX_GEO_CLIENTE_GEO_LOCALIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int RaioMaximoGeoLocalidadeGeoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MaximoThreadsMontagemCarga", Column = "CEM_MAX_THREADS_MONTAGEM_CARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int MaximoThreadsMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_DESABILITAR_VEICULOS_INUTILIZADOS_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int DesabilitarVeiculosInutilizadosDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarIntegracaoPedido", Column = "CEM_UTILIZAR_INTEGRACAO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarIntegracaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarDadosCargaRelatorioPedido", Column = "CEM_UTILIZAR_DADOS_CARGA_RELATORIO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDadosCargaRelatorioPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SistemaIntegracaoPadraoCarga", Column = "CEM_SISTEM_INTEGRACAO_PADRAO_CARGAS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao SistemaIntegracaoPadraoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteAdicionarNotaManualmente", Column = "CEM_PERMITIR_ADCIONAR_NOTA_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAdicionarNotaManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarValorCargaAoAdicionarNFe", Column = "CEM_VALIDAR_VALOR_CARGA_AO_ADICIONAR_NFE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValidarValorCargaAoAdicionarNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicarIntegracaoNFe", Column = "CEM_INDICAR_INTEGRACAO_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicarIntegracaoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarPedagioBaseCalculoIcmsCteComplementarPorRegraEstado", Column = "CEM_UTILIZAR_PEDAGIO_BASE_CALCULO_ICMS_CTE_COMPLEMENTAR_POR_REGRA_ESTADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool UtilizarPedagioBaseCalculoIcmsCteComplementarPorRegraEstado { get; set; }

        /// <summary>
        /// Utilizar valor de descarga por Pallet e cliente.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarValorDescarga", Column = "CEM_UTILIZAR_VALOR_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarValorDescarga { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCFOPOcorrencia", Column = "CEM_CODIGO_CFOP_OCORRENCIA", TypeType = typeof(string), NotNull = false, Length = 6)]
        //public virtual string CodigoCFOPOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos", Column = "CEM_EXIGE_INFORMAR_CIENCIA_DO_ENVIO_DAS_NOTAS_ANTES_DE_EMITIR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirNotaFiscalParaCalcularFreteCarga", Column = "CEM_EXIGIR_NOTA_FISCAL_PARA_CALCULAR_FRETE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirNotaFiscalParaCalcularFreteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArmazenarXMLCTeEmArquivo", Column = "CEM_ARMAZENAR_XML_CTE_EM_ARQUIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ArmazenarXMLCTeEmArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCargaSequencialUnico", Column = "CEM_NUMERO_CARGA_SEQUENCIA_UNICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NumeroCargaSequencialUnico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarNumeroSequencialCargaNoCarregamento", Column = "CEM_UTILIZAR_NUMERO_SEQUENCIAL_CARGA_NO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarNumeroSequencialCargaNoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ManterOperacaoUnicaEmCargasAgrupadas", Column = "CEM_MANTER_OPERACAO_UNICA_EM_CARGAS_AGRUPADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ManterOperacaoUnicaEmCargasAgrupadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoIntramunicipal", Column = "CEM_TIPO_EMISSAO_INTRAMUNICIPAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal TipoEmissaoIntramunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRestricaoPalletModeloVeicularCarga", Column = "CEM_TIPO_RESTRICAO_PALLET_MODELO_VEICULAR_CARGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRestricaoPalletModeloVeicularCarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRestricaoPalletModeloVeicularCarga TipoRestricaoPalletModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RatearNumeroPalletsModeloVeiculoEntrePedidoPorPeso", Column = "CEM_RATEAR_NUMERO_PALLETS_MODELO_VEICULAR_ENTRE_PEDIDOS_POR_PESO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RatearNumeroPalletsModeloVeiculoEntrePedidoPorPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarDocumentosAutomaticamenteParaImpressao", Column = "CEM_ENVIAR_DOCUMENTOS_AUTOMATICAMENTE_PARA_IMPRESSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDocumentosAutomaticamenteParaImpressao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarMDFeAutomaticamenteParaImpressao", Column = "CEM_ENVIAR_MDFE_AUTOMATICAMENTE_PARA_IMPRESSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarMDFeAutomaticamenteParaImpressao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigatorioRegrasOcorrencia", Column = "CEM_OBRIGATORIO_REGRAS_OCORRENCIA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao ObrigatorioRegrasOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FiltrarAlcadasDoUsuario", Column = "CEM_FILTRAR_ALCADAS_USUARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarAlcadasDoUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoExecutarFecharCarga", Column = "CEM_NAO_EXECUTAR_FECHAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExecutarFecharCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarMesmoNumeroPreCargaGerarCargaViaImportacao", Column = "CEM_USAR_MESMO_NUMERO_PRE_CARGA_GERAR_CARGA_VIA_IMPORTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarMesmoNumeroPreCargaGerarCargaViaImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CancelarCargaExistenteAutomaticamenteNaImportacaoDePedido", Column = "CEM_CANCELAR_CARGA_EXISTENTE_NA_IMPORTACAO_DE_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CancelarCargaExistenteAutomaticamenteNaImportacaoDePedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCargaDeNotasRecebidasPorEmail", Column = "CEM_GERAR_CARGA_DE_NOTAS_RECEBIDAS_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaDeNotasRecebidasPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SempreDuplicarCargaCancelada", Column = "CEM_SEMPRE_DUPLICAR_CARGA_CANCELADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreDuplicarCargaCancelada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DefaultTrueDuplicarCarga", Column = "CEM_DEFAULT_TRUE_DUPLICAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DefaultTrueDuplicarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailsRetornoProblemaGerarCargaEmail", Column = "CEM_EMAIL_RETORNO_PROBLEMA_GERAR_CARGA", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string EmailsRetornoProblemaGerarCargaEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirChamadoParaAbrirOcorrencia", Column = "CEM_EXIGIR_CHAMADO_ABRIR_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirChamadoParaAbrirOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImprimirObservacaoPedidoMDFe", Column = "CEM_IMPRIMIR_OBSERVACAO_PEDIDO_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirObservacaoPedidoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_SALVAR_DADOS_TRANSPORTE_CARGA_SEM_SOLICITAR_NFES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSalvarDadosTransporteCargaSemSolicitarNFes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_SOLICITAR_NOTAS_FISCAIS_AO_SALVAR_DADOS_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarNotasFiscaisAoSalvarDadosTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_CARGA_TRAJETO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaTrajeto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_ALTERAR_CARGA_HORARIO_CARREGAMENTO_INFERIOR_ATUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarCargaHorarioCarregamentoInferiorAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_POSSUI_WMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiWMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeNumeroDeAprovadoresNasAlcadas", Column = "CEM_EXIGE_NUMERO_APROVADORES_ALCADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeNumeroDeAprovadoresNasAlcadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_DIAS_AVISO_VENCIMENTO_COTRATO_FRETE", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasAvisoVencimentoCotratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EMAILS_AVISO_VENCIMENTO_COTRATO_FRETE", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string EmailsAvisoVencimentoCotratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OBSERVACAO_CTE_PADRAO_EMBARCADOR", Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoCTePadraoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoMDFePadraoEmbarcador", Column = "CEM_OBSERVACAO_MDFE_PADRAO_EMBARCADOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ObservacaoMDFePadraoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TIPO_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratoFreteTerceiro), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratoFreteTerceiro TipoContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TIPO_CHAMADO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoChamado), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoChamado TipoChamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO_DOCUMENTOS_DESTINADOS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoasDocumentosDestinados { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteComplementoFechamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_COMPONENTE_DESCONTO_SEGURO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteDescontoSeguro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_COMPONENTE_DESCONTO_FILIAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteDescontoFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_TABELA_FRETE_NO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarTabelaFreteNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ENCERRAR_MDFE_DE_OUTRAS_VIAGENS_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncerrarMDFesDeOutrasViagensAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ENVIAR_EMAIL_ENCERRAMENTO_MDFE_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailEncerramentoMDFeTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TIPO_GERACAO_TITULO_FATURA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoTituloFatura), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoTituloFatura TipoGeracaoTituloFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CARGA_TRANSBORDO_NA_ETAPA_INICIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaTransbordoNaEtapaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_SEMPRE_BUSCA_CTE_POR_CHAVE_EM_INTEGRACAO_VIA_WS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreBuscaCTePorChaveEmIntegracaoViaWS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_DATA_LIBERACAO_SEGURADORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarDataLiberacaoSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_DATA_LIBERACAO_SEGURADORA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarDataLiberacaoSeguradoraVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_EMISSAO_CARGA_SOMENTE_COM_TRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteEmissaoCargaSomenteComTracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INFORMAR_DADOS_CHEGADA_VEICULO_NO_FLUXO_PATIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDadosChegadaVeiculoNoFluxoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_REMETENTE_PADRAO_IMPORTACAO_PLANILHA_PEDIDO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente RemetentePadraoImportacaoPlanilhaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_DESTINATARIO_PADRAO_IMPORTACAO_PLANILHA_PEDIDO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente DestinatarioPadraoImportacaoPlanilhaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCargaPadraoImportacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO_CARGA_DISTRIBUIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacaoPadraoCargaDistribuidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PRAZO_SOLICITACAO_OCORRENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoSolicitacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_SELECIONAR_QUALQUER_NATUREZA_NF_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteSelecionarQualquerNaturezaNFEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_IGNORAR_TIPO_CONTRATO_NO_CONTRATO_FRETE_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IgnorarTipoContratoNoContratoFreteTransportador { get; set; }

        //criado temporariamente para e-commerce carrefour
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_CPF_RANDOMICAMENTE_DESTINATARIO_IMPORTACAO_PLANILHA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCPFRandomicamenteDestinatarioImportacaoPlanilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CHAMADO_OCORRENCIA_USA_REMETENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChamadoOcorrenciaUsaRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PADRAO_ARMAZENAMENTO_FISICO_CANHOTO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PadraoArmazenamentoFisicoCanhotoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NOTA_UNICA_EM_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotaUnicaEmCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AVERBAR_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbarRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AVERBAR_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbarSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_CODIGO_INTEGRACAO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirCodigoIntegracaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INCLUIR_ICMS_FRETE_INFORMADO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirICMSFreteInformadoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoComponentePadraoCTe", Column = "CEM_DESCRICAO_COMPONENTE_PADRAO_CTE", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string DescricaoComponentePadraoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoComponenteImpostoCTe", Column = "CEM_DESCRICAO_COMPONENTE_IMPOSTO_CTE", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string DescricaoComponenteImpostoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCasasDecimaisQuantidadeProduto", Column = "CEM_NUMERO_CASAS_DECIMAIS_QUANTIDADE_PRODUTO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroCasasDecimaisQuantidadeProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_MINUTOS_TOLERANCIA_PREVISAO_CHEGADA_DOCA_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int MinutosToleranciaPrevisaoChegadaDocaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PRAZO_CANCELAMENTO_TRANSFERENCIA_PALLETS", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoCancelamentoTransferenciaPallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ACERTO_DE_VIAGEM_COM_DIARIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AcertoDeViagemComDiaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ACERTO_DE_VIAGEM_IMPRESSAO_DETALHADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AcertoDeViagemImpressaoDetalhada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_USAR_VALOR_CTE_ANTERIOR_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarValorCTeAnteriorSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_USAR_IMPOSTOS_INTEGRACAO_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarImpostosIntegracaoSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_FATOR_METRO_CUBICO_PRODUTO_EMBARCADOR_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int FatorMetroCubicoProdutoEmbarcadorIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_METRO_CUBICO_POR_UNIDADE_PEDIDO_PRODUTO_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MetroCubicoPorUnidadePedidoProdutoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_METRAGEM_POR_CAIXA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CubagemPorCaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalizarViagemAnteriorAoEntrarFilaCarregamento", Column = "CEM_FINALIZAR_VIAGEM_ANTERIOR_AO_ENTRAR_FILA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarViagemAnteriorAoEntrarFilaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarFilaCarregamento", Column = "CEM_UTILIZAR_FILA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarFilaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarFilaCarregamentoReversa", Column = "CEM_UTILIZAR_FILA_CARREGAMENTO_REVERSA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarFilaCarregamentoReversa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MarcacaoFilaCarregamentoSomentePorVeiculo", Column = "CEM_MARCACAO_FILA_CARREGAMENTO_SOMENTE_POR_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MarcacaoFilaCarregamentoSomentePorVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirNumeroPedidoJanelaCarregamentoEDescarregamento", Column = "CEM_EXIBIR_NUMERO_PEDIDO_JANELA_CARREGAMENTO_E_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirNumeroPedidoJanelaCarregamentoEDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarPreCargaJanelaCarregamento", Column = "CEM_UTILIZAR_PRE_CARGA_JANELA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPreCargaJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirInformarTipoTransportadorPorDataCarregamentoJanelaCarregamento", Column = "CEM_PERMITIR_INFORMAR_TIPO_TRANSPORTADOR_POR_DATA_CARREGAMENTO_JANELA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarTipoTransportadorPorDataCarregamentoJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirImportarAlteracaoDataCarregamentoJanelaCarregamento", Column = "CEM_PERMITIR_IMPORTAR_ALTERACAO_DATA_CARREGAMENTO_JANELA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirImportarAlteracaoDataCarregamentoJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarDataCarregamentoDaJanelaCarregamentoAoSetarTransportadorPrioritarioPorRotaCarga", Column = "CEM_UTILIZAR_DATA_CARREGAMENTO_DA_JANELA_CARREGAMENTO_AO_SETAR_TRANSPORTADOR_PRIORITARIO_POR_ROTA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataCarregamentoDaJanelaCarregamentoAoSetarTransportadorPrioritarioPorRotaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarSenhaAgendamento", Column = "CEM_NAO_GERAR_SENHA_AGENDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarSenhaAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TrocarPreCargaPorCarga", Column = "CEM_TROCAR_PRE_CARGA_POR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TrocarPreCargaPorCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarProtocoloDaPreCargaNaCarga", Column = "CEM_UTILIZAR_PROTOCOLO_DA_PRE_CARGA_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarProtocoloDaPreCargaNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoExigeInformarDisponibilidadeDeVeiculo", Column = "CEM_NAO_EXIGE_INFORMAR_DISPONIBILIDADE_DE_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExigeInformarDisponibilidadeDeVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirDesagendarCargaJanelaCarregamento", Column = "CEM_PERMITIR_DESAGENDAR_CARGA_JANELA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirDesagendarCargaJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarConjuntoVeiculoPermiteEntrarFilaCarregamentoMobile", Column = "CEM_VALIDAR_CONJUNTO_VEICULO_PERMITE_ENTRAR_FILA_CARREGAMENTO_MOBILE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarConjuntoVeiculoPermiteEntrarFilaCarregamentoMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirResumoFilaCarregamentoSomentePorModeloVeicularCarga", Column = "CEM_EXIBIR_RESUMO_FILA_CARREGAMENTO_SOMENTE_POR_MODELO_VEICULAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirResumoFilaCarregamentoSomentePorModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_EMPRESA_ADITIVO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ClienteContratoAditivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AVERBAR_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbarMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_GERAR_NOTA_MESMO_PEDIDO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirGerarNotaMesmoPedidoCarga { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_ENVIAR_MESMA_NOTA_MULTIPLOS_PEDIDO_CARGA", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool PermiteEnviarMesmaNotaMultiplosPedidosCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_GERAR_NOTA_MESMA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirGerarNotaMesmaCarga { get; set; }

        [Obsolete("Criado para o fluxo de entrega que está desativado")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HABILITAR_FLUXO_ENTREGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarFluxoEntregas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_NUMERO_NOTA_FLUXO_ENTREGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarNumeroNotaFluxoEntregas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_APROVADORES_OCORRENCIA_PORTAL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirAprovadoresOcorrenciaPortalTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirAutomatizarPagamentoTransportador", Column = "CEM_PERMITIR_AUTOMATIZAR_PAGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAutomatizarPagamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_DIA_SEMANA_NOTIFICAR_CANHOTOS_PENDENTES", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DiaSemana DiaSemanaNotificarCanhotosPendentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_CONTROLE_PALLETS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarControlePallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_SITUACAO_JANELA_CARREGAMENTO_PADRAO_PESQUISA", TypeType = typeof(int), NotNull = false)]
        public virtual int? SituacaoJanelaCarregamentoPadraoPesquisa { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposIntegracaoValidarMotorista", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_EMBARCADOR_TIPO_INTEGRACAO_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoIntegracao", Column = "TPI_CODIGO")]
        public virtual ICollection<Cargas.TipoIntegracao> TiposIntegracaoValidarMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposIntegracaoValidarVeiculo", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_EMBARCADOR_TIPO_INTEGRACAO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoIntegracao", Column = "TPI_CODIGO")]
        public virtual ICollection<Cargas.TipoIntegracao> TiposIntegracaoValidarVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PROVISIONAR_DOCUMENTOS_EMITIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProvisionarDocumentosEmitidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_ROTA_PARA_EMISSAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirRotaParaEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_TABELA_FRETE_COM_DATA_ATUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarTabelaFreteComDataAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TIPO_PAGAMENTO_CONTRATO_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoContratoFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoContratoFrete TipoPagamentoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CADASTRAR_MOTORISTA_MOBILE_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarMotoristaMobileAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CADASTRAR_VEICULO_TERCEIRO_PARA_EMPRESA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarVeiculoTerceiroParaEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCargaAposLiberarParaFaturamento", Column = "CEM_SITUACAO_CARGA_APOS_LIBERAR_FATURAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCargaAposLiberarParaFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ENCERRAR_CARGA_QUANDO_FINALIZAR_GESTAO_PATIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncerrarCargaQuandoFinalizarGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirCFOPCompra", Column = "CEM_EXIBIR_CFOP_COMPRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirCFOPCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TIPO_GERACAO_TITULO_CONTRATO_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoTituloContratoFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoTituloContratoFrete TipoGeracaoTituloContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_FLUXO_DE_PATIO_COMO_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FluxoDePatioComoMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_USAR_CONTRATO_FRETE_ADITIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarContratoFreteAditivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMontagemCargaPadrao", Column = "CEM_TIPO_MONTAGEM_CARGARREGAMENTO_PADRAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga TipoMontagemCargaPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFiltroDataMontagemCarga", Column = "CEM_TIPO_FILTRO_DATA_MONTAGEM_CARGARREGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFiltroDataMontagemCarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFiltroDataMontagemCarga TipoFiltroDataMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BLOQUEAR_EMISSAO_CONTRATO_FRETE_ZERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearEmissaoComContratoFreteZerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NOTIFICAR_CANHOTOS_PENDENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarCanhotosPendentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGE_APROVACAO_DIGITACALIZACAO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeAprovacaoDigitalizacaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BAIXAR_CANHOTO_APOS_APROVACAO_DIGITALIZACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BaixarCanhotoAposAprovacaoDigitalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_ADICIONAR_CARGA_FLUXO_PATIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermitirAdicionarCargaFluxoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_EXIBIR_VARIACAO_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirVariacaoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_TOLERANCIA_PESO_MODELO_VEICULAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarToleranciaPesoModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_DACTE_OUTROS_DOCUMENTOS_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarDACTEOutrosDocumentosAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularFreteFilialEmissoraPorTabelaDeFrete", Column = "CEM_CALCULAR_FRETE_FILIAL_EMISSORA_POR_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularFreteFilialEmissoraPorTabelaDeFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarFreteFilialEmissoraEmbarcador", Column = "CEM_FRETE_FILIAL_EMISSORA_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarFreteFilialEmissoraEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_CARGA_DE_CTES_RECEBIDOS_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaDeCTesRecebidosPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_USAR_NUMERO_CARGA_NUMERO_CANHOTO_AVULSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarNumeroCargaParaNumeroCanhotoAvulso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_DATAS_VALIDADE_CADASTRO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirDatasValidadeCadastroMotorista { get; set; }

        /// <summary>
        /// A carga não pode ser emitida caso o valor do frete líquido esteja zerado.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_PERMITIR_VALOR_FRETE_LIQUIDO_ZERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirValorFreteLiquidoZerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_EMITIR_CARGA_COM_VALOR_ZERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEmitirCargaComValorZerado { get; set; }

        /// <summary>
        /// Ao ajustar os participantes da carga (expedidor/recebedor) deve selecionar automaticamente a empresa emissora dos conhecimentos.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ALTERAR_EMPRESA_EMISSORA_AO_AJUSTAR_PARTICIPANTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlterarEmpresaEmissoraAoAjustarParticipantes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_ADICIONAR_PRE_CARGA_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAdicionarPreCargaManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_DADOS_TRANSPORTE_OBRIGATORIO_PRE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DadosTransporteObrigatorioPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TRANSPORTADOR_OBRIGATORIO_PRE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TransportadorObrigatorioPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_LOCAL_CARREGAMENTO_OBRIGATORIO_PRE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LocalCarregamentoObrigatorioPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_NUMERO_PRE_CARGA_POR_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarNumeroPreCargaPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_REPLICAR_AJUSTE_TABELA_FRETE_TODAS_TABELAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReplicarAjusteTabelaFreteTodasTabelas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_REALIZAR_INTEGRACAO_GERENCIADORA_EM_HOMOLOGACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoGerenciadoraEmHomologacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_FRONTEIRA_OBRIGATORIA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FronteiraObrigatoriaMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TIPO_CARGA_OBRIGATORIO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoCargaObrigatorioMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INFORMAR_PERIODO_CARREGAMENTO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarPeriodoCarregamentoMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_DISTANCIA_ROTEIRIZACAO_CARREGAMENTO_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDistanciaRoteirizacaoCarregamentoNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TIPO_OPERACAO_OBRIGATORIO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoOperacaoObrigatorioMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_TIPOS_OPERACOES_DISTINTAS_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTiposOperacoesDistintasMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TRANSPORTADOR_OBRIGATORIO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TransportadorObrigatorioMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_SIMULACAO_FRETE_OBRIGATORIO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SimulacaoFreteObrigatorioMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ROTEIRIZACAO_OBRIGATORIA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RoteirizacaoObrigatoriaMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INFORMA_HORARIO_CARREGAMENTO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformaHorarioCarregamentoMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OCULTA_GERAR_CARREGAMENTOS_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultaGerarCarregamentosMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_LIMPAR_TELA_AO_SALVAR_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LimparTelaAoSalvarMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_PERMIIR_VINCULAR_MOTORISTA_EM_VARIOS_VEICULOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirVincularMotoristaEmVariosVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HABILITAR_RELATORIO_DE_TROCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarRelatorioDeTroca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_LIBERAR_PEDIDOS_PARA_MONTAGEM_CARGA_CANCELADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarPedidosParaMontagemCargaCancelada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZA_CHAT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaChat { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OBRIGAR_VIGENCIA_NO_AJUSTE_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarVigenciaNoAjusteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCompetenciaDocumentoEntrada", Column = "CEM_DATA_COMPETENCIA_DOCUMENTO_ENTRADA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada DataCompetenciaDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntradaDocumentoEntrada", Column = "CEM_DATA_ENTRADA_DOCUMENTO_ENTRADA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada DataEntradaDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_COMPONENTES_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarComponentesCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_DESATIVAR_MULTIPLOS_MOTORISTAS_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesativarMultiplosMotoristasMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ATIVAR_NOVA_DEFINICAO_TOMADOR_PARA_CARGAS_FEEDER_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarNovaDefinicaoDoTomadorParaCargasFeederMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AuditarConsultasWebService", Column = "CEM_AUDITAR_CONSULTAS_WEB_SERVICE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AuditarConsultasWebService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornosDuplicidadeWSSubstituirPorSucesso", Column = "CEM_RETORNOS_DUPLICIDADES_WS_POR_SUCESSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornosDuplicidadeWSSubstituirPorSucesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornarFalhaAdicionarCargaSeExistirCancelamentoCargaEmAberto", Column = "CEM_RETORNAR_FALHA_ADICIONAR_CARGA_SE_EXISTIR_CANCELAMENTO_EM_ABERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarFalhaAdicionarCargaSeExistirCancelamentoCargaEmAberto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OCULTAR_BUSCA_ROTA_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarBuscaRotaNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_SITUACAO_CARGA_JANELA_LIBERADA_FATURAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamento SituacaoCargaJanelaLiberarFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_VALIDAR_DATA_CANCELAMENTO_BAIXA_TITULO_RECEBER_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarDataCancelamentoTituloNaBaixaTituloReceberPorCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OBRIGATORIO_GERACAO_BLOCOS_PARA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioGeracaoBlocosParaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ATUALIZAR_PRODUTOS_PEDIDO_POR_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarProdutosPedidoPorIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ATUALIZAR_PEDIDO_POR_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarPedidoPorIntegracao { get; set; }

        [Obsolete("Criado para o fluxo de entrega que está desativado")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ENVIAR_EMAIL_FLUXO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailFluxoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INFORMAR_CIENCIA_OPERACAO_DOCUMENTO_DESTINADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarCienciaOperacaoDocumentoDestinado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_ALCADA_APROVACAO_ALTERACAO_VALOR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarAlcadaAprovacaoAlteracaoValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_ALCADA_APROVACAO_VALOR_TABELA_FRETE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarAlcadaAprovacaoValorTabelaFreteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_ALCADA_APROVACAO_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarAlcadaAprovacaoTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_ALCADA_APROVACAO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarAlcadaAprovacaoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_ALCADA_APROVACAO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarAlcadaAprovacaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_SITUACAO_AJUSTE_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirSituacaoAjusteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_ALCADA_APROVACAO_LIBERACAO_ESCRITURACAO_PAGAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarAlcadaAprovacaoLiberacaoEscrituracaoPagamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_ALCADA_APROVACAO_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarAlcadaAprovacaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_ALCADA_APROVACAO_ALTERACAO_REGRA_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarAlcadaAprovacaoAlteracaoRegraICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CST_CTE_SUBCONTRATACAO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CSTCTeSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_VALORES_PEDIDOS_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirValoresPedidosNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_SERVICO_PENDENTE_VEICULO_EXECUCAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarServicoPendenteVeiculoExecucaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BLOQUEAR_CAMPOS_TRANSPORTADOR_QUANDO_ETAPA_NOTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearCamposTransportadorQuandoEtapaNotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_EXIBIR_NOTAS_FISCAIS_NA_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirNotasFiscaisNaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_EXIBIR_TITULOS_NA_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirTitulosNaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_ROTA_ROTERIZADA_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirRotaRoteirizadaNaCarga { get; set; }

        /// <summary>
        /// Configuração para não considerar a roteirização do carregamneto na carga quando informado uma Rota de frete no carregamento.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_SUBSTITUIR_ROTEIRIZACAO_CARREGAMENTO_POR_ROTEIRIZACAO_ROTA_FRETE_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDescricaoRota", Column = "CEM_TIPO_DESCRIACAO_ROTA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota TipoDescricaoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INTPFAR_LIMITE_LINHAS_ARQUIVO_EDI", TypeType = typeof(int), NotNull = false)]
        public virtual int INTPFAR_LimiteLinhasArquivoEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INTPFAR_LINHAS_NECESSARIAS_OUTRAS_INFORMACOES", TypeType = typeof(int), NotNull = false)]
        public virtual int INTPFAR_LinhasNecessariasOutrasInformacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INTPFAR_NUMERO_LINHAS_FERADAS_POR_CTE", TypeType = typeof(int), NotNull = false)]
        public virtual int INTPFAR_NumeroLinhasFeradasPorCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TEMPO_PADRAO_TERMINO_CARREGAMENTO_PARA_VALIDAR_DISPONIBILIDADE_DESCARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCargaAcertoViagem", Column = "CEM_SITUACAO_CARGA_ACERTO_VIAGEM", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string SituacaoCargaAcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_VALIDAR_DATA_CANCELAMENTO_FECHAMENTO_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarDataCancelamentoTituloNoFechamentoDaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CALCULAR_FRETE_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularFreteCargaJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_DISPONIBILIZAR_CARGA_AUTOMATICAMENTE_PARA_TRANSPORTADOR_COM_MENOR_VALOR_FRETE_TABELA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ARMAZENAMENTO_CANHOTO_COM_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ArmazenamentoCanhotoComFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_CONTROLAR_KM_LANCADO_NO_DOCUMENTO_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoControlarKMLancadoNoDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_LANCAR_DOCUMENTO_ENTRADA_ABERTO_SE_KM_ESTIVER_ERRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancarDocumentoEntradaAbertoSeKMEstiverErrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VISUALIZAR_TODOS_ITENS_ORDEM_COMPRA_DOCUMENTO_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarTodosItensOrdemCompraDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_QUANTIDADE_REGISTROS_GRID_DOCUMENTO_ENTRADA", TypeType = typeof(int), NotNull = false)]
        public virtual int? QuantidadeRegistrosGridDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_LIBERAR_SELECAO_QUALQUER_VEICULO_JANELA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarSelecaoQualquerVeiculoJanelaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_LIBERAR_SELECAO_QUALQUER_VEICULO_JANELA_TRANSPORTADOR_COM_CONFIRMACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarSelecaoQualquerVeiculoJanelaTransportadorComConfirmacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BLOQUEAR_VEICULOS_COM_MDFE_EM_ABERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearVeiculosComMdfeEmAberto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_TRANSPORTADOR_ALTERAR_MODELO_VEICULAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransportadorAlterarModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_PERMITIR_EXCLUSAO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoPermitirExclusaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_VALOR_DETALHADO_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirValorDetalhadoJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_LIMITE_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirLimiteCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_PREVISAO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPrevisaoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_DISPONIBILIDADE_FROTA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDisponibilidadeFrotaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_INFORMAR_LACRE_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarLacreJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_REJEITAR_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRejeitarCargaJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_VALOR_MAXIMO_PENDENTE_PAGAMENTO_EXECUCAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ValidarValorMaximoPendentePagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OBRIGAR_MOTIVO_SOLICITACAO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarMotivoSolicitacaoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERCENTUAL_IMPOSTO_FEDERAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualImpostoFederal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_RETORNAR_CARGAS_PENDENTES_INTEGRACAO_SOMENTE_PARA_INTEGRADORA_NOTAS_FISCAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarCargasPentendesIntegracaoSomenteParaIntegradoraNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_CATEGORIA_CADASTRO_PESSOA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirCategoriaCadastroPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_POSSUI_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimentoPagamentoMotorista", Column = "CEM_TIPO_MOVIMENTO_PAGAMENTO_MOTORISTA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade TipoMovimentoPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimentoReversaoPagamentoMotorista", Column = "CEM_TIPO_MOVIMENTO_REVERSAO_PAGAMENTO_MOTORISTA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade TipoMovimentoReversaoPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_GERAR_CONTRATO_FRETE_PARA_CTE_EMITIDO_NO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarContratoFreteParaCTeEmitidoNoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TIPO_ORDEM_SERVICO_OBRIGATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoOrdemServicoObrigatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_CONTROLE_JORNADA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarControleJornadaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HABILITAR_FICHA_MOTORISTA_TODOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarFichaMotoristaTodos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_COMISSAO_POR_CARGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarComissaoPorCargo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoEmitirDocumentoNasCargas", Column = "CEM_NAO_EMITIR_DOCUMENTOS_NAS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEmitirDocumentoNasCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_JORNADA_DIARIA_MOTORISTA", TypeType = typeof(System.TimeSpan), NotNull = false)]
        public virtual System.TimeSpan? JornadaDiariaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_BAIXAR_CANHOTO_APENAS_COM_OCORRENCIA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteBaixarCanhotoApenasComOcorrenciaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZA_BUSCAR_ROTA_FRETE_MANUAL_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarBuscaRotaFreteManualCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_REPLICAR_CADASTRO_VEICULO_INTEGRACAO_TRANSPORTADOR_DIFERENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReplicarCadastroVeiculoIntegracaoTransportadorDiferente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CAMPOS_SECUNDARIOS_OBRIGATORIOS_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CamposSecundariosObrigatoriosPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_EXIBIR_PEDIDO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPedidoDeColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CAMPOS_SECUNDARIOS_OBRIGATORIOS_ORDEM_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CamposSecundariosObrigatoriosOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_IMPORTAR_CARGAS_MULTIEMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarCargasMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_FORCAR_FILTRO_MODELO_NA_CONSULTA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ForcarFiltroModeloNaConsultaVeiculo { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_PLANO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool UtilizarPlanoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HABILITAR_RELATORIO_BOLETIM_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarRelatorioBoletimViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HABILITAR_RELATORIO_DIARIO_BORDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarRelatorioDiarioBordo { get; set; }

        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_DATA_NFE_EMISSAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataNFeEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_EXIBIR_OPCAO_PARA_DENEGAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirOpcaoParaDelegar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_PERMITIR_DELEGAR_AO_USUARIO_LOGADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirDelegarAoUsuarioLogado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_FILTRAR_POR_PEDIDO_SEM_CARREGAMENTO_NA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarPorPedidoSemCarregamentoNaMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INFORMAR_APOLICE_SEGURO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformaApoliceSeguroMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INFORMAR_TIPO_CONDICAO_PAGAMENTO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarTipoCondicaoPagamentoMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_SEMPRE_INSERIR_NOVO_PRODUTO_POR_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreInserirNovoProdutoPorIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ATUALIZAR_PRODUTOS_CARREGAMENTO_POR_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarProdutosCarregamentoPorNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_CARGA_ROTEIRIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirCargaRoteirizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarVeiculoVinculadoContratoDeFrete", Column = "CEM_VALIDAR_VEICULO_VINCULADO_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarVeiculoVinculadoContratoDeFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoMinutosParaReenviarCancelamento", Column = "CEM_MINUTOS_REENVIO_CANCELAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoMinutosParaReenviarCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MaxDownloadsPorVez", Column = "CEM_MAX_DOWNLOADS_POR_VEZ", TypeType = typeof(int), NotNull = false)]
        public virtual int MaxDownloadsPorVez { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_CONTRATO_PRESTACAO_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarContratoPrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_ALTERAR_LACRES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarLacres { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_TIPO_LACRE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirTipoLacre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_UTILIZAR_USUARIO_TRANSPORTADOR_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarUsuarioTransportadorTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGE_PERFIL_USUARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigePerfilUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_PESO_EMBALAGEM_PRODUTO_PARA_RATEIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPesoEmbalagemProdutoParaRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_COMPARAR_TABELA_FRETE_PARA_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CompararTabelasDeFreteParaCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_USAR_PESO_PRODUTO_SUMARIZACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarPesoProdutoSumarizacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CADASTRAR_ROTA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarRotaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OBRIGAR_SELECAO_ROTA_QUANDO_EXISTIR_MULTIPLAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarSelecaoRotaQuandoExistirMultiplas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ABRIR_RATEIO_DESPESA_VEICULO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AbrirRateioDespesaVeiculoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_DESTINATARIO_PEDIDO_DIFERENTE_PRE_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValidarDestinatarioPedidoDiferentePreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_CONTROLE_VEICULO_EM_PATIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarControleVeiculoEmPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_RATEAR_FRETE_PEDIDOS_APOS_LIBERAR_EMISSAO_SEM_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RatearFretePedidosAposLiberarEmissaoSemNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_UTILIZAR_DEFAULT_PARA_PAGAMENTO_DE_TRIBUTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarDeafultParaPagamentoDeTributos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AVANCAR_CARGA_AO_RECEBER_NOTAS_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvancarCargaAoReceberNotasPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoUltimoPontoRoteirizacao", Column = "CEM_TIPO_ULTIMO_PONTO_ROTEIRIZACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao TipoUltimoPontoRoteirizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NUMERO_TENTATIVAS_REENVIO_ROTA_FRETE", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativasReenvioRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NUMERO_TENTATIVAS_REENVIO_CTE_REJEITADO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativasReenvioCteRejeitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BLOQUEAR_DATAS_RETROATIVAS_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearDatasRetroativasPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_INFORMAR_DATA_RETIRADA_CTRN_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarDataRetiradaCtrnCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_INFORMAR_NUMERO_CONTAINER_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarNumeroContainerCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_INFORMAR_TARA_CONTAINER_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarTaraContainerCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_INFORMAR_MAX_GROSS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarMaxGrossCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_INFORMAR_ANEXO_CONTAINER_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarAnexoContainerCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_INFORMAR_DATAS_CARREGAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarDatasCarregamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ROTEIRIZAR_POR_CIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RoteirizarPorCidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_FAIXA_TEMPERATURA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirFaixaTemperaturaNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_LANCAR_DESCONTOS_DAS_OCORRENCIAS_NO_ACERTO_DE_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZA_RATEIO_IMPOSTOS_PEDIDO_AGRUPADO", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool UtilizaRateioImpostosPedidoAgrupado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_RATEAR_VALOR_OCORRENCIA_PELO_VALOR_FRETE_CTE_ORIGINAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RatearValorOcorrenciaPeloValorFreteCTeOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_ESPECIE_DOCUMENTO_CTE_COMPLEMENTAR_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirEspecieDocumentoCteComplementarOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_SOMAR_DISTANCIA_PEDIDOS_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarDistanciaPedidosIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BLOQUEAR_BAIXA_PARCIAL_PARCELAMENTO_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearBaixaParcialOuParcelamentoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_VALIDAR_DADOS_BANCARIOS_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarDadosBancariosContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AJUSTAR_TIPO_OPERACAO_PELO_PESO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AjustarTipoOperacaoPeloPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_FECHAR_CARGA_POR_THREAD", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FecharCargaPorThread { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_PERMITIR_CANCELAR_CARGA_COM_INICIO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirCancelarCargaComInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_PDF_CTE_CANCELADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPDFCTeCancelado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_FILTRAR_CARGAS_POR_PARTE_DO_NUMERO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarCargasPorParteDoNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_ATUALIZAR_PESO_DO_PEDIDO_PELA_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAtualizarPesoPedidoPelaNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BUSCAR_CLIENTES_CADASTRADOS_INTEGRACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarClientesCadastradosNaIntegracaoDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_PRODUTOS_DIVERSOS_INTEGRACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarProdutosDiversosNaIntegracaoDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_EXPORTACAO_RELATORIO_CSV", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarExportacaoRelatorioCSV { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXPORTAR_CNPJ_E_CHAVE_DE_ACESSO_FORMATADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExportarCNPJEChaveDeAcessoFormatado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INFORMACAO_ADICIONAL_MOTORISTA_ORDEM_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformacaoAdicionalMotoristaOrdemColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_CONTROLE_HIGIENIZACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarControleHigienizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_PEDIDO_IMPORTACAO_NOTFIS_ETAPA_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPedidoImportacaoNotfisEtapaNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AGRUPAR_NOTAS_PEDIDOS_VALORES_ZERADOS_FECHAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparNotasPedidosValoresZeradosFechamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZA_EMISSAO_MULTIMODAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaEmissaoMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_NAO_EXIGE_ACEITE_TRANSPORTADOR_PARA_NOTA_DE_DEBITO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExigeAceiteTransportadorParaNFDebito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_EXIBIR_ASSOCIACAO_CLIENTES_NOS_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirAssociacaoClientesNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_EMAIL_PRINCIPAL_CADASTRO_PESSOA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirEmailPrincipalCadastroPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_EMAIL_PRINCIPAL_CADASTRO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirEmailPrincipalCadastroTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_NUMERO_PEDIDO_EMBARCADOR_NAO_INFORMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAutomaticamenteNumeroPedidoEmbarcardorNaoInformado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_INFORMAR_NOTAS_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformarNotasFiscaisNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_GERAR_CARREGAMENTO_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarCarregamentoRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PADRAO_PARA_INCLUSAO_ISS_DESMARCADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PadraoInclusaiISSDesmarcado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CONTROLE_ENTREGA_VISAO_PREVISAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControleEntregaVisaoPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_CONTATO_WHATS_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirContatoWhatsApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HABILITAR_WIDGET_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarWidgetAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_FILTRAR_WIDGET_ATENDIMENTO_POR_FILTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarWidgetAtendimentoProFiltro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_REMOVER_REENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteRemoverReentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_ENVIAR_NUMERO_PEDIDO_EMBARCADOR_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirEnviarNumeroPedidoEmbarcadorViaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NOTIFICAR_ALTERACAO_CARGA_AO_OPERADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarAlteracaoCargaAoOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_SEQUENCIA_NUMERACAO_CARGA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarSequenciaNumeracaoCargasViaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_APP_UTILIZA_CONTROLE_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AppUtilizaControleColetaEntrega { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_MUNICIPIO_DIFERENTE_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarMunicipioDiferentePedido { get; set; }

        [Obsolete("Migrado a informação para o enumerador TipoImpressaoFatura")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_LAYOUT_FINANCEIRO_MULTIMODAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarLayoutFinanceiroMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_PERCENTUAL_EM_RELACAO_VALOR_FRETE_LIQUIDO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPercentualEmRelacaoValorFreteLiquidoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImportarPedidoDeixarCargaPendente", Column = "CEM_PEDIDO_CARGA_PENDENTE ", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarPedidoDeixarCargaPendente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarRegraICMSParaDescontarValorICMS", Column = "CEM_UTILIZAR_REGRA_ICMS_PARA_DESCONTAR_VALOR_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarRegraICMSParaDescontarValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_PERMITIR_IMPRESSAO_CONTRATO_FRETE_PENDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirImpressaoContratoFretePendente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarSomenteFreteLiquidoNaImportacaoCTe", Column = "CEM_VALIDAR_SOMENTE_FRETE_LIQUIDO_NA_IMPORTACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarSomenteFreteLiquidoNaImportacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarPorRaizDoTransportadorNaImportacaoCTe", Column = "CEM_VALIDAR_POR_RAIZ_DO_TRANSPORTADOR_IMPORTACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarPorRaizDoTransportadorNaImportacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoValidarDadosParticipantesNaImportacaoCTe", Column = "CEM_NAO_VALIDAR_DADOS_PARTICIPANTES_IMPORTACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarDadosParticipantesNaImportacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCIOTDescontadoTerceiros", Column = "CEM_PERCENTUAL_CIOT_DESCONTO_TERCEIRO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PercentualCIOTDescontadoTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_QUANTIDADE_MAXIMA_DIAS_RELATORIOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeMaximaDiasRelatorios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_SELECIONAR_ROTA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteSelecionarRotaMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_PRIORIDADES_AUTORIZACAO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPrioridadesAutorizacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_FIXAR_OPERADOR_CONTRATOU_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FixarOperadorContratouCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_ACEITE_TERMO_USO_SISTEMA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirAceiteTermoUsoSistema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_DOWNLOAD_DANFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirDownloadDANFE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_DOWNLOAD_XML_ETAPA_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirDownloadXmlEtapaNfe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VERIFICAR_NFE_EM_OUTRA_CARGA_NA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VerificarNFeEmOutraCargaNaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_REMETENTE_DESTINATARIO_UNICO_INTEGRACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarRemetenteDestinatarioUnicoIntegracaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AUTOMATIZAR_GERACAO_LOTE_ESCRITURACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AutomatizarGeracaoLoteEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AUTOMATIZAR_GERACAO_LOTE_ESCRITURACAO_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AutomatizarGeracaoLoteEscrituracaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AUTOMATIZAR_GERACAO_LOTE_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AutomatizarGeracaoLotePagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarNotasParciaisEnvioEmissao", Column = "CEM_VALIDAR_NOTAS_PARCIAIS_ENVIO_EMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarNotasParciaisEnvioEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_ATUALIZAR_NOTA_FISCAL_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAutalizarNotaFiscalCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_INFORMAR_DISTANCIA_NO_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarDistanciaNoRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_DESABILITAR_SALDO_VIAGEM_ACERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesabilitarSaldoViagemAcerto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_ADICIONAR_CARGAS_TRANSPORDO_ACERTO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAdicionarCargasTransbordoAcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ATIVAR_AUTORIZACAO_AUTOMATICA_DE_TODAS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarAutorizacaoAutomaticaDeTodasCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_EXPEDIDOR_IGUAL_REMETENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirExpedidorIgualRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_RECEBEDOR_IGUAL_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRecebedorIgualDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_CONFIGURACAO_FATURAMENTO_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarConfiguracaoFaturamentoTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_VALIDAR_REMETENTE_NOTA_REMETENTE_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarRemetenteNotaComRemetentePedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_IMPRIMIR_NOTAS_BOLETOS_COM_RECEBEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoImprimirNotasBoletosComRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_CARGA_DE_MDFES_NAO_VINCULADOS_A_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaDeMDFesNaoVinculadosACargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_CARGA_DE_CTES_NAO_VINCULADOS_A_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaDeCTEsNaoVinculadosACargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HORA_GERACAO_CARGA_DE_CTES_NAO_VINCULADOS_A_CARGAS", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan HoraGeracaoCargaDeCTEsNaoVinculadosACargas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "CEM_INTEGRACAO_VALE_PEDAGIO_PADRAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PADRAO_TAG_VALE_PEDAGIO_VEICULOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PadraoTagValePedagioVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_EXIGIR_EXPEDIDOR_NO_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExigirExpedidorNoRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_OBSERVACAO_DA_NOTA_POR_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarObservacaoDaNotaPorCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OBRIGAR_FOTO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarFotoNaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OBRIGAR_FOTO_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarFotoNaDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_QRCODE_MOBILE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteQRCodeMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZA_APP_TRIZY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaAppTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_REGISTRAR_CHEGADA_APP_EM_METODO_DIFERENTE_DO_CONFIRMAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegistrarChegadaAppEmMetodoDiferenteDoConfirmar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ServerChatURL", Column = "CEM_MOBILE_SERVER_CHAR_URL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ServerChatURL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_ENTREGA_ANTES_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirEntregaAntesEtapaTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HORAS_CARGA_EXIBIDA_NO_APP", TypeType = typeof(int), NotNull = false)]
        public virtual int? HorasCargaExibidaNoApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_TIPO_SEPARACAO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirTipoSeparacaoMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_PERMITIR_LANCAR_OCORRENCIAS_EM_DUPLICIDADE_NA_SEQUENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirLancarOcorrenciasEmDuplicidadeNaSequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_PERMITIR_LANCAR_OCORRENCIA_DEPOIS_DE_OCORRENCIA_FINAL_GERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirLancarOcorrenciasDepoisDeOcorrenciaFinalGerada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_RETORNAR_DATA_CARREGAMENTO_DA_CARGA_NA_CONSULTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarDataCarregamentoDaCargaNaConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_DEIXAR_CARGA_PENDENTE_INTEGRACAO_APOS_CTE_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeixarCargaPendenteDeIntegracaoAposCTeManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_IMPRIMIR_DACTE_E_CARTA_CORRECAO_JUNTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirDACTEeCartaCorrecaoJunto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_SALVAR_DADOS_PARCIALMENTE_INFORMADOS_ETAPA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSalvarDadosParcialmenteInformadosEtapaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_LER_EDI_FTP_ARMAZENAR_NOTAS_FISCAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LerEDIDoFTPEArmazenarNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_QUANTIDADE_CARGA_PEDIDO_PROCESSAMENTO_LOTE", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCargaPedidoProcessamentoLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_FLUXO_PATIO_AO_FECHAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFluxoPatioAoFecharCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OBTER_NOVA_NUMERACAO_AO_DUPLICAR_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObterNovaNumeracaoAoDuplicarContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OBSERVACAO_GERAL_PEDIDO", Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoGeralPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CALCULAR_INCLUSAO_ICMS_ALIQUOTA_INTERNA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularInclusaoICMSAliquotaInterna { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_REENVIO_CIOT_CARGA_EMITIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirReenvioCIOTCargaEmitida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_IMPRIMIR_PERCURSO_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirPercursoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_AVANCAR_ETAPA_COM_INTEGRACAO_TRANSPORTADOR_REJEITADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ATUALIZAR_CARGA_COM_VEICULO_MDFE_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarCargaComVeiculoMDFeManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_REDUZIR_RETENCAO_ISS_VALOR_A_RECEBER_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReduzirRetencaoISSValorAReceberNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ADICIONAR_OUTRO_DOCUMENTO_QUANTO_CTE_ANTERIOR_NAO_TEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarOutroDocumentoQuandoCTeAnteriorNaoTem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OBRIGAR_TER_GUARITA_PARA_LANCAMENTO_E_FINALIZACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarTerGuaritaParaLancamentoEFinalizacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INCLUIR_TODOS_ACRESCIMOS_E_DESCONTOS_NO_CALCULO_DE_IMPOSTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirTodosAcrescimosEDescontosNoCalculoDeImpostos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ACOPLAR_MOTORISTA_AO_SELECIONAR_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AcoplarMotoristaAoVeiculoAoSelecionarNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_CIOT_PARA_TODAS_AS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCIOTParaTodasAsCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_VARIACAO_NEGATIVA_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirVariacaoNegativaContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CONSIDERAR_PEDAGIO_DESCARGA_VARIACAO_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarPedagioDescargaVariacaoContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteEmitirCargaDiferentesOrigensParcialmente", Column = "CEM_PERMITE_EMITIR_CARGA_DIFERENTES_ORIGENS_PARCIALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteEmitirCargaDiferentesOrigensParcialmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ORDENAR_CARGA_MOBILE_CRESCENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OrdenarCargasMobileCrescente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoImpressaoPedido", Column = "CEM_TIPO_IMPRESSAO_PEDIDO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoPedido), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoPedido TipoImpressaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoImpressaoPedidoPrestacaoServico", Column = "CEM_TIPO_IMPRESSAO_PEDIDO_PRESTACAO_SERVICO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoPedidoPrestacaoServico), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoPedidoPrestacaoServico TipoImpressaoPedidoPrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pais", Column = "CEM_PAIS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPais), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPais Pais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRecibo", Column = "CEM_TIPO_RECIBO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRecibo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRecibo TipoRecibo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INFORMAR_PERCENTUAL_ADIANTAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarPercentualAdiantamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_INFORMACOES_BOVINOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirInformacoesBovinos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CENTRO_RESULTADO_PEDIDO_OBRIGATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CentroResultadoPedidoObrigatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZA_MOEDA_ESTRANGEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CampoObsContribuinteCTeCargaRedespacho", Column = "CEM_CAMPO_OBSCONT_CTE_CARGA_REDESPACHO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CampoObsContribuinteCTeCargaRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TextoObsContribuinteCTeCargaRedespacho", Column = "CEM_TEXTO_OBSCONT_CTE_CARGA_REDESPACHO", TypeType = typeof(string), Length = 160, NotNull = false)]
        public virtual string TextoObsContribuinteCTeCargaRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_FILTRAR_CARGAS_SEM_DOCUMENTOS_PARA_CHAMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarCargasSemDocumentosParaChamados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_ASSUMIR_CHAMADO_DE_OUTRO_RESPONSAVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAssumirChamadoDeOutroResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZA_PGTO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaPgtoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_CADASTRAR_PRODUTO_AUTOMATICAMENTE_DOCUMENTO_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCadastrarProdutoAutomaticamenteDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PREENCHER_ULTIMO_KM_ENTRADA_GUARITA_TMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PreencherUltimoKMEntradaGuaritaTMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CONFIRMAR_PAGAMENTO_MOTORISTA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConfirmarPagamentoMotoristaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_PAGAMENTO_BLOQUEADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPagamentoBloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_LIBERAR_PAGAMENTO_AO_CONFIRMAR_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarPagamentoAoConfirmarEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CONFIRMAR_ENTREGA_DIGITALIZACAO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConfirmarEntregaDigitilizacaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_SOMENTE_DOCUMENTOS_DESBLOQUEADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarSomenteDocumentosDesbloqueados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BLOQUEAR_GERACAO_CARGA_COM_JANELA_CARREGAMENTO_EXCEDENTE_NA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearGeracaoCargaComJanelaCarregamentoExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_TIPO_DE_CARGA_NA_ABA_CARREGAMENTO_NA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirTipoDeCargaNaAbaCarregamentoNaMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_ABA_DETALHE_PEDIDO_EXPORTACAO_NA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirAbaDetalhePedidoExportacaoNaMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_CAPACIDADE_MODELO_VEICULAR_CARGA_NA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarCapacidadeModeloVeicularCargaNaMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HABILITAR_MULTIPLA_SELECAO_EMPRESA_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarMultiplaSelecaoEmpresaNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INFORMAR_DATA_VIAGEM_EXECUTADA_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDataViagemExecutadaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_TITULO_FOLHA_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTituloFolhaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_VALIDAR_GRUPO_PESSOAS_NA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarGrupoPessoaNaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_RETORNAR_CARGAS_AGRUPADAS_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarCargasAgrupadasCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_PAGAMENTO_MOTORISTA_SEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirPagamentoMotoristaSemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_PROPRIETARIO_VEICULO_MOVIMENTACAO_PLACA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarProprietarioVeiculoMovimentacaoPlaca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BLOQUEAR_FECHAMENTO_ABASTECIMENTO_SEM_PLACA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearFechamentoAbastecimentoSemplaca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AGRUPAR_INTEGRACAO_CARGA_COM_TIPO_OPERACAO_DIFERENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparIntegracaoCargaComTipoOperacaoDiferente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_GERAR_CARGA_DE_PEDIDO_SEM_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarCargaDePedidoSemTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TEMPO_MINUTOS_PERMANENCIA_CLIENTE", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoMinutosPermanenciaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TEMPO_MINUTOS_PERMANENCIA_SUBAREA_CLIENTE", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoMinutosPermanenciaSubareaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VELOCIDADE_MAXIMA_EXTREMA_ENTRE_POSICOES", TypeType = typeof(int), NotNull = false)]
        public virtual int VelocidadeMaximaExtremaEntrePosicoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INICIAR_CADASTRO_FUNCIONARIO_MOTORISTA_SEMPRE_INATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IniciarCadastroFuncionarioMotoristaSempreInativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_NAO_PROCESSAR_TROCA_ALVO_VIA_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoProcessarTrocaAlvoViaMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_DATA_EMISSAO_CONTRATO_PARA_MOVIMENTO_FINANCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataEmissaoContratoParaMovimentoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TEMPO_HORAS_PARA_LIBERACAO_CTE_APOS_FINALIZACAO_EMISSAO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoHorasParaRetornoCTeAposFinalizacaoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AGRUPAR_CARGA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparCargaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_FINALIZAR_CARGAS_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFinalizarCargasAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BUSCAR_POR_CARGA_PEDIDO_CARGAS_PENDENTES_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarPorCargaPedidoCargasPendentesIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_DATA_AUTORIZACAO_PARA_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirDataAutorizacaoParaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_DADOS_BANCARIOS_DA_EMPRESA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDadosBancariosDaEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_CALCULAR_DIFAL_PARA_CST_NAO_TRIBUTAVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCalcularDIFALParaCSTNaoTributavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_PARTICIOANTES_DA_CARGA_PELO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarParticipantesDaCargaPeloPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_NUMERO_FROTA_PARA_PESQUISA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaNumeroDeFrotaParaPesquisaDeVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_FLUXO_PATIO_POR_CARGAS_AGRUPADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFluxoPatioPorCargaAgrupada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_FLUXO_PATIO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFluxoPatioDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_OBSERVACAO_APROVADOR_AUTORIZACAO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirObservacaoAprovadorAutorizacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ATUALIZAR_ROTAS_QUANDO_ALTERAR_LOCALIZACAO_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarRotasQuandoAlterarLocalizacaoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_MONITORAMENTO_PARA_CARGA_RETORNO_VAZIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMonitoramentoParaCargaRetornoVazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AGRUPAR_CTES_DIFERENTES_PEDIDOS_MESMO_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparCTesDiferentesPedidosMesmoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_CODIFICACAO_UTF8_CONVERSAO_PDF", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCodificacaoUTF8ConversaoPDF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_SOLICITAR_CONFIRMACAO_PEDIDO_SEM_MOTORISTA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarConfirmacaoPedidoSemMotoristaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_SOLICITAR_CONFIRMACAO_PEDIDO_DUPLICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarConfirmacaoPedidoDuplicado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_SOLICITAR_CONFIRMACAO_MOVIMENTO_FINANCEIRO_DUPLICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarConfirmacaoMovimentoFinanceiroDuplicado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_QUANTIDADE_MAXIMA_REGISTROS_RELATORIOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeMaximaRegistrosRelatorios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MonitorarPosicaoAtualVeiculo", Column = "CEM_MONITORAR_POSICAO_ATUAL_VEICULO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitorarPosicaoAtualVeiculo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitorarPosicaoAtualVeiculo MonitorarPosicaoAtualVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ATUALIZAR_VINCULO_VEICULO_MOTORISTA_INTEGRACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarVinculoVeiculoMotoristaIntegracaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_OBRIGAR_DATA_SAIDA_RETORNO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoObrigarDataSaidaRetornoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_MOVIMENTAR_KM_APENAS_PELA_GUARITA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MovimentarKMApenasPelaGuarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BLOQUEAR_EMISSAO_CARGA_SEM_TEMPO_ROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearEmissaoCargaSemTempoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_SELECIONAR_REBOQUE_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSelecionarReboquePedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_VALIDAR_CODIGO_BARRAS_BOLETO_TITULO_PAGAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarCodigoBarrasBoletoTituloAPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ENCERRAR_MDFE_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncerrarMDFeAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_MOVIMENTACAO_NA_BAIXA_INDIVIDUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentacaoNaBaixaIndividualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BLOQUEAR_ALTERACAO_VEICULO_PORTAL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearAlteracaoVeiculoPortalTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HABILITAR_ENVIO_ABASTECIMENTO_EXTERNO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarEnvioAbastecimentoExterno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AGRUPAR_UNIDADES_MEDIDAS_POR_DESCRICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparUnidadesMedidasPorDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HABILITAR_FSDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarFSDA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_EXIBIR_VALOR_FRETE_CTE_COMPLEMENTAR_RELATORIO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirValorFreteCTeComplementarRelatorioCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HABILITAR_HORA_FILTRO_DATA_INICIAL_FINAL_RELATORIO_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarHoraFiltroDataInicialFinalRelatorioCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VINCULAR_NOTAS_PARCIAIS_PEDIDO_POR_PROCESSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularNotasParciaisPedidoPorProcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_EMITIR_CTE_COMPLEMENTAR_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteEmitirCTeComplementarManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_PERMITE_EMITIR_CARGA_SEM_SEGURO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermiteEmitirCargaSemSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_VALOR_DESCONTATO_COMISSAO_MOTORISTA_INFRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarValorDescontatoComissaoMotoristaInfracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_INFORMAR_CHAMADOS_NO_LANCAMENTO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarChamadosNoLancamentoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_FECHAR_ACERTO_VIAGEM_ATE_RECEBER_CANHOTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFecharAcertoViagemAteReceberCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_RAIO_PADRAO", TypeType = typeof(int), NotNull = false)]
        public virtual int RaioPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TEMPO_PADRAO_DE_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoPadraoDeEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TEMPO_PADRAO_DE_COLETA_PARA_CALCULAR_PREVISAO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoPadraoDeColetaParaCalcularPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TEMPO_PADRAO_DE_ENTREGA_PARA_CALCULAR_PREVISAO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoPadraoDeEntregaParaCalcularPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_EXIBIR_INFOS_ADICIONAIS_GRID_PATIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirInfosAdicionaisGridPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_RATEAR_FRETE_CARGA_PEDIDO_ENTRE_NOTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRatearFreteCargaPedidoEntreNotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_COLUNA_CODIGOS_AGRUPADOS_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirColunaCodigosAgrupadosOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_COLUNA_VALOR_FRETE_CARGA_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirColunaValorFreteCargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CRIAR_NOTA_FISCAL_TRANSPORTE_POR_DOCUMENTO_DESTINADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CriarNotaFiscalTransportePorDocumentoDestinado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_ALIQUOTA_ETAPA_FRETE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirAliquotaEtapaFreteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_EMPRESA_TITULO_FINANCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirEmpresaTituloFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_ROTA_FRETE_INFORMADO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarRotaFreteInformadoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OBRIGATORIO_CADASTRAR_RASTREADOR_NOS_VEICULOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioCadastrarRastreadorNosVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_ADICIONAR_NUMERO_PEDIDO_EMBARCADOR_OBSERVACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAdicionarNumeroPedidoEmbarcadorObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_JUSTIFICATIVA_CANCELAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirJustificativaCancelamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EMITIR_COMPLEMENTAR_REDESPACHO_FILIAL_EMISSORA_DIFERENTE_UF_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirComplementarRedespachoFilialEmissoraDiferenteUFOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_RETORNAR_CTE_INUTILIZADO_NO_FLUXO_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarCTeIntulizadoNoFluxoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_IMPORTAR_OCORRENCIAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirImportarOcorrencias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AVANCAR_ETAPA_DOCUMENTOS_EMISSAO_AO_VINCULAR_TODAS_NOTAS_PARCIAIS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvancarEtapaDocumentosEmissaoAoVincularTodasNotasParciaisCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ARMAZENAR_CENTRO_CUSTO_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ArmazenarCentroCustoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EMITIR_NFE_REMESSA_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirNFeRemessaNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_ALTERAR_DATA_CARREGAMENTO_CARGA_NO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarDataCarregamentoCargaNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_CARGA_COM_AGRUPAMENTO_NA_MONTAGEM_CARGA_COMO_CARGA_DE_COMPLEMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaComAgrupamentoNaMontagemCargaComoCargaDeComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_ADICIONAR_NFE_REPETIDA_PARA_OUTRO_PEDIDO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAdicionarNFeRepetidaParaOutroPedidoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_PESO_PEDIDO_PARA_RATEAR_PESO_NFE_REPETIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPesoPedidoParaRatearPesoNFeRepetida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_REGRA_ICMS_CTE_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarRegraICMSCTeSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BLOQUEAR_CAMPOS_OCORRENCIA_IMPORTADOS_DO_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearCamposOcorrenciaImportadosDoAtendimento { get; set; }

        [Obsolete("Migrado a informação para o enumerador TipoImpressaoFatura")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_LAYOUT_FATURA_POR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarLayoutFaturaPorDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoImpressaoFatura", Column = "CEM_TIPO_IMPRESSAO_FATURA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura TipoImpressaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_RENAVAM_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarRENAVAMVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_PLACA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarPlacaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_IDENTIFICAR_MONITORAMENO_STATUS_VIAGEM_EM_TRANSITO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IdentificarMonitoramentoStatusViagemEmTransito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_IDENTIFICAR_MONITORAMENO_STATUS_VIAGEM_EM_TRANSITO_KM", TypeType = typeof(int), NotNull = false)]
        public virtual int IdentificarMonitoramentoStatusViagemEmTransitoKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_IDENTIFICAR_MONITORAMENO_STATUS_VIAGEM_EM_TRANSITO_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int IdentificarMonitoramentoStatusViagemEmTransitoMinutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_REALIZAR_MOVIMENTACAO_PAMCARD_PROXIMO_DIA_UTIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarMovimentacaoPamcardProximoDiaUtil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_OPCAO_REENVIAR_NOTFIS_COM_FALHAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirOpcaoReenviarNotfisComFalhas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_OPCAO_DOWNLOAD_PLANILHA_RATEIO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirOpcaoDownloadPlanilhaRateioOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_RETORNAR_CANHOTOS_VIA_INTEGRACAO_EM_QUALQUER_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarCanhotosViaIntegracaoEmQualquerSituacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_NUMERO_PAGER_ETAPA_INICIAL_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirNumeroPagerEtapaInicialCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_IDENTIFICAR_VEICULO_PARADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IdentificarVeiculoParado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_IDENTIFICAR_VEICULO_PARADO_DISTANCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int IdentificarVeiculoParadoDistancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_IDENTIFICAR_VEICULO_PARADO_TEMPO", TypeType = typeof(int), NotNull = false)]
        public virtual int IdentificarVeiculoParadoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_DATA_ENTREGA_NOTA_CLIENTE_CANHOTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirDataEntregaNotaClienteCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_DATA_BASE_PARA_CALCULO_PREVISAO_CONTROLE_ENTREGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega DataBaseCalculoPrevisaoControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_ATUALIZAR_PREVISAO_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAtualizarPrevisaoControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_ATUALIZAR_PREVISAO_ENTREGA_PEDIDO_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAtualizarPrevisaoEntregaPedidoControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NUMERO_SERIE_NOTA_DEBITO_PADRAO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroSerieNotaDebitoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NUMERO_SERIE_NOTA_CREDITO_PADRAO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroSerieNotaCreditoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_REALIZAR_MOVIMENTACAO_MOTORISTA_PELA_DATA_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarMovimentacaPagamentoMotoristaPelaDataPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_USAR_RECIBO_PAGAMENTO_GERACAO_AUTORIZACAO_TITULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarReciboPagamentoGeracaoAutorizacaoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HABILITAR_CONTROLE_FLUXO_NFE_DEVOLUCAO_CHAMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarControleFluxoNFeDevolucaoChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PROCESSAR_FILA_DOCUMENTOS_EM_LOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProcessarFilaDocumentosEmLote { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_CRIACAO_PEDIDO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoDeOcorrenciaCriacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_RECEBIMENTO_MERCADORIA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoDeOcorrenciaRecebimentoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_REENTREGA_PEDIDO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoDeOcorrenciaReentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INCLUIR_BC_COMPONENTES_DESCONTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirBCCompontentesDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PREVISAO_ENTREGA_PERIODO_UTIL_HORARIO_INICIAL", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan PrevisaoEntregaPeriodoUtilHorarioInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PREVISAO_ENTREGA_PERIODO_UTIL_HORARIO_FINAL", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan PrevisaoEntregaPeriodoUtilHorarioFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PREVISAO_ENTREGA_TEMPO_UTIL_DIARIO_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int PrevisaoEntregaTempoUtilDiarioMinutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PREVISAO_ENTREGA_VELOCIDADE_MEDIA_VAZIO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrevisaoEntregaVelocidadeMediaVazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PREVISAO_ENTREGA_VELOCIDADE_MEDIA_CARREGADO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrevisaoEntregaVelocidadeMediaCarregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_ENCERRAR_MDFE_EMITIDO_NO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteEncerrarMDFeEmitidoNoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExpressaoLacreContainer", Column = "CEM_EXPRESSAO_LACRE_CONTAINER", TypeType = typeof(string), Length = 160, NotNull = false)]
        public virtual string ExpressaoLacreContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApresentarCodigoIntegracaoComNomeFantasiaCliente", Column = "CEM_APRESENTAR_CODIGO_INTEGRACAO_COM_NOME_FANTASIA_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ApresentarCodigoIntegracaoComNomeFantasiaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LancarOsServicosDaOrdemDeServicoAutomaticamente", Column = "CEM_LANCAR_OS_SERVICOS_DE_ORDEM_DE_SERVICO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancarOsServicosDaOrdemDeServicoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AJUSTAR_DATA_CONTRATO_IGUAL_DATA_FINALIZACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AjustarDataContratoIgualDataFinalizacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_CLASSIFICACAO_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirClassificacaoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_SENHA_CADASTRO_PESSOA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirSenhaCadastroPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXPEDIDOR_IGUAL_REMETENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExpedidorIgualRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_RECEBEDOR_IGUAL_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RecebedorIgualDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VISUALIZAR_TIPO_OPERACAO_DO_PEDIDO_POR_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarTipoOperacaoDoPedidoPorTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_USAR_GRUPO_TIPO_OPERACAO_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarGrupoDeTipoDeOperacaoNoMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_USAR_GRUPO_TIPO_OPERACAO_MONITORAMENTO_OCULTAR_GRUPO_STATUS_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_LANCAR_FOLGA_AUTOMATICAMENTE_NO_ACERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancarFolgaAutomaticamenteNoAcerto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_CARGA_MDFE_DESTINADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaMDFeDestinado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_SOMAR_SALDO_ATUAL_MOTORISTA_NO_ACERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomarSaldoAtualMotoristaNoAcerto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_JUSTIFICAR_ENTREGA_FORA_RAIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool JustificarEntregaForaDoRaio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TOKEN_SMS", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string TokenSMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_SENDER_SMS", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string SenderSMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CADASTRAR_MOTORISTA_E_VEICULO_AUTOMATICAMENTE_CARGA_IMPORTADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarMotoristaEVeiculoAutomaticamenteCargaImportada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_BLOQUEAR_CARGA_COM_PROBLEMA_INTEGRACAO_GR_MOTORISTA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoBloquearCargaComProblemaIntegracaoGrMotoristaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AVISAR_MDFE_EMITIDO_EMBARCADOR_SEM_SEGURO_VALIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisarMDFeEmitidoEmbarcadorSemSeguroValido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_CANHOTO_SEMPRE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCanhotoSempre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OCULTAR_INFORMACOES_FATURAMENTO_ACERTO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarInformacoesFaturamentoAcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OCULTAR_INFORMACOES_RESULTADO_VIAGEM_ACERTO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarInformacoesResultadoViagemAcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_RECBIDO_ACERTO_VIAGEM_DETALHADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarReciboAcertoViagemDetalhado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_PERMITIR_AVANCAR_CARGA_SEM_ESTOQUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAvancarCargaSemEstoque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_NFSE_IMPORTACAO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarNFSeImportacaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INFORMAR_AJUSTE_MANUAL_CARGAS_IMPORTADAS_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarAjusteManualCargasImportadasEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_FILTRAR_NOTAS_COMPATIVEIS_PELO_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarNotasCompativeisPeloDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_FILTROS_NOTAS_COMPATIVEIS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirFiltrosNotasCompativeisCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_EXIBIR_PESSOAS_CHAMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirPessoasChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_SALVAR_ANALISE_EM_ANEXO_AO_LIBERAR_OCORRENCIA_CHAMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SalvarAnaliseEmAnexoAoLiberarOcorrenciaChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_RESPONDER_ANALISE_AO_LIBERAR_OCORRENCIA_CHAMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ResponderAnaliseAoLiberarOcorrenciaChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_ESTORNAR_APROVACAO_CHAMADO_LIBERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirEstornarAprovacaoChamadoLiberado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZA_LISTA_DINAMICA_DATAS_CHAMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaListaDinamicaDatasChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LinkVideoMobile", Column = "CEM_LINK_VIDEO_MOBILE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string LinkVideoMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivoPoliticaPrivacidadeMobile", Column = "CEM_CAMINHO_ARQUIVO_POLITICA_PRIVACIDADE_MOBILE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CaminhoArquivoPoliticaPrivacidadeMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_FINALIZAR_DOCUMENTO_ENTRADA_OS_VALOR_DIVERGENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFinalizarDocumentoEntradaOSValorDivergente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_FINALIZAR_DOCUMENTO_ENTRADA_OC_VALOR_DIVERGENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFinalizarDocumentoEntradaOrdemCompraValorDivergente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_FINALIZAR_DOCUMENTO_ENTRADA_COM_ABASTECIMENTO_INCONSISTENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFinalizarDocumentoEntradaComAbastecimentoInconsistente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_NOTA_FISCAL_EXISTENTE_IMPORTACAO_CTE_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarNotaFiscalExistenteNaImportacaoCTeEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BUSCAR_CARGA_POR_NUMERO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarCargaPorNumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ATUALIZAR_ROTA_REALIZADA_MONITORAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento AtualizarRotaRealizadaDoMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_OS_AUTOMATICAMENTE_CADASTRO_VEICULO_EQUIPAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOSAutomaticamenteCadastroVeiculoEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_RAIZ_CNPJ_GRUPO_PESSOA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarRaizCNPJGrupoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_OUTROS_OPERADORES_ALTERAR_LANCAMENTO_PROSPECCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteOutrosOperadoresAlterarLancamentoProspeccao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_BLOQUEAR_SEM_REGRA_APROVACAO_ORDEM_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearSemRegraAprovacaoOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_EXIBIR_ROTA_JANELA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirRotaJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_EXIBIR_LOCAL_CARREGAMENTO_JANELA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirLocalCarregamentoJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ARMAZENAR_PDF_DANFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ArmazenarPDFDANFE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_DISTANCIA_MINIMA_PERCORRIDA_PARA_SAIDA_DO_ALVO", TypeType = typeof(int), NotNull = false)]
        public virtual int DistanciaMinimaPercorridaParaSaidaDoAlvo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CONTROLAR_AGENDAMENTO_SKU", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlarAgendamentoSKU { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NOTIFICAR_CARGA_AG_CONFIRMACAO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarCargaAgConfirmacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_DEIXAR_ABASTECIMENTO_MESMA_DATA_HORA_INCONSISTENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeixarAbastecimentosMesmaDataHoraInconsistentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_ACEITAR_NOTAS_NA_ETAPA1", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAceitarNotasNaEtapa1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_OCORRENCIA_COMPLEMENTO_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaComplementoSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_CONTROLAR_SITUACAO_VEICULO_ORDEM_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoControlarSituacaoVeiculoOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_PREVIEW_DOCCOB_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPreviewDOCCOBFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_ALTERAR_DATA_PREVISAO_ENTREGA_PEDIDO_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarDataPrevisaoEntregaPedidoNoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NECESSARIO_INFORMAR_JUSTIFICATIVA_AO_ALTERAR_DATA_SAIDA_OU_PREVISAO_ENTREGA_PEDIDO_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NecessarioInformarJustificativaAoAlterarDataSaidaOuPrevisaoEntregaPedidoNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMLimiteEntreAbastecimentos", Column = "CEM_KM_LIMITE_ENTRE_ABASTECIMENTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int KMLimiteEntreAbastecimentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMLimiteEntreAbastecimentosArla", Column = "CEM_KM_LIMITE_ENTRE_ABASTECIMENTOS_ARLA", TypeType = typeof(int), NotNull = false)]
        public virtual int KMLimiteEntreAbastecimentosArla { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "HorimetroLimiteEntreAbastecimentos", Column = "CEM_HORIMETRO_LIMITE_ENTRE_ABASTECIMENTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int HorimetroLimiteEntreAbastecimentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoSemPosicaoParaVeiculoPerderSinal", Column = "CEM_TEMPO_SEM_POSICAO_PARA_VEICULO_PERDER_SINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoSemPosicaoParaVeiculoPerderSinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_VALIDAR_TOMADOR_CTE_SUBCONTRATACAO_COM_TOMADOR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarTomadorCTeSubcontratacaoComTomadorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_LIMITAR_APENAS_UM_MONITORAMENTO_POR_PLACA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LimitarApenasUmMonitoramentoPorPlaca { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_POSTO_PADRAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente PostoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_COMBUSTIVEL_PADRAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Produto CombustivelPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_RETORNAR_CARGAS_PENDENCIA_EMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarCargaPendenciaEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_VALOR_FRETE_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarValorFreteNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_IMPORTAR_EMAIL_CLIENTE_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarEmailCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AVISAR_DIVERGENCIA_VALORES_CTE_EMITIDO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisarDivergenciaValoresCTeEmitidoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ADICIONAR_RELATORIO_RELACAO_ENTREGA_DOWNLOAD_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarRelatorioRelacaoEntregaDownloadDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_QUANTIDADE_VOLUMES_NF", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirQuantidadeVolumesNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_CONTROLAR_ESTOQUE_NEGATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlarEstoqueNegativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_CADASTRAR_LAT_LNG_ENTREGA_LOCALIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteCadastrarLatLngEntregaLocalidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_UTILIZAR_DATA_TERMINO_PROGRAMACAO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarDataTerminoProgramacaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_LOCALIDADE_PRESTACAO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarLocalidadePrestacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_ENCERRAR_VIAGEM_AO_ENCERRAR_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEncerrarViagemAoEncerrarControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_VALIDADE_SERVICO_PELO_GRUPO_SERVICO_ORDEM_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarValidadeServicoPeloGrupoServicoOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_LANCAR_AVARIAS_SOMENTE_PARA_PRODUTOS_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirLancarAvariasSomenteParaProdutosDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_RATEAR_VALOR_DO_FRETE_NOS_PRODUTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRatearValorFreteProtudos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_OCULTAR_OCORRENCIAS_GERADAS_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarOcorrenciasGeradasAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_UTILIZAR_REGRA_ENTRADA_DOCUMENTO_POR_GRUPO_NCM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarRegraEntradaDocumentoGrupoNCM { get; set; }

        /// <summary>
        /// Código para envio para o Oracle para emissão (Configurar maior que 1)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTipoEnvioEmissaoCTe", Column = "CEM_CODIGO_TIPO_ENVIO_EMISSAO_CTE", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoTipoEnvioEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_ALTERAR_ROTA_EM_CARGA_FINALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAlterarRotaEmCargaFinalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_PREENCHER_SERIE_CTE_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPreencherSerieCTeManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_OBRIGAR_INFORMAR_SEGMENTO_NO_ACERTO_DE_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoObrigarInformarSegmentoNoAcertoDeViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_RECIBO_DETALHADO_ACERTO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarReciboDetalhadoAcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_BUSCAR_DATA_INICIO_VIAGEM_ACERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoBuscarDataInicioViagemAcerto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_WEB_SERVICE_REST_ATM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarWebServiceRestATM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_INTEGRACAO_AVERBACAO_BRADESCO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarIntegracaoAverbacaoBradescoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIBIR_JANELA_DESCARGA_POR_PERIODO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirJanelaDescargaPorPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VISUALIZAR_VALOR_NFSE_DESCONTANDO_ISS_RETIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarValorNFSeDescontandoISSRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TELA_MONITORAMENTO_APRESENTAR_CARGAS_QUANDO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento TelaMonitoramentoApresentarCargasQuando { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TELA_CARGA_APRESENTAR_ULTIMA_POSICAO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TelaCargaApresentarUltimaPosicaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_PREENCHER_MOTORISTA_VEICULO_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPreencherMotoristaVeiculoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_DUPLICAR_CARGA_AO_CANCELAR_POR_IMPORTACAO_XML_CTE_CANCELADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoDuplicarCargaAoCancelarPorImportacaoXMLCTeCancelado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_VALIDAR_MEDIA_IDEAL_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarMediaIdealAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_VALIDAR_MEDIA_IDEAL_DE_ARLA_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarMediaIdealDeArlaAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_MESMO_KM_COM_LITROS_DIFERENTE_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarMesmoKMComLitrosDiferenteAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_DEIXAR_ABASTECIMENTO_TERCEIRO_INCONSISTENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoDeixarAbastecimentoTerceiroInconsistente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_USAR_DATA_CHECKLIST_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarDataChecklistVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_DESCONTAR_VALOR_SALDO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoDescontarValorSaldoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_CARGO_CONSULTA_FUNCIONARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarCargoConsultaFuncionario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_UTILIZAR_SERIE_CARGA_CTE_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarSerieCargaCTeManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZA_MULTIPLOS_LOCAIS_ARMAZENAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaMultiplosLocaisArmazenamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZA_SUPRIMENTO_DE_GAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaSuprimentoDeGas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITE_IMPORTAR_PLANILHA_VALOR_FRETE_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteImportarPlanilhaValoresFreteNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VISUALIZAR_DATAS_RAIO_NO_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarDatasRaioNoAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_CLIENTE_RESPONSAVEL_PELO_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirClienteResponsavelPeloAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_MONITORAR_PASSAGENS_FRONTEIRAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MonitorarPassagensFronteiras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_LIMITE_CREDITO_NO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarLimiteCreditoNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ATIVAR_EMISSAO_SUBCONTRATACAO_AGRUPADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEmissaoSubcontratacaoAgrupado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ATUALIZAR_ENDERECO_MOTORISTA_INTEGRACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarEnderecoMotoristaIntegracaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_MODELO_VEICULAR_CARGA_NAO_OBRIGATORIO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ModeloVeicularCargaNaoObrigatorioMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_MOTORISTA_OBRIGATORIO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MotoristaObrigatorioMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VEICULO_OBRIGATORIO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VeiculoObrigatorioMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_OCORRENCIA_PARA_CARGA_AGRUPADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaParaCargaAgrupada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AprovarAutomaticamenteCteEmitidoComValorInferiorAoEsperado", Column = "CEM_APROVAR_AUTOMATICAMENTE_CTE_EMITIDO_COM_VALOR_INFERIOR_AO_ESPERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AprovarAutomaticamenteCteEmitidoComValorInferiorAoEsperado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearVeiculoExistenteEmCargaNaoFinalizada", Column = "CEM_BLOQUEAR_VEICULO_EXISTENTE_EM_CARGA_NAO_FINALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearVeiculoExistenteEmCargaNaoFinalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ATIVAR_FATURAMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarFaturamentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_ENVIAR_BOLETO_APENAS_PARA_EMAIL_SECUNDARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarBoletoApenasParaEmailSecundario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_FORMA_PREENCHIMENTO_CENTRO_RESULTADO_PEDIDO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaPreenchimentoCentroResultadoPedido), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaPreenchimentoCentroResultadoPedido FormaPreenchimentoCentroResultadoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VALIDAR_EXISTENCIA_DE_CONFIGURACAO_FATURA_DO_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarExistenciaDeConfiguracaoFaturaDoTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EXIGIR_NUMERO_DOCUMENTO_TITULO_FINANCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirNumeroDocumentoTituloFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_USA_PERMISSAO_CONTROLADOR_RELATORIOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsaPermissaoControladorRelatorios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDiasLimiteVencimentoFaturaManual", Column = "CEM_QUANTIDADE_DIAS_LIMITE_VENCIMENTO_FATURA_MANUAL", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasLimiteVencimentoFaturaManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearLancamentoServicoDuplicadoOrdemServico", Column = "CEM_BLOQUEAR_LANCAMENTO_SERVICO_DUPLICADO_ORDEM_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearLancamentoServicoDuplicadoOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MonitoramentoStatusViagemQuandoFicarSemStatusManterUltimo", Column = "CEM_MONITORAMENTO_STATUS_VIAGEM_QUANDO_FICAR_SEM_STATUS_MANTER_ULTIMO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MonitoramentoStatusViagemQuandoFicarSemStatusManterUltimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MonitoramentoConsiderarPosicaoTardiaParaAtualizarInicioFimEntregaViagem", Column = "CEM_MONITORAMENTO_CONSIDERAR_POSICAO_TARDIA_PARA_ATUALIZAR_INICIO_FIM_ENTREGA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MonitoramentoConsiderarPosicaoTardiaParaAtualizarInicioFimEntregaViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TelaMonitoramentoPadraoFiltroDataInicialFinal", Column = "CEM_TELA_MONITORAMENTO_PADRAO_FILTRO_DATA_INICIAL_FINAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TelaMonitoramentoPadraoFiltroDataInicialFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasAnterioresPesquisaCarga", Column = "CEM_DIAS_ANTERIORES_PESQUISA_CARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasAnterioresPesquisaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AoInativarMotoristaTransformarEmFuncionario", Column = "CEM_AO_INATIVAR_MOTORISTA_TRANSFORMAR_EM_FUNCIONARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AoInativarMotoristaTransformarEmFuncionario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarCTesComValoresZerados", Column = "CEM_NAO_GERAR_CTES_COM_VALORES_ZERADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarCTesComValoresZerados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularFreteCliente", Column = "CEM_CALCULAR_FRETE_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirAtualizarModeloVeicularCargaDoVeiculoNoWebService", Column = "CEM_PERMITIR_ATUALIZAR_MODELO_VEICULAR_CARGA_DO_VEICULO_NO_WEBSERVICE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAtualizarModeloVeicularCargaDoVeiculoNoWebService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BuscarMotoristaDaCargaLancamentoAbastecimentoAutomatico", Column = "CEM_BUSCAR_MOTORISTA_DA_CARGA_LANCAMENTO_ABASTECIMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarMotoristaDaCargaLancamentoAbastecimentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarObservacaoRegraICMSAposObservacaoCTe", Column = "CEM_GERAR_OBSERVACAO_REGRA_ICMS_APOS_OBSERVACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarObservacaoRegraICMSAposObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarComponentesDeFreteComImpostoIncluso", Column = "CEM_GERAR_COMPONENTES_DE_FRETE_COM_IMPOSTO_INCLUSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarComponentesDeFreteComImpostoIncluso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsultarRegraICMSGeracaoCTeSubstitutoAutomaticamente", Column = "CEM_CONSULTAR_REGRA_ICMS_GERACAO_CTE_SUBSTITUTO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarRegraICMSGeracaoCTeSubstitutoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirInformarQuilometragemTabelaFreteCliente", Column = "CEM_PERMITIR_INFORMAR_QUILOMETRAGEM_TABELA_FRETE_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarQuilometragemTabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TabelaFretePrecisaoDinheiroDois", Column = "CEM_TABELA_FRETE_PRECISAO_DINHEIRO_DOIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TabelaFretePrecisaoDinheiroDois { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoImpressaoPadraoCarga", Column = "CEM_DOCUMENTO_IMPRESSAO_PADRAO_CARGA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DocumentoImpressaoPadraoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirSaldoPrevistoAcertoViagem", Column = "CEM_EXIBIR_SALDO_PREVISTO_ACERTO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirSaldoPrevistoAcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BuscarAdiantamentosSemDataInicialAcertoViagem", Column = "CEM_BUSCAR_ADIANTAMENTO_SEM_DATA_INICIAL_ACERTO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarAdiantamentosSemDataInicialAcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarAverbacaoCTeQuandoPedidoTiverAverbacao", Column = "CEM_NAO_GERAR_AVERBACAO_CTE_QUANDO_PEDIDO_TIVER_AVERBACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarAverbacaoCTeQuandoPedidoTiverAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoConsiderarProdutosSemPesoParaSumarizarVolumes", Column = "CEM_NAO_CONSIDERAR_PRODUTOS_SEM_PESO_PARA_SUMARIZAR_VOLUMES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoConsiderarProdutosSemPesoParaSumarizarVolumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoEmitirDocumentosEmCargasDeReentrega", Column = "CEM_NAO_EMITIR_DOCUMENTO_EM_CARGAS_DE_REENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEmitirDocumentosEmCargasDeReentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirInativarFuncionarioComSaldo", Column = "CEM_NAO_PERMITIR_INATIVAR_FUNCIONARIO_COM_SALDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirInativarFuncionarioComSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarPesoLiquidoNFeParaCTeMDFe", Column = "CEM_UTILIZAR_PESO_LIQUIDO_NFE_PARA_CTE_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPesoLiquidoNFeParaCTeMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteInformarModeloVeicularCargaOrigem", Column = "CEM_PERMITE_INFORMAR_MODELO_VEICULAR_CARGA_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarModeloVeicularCargaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoSolicitarAtuorizacaoAbastecimento", Column = "CEM_NAO_SOLICITAR_AUTORIZACAO_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSolicitarAtuorizacaoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VelocidadeMinimaAceitaDasTecnologias", Column = "CEM_VELOCIDADE_MINIMA_ACEITA_DAS_TECNOLOGIAS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal VelocidadeMinimaAceitaDasTecnologias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VelocidadeMaximaAceitaDasTecnologias", Column = "CEM_VELOCIDADE_MAXIMA_ACEITA_DAS_TECNOLOGIAS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal VelocidadeMaximaAceitaDasTecnologias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TemperaturaMinimaAceitaDasTecnologias", Column = "CEM_TEMPERATURA_MINIMA_ACEITA_DAS_TECNOLOGIAS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TemperaturaMinimaAceitaDasTecnologias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TemperaturaMaximaAceitaDasTecnologias", Column = "CEM_TEMPERATURA_MAXIMA_ACEITA_DAS_TECNOLOGIAS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal TemperaturaMaximaAceitaDasTecnologias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PreencherDataProgramadaComAtualCheckList", Column = "CEM_PREENCHER_DATA_PROGRAMADA_COM_ATUAL_CHECK_LIST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PreencherDataProgramadaComAtualCheckList { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_LOCAL_MANUTENCAO_PADRAO_CHECK_LIST", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente LocalManutencaoPadraoCheckList { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCalculoPercentualViagem", Column = "CEM_TIPO_CALCULO_PERCENTUAL_VIAGEM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPercentualViagem), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPercentualViagem TipoCalculoPercentualViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MonitoramentoStatusViagemTipoRegraParaCalcularPercentualViagem", Column = "CEM_MONITORAMENTO_STATUS_VIAGEM_TIPO_REGRA_PARA_CALCULAR_PERCENTUAL_VIAGEM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra? MonitoramentoStatusViagemTipoRegraParaCalcularPercentualViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SomenteAutorizadoresPodemDelegarOcorrencia", Column = "CEM_SOMENTE_AUTORIZADORES_PODEM_DELEGAR_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomenteAutorizadoresPodemDelegarOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirMotivoOcorrencia", Column = "CEM_EXIGIR_MOTIVO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirMotivoOcorrencia { get; set; }

        [Obsolete("Migrado a informação para o TipoOperacao")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermiteAvancarCargaSemDataPrevisaoDeEntrega", Column = "CEM_NAO_PERMITE_AVANCAR_CARGA_SEM_DATA_PREVISAO_DE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermiteAvancarCargaSemDataPrevisaoDeEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarEtiquetaDetalhadaWMS", Column = "CEM_UTILIZAR_ETIQUETA_DETALHADA_WMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarEtiquetaDetalhadaWMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarNumeracaoFaturaAnual", Column = "CEM_GERAR_NUMERACAO_FATURA_ANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarNumeracaoFaturaAnual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarSeExisteVeiculoCadastradoComMesmoNrDeFrota", Column = "CEM_VALIDAR_SE_EXISTE_VEICULO_CADASTRADO_COM_MESMO_NR_DE_FROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarSeExisteVeiculoCadastradoComMesmoNrDeFrota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FiltrarAgendamentoColetaNaMontagemDaCarga", Column = "CEM_FILTRAR_AGENDAMENTO_COLETA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarAgendamentoColetaNaMontagemDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarAlertaCargasParadas", Column = "CEM_HABILITAR_ALERTA_CARGAS_PARADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarAlertaCargasParadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoMinutosAlertaCargasParadas", Column = "CEM_TEMPO_MINUTOS_ALERTA_CARGAS_PARADAS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoMinutosAlertaCargasParadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailsAlertaCargasParadas", Column = "CEM_EMAILS_ALERTA_CARGAS_PARADAS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailsAlertaCargasParadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPosicoesPendentesAlerta", Column = "CEM_NUMERO_POSICOES_PENDENTES_ALERTA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroPosicoesPendentesAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_RETORNAR_TITULOS_NAO_GERADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarTitulosNaoGerados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoExigirMotivoAprovacaoCTeInconsistente", Column = "CEM_NAO_EXIGIR_MOTIVO_APROVACAO_CTE_INCONSISTENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExigirMotivoAprovacaoCTeInconsistente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoRetornarCarregamentosSemData", Column = "CEM_NAO_RETORNAR_CARREGAMENTOS_SEM_DATA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRetornarCarregamentosSemData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarEmailAnalistasChamado", Column = "CEM_ENVIAR_EMAIL_ANALISTAS_CHAMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailAnalistasChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuandoIniciarViagemViaMonitoramento", Column = "CEM_QUANDO_INICIAR_VIAGEM_MONITORAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarViagemViaMonitoramento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarViagemViaMonitoramento? QuandoIniciarViagemViaMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornarCargaDocumentoEmitido", Column = "CEM_RETORNAR_CARGA_DOCUMENTO_EMITIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarCargaDocumentoEmitido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiValidacaoParaLiberacaoCargaComNotaJaUtilizada", Column = "CEM_POSSUI_VALIDACAO_PARA_LIBERACAO_CARGA_COM_NOTA_JA_UTILIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiValidacaoParaLiberacaoCargaComNotaJaUtilizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirTipoMovimentoLancamentoMovimentoFinanceiroManual", Column = "CEM_EXIGIR_TIPO_MOVIMENTO_LANCAMENTO_MOVIMENTO_FINANCEIRO_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirTipoMovimentoLancamentoMovimentoFinanceiroManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuandoIniciarMonitoramento", Column = "CEM_QUANDO_INICIAR_MONITORAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento? QuandoIniciarMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcaoAoFinalizarMonitoramento", Column = "CEM_ACAO_AO_FINALIZAR_MONITORAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoAoFinalizarMonitoramento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoAoFinalizarMonitoramento? AcaoAoFinalizarMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalizarMonitoramentoEmAndamentoDoVeiculoAoIniciar", Column = "CEM_FINALIZAR_MONITORAMENTO_EM_ANDAMENTO_DO_VEICULO_AO_INICIAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarMonitoramentoEmAndamentoDoVeiculoAoIniciar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirFiltroEColunaCodigoPedidoClienteGestaoDocumentos", Column = "CEM_EXIBIR_FILTRO_E_COLUNA_CODIGO_PEDIDO_CLIENTE_GESTAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirFiltroEColunaCodigoPedidoClienteGestaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_CENTRO_RESULTADO_NO_RATEIO_DESPESA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCentroResultadoNoRateioDespesaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PEDIDO_OCORRENCIA_COLETA_ENTREGA_INTEGRACAO_NOVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoOcorrenciaColetaEntregaIntegracaoNova { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HABILITAR_ESTADO_PASSOU_RAIO_SEM_CONFIRMAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarEstadoPassouRaioSemConfirmar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_HABILITAR_ICONE_ENTREGA_ATRASADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarIconeEntregaAtrasada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_TEMPO_MINIMO_ACIONAR_PASSOU_RAIO_SEM_CONFIRMAR", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoMinimoAcionarPassouRaioSemConfirmar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegistrarEntregasApenasAposAtenderTodasColetas", Column = "CEM_REGISTRAR_ENTREGAS_APENAS_APOS_ATENDER_TODAS_COLETAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegistrarEntregasApenasAposAtenderTodasColetas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarDescontoGestaoDocumento", Column = "CEM_HABILITAR_DESCONTO_GESTAO_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarDescontoGestaoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CopiarDataTerminoCarregamentoCargaParaPrevisaoEntregaPedidos", Column = "CEM_COPIAR_DATA_TERMINO_CARREGAMENTO_CARGA_PARA_PREVISAO_ENTREGA_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CopiarDataTerminoCarregamentoCargaParaPrevisaoEntregaPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarContratoTerceiroSemInformacaoDoFrete", Column = "CEM_GERAR_CONTRATO_TERCEIRO_SEM_INFORMACAO_DO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarContratoTerceiroSemInformacaoDoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_LIBERAR_INTEGRACAO_TRANSPORTADOR_CARGA_IMPORTAR_DOCUMENTO_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarIntegracaoTransportadorDeCargaImportarDocumentoManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_VALIDAR_TABELA_FRETE_MESMA_INCIDENCIA_IMPORTACAO_TABELA_ARQUIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarTabelaFreteMesmaIncidenciaImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MaximoThreadsImportacaoTabelaFrete", Column = "CEM_MAXIMO_THREADS_IMPORTACAO_TABELA_FRETE_ARQUIVO", TypeType = typeof(int), NotNull = false)]
        public virtual int MaximoThreadsImportacaoTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_USAR_ALCADA_APROVACAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarAlcadaAprovacaoGestaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "CEM_MOTIVO_RETENCAO_INICIO_VIAGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.MotivoChamado MotivoChamadoRetencaoInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_ADIANTAMENTO_FORNECEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoContaAdiantamentoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_ADIANTAMENTO_CLIENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoContaAdiantamentoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SolicitarValorFretePorTonelada", Column = "CEM_SOLICITAR_VALOR_FRETE_POR_TONELADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarValorFretePorTonelada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarMultiplosModelosVeicularesPedido", Column = "CEM_UTILIZAR_MULTIPLOS_MODELOS_VEICULARES_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarMultiplosModelosVeicularesPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoRetornarNotasEmDocumentoComplementar", Column = "CEM_NAO_RETORNAR_NOTAS_EM_DOCUMENTO_COMPLEMENTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRetornarNotasEmDocumentoComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SolicitarAprovacaoFolgaAcertoViagem", Column = "CEM_SOLICITAR_APROVACAO_FOLGA_ACERTO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarAprovacaoFolgaAcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirDarBaixaFaturasCTe", Column = "CEM_PERMITIR_BAIXA_FATURAS_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirDarBaixaFaturasCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirPesoCargaEPesoCubadoGestaoDocumentos", Column = "CEM_EXIBIR_PESO_CARGA_E_PESO_CUBADO_GESTAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPesoCargaEPesoCubadoGestaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamentoCIOT", Column = "CEM_TIPO_PAGAMENTO_CIOT", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? TipoPagamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarBotaoImportacao", Column = "CEM_ATIVAR_BOTAO_IMPORTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarBotaoImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarDadosTransporteCargaCancelada", Column = "CEM_UTILIZAR_DADOS_TRANSPORTE_CARGA_CANCELADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDadosTransporteCargaCancelada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RelatorioEntregaPorPedido", Column = "CEM_RELATORIO_ENTREGA_POR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RelatorioEntregaPorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConcatenarFrotaPlaca", Column = "CEM_CONCATENAR_FROTA_PLACA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConcatenarFrotaPlaca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoObrigarInformarFrotaNoAcertoDeViagem", Column = "CEM_NAO_OBRIGAR_INFORMAR_FROTA_NO_ACERTO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoObrigarInformarFrotaNoAcertoDeViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirConsultaDeValoresPedagio", Column = "CEM_PERMITIR_CONSULTA_VALORES_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirConsultaDeValoresPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERCENTUAL_ADIANTAMENTO_TERCEIRO_PADRAO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualAdiantamentoTerceiroPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERCENTUAL_MINIMO_ADIANTAMENTO_TERCEIRO_PADRAO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualMinimoAdiantamentoTerceiroPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERCENTUAL_MAXIMO_ADIANTAMENTO_TERCEIRO_PADRAO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualMaximoAdiantamentoTerceiroPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirEnviarEmailAutorizacaoEmbarque", Column = "CEM_PERMITIR_ENVIAR_EMAIL_AUTORIZACAO_EMBARQUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirEnviarEmailAutorizacaoEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERCENTUAL_COMISSAO_PADRAO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualComissaoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERCENTUAL_MEDIA_EQUIVALENTE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualMediaEquivalente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERCENTUAL_EQUIVALE_EQUIVALENTE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualEquivaleEquivalente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERCENTUAL_ADVERTENCIA_EQUIVALENTE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualAdvertenciaEquivalente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirAnexosNoCadastroDoTransportador", Column = "CEM_EXIGIR_ANEXOS_NO_CADASTRO_DO_TRANSPORTAODR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirAnexosNoCadastroDoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirCancelarPedidosSemDocumentos", Column = "CEM_PERMITIR_CANCELAR_PEDIDOS_SEM_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirCancelarPedidosSemDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VisualizarVeiculosPropriosETerceiros", Column = "CEM_VISUALIZAR_VEICULOS_PROPRIOS_E_TERCEIROS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarVeiculosPropriosETerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_USAR_PESO_NOTAS_PALLET", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUsarPesoNotasPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_LANCAMENTO_OUTRAS_DESPESAS_DENTRO_PERIODO_ACERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirLancamentoOutrasDespesasDentroPeriodoAcerto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_VISUALIZAR_RECIBO_POR_MOTORISTA_NO_ACERTO_DE_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarReciboPorMotoristaNoAcertoDeViagem { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarCubagemPedidoSeNFeSemCubagem", Column = "CEM_USAR_CUBAGEM_PEDIDO_SE_NFE_SEM_CUBAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarCubagemPedidoSeNFeSemCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitirCTesSeparandoPorTipoCarga", Column = "CEM_EMITIR_CTES_SEPARANDO_POR_TIPO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirCTesSeparandoPorTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_INATIVAR_NOTAS_AO_CANCELAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoInativarNotasAoCancelarCarga { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_EMITIR_NFS_MANUAL_PARA_TRANSPORTADOR_FILIAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirNFSManualParaTransportadorEFiliais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LinkUrlAcessoCliente", Column = "CEM_LINK_URL_ACESSO_CLIENTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string LinkUrlAcessoCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTabelaFrete", Column = "CTF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete ConfiguracaoTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INCLUIR_CARGA_CANCELADA_PROCESSAR_DT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirCargaCanceladaProcessarDT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_PERMITIR_ADICIONAR_ANEXOS_CHECKLIST_GESTAO_PATIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAdicionarAnexosCheckListGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_COMPRAR_VP_INF_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoComprarValePedagioViaIntegracaoSeInformadoManualmenteNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_MESMO_NUMERO_CRT_CANCELAMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarMesmoNumeroCRTCancelamentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_MESMO_NUMERO_MICDTA_CANCELAMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarMesmoNumeroMICDTACancelamentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_MESMO_NUMERO_CRT_AVERBACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCRTAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoExibirCodigoIntegracaoDoDestinatarioResumoCarga", Column = "CEM_NAO_EXIBIR_CODIGO_INTEGRACAO_DESTINATARIO_RESUMO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirCodigoIntegracaoDoDestinatarioResumoCarga { get; set; }

        // #613 - Configuração de uso exclusivo Danone
        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarRegraExclusivaCodigoImpostoLayoutINTNC", Column = "CEM_ENVIAR_REGRA_EXCLUSIVA_CODIGO_IMPOSTO_LAYOUT_INTNC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarRegraExclusivaCodigoImpostoLayoutINTNC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarICMSTelaCotacaoPedidosRegraICMS", Column = "CEM_VALIDAR_ICMS_TELA_COTACAO_PEDIDOS_REGRA_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarICMSTelaCotacaoPedidosRegraICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DesconsiderarSobraRateioParaBaseCalculoIBSCBS", Column = "CEM_DESCONSIDERAR_SOBRA_RATEIO_PARA_BASE_CALCULO_IBS_CBS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesconsiderarSobraRateioParaBaseCalculoIBSCBS { get; set; }

        //FAVOR ADICIONAREM AS NOVAS CONFIGURAÇÕES EM TABELAS RELACIONADAS, JÁ TEM VÁRIAS CRIADAS, SEGUIR EXEMPLO CASO AINDA NÃO TENHA
        //QUANDO CRIAR TABELA NOVA, FAZER O INSERT DO PRIMEIRO REGISTRO
    }
}
