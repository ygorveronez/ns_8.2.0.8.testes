/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Tranportador.js" />

// #region Objetos Globais do Arquivo

var _cadastroEmpresa;
var _empresa;
var _gridEmpresa;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CadastroEmpresa = function () {
    var self = this;
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Transportador:", idBtnSearch: guid(), required: true });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo Veicular de Carga:", idBtnSearch: guid(), required: _CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga, visible: _CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga });
    this.PercentualCargasDaRota = PropertyEntity({ text: ko.observable("*Percentual de Cargas:"), required: !_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade.DisponibilizarCargaParaTransportadoresPorPrioridade, getType: typesKnockout.decimal, visible: ko.observable(!_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade), enable: ko.observable(true), maxlength: 6 });
    this.Prioridade = PropertyEntity({ text: ko.observable("*Prioridade:"), required: _CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade, getType: typesKnockout.int, visible: ko.observable(_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade), enable: ko.observable(true), maxlength: 6 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarEmpresaClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarEmpresaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarEmpresaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirEmpresaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var Empresa = function () {
    this.ListaEmpresa = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarEmpresaModalClick, type: types.event, text: ko.observable("Adicionar Empresa"), visible: true });
}

// #endregion Classes

// #region Funções de Inicialização

function loadEmpresa() {
    _empresa = new Empresa();
    KoBindings(_empresa, "knockoutEmpresa");

    _cadastroEmpresa = new CadastroEmpresa();
    KoBindings(_cadastroEmpresa, "knockoutCadastroEmpresa");

    new BuscarTransportadores(_cadastroEmpresa.Empresa);
    new BuscarModelosVeicularesCarga(_cadastroEmpresa.ModeloVeicularCarga);

    loadGridEmpresa();
}

function loadGridEmpresa() {
    var ordenacaoPadrao = { column: (_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade ? 6 : 3), dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarEmpresaClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoEmpresa", visible: false },
        { data: "CodigoModeloVeicularCarga", visible: false },
        { data: "DescricaoEmpresa", title: "Transportador", width: "70%" },
        { data: "DescricaoModeloVeicularCarga", title: "Modelo Veicular de Carga", width: "70%", visible: _CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga },
        { data: "PercentualCargasDaRota", title: "Percentual de Cargas", width: "20%", className: "text-align-center", visible: !_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade },
        { data: "Prioridade", title: "Prioridade", width: "20%", className: "text-align-center", visible: _CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade }
    ];

    _gridEmpresa = new BasicDataTable(_empresa.ListaEmpresa.idGrid, header, menuOpcoes, ordenacaoPadrao);
    _gridEmpresa.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarEmpresaClick() {
    if (!validarCadastroEmpresa())
        return;

    _empresa.ListaEmpresa.val().push(obterCadastroEmpresaSalvar());

    recarregarGridEmpresa();
    fecharModalCadastroEmpresa();
}

function adicionarEmpresaModalClick() {
    _cadastroEmpresa.Codigo.val(guid());
    
    controlarBotoesEmpresaHabilitados(false);
    exibirModalCadastroEmpresa();
}

function atualizarEmpresaClick() {
    if (!validarCadastroEmpresa())
        return;

    var listaEmpresa = obterListaEmpresa();

    for (var i = 0; i < listaEmpresa.length; i++) {
        if (listaEmpresa[i].Codigo = _cadastroEmpresa.Codigo.val()) {
            listaEmpresa.splice(i, 1, obterCadastroEmpresaSalvar());
            break;
        }
    }

    _empresa.ListaEmpresa.val(listaEmpresa);

    recarregarGridEmpresa();
    fecharModalCadastroEmpresa();
}

function cancelarEmpresaClick() {
    fecharModalCadastroEmpresa();
}

function editarEmpresaClick(registroSelecionado) {
    preencherDadosEmpresa(registroSelecionado);
    controlarBotoesEmpresaHabilitados(true);
    exibirModalCadastroEmpresa();
}

function excluirEmpresaClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir este transportador?", function () {
        var listaEmpresa = obterListaEmpresa();

        for (var i = 0; i < listaEmpresa.length; i++) {
            if (listaEmpresa[i].Codigo == _cadastroEmpresa.Codigo.val()) {
                listaEmpresa.splice(i, 1);
            }
        }

        _empresa.ListaEmpresa.val(listaEmpresa);

        recarregarGridEmpresa();
        fecharModalCadastroEmpresa();
    });
}

    // #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposEmpresa() {
    _empresa.ListaEmpresa.val([]);
    recarregarGridEmpresa();
}

function preencherEmpresa(empresas) {
    _empresa.ListaEmpresa.val(empresas);
    recarregarGridEmpresa();
}

