/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Consultas/MotivoChamado.js" />
/// <reference path="../../Consultas/GrupoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumEmitenteTipoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumOrigemOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumPeriodoAcordoContratoFreteTransportador.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoDocumentoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoInclusaoImpostoComplemento.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumTomadorTipoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumParametroRateioFormula.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Enumeradores/EnumClassificacaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoEmissao.js" />
/// <reference path="Configuracao.js" />
/// <reference path="FiltroPeriodo.js" />
/// <reference path="Gatilho.js" />
/// <reference path="Parametro.js" />
/// <reference path="PermissaoPerfilAcesso.js" />
/// <reference path="Notificacao.js" />
/// <reference path="CanaisDeEntrega.js" />
/// <reference path="Causas.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _configuracaoEmissaoCTeOpcoesTipoIntegracao;
var _gridTipoOcorrencia;
var _pesquisaTipoOcorrencia;
var _tipoOcorrencia;
var _crudTipoOcorrencia;
var _carregandoEdicaoTipoOcorrencia = false;


var _cst = [
    { text: "Não Informado", value: "" },
    { text: "00 - tributação normal ICMS", value: "00" },
    { text: "20 - tributação com BC reduzida do ICMS", value: "20" },
    { text: "40 - ICMS isenção", value: "40" },
    { text: "41 - ICMS não tributada", value: "41" },
    { text: "51 - ICMS diferido", value: "51" },
    { text: "60 - ICMS cobrado anteriormente por substituição tributária", value: "60" },
    { text: "90 - ICMS outros", value: "91" },
    { text: "90 - ICMS devido à UF de origem da prestação, quando diferente da UF do emitente", value: "90" },
    { text: "Simples Nacional", value: "SN" }
];

/*
 * Declaração das Classes
 */

