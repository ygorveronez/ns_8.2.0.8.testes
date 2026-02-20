/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _crudParcelamentoCadastro;
var _gridParcelamento;
var _parcelamento;
var _parcelamentoCadastro;

// #endregion

// #region Classes

var CRUDParcelamentoCadastro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarParcelamentoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarParcelamentoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirParcelamentoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var Parcelamento = function () {
    this.ListaParcelamento = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarParcelamentoModalClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ParcelamentoCadastro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PercentualInicial = PropertyEntity({ val: ko.observable(""), text: "*Percentual Inicial:", def: "", getType: typesKnockout.decimal, required: true, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, maxlength: 5 });
    this.PercentualFinal = PropertyEntity({ val: ko.observable(""), text: "*Percentual Final:", def: "", getType: typesKnockout.decimal, required: true, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, maxlength: 6 });
    this.NumeroParcelas = PropertyEntity({ val: ko.observable(""), text: "*Número de Parcelas:", def: "", getType: typesKnockout.int, required: true, configInt: { precision: 0, allowZero: false }, maxlength: 3 });
    this.PercentualJurosParcela = PropertyEntity({ val: ko.observable(""), text: "*Percentual de Juros:", def: "", getType: typesKnockout.decimal, required: true, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, maxlength: 6 });
}

// #endregion

// #region Funções de Inicialização

function loadGridParcelamento() {
    var ordenacao = { column: 1, dir: orderDir.asc }
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [{ descricao: "Editar", id: guid(), metodo: editarParcelamentoClick, tamanho: 20 }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "PercentualInicial", visible: false },
        { data: "PercentualFinal", visible: false },
        { data: "Descricao", title: "Faixa de Percentual", width: "40%" },
        { data: "NumeroParcelas", title: "Número de Parcelas", width: "20%", className: "text-align-center" },
        { data: "PercentualJurosParcela", title: "Percentual de Juros", width: "20%", className: "text-align-center" }
    ];

    _gridParcelamento = new BasicDataTable(_parcelamento.ListaParcelamento.idGrid, header, menuOpcoes, ordenacao);
    _gridParcelamento.CarregarGrid([]);
}

function loadParcelamento() {
    _parcelamento = new Parcelamento();
    KoBindings(_parcelamento, "knockoutParcelamento");

    _parcelamentoCadastro = new ParcelamentoCadastro();
    KoBindings(_parcelamentoCadastro, "knockoutParcelamentoCadastro");

    _crudParcelamentoCadastro = new CRUDParcelamentoCadastro();
    KoBindings(_crudParcelamentoCadastro, "knockoutCRUDParcelamentoCadastro");

    loadGridParcelamento();
}

// #endregion

// #region Funções Associadas a Eventos

function adicionarParcelamentoClick() {
    if (!validarParcelamento())
        return;

    _parcelamento.ListaParcelamento.val().push(obterParcelamentoSalvar());

    recarregarGridParcelamento();
    fecharModalCadastroParcelamento();
}

function adicionarParcelamentoModalClick() {
    _parcelamentoCadastro.Codigo.val(guid());

    controlarBotoesParcelamentoCadastroHabilitados(false);
    exibirModalCadastroParcelamento();
}

function atualizarParcelamentoClick() {
    if (!validarParcelamento())
        return;

    var listaParcelamento = obterListaParcelamento();

    for (var i = 0; i < listaParcelamento.length; i++) {
        if (_parcelamentoCadastro.Codigo.val() == listaParcelamento[i].Codigo) {
            listaParcelamento.splice(i, 1, obterParcelamentoSalvar());
            break;
        }
    }

    _parcelamento.ListaParcelamento.val(listaParcelamento);

    recarregarGridParcelamento();
    fecharModalCadastroParcelamento();
}

function editarParcelamentoClick(registroSelecionado) {
    var parcelamento = obterParcelamentoPorCodigo(registroSelecionado.Codigo);

    if (parcelamento) {
        PreencherObjetoKnout(_parcelamentoCadastro, { Data: parcelamento });

        controlarBotoesParcelamentoCadastroHabilitados(true);
        exibirModalCadastroParcelamento();
    }
}

function excluirParcelamentoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o parcelamento?", function () {
        var listaParcelamento = obterListaParcelamento();

        for (var i = 0; i < listaParcelamento.length; i++) {
            if (_parcelamentoCadastro.Codigo.val() == listaParcelamento[i].Codigo)
                listaParcelamento.splice(i, 1);
        }

        _parcelamento.ListaParcelamento.val(listaParcelamento);

        recarregarGridParcelamento();
        fecharModalCadastroParcelamento();
    });
}

// #endregion

// #region Funções Públicas

function preencherParcelamento(parcelamentos) {
    _parcelamento.ListaParcelamento.val(parcelamentos);

    recarregarGridParcelamento();
}