function preencherEmpresaSalvar(configuracaoRotaFrete) {
    configuracaoRotaFrete["Empresas"] = obterListaEmpresaSalvar();
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarBotoesEmpresaHabilitados(isEdicao) {
    _cadastroEmpresa.Adicionar.visible(!isEdicao);
    _cadastroEmpresa.Atualizar.visible(isEdicao);
    _cadastroEmpresa.Cancelar.visible(isEdicao);
    _cadastroEmpresa.Excluir.visible(isEdicao);
}

function exibirModalCadastroEmpresa() {
    Global.abrirModal('divModalCadastroEmpresa');
    $("#divModalCadastroEmpresa").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroEmpresa);
    });
}

function fecharModalCadastroEmpresa() {
    Global.fecharModal('divModalCadastroEmpresa');
}

function isCadastroEmpresaDuplicado() {
    var listaEmpresa = obterListaEmpresa();

    for (var i = 0; i < listaEmpresa.length; i++) {
        var empresa = listaEmpresa[i];

        if ((empresa.Codigo != _cadastroEmpresa.Codigo.val()) && (empresa.CodigoEmpresa == _cadastroEmpresa.Empresa.codEntity()) && (empresa.CodigoModeloVeicularCarga == _cadastroEmpresa.ModeloVeicularCarga.codEntity()))
            return true;
    }

    return false;
}

function isPercentualDeCargasExcedido() {
    if (_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade)
        return false;

    var listaEmpresa = obterListaEmpresa();
    var percentual = Globalize.parseFloat(_cadastroEmpresa.PercentualCargasDaRota.val());

    for (var i = 0; i < listaEmpresa.length; i++) {
        var empresa = listaEmpresa[i];

        if ((empresa.Codigo != _cadastroEmpresa.Codigo.val()) && (empresa.CodigoModeloVeicularCarga == _cadastroEmpresa.ModeloVeicularCarga.codEntity()))
            percentual += Globalize.parseFloat(empresa.PercentualCargasDaRota)
    }

    return (percentual > 100);
}

function isPrioridadeDuplicada() {
    if (!_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade)
        return false;

    var listaEmpresa = obterListaEmpresa();

    for (var i = 0; i < listaEmpresa.length; i++) {
        var empresa = listaEmpresa[i];

        if ((empresa.Codigo != _cadastroEmpresa.Codigo.val()) && (empresa.Prioridade == _cadastroEmpresa.Prioridade.val()))
            return true;
    }

    return false;
}

function obterCadastroEmpresaSalvar() {
    return {
        Codigo: _cadastroEmpresa.Codigo.val(),
        CodigoEmpresa: _cadastroEmpresa.Empresa.codEntity(),
        CodigoModeloVeicularCarga: _cadastroEmpresa.ModeloVeicularCarga.codEntity(),
        DescricaoEmpresa: _cadastroEmpresa.Empresa.val(),
        DescricaoModeloVeicularCarga: _cadastroEmpresa.ModeloVeicularCarga.val(),
        PercentualCargasDaRota: _cadastroEmpresa.PercentualCargasDaRota.val(),
        Prioridade: _cadastroEmpresa.Prioridade.val()
    };
}

function obterListaEmpresa() {
    return _empresa.ListaEmpresa.val().slice();
}

function obterListaEmpresaSalvar() {
    var listaEmpresa = obterListaEmpresa();

    return JSON.stringify(listaEmpresa);
}

function preencherDadosEmpresa(registroSelecionado) {
    _cadastroEmpresa.Codigo.val(registroSelecionado.Codigo);
    _cadastroEmpresa.Empresa.codEntity(registroSelecionado.CodigoEmpresa);
    _cadastroEmpresa.Empresa.val(registroSelecionado.DescricaoEmpresa);
    _cadastroEmpresa.ModeloVeicularCarga.codEntity(registroSelecionado.CodigoModeloVeicularCarga);
    _cadastroEmpresa.ModeloVeicularCarga.val(registroSelecionado.DescricaoModeloVeicularCarga);
    _cadastroEmpresa.PercentualCargasDaRota.val(registroSelecionado.PercentualCargasDaRota);
    _cadastroEmpresa.Prioridade.val(registroSelecionado.Prioridade);
}

function recarregarGridEmpresa() {
    var listaEmpresa = obterListaEmpresa();

    _gridEmpresa.CarregarGrid(listaEmpresa);
}

function validarCadastroEmpresa() {
    if (!ValidarCamposObrigatorios(_cadastroEmpresa)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, informe os campos obrigatórios");
        return false;
    }

    if (isCadastroEmpresaDuplicado()) {
        exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "O transportador informado já está cadastrado");
        return false;
    }

    if (isPercentualDeCargasExcedido()) {
        exibirMensagem(tipoMensagem.atencao, "Percentual Inválido", "A soma de todos os percentuais não deve exceder 100%");
        return false;
    }

    if (isPrioridadeDuplicada()) {
        exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "A prioridade informada já está cadastrada");
        return false;
    }

    return true;
}

// #endregion Funções Privadas
