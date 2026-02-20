/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoFrete.js" />
/// <reference path="Sumarizados.js" />

// #region Objetos Globais do Arquivo

var _gridFechamentoFrete;
var _fechamentoFrete;
var _CRUDFechamentoFrete;
var _pesquisaFechamentoFrete;
var _justificativaAcrescimoDesconto;

// #endregion Objetos Globais do Arquivo

// #region Classes

var FechamentoFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoFechamentoFrete.Aberto), def: EnumSituacaoFechamentoFrete.Aberto });
    this.AguardandoNFSManual = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
};

var CRUDFechamentoFrete = function () {
    this.Limpar = PropertyEntity({ eventClick: limparCamposFechamentoClick, type: types.event, text: "Limpar (Gerar Novo Fechamento)", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.GerarFechamento = PropertyEntity({ eventClick: gerarFechamentoClick, type: types.event, text: "Gerar Fechamento", visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarFechamentoClick, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarFechamentoClick, type: types.event, text: "Finalizar", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Reabrir = PropertyEntity({ eventClick: reabrirFechamentoClick, type: types.event, text: "Reabrir", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: imprimirFechamentoClick, type: types.event, text: "Imprimir", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.ImprimirExcel = PropertyEntity({ eventClick: imprimirExcelFechamentoClick, type: types.event, text: "Imprimir Excel", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.NaoEmitirComplemento = _resumo.NaoEmitirComplemento;
};

var PesquisaFechamentoFrete = function () {
    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.AcrescimoDesconto = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Acréscimo e Desconto", idBtnSearch: guid() });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Contrato = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Contrato de Frete:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoFechamentoFrete.Todas), options: EnumSituacaoFechamentoFrete.obterOpcoesPesquisa(), def: EnumSituacaoFechamentoFrete.Todas, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFechamentoFrete.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

// #endregion Classes

// #region Funções de Inicialização

function loadFechamentoFrete() {
    loadSumarizados();
    LoadResumo();

    _fechamentoFrete = new FechamentoFrete();
    HeaderAuditoria("FechamentoFrete", _fechamentoFrete);

    _CRUDFechamentoFrete = new CRUDFechamentoFrete();
    KoBindings(_CRUDFechamentoFrete, "knockoutCRUD");

    _pesquisaFechamentoFrete = new PesquisaFechamentoFrete();
    KoBindings(_pesquisaFechamentoFrete, "knockoutPesquisaFechamentoFrete", false, _pesquisaFechamentoFrete.Pesquisar.id);

    loadFechamentoFreteAcrescimoDesconto();
    loadDadosFechamento();
    LoadEtapasFechamento();
    LoadOcorrenciasFechamento();
    loadValoresOutrosRecursosFechamento();
    //LoadParcelas();
    LoadDetalhes();
    loadFechamentoFreteDocumentoComplementar();
    loadIntegracaoFechamento();
    LoadConexaoSignalRFechamento();
   
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FechamentoFrete_Cancelar, _PermissoesPersonalizadasFechamento))
        _CRUDFechamentoFrete.Cancelar.enable(false);

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FechamentoFrete_Finalizar, _PermissoesPersonalizadasFechamento))
        _CRUDFechamentoFrete.Finalizar.enable(false);

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FechamentoFrete_Reabrir, _PermissoesPersonalizadasFechamento))
        _CRUDFechamentoFrete.Reabrir.enable(false);

    BuscarTransportadores(_pesquisaFechamentoFrete.Transportador);
    BuscarContratoFreteTransportador(_pesquisaFechamentoFrete.Contrato);

    if (_CONFIGURACAO_TMS.TipoFechamentoFrete == EnumTipoFechamentoFrete.FechamentoPorFaixaKm) {
        $("#knockoutSumarizadoFranquia").hide();
        $("#dados-fechamento-periodo").hide();
        $("#knockoutVeiculosUtilizados").hide();
        $("#knockoutMotoristasUtilizados").hide();
        esconderCamposAcrescimosDescontos();
        $("#knockoutSumarizadoFranquiaPorFaixaKm").show();
    }

    BuscarFechamentoFrete();
    AjustarLayoutPorTipoServico();
}

function AjustarLayoutPorTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaFechamentoFrete.Transportador.text("Empresa/Filial:");
    }
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function limparCamposFechamentoClick(e, sender) {
    LimparCamposFechamento();
}

function cancelarFechamentoClick(e, sender) {
    var msg = "Você tem certeza que deseja cancelar o fechamento para o contrato " + _dadosFechamento.Contrato.val() + " no período de " + _dadosFechamento.DataInicio.val() + " até " + _dadosFechamento.DataFim.val() + "?";
    exibirConfirmacao("Gerar Fechamento", msg, function () {
        Salvar(_dadosFechamento, "FechamentoFrete/CancelarFechamento", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento cancelado com sucesso.");
                    _gridFechamentoFrete.CarregarGrid();
                    //BuscarFechamentoPorCodigo(arg.Data);
                    LimparCamposFechamento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function gerarFechamentoClick(e, sender) {
    var msg = "Você tem certeza que deseja gerar fechamento para o contrato " + _dadosFechamento.Contrato.val() + " no período de " + _dadosFechamento.DataInicio.val() + " até " + _dadosFechamento.DataFim.val() + "?";
    exibirConfirmacao("Gerar Fechamento", msg, function () {
        executarReST("FechamentoFrete/GerarFechamento", obterDadosFechamentoSalvar(), function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento gerado com sucesso.");
                    LimparCamposFechamento();
                    _gridFechamentoFrete.CarregarGrid();
                    BuscarFechamentoPorCodigo(arg.Data);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        });
    });
}

function reabrirFechamentoClick(e, sender) {
    var msg = "Você tem certeza que deseja gerar reabrir fechamento do contrato " + _dadosFechamento.Contrato.val() + " no período de " + _dadosFechamento.DataInicio.val() + " até " + _dadosFechamento.DataFim.val() + "?";
    exibirConfirmacao("Reabrir Fechamento", msg, function () {
        Salvar(_dadosFechamento, "FechamentoFrete/ReabrirFechamento", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento reaberto com sucesso.");
                    _gridFechamentoFrete.CarregarGrid();
                    BuscarFechamentoPorCodigo(arg.Data);
                    //LimparCamposFechamento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function finalizarFechamentoClick(e, sender) {
    var msg = "Você tem certeza que deseja finalizar o fechamento para o contrato " + _dadosFechamento.Contrato.val() + " no período de " + _dadosFechamento.DataInicio.val() + " até " + _dadosFechamento.DataFim.val() + "?";
    exibirConfirmacao("Finalizar Fechamento", msg, function () {
        executarReST("FechamentoFrete/FinalizarFechamento", obterFechamentoFreteFinalizar(), function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento finalizado com sucesso.");
                    _gridFechamentoFrete.CarregarGrid();
                    BuscarFechamentoPorCodigo(arg.Data);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function imprimirFechamentoClick() {
    executarDownload("FechamentoFrete/Imprimir", { Codigo: _fechamentoFrete.Codigo.val(), Tipo: "PDF" });
}

function imprimirExcelFechamentoClick() {
    executarDownload("FechamentoFrete/Imprimir", { Codigo: _fechamentoFrete.Codigo.val(), Tipo: "Excel" });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function BuscarFechamentoPorCodigo(codigo, callback) {
    executarReST("FechamentoFrete/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            EditarFechamento(arg.Data);
            preencherDadosSumarizados(arg.Data.DadosFechamento);
            EditarOcorrenciasFechamento(arg.Data);
            EditarResumo(arg.Data);

            preencherValoresOutrosRecursosFechamento(arg.Data.ValoresOutrosRecursosFechamento);
            preencherFechamentoFreteAcrescimoDesconto(arg.Data.FechamentoFreteAcrescimoDesconto);
            CarregaIntegracaoFechamento();

            _gridHistoricoCargas.CarregarGrid();

            if (_fechamentoFrete.Situacao.val() == EnumSituacaoFechamentoFrete.Aberto) {
                _CRUDFechamentoFrete.Cancelar.visible(true);
                _CRUDFechamentoFrete.Finalizar.visible(true);
                _resumo.NaoEmitirComplemento.enable(isHabilitararCampoNaoEmitirComplemento());
            }
            else {
                _CRUDFechamentoFrete.Cancelar.visible(false);
                _CRUDFechamentoFrete.Finalizar.visible(false);
                _resumo.NaoEmitirComplemento.enable(false);
            }

            _CRUDFechamentoFrete.Reabrir.visible(_fechamentoFrete.Situacao.val() == EnumSituacaoFechamentoFrete.Fechado);
            _CRUDFechamentoFrete.Limpar.visible(true);

            SetarEtapasFechamento();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

        if (callback != null)
            callback();
    }, null);
}

// #endregion Funções Públicas

// #region Funções Privadas

function obterDadosFechamentoSalvar() {
    var dadosFechamento = RetornarObjetoPesquisa(_dadosFechamento);

    preencherVeiculosUtilizadosSalvar(dadosFechamento);
    preencherMotoristasUtilizadosSalvar(dadosFechamento);

    return dadosFechamento;
}

function obterFechamentoFreteFinalizar() {

    var fechamentoFreteFinalizar = RetornarObjetoPesquisa(_dadosFechamento);

    preencherValoresOutrosRecursoFechamentoSalvar(fechamentoFreteFinalizar);
    preencherFechamentoFreteAcrescimoDescontoSalvar(fechamentoFreteFinalizar);

    return fechamentoFreteFinalizar;
}

function BuscarFechamentoFrete() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarFechamento, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridFechamentoFrete = new GridView(_pesquisaFechamentoFrete.Pesquisar.idGrid, "FechamentoFrete/Pesquisa", _pesquisaFechamentoFrete, menuOpcoes);
    _gridFechamentoFrete.CarregarGrid();
}

function editarFechamento(itemGrid) {
    // Limpa os campos
    LimparCamposFechamento();

    // Esconde filtros
    _pesquisaFechamentoFrete.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarFechamentoPorCodigo(itemGrid.Codigo);
}

function EditarFechamento(data) {
    _fechamentoFrete.AguardandoNFSManual.val(data.AguardandoNFSManual);
    _fechamentoFrete.Situacao.val(data.Situacao);
    _fechamentoFrete.Codigo.val(data.Codigo);

    preencherDadosFechamento(data);

    _sumarizadoViagensRealizadas.VerContrato.visible(_CONFIGURACAO_TMS.TipoFechamentoFrete == EnumTipoFechamentoFrete.FechamentoPorKm);
    _sumarizadoViagensRealizadas.VerHistorico.visible(true);

    _CRUDFechamentoFrete.Finalizar.visible(true);
    _CRUDFechamentoFrete.Imprimir.visible(true);
}

function LimparCamposFechamento() {
    LimparCampos(_fechamentoFrete);
    limparCamposDadosFechamento();

    _CRUDFechamentoFrete.GerarFechamento.visible(false);
    _CRUDFechamentoFrete.Cancelar.visible(false);
    _CRUDFechamentoFrete.Finalizar.visible(false);
    _CRUDFechamentoFrete.Imprimir.visible(false);
    _CRUDFechamentoFrete.Reabrir.visible(false);
    _CRUDFechamentoFrete.Limpar.visible(false);

    _resumo.NaoEmitirComplemento.enable(true);
    ControleCamposResumo(false);

    _sumarizadoViagensRealizadas.VerContrato.visible(false);
    _sumarizadoViagensRealizadas.VerHistorico.visible(false);

    //LimparCamposParcelas();
    limparSumarizados();
    GetSetCargasRemovidas([]);
    SetarEtapaInicioFechamento();
}

function isHabilitararCampoNaoEmitirComplemento() {
    if (_dadosFechamento.EnumPeriodoAcordo.val() == EnumPeriodoAcordoContratoFreteTransportador.Semanal)
        return _dadosFechamento.PeriodoDezena.val() !== EnumSemana.Quarta;
    else if (_dadosFechamento.EnumPeriodoAcordo.val() == EnumPeriodoAcordoContratoFreteTransportador.Decendial)
        return _dadosFechamento.PeriodoDezena.val() !== EnumDezena.Terceira;
    else if (_dadosFechamento.EnumPeriodoAcordo.val() == EnumPeriodoAcordoContratoFreteTransportador.Quinzenal)
        return _dadosFechamento.PeriodoQuinzena.val() !== EnumQuinzena.Segunda;

    return false;
}

// #endregion Funções Privadas