function preencherParcelamentoSalvar(regraParcelamentoOcorrencia) {
    regraParcelamentoOcorrencia["ListaParcelamento"] = obterListaParcelamentoSalvar();
}

function limparCamposParcelamento() {
    _parcelamento.ListaParcelamento.val([]);

    recarregarGridParcelamento();
}

// #endregion

// #region Funções Privadas

function controlarBotoesParcelamentoCadastroHabilitados(isEdicao) {
    _crudParcelamentoCadastro.Atualizar.visible(isEdicao);
    _crudParcelamentoCadastro.Excluir.visible(isEdicao);
    _crudParcelamentoCadastro.Adicionar.visible(!isEdicao);
}

function exibirModalCadastroParcelamento() {
    Global.abrirModal('divModalCadastroParcelamento');
    $("#divModalCadastroParcelamento").one('hidden.bs.modal', function () {
        limparCamposParcelamentoCadastro();
    });
}

function fecharModalCadastroParcelamento() {
    Global.fecharModal('divModalCadastroParcelamento');
}

function limparCamposParcelamentoCadastro() {
    LimparCampos(_parcelamentoCadastro);
}

function obterParcelamentoPorCodigo(codigo) {
    var listaParcelamento = obterListaParcelamento();

    for (var i = 0; i < listaParcelamento.length; i++) {
        var parcelamento = listaParcelamento[i];

        if (codigo == parcelamento.Codigo)
            return parcelamento;
    }

    return undefined;
}

function obterParcelamentoSalvar() {
    return {
        Codigo: _parcelamentoCadastro.Codigo.val(),
        Descricao: "De " + _parcelamentoCadastro.PercentualInicial.val() + " até " + _parcelamentoCadastro.PercentualFinal.val() + " por cento",
        PercentualInicial: _parcelamentoCadastro.PercentualInicial.val(),
        PercentualFinal: _parcelamentoCadastro.PercentualFinal.val(),
        NumeroParcelas: _parcelamentoCadastro.NumeroParcelas.val(),
        PercentualJurosParcela: _parcelamentoCadastro.PercentualJurosParcela.val()
    };
}

function obterListaParcelamento() {
    return _parcelamento.ListaParcelamento.val().slice();
}

function obterListaParcelamentoSalvar() {
    var listaParcelamento = obterListaParcelamento();
    var listaParcelamentoSalvar = new Array();

    for (var i = 0; i < listaParcelamento.length; i++) {
        var parcelamento = listaParcelamento[i];
        
        listaParcelamentoSalvar.push({
            Codigo: parcelamento.Codigo,
            PercentualInicial: parcelamento.PercentualInicial,
            PercentualFinal: parcelamento.PercentualFinal,
            NumeroParcelas: Globalize.parseInt(parcelamento.NumeroParcelas),
            PercentualJurosParcela: parcelamento.PercentualJurosParcela
        });
    }

    return JSON.stringify(listaParcelamentoSalvar);
}

function recarregarGridParcelamento() {
    var listaParcelamento = obterListaParcelamento();

    _gridParcelamento.CarregarGrid(listaParcelamento);
}

function validarParcelamento() {
    if (!ValidarCamposObrigatorios(_parcelamentoCadastro)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return false;
    }

    var percentualInicial = Globalize.parseFloat(_parcelamentoCadastro.PercentualInicial.val());
    var percentualFinal = Globalize.parseFloat(_parcelamentoCadastro.PercentualFinal.val());

    if (percentualFinal <= percentualInicial) {
        exibirMensagem(tipoMensagem.atencao, "Dados Inválidos", "O percentual final deve ser maior que o inicial!");
        return false;
    }

    if (percentualFinal > 100) {
        exibirMensagem(tipoMensagem.atencao, "Dados Inválidos", "O percentual final deve ser menor ou igual a 100!");
        return false;
    }

    var listaParcelamento = obterListaParcelamento();

    for (var i = 0; i < listaParcelamento.length; i++) {
        var parcelamento = listaParcelamento[i];

        if (parcelamento.Codigo == _parcelamentoCadastro.Codigo.val())
            continue;

        var percentualInicialCadastrada = Globalize.parseFloat(parcelamento.PercentualInicial);
        var percentualFinalCadastrada = Globalize.parseFloat(parcelamento.PercentualFinal);

        if (
            (percentualInicial >= percentualInicialCadastrada && percentualInicial <= percentualFinalCadastrada) ||
            (percentualFinal >= percentualInicialCadastrada && percentualFinal <= percentualFinalCadastrada) ||
            (percentualInicialCadastrada >= percentualInicial && percentualInicialCadastrada <= percentualFinal) ||
            (percentualFinalCadastrada >= percentualInicial && percentualFinalCadastrada <= percentualFinal)
        ) {
            exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "Já existe um cadastro que contém a faixa de percentual informada!");
            return false;
        }
    }

    return true;
}

// #endregion