var PesquisaTipoOcorrencia = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.Descricao.getFieldDescription(), issue: 586 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Ocorrencias.TipoOcorrencia.Situacao.getFieldDescription(), issue: 556 });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.MotivoAtendimento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.MotivoAtendimento.getFieldDescription(), idBtnSearch: guid() });
    this.SomenteTiposDeOcorrenciaUtilizadosControleEntrega = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.TipoOcorrencia.SomenteOcorrenciasUtilizadasParaControle, def: false });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoOcorrencia.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var TipoOcorrencia = function () {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.Descricao.getRequiredFieldDescription(), issue: 586, required: true, maxlength: 100, visible: ko.observable(true) });
    this.PrefixoFaturamentoOutrosModelos = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.PrefiroFaturamentoOutrosModelos.getFieldDescription(), required: false, maxlength: 3, visible: ko.observable(true) });
    this.CodigoProceda = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.CodigoIntegracao.getFieldDescription(), issue: 15, maxlength: 50, visible: ko.observable(true) });
    this.CodigoObservacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.CodigoObservacao.getFieldDescription(), issue: 0, maxlength: 50, visible: ko.observable(true) });
    this.CaracteristicaAdicionalCTe = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.CaracteristicaAdicionalCTe.getFieldDescription(), maxlength: 15, visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ val: ko.observable(true), options: EnumTipoOcorrencia.obterOpcoes(), def: true, text: Localization.Resources.Ocorrencias.TipoOcorrencia.DescricaoTipoOcorrencias.getRequiredFieldDescription(), issue: 912, visible: ko.observable(true) });
    this.TipoEmissaoDocumentoOcorrencia = PropertyEntity({ val: ko.observable(EnumTipoEmissaoDocumentoOcorrencia.Todos), options: EnumTipoEmissaoDocumentoOcorrencia.obterOpcoes(), def: EnumTipoEmissaoDocumentoOcorrencia.Todos, text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoEmissaoDocumentos.getRequiredFieldDescription(), visible: ko.observable(true) });
    this.CSTFilialEmissora = PropertyEntity({ val: ko.observable(""), options: _cst, text: Localization.Resources.Ocorrencias.TipoOcorrencia.CSTFilialEmissora.getRequiredFieldDescription(), visible: ko.observable(false) });
    this.CSTSubContratada = PropertyEntity({ val: ko.observable(""), options: _cst, text: Localization.Resources.Ocorrencias.TipoOcorrencia.CSTSubContratada.getRequiredFieldDescription(), visible: ko.observable(true) });
    this.TipoInclusaoImpostoComplemento = PropertyEntity({ val: ko.observable(EnumTipoInclusaoImpostoComplemento.ConformeCTeAnterior), options: EnumTipoInclusaoImpostoComplemento.obterOpcoes(), def: EnumTipoInclusaoImpostoComplemento.ConformeCTeAnterior, text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoInclusaoImposto.getFieldDescription(), visible: ko.observable(true) });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoPessoa.getFieldDescription(), issue: 306, required: true, visible: ko.observable(true) });
    this.OrigemOcorrencia = PropertyEntity({ val: ko.observable(EnumOrigemOcorrencia.PorCarga), options: EnumOrigemOcorrencia.obterOpcoes(), def: EnumOrigemOcorrencia.PorCarga, text: Localization.Resources.Ocorrencias.TipoOcorrencia.OrigemOcorrencia.getFieldDescription(), issue: 911, required: true, visible: ko.observable(true) });
    this.Emitente = PropertyEntity({ val: ko.observable(false), options: EnumEmitenteTipoOcorrencia.obterOpcoes(), def: EnumEmitenteTipoOcorrencia.IgualAoDocumentoAnterior, text: Localization.Resources.Ocorrencias.TipoOcorrencia.Emitente.getFieldDescription(), required: true, visible: ko.observable(true) });
    this.OutroEmitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.OutroEmitente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Tomador = PropertyEntity({ val: ko.observable(false), options: EnumTomadorTipoOcorrencia.obterOpcoes(), def: EnumTomadorTipoOcorrencia.IgualAoDocumentoAnterior, text: Localization.Resources.Ocorrencias.TipoOcorrencia.Tomador.getFieldDescription(), required: true, visible: ko.observable(true) });
    this.FinalidadeTipoOcorrencia = PropertyEntity({ val: ko.observable(""), options: EnumFinalidadeTipoOcorrencia.obterOpcoes(), def: "", text: Localization.Resources.Ocorrencias.TipoOcorrencia.Finalidade.getFieldDescription(), issue: 1060, cssClass: ko.observable("col col-xs-12 col-sm-6 col-md-2 col-lg-2"), visible: ko.observable(true) });
    this.OutroTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.OutroTomador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.GrupoPessoas.getFieldDescription(), issue: 58, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.Pessoa.getFieldDescription(), issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visible: ko.observable(true), enable: ko.observable(true), text: Localization.Resources.Ocorrencias.TipoOcorrencia.ComponenteFrete.getFieldDescription(), issue: 85, idBtnSearch: guid() });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visible: ko.observable(true), enable: ko.observable(true), text: Localization.Resources.Ocorrencias.TipoOcorrencia.ModeloDocumento.getFieldDescription(), issue: 1644, idBtnSearch: guid(), eventChange: modeloDocumentoBlur });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Ocorrencias.TipoOcorrencia.Situacao.getRequiredFieldDescription(), cssClass: ko.observable("col col-xs-12 col-sm-2 col-md-3 col-lg-3") });
    this.DiasAprovacaoAutomatica = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.DiasAprovacaoAutomatica.getFieldDescription(), getType: typesKnockout.int, maxlength: 11, visible: ko.observable(true) });
    this.PrazoSolicitacaoOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.PrazoSolicitacaoOcorrencia.getFieldDescription(), getType: typesKnockout.int, maxlength: 11, visible: ko.observable(true) });
    this.HorasSemFranquia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.HorasSemFranquia.getFieldDescription(), getType: typesKnockout.int, maxlength: 11, visible: ko.observable(false) });
    this.HorasToleranciaEntradaSaidaRaio = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.HorasToleranciaEntradaSaidaRaio.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(false) });
    this.TipoClassificacaoOcorrencia = PropertyEntity({ val: ko.observable(""), options: EnumClassificacaoOcorrencia.obterOpcoes(), def: "", text: Localization.Resources.Ocorrencias.TipoOcorrencia.ClassificacaoOcorrencia.getFieldDescription(), visible: ko.observable(true) });
    this.BloqueiaOcorrenciaDuplicada = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.BloqueiaOcorrenciaDuplicada.getFieldDescription(), issue: 917, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.TodosCTesSelecionados = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.TodosCTesSelecionados.getFieldDescription(), issue: 1370, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.BloquearOcorrenciaDuplicadaCargaMesmoMDFe = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.BloquearOcorrenciaDuplicadaCargaMesmoMDFe, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.BloquearOcorrenciaCargaCanhotoDigitalizadoAprovado = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.BloquearOcorrenciaCargaCanhotoDigitalizadoAprovado, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NaoPermiteSelecionarTodosCTes = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.NaoPermiteSelecionarTodosCTes, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.AnexoObrigatorio = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ObrigatorioInformarAenxo, issue: 916, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.OcorrenciaExclusivaParaIntegracao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ExclusivaParaIntegracao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PermiteInformarValor = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.PermiteQueUsuarioInformeValor, issue: 913, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.CalculaValorPorTabelaFrete = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.CalcularValorComplementoTabelaFrte, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PermiteInformarAprovadorResponsavel = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.PermitirInformarAprovadorResponsavelAlcada, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PermiteSelecionarTomador = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.PermiteQueUsuarioSelecioneTomador, issue: 914, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.FiltrarCargasPeriodo = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.FiltrarCargasPeriodo, issue: 915, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.OcorrenciaComplementoValorFreteCarga = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ComplementaValorFreteCarga, issue: 1366, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.OcorrenciaComVeiculo = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.OcorrenciaComVeiculo, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.UsarMobile = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.HabilitarUsoOcorrenciaSuperApp, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NaoGerarIntegracao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.NaoGerarIntegracao, issue: 1681, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.OcorrenciaDestinadaFranquias = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.OcorrenciaDestinadaFranquias, issue: 2048, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.EntregaRealizada = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.IndicaEntregaFoiRealizada, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.OcorrenciaPorQuantidade = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.OcorrenciaPorQuantidade, issue: 2058, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.OcorrenciaPorPercentualDoFrete = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.OcorrenciaPorcentagemFrete, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.CalcularValorCTEComplementarPeloValorCTESemImposto = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.CalcularValorCTEComplementarPeloValorCTESemImposto, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.TipoRateio = PropertyEntity({ val: ko.observable(EnumParametroRateioFormula.todos), options: EnumTipoRateio.obterOpcoes(), text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoRateio, def: EnumParametroRateioFormula.todos, visible: ko.observable(false) });
    this.TipoIntegracao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.Integracao, getType: typesKnockout.selectMultiple, val: ko.observable([]), options: _configuracaoEmissaoCTeOpcoesTipoIntegracao, def: [], required: ko.observable(false), visible: ko.observable(true) });
    this.ExigirInformarObservacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ExigirInformarObservacao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.BuscarCSTQuandoDocumentoOrigemIsento = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.BuscarNovamenteRegraDosImpostos, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ExigirChamadoParaAbrirOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ExigirAtendimentoParaAbrirOcorrencia, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(!_CONFIGURACAO_TMS.ExigirChamadoParaAbrirOcorrencia) });
    this.PeriodoOcorrencia = PropertyEntity({ val: ko.observable(EnumPeriodoAcordoContratoFreteTransportador.Quinzenal), options: EnumPeriodoAcordoContratoFreteTransportador.obterOpcoes(), def: EnumPeriodoAcordoContratoFreteTransportador.Quinzenal, text: Localization.Resources.Ocorrencias.TipoOcorrencia.PeriodoOcorrencia, visible: ko.observable(false) });
    this.TipoCTeIntegracao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoCTeIntegracao, issue: 1183, required: false, maxlength: 150, visible: ko.observable(true) });
    this.TipoEmissaoIntramunicipal = PropertyEntity({ val: ko.observable(EnumTipoEmissaoIntramunicipal.NaoEspecificado), options: EnumTipoEmissaoIntramunicipal.obterOpcoes(), def: EnumTipoEmissaoIntramunicipal.NaoEspecificado, text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoEmissaoParaOcorrenciaMunicipais, issue: 606, enable: ko.observable(true), required: true, visible: ko.observable(true) });
    this.Valor = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ValorQauntiade, getType: typesKnockout.decimal, visible: ko.observable(false) });
    this.PercentualDoFrete = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ValorPorcentagemFrete, getType: typesKnockout.decimal, maxlength: 6, visible: ko.observable(false) });
    this.AdicionarPISCOFINS = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.AdicionarPISCOFINS, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.AdicionarPISCOFINSBaseCalculoICMS = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.AdicionarPISCOFINSBaseCalculoICMS, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.SomenteCargasFinalizadas = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.SomenteCargasFinalizadas, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ExigirMotivoDeDevolucao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ExigirMotivoDeDevolucao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.OcorrenciaTerceiros = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.OcorrenciaTerceiros, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.OcorrenciaPorNotaFiscal = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.OcorrenciaPorNotaFiscal, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.DisponibilizarDocumentosParaNFsManual = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.DisponibilizarDocumentosParaNFsManual, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PermiteAlterarNumeroDocumentoOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.PermiteAlterarNumeroDocumentoOcorrencia, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.ExibirParcelasNaAprovacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ExibirParcelasNaAprovacao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.OcorrenciaParaCobrancaDePedagio = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.OcorrenciaParaCobrancaDePedagio, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.OcorrenciaPorAjudante = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.OcorrenciaPorAjudante, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.UtilizarParcelamentoAutomatico = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.UtilizarParcelamentoAutomatico, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.FiltrarOcorrenciasPeriodoPorFilial = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.FiltrarOcorrenciasPeriodoPorFilial, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.NaoGerarDocumento = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.NaoGerarDocumento, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.DataComplementoIgualDataOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.DataComplementoIgualDataOcorrencia, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.GerarApenasUmComplemento = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.GerarApenasUmComplemento, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.OcorrenciaExclusivaParaCanhotosDigitalizados = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.OcorrenciaGeradaExclusivamenteParaCanhotos, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ExibirMotivoUltimaAprovacaoPortalTransportador = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ExibirMotivoUltimaAprovacaoPortalTransportador, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.EfetuarCalculoValorOcorrenciaBaseadoNotasDevolucao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.EfetuarCalculoValorOcorrenciaBaseadoNotasDevolucao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.SalvarCausadorDaOcorrenciaNaGestaoDeOcorrencias = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.SalvarCausadorDaOcorrenciaNaGestaoDeOcorrencias, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.EfetuarOControleQuilometragem = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.EfetuaControleQuilometragem, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PermitirSelecionarEssaOcorrenciaNoPortalDoCliente = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.PermitirSelecionarEssaOcorrenciaNoPortalDoCliente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.DataOcorrenciaIgualDataCTeComplementado = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.DataOcorrenciaIgualDataCTeComplementado, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.BloquearAberturaAtendimentoParaVeiculoEmContratoFrete = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.BloquearAberturaAtendimentoParaVeiculoEmContratoFrete, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NaoPermiteInformarValorDaOcorrenciaAoSelecionarAtendimentos = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.NaoPermiteInformarValorDaOcorrenciaAoSelecionarAtendimentos, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.GerarNFSeParaComplementosTomadorIgualDestinatario = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.GerarNFSeParaComplementosTomadorIgualDestinatario, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.InformarMotivoNaAprovacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.InformarMotivoNaAprovacao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.JustificativaPadraoAprovacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.JustificativaPadraoAprovacao, type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), enable: true, idBtnSearch: guid(), visible: ko.observable(false) });
    this.OcorrenciaProvisionada = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.OcorrenciaSupervisionada, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.UtilizarEntradaSaidaDoRaioCargaEntrega = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.UtilizarEntradaSaidaDoRaioCargaEntrega, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.GerarOcorrenciaComMesmoValorCTesAnteriores = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.GerarOcorrenciaComMesmoValorCTesAnteriores, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.GerarOcorrenciaComValorGrossPedido = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.GerarOcorrenciaComValorDasNotas, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.OcorrenciaDiferencaValorFechamento = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoOcorrenciaParaDiferencaValorFechamento, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ImpedirCriarOcorrenciaCasoExistirCanhotosPendentes = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ImpedirCriarOcorrenciaCasoExistirCanhotosPendentes, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NaoPermitirValorSuplementoMaiorQueDocumento = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.NaoPermitirValorSuplementoMaiorQueDocumento, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.InformarProdutoLancamentoOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.InformarProdutoLancamentoOcorrencia, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PermitirAlterarDataPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.PermitirAlterarDataPrevisaoEntrega, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PermitirInformarGrupoOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.PermiteInformarGrupoOcorrencia, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PermitirEnviarOcorrenciaSemAprovacaoPreCTe = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.PermitirEnviarOcorrenciaSemAprovacaoPreCTe, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NaoCalcularValorOcorrenciaAutomaticamente = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.NaoCalcularValorOcorrenciaAutomaticamente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.CanaisDeEntrega = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.TipoConhecimentoProceda = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoConhecimentoProceda, required: false, maxlength: 1, visible: ko.observable(true) });

    this.TipoOcorrenciaControleEntrega = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.EsseTipoOcorrenciaSeraUtilizadoControleEntrega, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NaoIndicarAoCliente = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.NaoIndicarOcorrenciasDesseTipoCliente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NaoAlterarSituacaoColetaEntrega = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.NaoAlterarSituacaoColetaEntrega, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.UsadoParaMotivoRejeicaoColetaEntrega = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.EsseTipoOcorrenciaSeraUtilizadoComoMotivoRejeicao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.TipoAplicacaoColetaEntrega = PropertyEntity({ val: ko.observable(EnumTipoAplicacaoColetaEntrega.Entrega), options: EnumTipoAplicacaoColetaEntrega.obterOpcoes(), def: EnumTipoAplicacaoColetaEntrega.Entrega, text: Localization.Resources.Ocorrencias.TipoOcorrencia.AplicacaoOcorrenciaPara, visible: ko.observable(false) });
    this.TipoOperacaoColeta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoOperacao, idBtnSearch: guid() });
    this.MotivoChamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.MotivoAtendimentoOcorrenciaPrecisaSerAprovadaAtendimento, issue: 72560, idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.GrupoOcorrencia, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DescricaoAuxiliar = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.DescricaoAuxiliar.getFieldDescription(), required: false, maxlength: 100, visible: ko.observable(true) });
    this.CodigoIntegracaoAuxiliar = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.CodigoIntegracaoAuxiliar.getFieldDescription(), maxlength: 50, visible: ko.observable(true) });
    this.FreeTime = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.FreeTime.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true) });

    this.TipoProposta = PropertyEntity({ val: ko.observable(0), options: EnumTipoProposta.obterOpcoes(), def: "", text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoProposta.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadeProjetoNFTP) });
    this.TipoReceita = PropertyEntity({ val: ko.observable(0), options: EnumTipoReceita.obterOpcoesOcorrencia(), def: "", text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoReceita.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadeProjetoNFTP) });
    this.RemarkSped = PropertyEntity({ val: ko.observable(EnumRemarkSped.OutrosServicos), options: EnumRemarkSped.obterOpcoes(), def: EnumRemarkSped.OutrosServicos, text: Localization.Resources.Ocorrencias.TipoOcorrencia.RemarkSped.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.AtivarIntegracaoNFTPEMP) });

    this.GerarPedidoDevolucaoAutomaticamente = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.GerarPedidoDevolucaoAutomaticamente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.DisponibilizarPedidoParaNovaIntegracao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.DisponibilizarPedidoParaNovaIntegracao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.TipoOperacaoDevolucao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoOperacaoPedidoDevolucao, idBtnSearch: guid(), visible: ko.observable(false) });
    this.PermiteInformarSobras = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.PermiteInformarSobras, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ApresentarValorPesoDaCarga = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ApresentarValorPesoDaCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.DebitaFreeTimeCalculoValorOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.DebitaFreeTimeCalculoValorOcorrencia, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NovaOcorrenciaAguardandoInformacoes = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.NovaOcorrenciaAguardandoInformacoes, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.ExigirCodigoParaAprovacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.ExigeCodigoParaAprovacao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(_CONFIGURACAO_TMS.Pais == EnumPaises.Exterior) });
    this.CalcularDistanciaPorCTe = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.CalcularDistanciaPorCTe, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.CopiarObservacoesDoCTeDeOrigemAoGerarCTeComplementar = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.CopiarObservacoesDoCTeDeOrigemAoGerarCTeComplementar, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.GerarAtendimentoAutomaticamente = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.GerarAtendimentoAutomaticamente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.MotivoChamadoGeracaoAutomatica = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.MotivoChamadoGeracaoAutomatica, idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });
    this.GerarOcorrenciaAutomaticamenteRejeicaoMobile = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.GerarOcorrenciaAutomaticamenteRejeicaoMobile, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.EmitirDocumentoParaFilialEmissoraComPreCTe = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.EmitirDocumentoParaFilialEmissoraComPreCTe, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PermitirReabrirOcorrenciaEmCasoDeRejeicao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.PermitirReabrirOcorrenciaEmCasoDeRejeicao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.OcorrenciaParaQuebraRegraPallet = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.OcorrenciaParaQuebraRegraPallet, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PermiteInformarCausadorOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.PermiteInformarCausadorOcorrencia, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.IdSuperApp = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.IdSuperApp, maxlength: 24, visible: ko.observable(true), enable: false });
    this.ChecklistSuperApp = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: ko.observable(""), text: Localization.Resources.Ocorrencias.TipoOcorrencia.ChecklistSuperApp, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: ko.observable(false) });

    this.GerarAtendimentoAutomaticamente.val.subscribe(function (novoValor) {
        _tipoOcorrencia.MotivoChamadoGeracaoAutomatica.visible(novoValor);
        _tipoOcorrencia.MotivoChamadoGeracaoAutomatica.required(novoValor);
    })

    this.Emitente.val.subscribe(emitenteChange);
    this.FiltrarCargasPeriodo.val.subscribe(controlarExibicaoAbaFiltroPeriodo);
    this.OcorrenciaPorQuantidade.val.subscribe(ocorrenciaPorQuantidadeChange);
    this.OcorrenciaPorPercentualDoFrete.val.subscribe(ocorrenciaPorPercentualDoFreteChange);
    this.OrigemOcorrencia.val.subscribe(origemOcorrenciaChange);
    this.Tomador.val.subscribe(tomadorChange);
    this.TipoIntegracao.val.subscribe(tipoIntegracaoChange);
    this.ModeloDocumentoFiscal.val.subscribe(modeloDocumentoFiscalChange);

    this.TipoEmissaoDocumentoOcorrencia.val.subscribe(TipoEmissaoDocumentoOcorrenciaChange);
    this.UsarMobile.val.subscribe(function (novoValor) {
        if (novoValor && _CONFIGURACAO_TMS.VersaoSuperAppV3) {
            _tipoOcorrencia.ChecklistSuperApp.required(true);
        } else {
            _tipoOcorrencia.ChecklistSuperApp.required(false);
        }
    });


    this.DescricaoPortal = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.DescricaoPortal.getFieldDescription(), maxlength: 150 });
    this.TagNumeroCarga = PropertyEntity({ eventClick: function (e) { InserirTag(self.DescricaoPortal.id, "#NumeroCarga"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.NumeroCarga, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagNomeCliente = PropertyEntity({ eventClick: function (e) { InserirTag(self.DescricaoPortal.id, "#NomeCliente"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.NomeCliente, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagNomeRemetente = PropertyEntity({ eventClick: function (e) { InserirTag(self.DescricaoPortal.id, "#NomeRemetente"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.NomeRemetente, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagCidadeOrigem = PropertyEntity({ eventClick: function (e) { InserirTag(self.DescricaoPortal.id, "#CidadeOrigem"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.CidadeOrigem, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagCidadeDestino = PropertyEntity({ eventClick: function (e) { InserirTag(self.DescricaoPortal.id, "#CidadeDestino"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.CidadeDestino, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagMotivoOcorrencia = PropertyEntity({ eventClick: function (e) { InserirTag(self.DescricaoPortal.id, "#MotivoOcorrencia"); }, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.MotivoOcorrencia, enable: ko.observable(true), visible: ko.observable(true) });

    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoPessoaGrupo.Pessoa) {
            _tipoOcorrencia.Pessoa.visible(true);
            _tipoOcorrencia.GrupoPessoas.visible(false);
            LimparCampoEntity(_tipoOcorrencia.GrupoPessoas);
        }
        else if (novoValor == EnumTipoPessoaGrupo.GrupoPessoa) {
            _tipoOcorrencia.Pessoa.visible(false);
            _tipoOcorrencia.GrupoPessoas.visible(true);
            LimparCampoEntity(_tipoOcorrencia.Pessoa);
        }
    });

    this.ExibirParcelasNaAprovacao.val.subscribe(function (novoValor) {
        _tipoOcorrencia.DiasAprovacaoAutomatica.visible(true);
        if (novoValor) {
            _tipoOcorrencia.DiasAprovacaoAutomatica.visible(false);
            _tipoOcorrencia.UtilizarParcelamentoAutomatico.visible(true);
            LimparCampo(_tipoOcorrencia.DiasAprovacaoAutomatica);
        } else {
            _tipoOcorrencia.UtilizarParcelamentoAutomatico.visible(false);
        }
    });

    this.UtilizarEntradaSaidaDoRaioCargaEntrega.val.subscribe(function (novoValor) {
        _tipoOcorrencia.PermitirTransportadorInformarDataInicioFimRaioCarga.visible(novoValor);
        if (novoValor == false)
            _tipoOcorrencia.PermitirTransportadorInformarDataInicioFimRaioCarga.val(false);
    });

    this.AdicionarPISCOFINS.val.subscribe(function (novoValor) {
        _tipoOcorrencia.AdicionarPISCOFINSBaseCalculoICMS.visible(novoValor);
        if (!novoValor)
            _tipoOcorrencia.AdicionarPISCOFINSBaseCalculoICMS.val(false);
    });

    this.UsadoParaMotivoRejeicaoColetaEntrega.val.subscribe(function (novoValor) {
        if (novoValor) {
            _tipoOcorrencia.OcorrenciaPorNotaFiscal.visible(false);
            LimparCampo(_tipoOcorrencia.OcorrenciaPorNotaFiscal);
        } else {
            _tipoOcorrencia.OcorrenciaPorNotaFiscal.visible(true);
        }
    });

    //Permissões
    this.BloquearVisualizacaoTipoOcorrenciaTransportador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Ocorrencias.TipoOcorrencia.BloquearVisualizacaoTipoOcorrenciaTransportador });
    this.NaoPermitirQueTransportadorSelecioneTipoOcorrencia = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Ocorrencias.TipoOcorrencia.NaoPermitirQueTransportadorSelecioneTipoOcorrencia });
    this.PermitirTransportadorInformarDataInicioFimRaioCarga = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Ocorrencias.TipoOcorrencia.PermitirTransportadorInformarDataInicioFimRaioCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PermitirConsultarCTesComEsseTipoDeOcorrencia = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Ocorrencias.TipoOcorrencia.PermitirConsultarCTesComEsseTipoDeOcorrencia });
    this.AdicionarPerfilAcesso = PropertyEntity({ type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.AdicionarPerfilAcesso, idBtnSearch: guid() });
    this.GridPermissaoPerfilAcesso = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });

    //Integração
    this.QuantidadeReenvioIntegracao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.QuantidadeReenvioIntegracao.getFieldDescription(), getType: typesKnockout.int, maxlength: 11, visible: ko.observable(true) });
    this.GerarEventoIntegracaoCargaEntregaPorCarga = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.GerarEventoIntegracaoCargaEntregaPorCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.AtivarEnvioAutomaticoTipoOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.AtivarEnvioAutomaticoTipoOcorrencia, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoDiageo) });
    this.RecalcularPrevisaoEntregaPendentes = PropertyEntity({ text: "Recalcular previsão de entrega para entregas pendentes (restantes)", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.OcorrenciaFinalizaViagem = PropertyEntity({ text: "Essa Ocorrência finaliza a viagem", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.CodigoEventoOcorrenciaPrimeiro = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.CodigoEventoOcorrenciaPrimeiro });
    this.CodigoEventoOcorrenciaSegundo = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.CodigoEventoOcorrenciaSegundo });
    this.DescricaoTipoPrevisao = PropertyEntity({ text: "Descrição do tipo de previsão", val: ko.observable(""), visible: ko.observable(false) });
    this.EssaOcorrenciaGeraOutraOcorrenciaIntegracao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.EssaOcorrenciaGeraOutraOcorrenciaIntegracao, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TipoOcorrenciaIntegracao = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoOcorrenciaDescricao, type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), enable: true, idBtnSearch: guid(), visible: ko.observable(false) });

    this.EssaOcorrenciaGeraOutraOcorrenciaIntegracao.val.subscribe(function (novoValor) {
        _tipoOcorrencia.TipoOcorrenciaIntegracao.visible(novoValor);
        _tipoOcorrencia.TipoOcorrenciaIntegracao.required(novoValor);
    })

    //Integração Neokohm
    this.TipoEnvio = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoEnvio, val: ko.observable(EnumTipoEnvioIntegracaoNeokohm.NaoSelecionado), options: EnumTipoEnvioIntegracaoNeokohm.obterOpcoes(), def: EnumTipoEnvioIntegracaoNeokohm.NaoSelecionado, visible: ko.observable(true) });


    //Integração Comprovei
    this.AtivarIntegracaoComprovei = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.AtivarIntegracaoComprovei, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.TipoIntegracaoComprovei = PropertyEntity({ text: Localization.Resources.Ocorrencias.TipoOcorrencia.TipoDIntegracao, val: ko.observable(EnumTipoIntegracaoComprovei.obterOpcoes()), options: EnumTipoIntegracaoComprovei.obterOpcoes(), def: EnumTipoIntegracaoComprovei.obterOpcoes(), visible: ko.observable(true) });

};

var CRUDTipoOcorrencia = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.CancelarNovo, visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTipoOcorrencia() {
    var editar = { descricao: Localization.Resources.Ocorrencias.TipoOcorrencia.Editar, id: "clasEditar", evento: "onclick", metodo: editarTipoOcorrenciaClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();

    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoOcorrencia = new GridView(_pesquisaTipoOcorrencia.Pesquisar.idGrid, "TipoOcorrencia/Pesquisa", _pesquisaTipoOcorrencia, menuOpcoes, null);
    _gridTipoOcorrencia.CarregarGrid();
}

function loadTipoOcorrencia() {

    ObterTiposIntegracao().then(function () {
        _tipoOcorrencia = new TipoOcorrencia();
        KoBindings(_tipoOcorrencia, "knockoutCadastroTipoOcorrencia");

        HeaderAuditoria("TipoDeOcorrenciaDeCTe", _tipoOcorrencia);

        _crudTipoOcorrencia = new CRUDTipoOcorrencia();
        KoBindings(_crudTipoOcorrencia, "knockoutCRUDTipoOcorrencia");

        _pesquisaTipoOcorrencia = new PesquisaTipoOcorrencia();
        KoBindings(_pesquisaTipoOcorrencia, "knockoutPesquisaTipoOcorrencia", false, _pesquisaTipoOcorrencia.Pesquisar.id);

        new BuscarTiposOperacao(_pesquisaTipoOcorrencia.TipoOperacao);
        new BuscarMotivoChamado(_pesquisaTipoOcorrencia.MotivoAtendimento);
        new BuscarClientes(_tipoOcorrencia.Pessoa, null, true);
        new BuscarClientes(_tipoOcorrencia.OutroTomador);
        new BuscarEmpresa(_tipoOcorrencia.OutroEmitente);
        new BuscarGruposPessoas(_tipoOcorrencia.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
        new BuscarComponentesDeFrete(_tipoOcorrencia.ComponenteFrete);
        new BuscarModeloDocumentoFiscal(_tipoOcorrencia.ModeloDocumentoFiscal, retornoModeloDocumento, null, true);
        new BuscarMotivoChamado(_tipoOcorrencia.MotivoChamado);
        new BuscarMotivoChamado(_tipoOcorrencia.MotivoChamadoGeracaoAutomatica);
        new BuscarGrupoOcorrencia(_tipoOcorrencia.GrupoOcorrencia);
        new BuscarTiposOperacao(_tipoOcorrencia.TipoOperacaoColeta);
        new BuscarTiposOperacao(_tipoOcorrencia.TipoOperacaoDevolucao);
        new BuscarMotivoRejeicaoOcorrencia(_tipoOcorrencia.JustificativaPadraoAprovacao, null, EnumAprovacaoRejeicao.Aprovacao);
        new BuscarTipoOcorrencia(_tipoOcorrencia.TipoOcorrenciaIntegracao, null, null, null, null, null, null, null, true);
        new BuscarChecklistsSuperApp(_tipoOcorrencia.ChecklistSuperApp);


        controlarVisibilidadeCamposTipoOcorrencia();

        loadGridTipoOcorrencia();
        loadConfiguracaoTipoOcorrencia();
        loadGatilho();
        loadParametro();
        loadFiltroPeriodo();
        loadGridPermissaoPerfilAcesso();
        loadNotificacao();
        loadCanaisDeEntrega();
        loadCausas();
        configuraCST(_tipoOcorrencia.TipoEmissaoDocumentoOcorrencia.val());
    });
}

function controlarVisibilidadeCamposTipoOcorrencia() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _tipoOcorrencia.TipoPessoa.visible(false);
        _tipoOcorrencia.Pessoa.visible(false);
        _tipoOcorrencia.GrupoPessoas.visible(false);
        _tipoOcorrencia.Ativo.cssClass("col col-xs-12 col-sm-6 col-md-2 col-lg-2");
        _tipoOcorrencia.OcorrenciaDestinadaFranquias.visible(true);
        _tipoOcorrencia.OcorrenciaPorQuantidade.visible(true);
        _tipoOcorrencia.OcorrenciaPorPercentualDoFrete.visible(true);
        _tipoOcorrencia.EntregaRealizada.visible(false);
        _tipoOcorrencia.ExibirParcelasNaAprovacao.visible(true);
        _tipoOcorrencia.HorasSemFranquia.visible(false);
    }

    _tipoOcorrencia.TipoRateio.visible(!_CONFIGURACAO_TMS.RatearValorOcorrenciaPeloValorFreteCTeOriginal);
    _tipoOcorrencia.GerarEventoIntegracaoCargaEntregaPorCarga.visible(_configuracaoEmissaoCTeOpcoesTipoIntegracao.some(tipo => tipo['value'] === EnumTipoIntegracao.ArcelorMittal));
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function configuraCST(tipoEmissaoDocumento) {
    if (_CONFIGURACAO_TMS.PermitirDefinirCSTnoTipoDeOcorrencia) {
        if (tipoEmissaoDocumento == EnumTipoEmissaoDocumentoOcorrencia.Todos) {
            _tipoOcorrencia.CSTFilialEmissora.visible(true);
            _tipoOcorrencia.CSTSubContratada.visible(true);
        }
        else if (tipoEmissaoDocumento == EnumTipoEmissaoDocumentoOcorrencia.SomenteFilialEmissora) {
            _tipoOcorrencia.CSTFilialEmissora.visible(true);
            _tipoOcorrencia.CSTSubContratada.visible(false);
        }
        else if (tipoEmissaoDocumento == EnumTipoEmissaoDocumentoOcorrencia.SomenteSubcontratada) {
            _tipoOcorrencia.CSTFilialEmissora.visible(false);
            _tipoOcorrencia.CSTSubContratada.visible(true);
        }

    } else {
        _tipoOcorrencia.CSTFilialEmissora.visible(false);
        _tipoOcorrencia.CSTSubContratada.visible(false);
    }
}


function TipoEmissaoDocumentoOcorrenciaChange(origemSelecionada) {
    configuraCST(origemSelecionada);
}


function adicionarClick() {
    if (!validarCamposObrigatoriosTipoOcorrencia())
        return;

    executarReST("TipoOcorrencia/Adicionar", ObterTipoOcorrenciaSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.TipoOcorrencia.CadastradoComSucesso);
                _gridTipoOcorrencia.CarregarGrid();
                limparCamposTipoOcorrencia();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function atualizarClick() {
    if (!validarCamposObrigatoriosTipoOcorrencia())
        return;

    executarReST("TipoOcorrencia/Atualizar", ObterTipoOcorrenciaSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.TipoOcorrencia.AtualizadoComSucesso);
                _gridTipoOcorrencia.CarregarGrid();
                limparCamposTipoOcorrencia();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function cancelarClick() {
    limparCamposTipoOcorrencia();
}

function editarTipoOcorrenciaClick(registroSelecionado) {
    limparCamposTipoOcorrencia();

    executarReST("TipoOcorrencia/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaTipoOcorrencia.ExibirFiltros.visibleFade(false);
                _carregandoEdicaoTipoOcorrencia = true;

                PreencherObjetoKnout(_tipoOcorrencia, { Data: retorno.Data });

                recarregarGridCanais();

                preencherConfiguracaoTipoOcorrencia(retorno.Data.Configuracao);
                preencherFiltroPeriodo(retorno.Data.FiltrosPeriodo);
                preencherParametro(retorno.Data.Parametros);
                preencherGatilho(retorno.Data.Gatilho);
                preencherPerfisAcesso(retorno.Data.PerfisAcesso);
                preencherNotificacao(retorno.Data.Notificacao);
                preencherCausas(retorno.Data.Causas);
                controlarBotoesHabilitados();
                controleCamposModeloDocumento(retorno.Data.ModeloDocumentoFiscal);
                verificarTipoEmissaoIntramunicipal();
                habilitarAbaNotificacao();

                _carregandoEdicaoTipoOcorrencia = false;
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function emitenteChange() {
    if (_tipoOcorrencia.Emitente.val() == EnumEmitenteTipoOcorrencia.Outros) {
        _tipoOcorrencia.OutroEmitente.visible(true);
    } else {
        _tipoOcorrencia.OutroEmitente.visible(false);
    }
}

function excluirClick() {
    exibirConfirmacao(Localization.Resources.Ocorrencias.TipoOcorrencia.Confirmacao, Localization.Resources.Ocorrencias.TipoOcorrencia.RealmenteDesejaExcluirTipoOcorrencia.format(_tipoOcorrencia.Descricao.val()), function () {
        ExcluirPorCodigo(_tipoOcorrencia, "TipoOcorrencia/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.TipoOcorrencia.ExcluidoComSucesso);
                    _gridTipoOcorrencia.CarregarGrid();
                    limparCamposTipoOcorrencia();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.TipoOcorrencia.Sugestao, retorno.Msg, 16000);
                }
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function modeloDocumentoBlur() {
    if (_tipoOcorrencia.ModeloDocumentoFiscal.val() == "") {
        _tipoOcorrencia.ModeloDocumentoFiscal.codEntity(0);
        verificarTipoEmissaoIntramunicipal();
    }
}

function ocorrenciaPorQuantidadeChange() {
    if (_tipoOcorrencia.OcorrenciaPorQuantidade.val())
        _tipoOcorrencia.Valor.visible(true);
    else {
        _tipoOcorrencia.Valor.visible(false);
        LimparCampo(_tipoOcorrencia.Valor);
    }
}

function ocorrenciaPorPercentualDoFreteChange() {
    if (_tipoOcorrencia.OcorrenciaPorPercentualDoFrete.val()) {
        _tipoOcorrencia.PercentualDoFrete.visible(true);
        _tipoOcorrencia.CalcularValorCTEComplementarPeloValorCTESemImposto.visible(true);
    }
    else {
        _tipoOcorrencia.PercentualDoFrete.visible(false);
        _tipoOcorrencia.CalcularValorCTEComplementarPeloValorCTESemImposto.visible(false);
        LimparCampo(_tipoOcorrencia.PercentualDoFrete);
        LimparCampo(_tipoOcorrencia.CalcularValorCTEComplementarPeloValorCTESemImposto);
    }
}



function origemOcorrenciaChange(origemSelecionada) {
    if ((origemSelecionada == EnumOrigemOcorrencia.PorPeriodo) || (origemSelecionada == EnumOrigemOcorrencia.PorContrato)) {
        _tipoOcorrencia.PeriodoOcorrencia.visible(true);
        _tipoOcorrencia.FiltrarOcorrenciasPeriodoPorFilial.visible(true);
    }
    else {
        _tipoOcorrencia.PeriodoOcorrencia.val(EnumPeriodoAcordoContratoFreteTransportador.Quinzenal);
        _tipoOcorrencia.PeriodoOcorrencia.visible(false);
    }

    if (origemSelecionada == EnumOrigemOcorrencia.PorCarga) {
        _tipoOcorrencia.TodosCTesSelecionados.visible(true);
        _tipoOcorrencia.BloquearOcorrenciaDuplicadaCargaMesmoMDFe.visible(true);
        _tipoOcorrencia.BloquearOcorrenciaCargaCanhotoDigitalizadoAprovado.visible(true);
        _tipoOcorrencia.OcorrenciaComplementoValorFreteCarga.visible(true);
        _tipoOcorrencia.CalculaValorPorTabelaFrete.visible(true);
    }
    else {
        _tipoOcorrencia.TodosCTesSelecionados.visible(false);
        _tipoOcorrencia.TodosCTesSelecionados.val(false);
        _tipoOcorrencia.BloquearOcorrenciaDuplicadaCargaMesmoMDFe.visible(false);
        _tipoOcorrencia.BloquearOcorrenciaDuplicadaCargaMesmoMDFe.val(false);
        _tipoOcorrencia.BloquearOcorrenciaCargaCanhotoDigitalizadoAprovado.visible(false);
        _tipoOcorrencia.BloquearOcorrenciaCargaCanhotoDigitalizadoAprovado.val(false);
        _tipoOcorrencia.OcorrenciaComplementoValorFreteCarga.visible(false);
        _tipoOcorrencia.OcorrenciaComplementoValorFreteCarga.val(false);
        _tipoOcorrencia.CalculaValorPorTabelaFrete.val(false);
        _tipoOcorrencia.CalculaValorPorTabelaFrete.visible(false);
    }

    controlarExibicaoAbaGatilho();
}

function tomadorChange() {
    if (_tipoOcorrencia.Tomador.val() == EnumTomadorTipoOcorrencia.Outros) {
        _tipoOcorrencia.OutroTomador.visible(true);
    } else {
        _tipoOcorrencia.OutroTomador.visible(false);
    }
}

/*
 * Declaração das Funções Públicas
 */

function controlarExibicaoAbaParametro() {
    if (_gatilho.UtilizarGatilhoGeracaoOcorrencia.val() && _gatilho.GerarAutomaticamente.val())
        $("#liTabParametros").hide();
    else
        $("#liTabParametros").show();
}

/*
 * Declaração das Funções Privadas
 */

function controlarBotoesHabilitados() {
    var isEdicao = _tipoOcorrencia.Codigo.val() > 0;

    _crudTipoOcorrencia.Atualizar.visible(isEdicao);
    _crudTipoOcorrencia.Excluir.visible(isEdicao);
    _crudTipoOcorrencia.Adicionar.visible(!isEdicao);
}

function controlarExibicaoAbaFiltroPeriodo() {
    if (_tipoOcorrencia.FiltrarCargasPeriodo.val())
        $("#liTabFiltroPeriodo").show();
    else
        $("#liTabFiltroPeriodo").hide();
}

function controlarExibicaoAbaGatilho() {
    if (_tipoOcorrencia.OrigemOcorrencia.val() == EnumOrigemOcorrencia.PorCarga)
        $("#liTabGatilho").show();
    else {
        $("#liTabGatilho").hide();
        _gatilho.UtilizarGatilhoGeracaoOcorrencia.val(false);
    }
}

function limparCamposTipoOcorrencia() {
    LimparCampos(_tipoOcorrencia);
    limparCamposConfiguracaoTipoOcorrencia();
    limparCamposParametro();
    limparCamposFiltroPeriodo();
    limparCamposGatilho();
    limparCamposPermissoes();
    limparCamposNotificacao();
    controlarBotoesHabilitados();
    verificarTipoEmissaoIntramunicipal();
    limparGridCanaisDeEntrega();
    limparGridCausas();

    Global.ResetarAbas();
}

function ObterTipoOcorrenciaSalvar() {
    var tipoOcorrencia = RetornarObjetoPesquisa(_tipoOcorrencia);

    preencherConfiguracaoTipoOcorrenciaSalvar(tipoOcorrencia);
    preencherFiltroPeriodoSalvar(tipoOcorrencia);
    preencherParametroSalvar(tipoOcorrencia);
    preencherGatilhoSalvar(tipoOcorrencia);
    preencherPerfilAcesso(tipoOcorrencia);
    preencherNotificacaoSalvar(tipoOcorrencia);
    preencherGridCanaisDeEntregaSalvar(tipoOcorrencia);
    preencherCausasSalvar(tipoOcorrencia);

    return tipoOcorrencia;
}

function ObterTiposIntegracao() {
    var p = new promise.Promise();

    var tiposIntegracao = [
        EnumTipoIntegracao.Marfrig,
        EnumTipoIntegracao.Minerva,
        EnumTipoIntegracao.InteliPost,
        EnumTipoIntegracao.Magalog,
        EnumTipoIntegracao.MagalogEscrituracao,
        EnumTipoIntegracao.Boticario,
        EnumTipoIntegracao.Riachuelo,
        EnumTipoIntegracao.Dansales,
        EnumTipoIntegracao.AX,
        EnumTipoIntegracao.Emillenium,
        EnumTipoIntegracao.VTEX,
        EnumTipoIntegracao.Cobasi,
        EnumTipoIntegracao.Isis,
        EnumTipoIntegracao.Havan,
        EnumTipoIntegracao.Simonetti,
        EnumTipoIntegracao.Marisa,
        EnumTipoIntegracao.ArcelorMittal,
        EnumTipoIntegracao.Deca,
        EnumTipoIntegracao.Unilever,
        EnumTipoIntegracao.Neokohm,
        EnumTipoIntegracao.SAP,
        EnumTipoIntegracao.Diageo,
        EnumTipoIntegracao.Brado,
        EnumTipoIntegracao.Comprovei,
        EnumTipoIntegracao.Frimesa,
        EnumTipoIntegracao.Obramax,
        EnumTipoIntegracao.Runtec,
        EnumTipoIntegracao.LoggiEventosEntrega,
        EnumTipoIntegracao.YPEEventosEntrega,
        EnumTipoIntegracao.Buntech,
        EnumTipoIntegracao.Electrolux,
        EnumTipoIntegracao.ConfirmaFacil,
        EnumTipoIntegracao.CTePagamentoLoggi,
        EnumTipoIntegracao.Mondelez,
        EnumTipoIntegracao.Camil,
        EnumTipoIntegracao.GrupoSC,
        EnumTipoIntegracao.Mars,
        EnumTipoIntegracao.Olfar,
        EnumTipoIntegracao.CassolEventosEntrega,
        EnumTipoIntegracao.Carrefour,
        EnumTipoIntegracao.AcocearenseEventosEntrega,
    ];    

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        tiposIntegracao.push(EnumTipoIntegracao.MultiEmbarcador);

    executarReST("TipoIntegracao/BuscarTodos", {
        Tipos: JSON.stringify(tiposIntegracao)
    }, function (r) {
        if (r.Success) {
            _configuracaoEmissaoCTeOpcoesTipoIntegracao = new Array();

            for (var i = 0; i < r.Data.length; i++)
                _configuracaoEmissaoCTeOpcoesTipoIntegracao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
};

function tipoIntegracaoChange() {
    if (_tipoOcorrencia.TipoIntegracao.val().includes(EnumTipoIntegracao.Minerva)) {
        _tipoOcorrencia.OcorrenciaProvisionada.visible(true);
    } else {
        _tipoOcorrencia.OcorrenciaProvisionada.visible(false);
        _tipoOcorrencia.OcorrenciaProvisionada.val(false);
    }

    if (_tipoOcorrencia.TipoIntegracao.val().includes(EnumTipoIntegracao.Neokohm))
        $("#liTabNeokohm").show();
    else
        $("#liTabNeokohm").hide();

    if (_tipoOcorrencia.TipoIntegracao.val().includes(EnumTipoIntegracao.Comprovei))
        $("#liTabComprovei").show();
    else
        $("#liTabComprovei").hide();
}

function modeloDocumentoFiscalChange() {
    if (_tipoOcorrencia.ModeloDocumentoFiscal.val()) {
        _tipoOcorrencia.DisponibilizarDocumentosParaNFsManual.visible(true);
    } else {
        _tipoOcorrencia.DisponibilizarDocumentosParaNFsManual.visible(false);
        _tipoOcorrencia.DisponibilizarDocumentosParaNFsManual.val(false);
    }

    controleCamposModeloDocumento(undefined);
}

function retornoModeloDocumento(data) {
    _tipoOcorrencia.ModeloDocumentoFiscal.codEntity(data.Codigo);
    _tipoOcorrencia.ModeloDocumentoFiscal.val(data.Descricao);
    verificarTipoEmissaoIntramunicipal();
    controleCamposModeloDocumento(data);
}

function controleCamposModeloDocumento(data) {
    var limparValor = !_carregandoEdicaoTipoOcorrencia;
    if ((data?.Abreviacao) == "ND" || (data?.Abreviacao) == "NC") {
        _tipoOcorrencia.GerarApenasUmComplemento.visible(true);
        _tipoOcorrencia.InformarProdutoLancamentoOcorrencia.visible(true);
    }
    else {
        _tipoOcorrencia.GerarApenasUmComplemento.visible(false);
        _tipoOcorrencia.InformarProdutoLancamentoOcorrencia.visible(false);
        if (limparValor) {
            _tipoOcorrencia.GerarApenasUmComplemento.val(false);
            _tipoOcorrencia.InformarProdutoLancamentoOcorrencia.val(false);
        }
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        if ((data?.TipoDocumentoEmissao) == EnumTipoDocumentoEmissao.Outros)
            _tipoOcorrencia.PermiteAlterarNumeroDocumentoOcorrencia.visible(true);
        else {
            _tipoOcorrencia.PermiteAlterarNumeroDocumentoOcorrencia.visible(false);
            if (limparValor)
                _tipoOcorrencia.PermiteAlterarNumeroDocumentoOcorrencia.val(false);
        }
    }
}

function validarCamposObrigatoriosTipoOcorrencia() {
    var checklistInvalido = _tipoOcorrencia.ChecklistSuperApp.required() && !_tipoOcorrencia.ChecklistSuperApp.val();

    if (!ValidarCamposObrigatorios(_tipoOcorrencia)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeOsCamposObrigatorios);

        if (checklistInvalido) {
            $("a[href='#tabSuperAppTrizy']")[0].click();
        } else {
            $("a[href='#tabCadastro']").click();
        }
        return false;
    }

    if (_gatilho.UtilizarGatilhoGeracaoOcorrencia.val() && !ValidarCamposObrigatorios(_gatilho)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeOsCamposObrigatorios);
        $("a[href='#knockoutGatilho']").click();
        return false;
    }

    return true;
}

function isParametrosValidos() {
    var parametros = _parametro.ListaParametro.val() || [];

    if (!$.isArray(parametros))
        return true;

    var parametroPeriodoFoiAdicionado = parametros.some(function (par) {
        return par.TipoParametro == EnumTipoParametroOcorrencia.Periodo;
    });

    return !parametroPeriodoFoiAdicionado;
}

function verificarTipoEmissaoIntramunicipal() {
    if (_tipoOcorrencia.ModeloDocumentoFiscal.codEntity() > 0) {
        _tipoOcorrencia.TipoEmissaoIntramunicipal.enable(false);
    } else {
        _tipoOcorrencia.TipoEmissaoIntramunicipal.enable(true);
    }
}
