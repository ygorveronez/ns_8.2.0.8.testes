/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pracaPedagioTarifa;
var _cadastroPracaPedagioTarifa;
var _CRUDPracaPedagioTarifa;
var _gridPracaPedagioTarifa;

var CadastroPracaPedagioTarifa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ModeloVeicularCarga = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.ModeloVeicular.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
    this.Tarifa = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.Tarifa.getFieldDescription(), getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false, allowNegative: false }, required: true, maxlength: 15 });
}

var PracaPedagioTarifa = function () {
    this.Lista = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarPracaPedagioTarifaModalClick, type: types.event, text: Localization.Resources.Logistica.PracaPedagio.AdicionarTarifa });
}

var CRUDCadastroPracaPedagioTarifa = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarPracaPedagioTarifaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPracaPedagioTarifaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirPracaPedagioTarifaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir , visible: ko.observable(false) });
}

function loadPracaPedagioTarifa() {
    _pracaPedagioTarifa = new PracaPedagioTarifa();
    KoBindings(_pracaPedagioTarifa, "knockoutPracaPedagioTarifa");

    _cadastroPracaPedagioTarifa = new CadastroPracaPedagioTarifa();
    KoBindings(_cadastroPracaPedagioTarifa, "knockoutCadastroPracaPedagioTarifa");

    _CRUDPracaPedagioTarifa = new CRUDCadastroPracaPedagioTarifa();
    KoBindings(_CRUDPracaPedagioTarifa, "knockoutCRUDCadastroPracaPedagioTarifa");

    new BuscarModelosVeicularesCarga(_cadastroPracaPedagioTarifa.ModeloVeicularCarga);

    loadGridPracaPedagioTarifa();
}

function loadGridPracaPedagioTarifa() {
    var linhasPorPaginas = 10;
    var ordenacao = { column: 2, dir: orderDir.asc };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarPracaPedagioTarifaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoModeloVeicularCarga", visible: false },
        { data: "DescricaoModeloVeicularCarga", title: Localization.Resources.Logistica.PracaPedagio.ModeloVeicularCarga, width: "60%", className: "text-align-left", orderable: true },
        { data: "Data", title: Localization.Resources.Gerais.Geral.Data, width: "20%", className: "text-align-center", orderable: false },
        { data: "Tarifa", title: Localization.Resources.Logistica.PracaPedagio.Tarifa, width: "20%", className: "text-align-right", orderable: false }
    ];
    _gridPracaPedagioTarifa = new BasicDataTable(_pracaPedagioTarifa.Lista.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas, null, null, null);
    _gridPracaPedagioTarifa.CarregarGrid([]);
}

function adicionarPracaPedagioTarifaModalClick() {
    _cadastroPracaPedagioTarifa.Codigo.val(guid());
    controlarBotoesCadastroPracaPedagioTarifaHabilitados(false);
    exibirModalCadastroPracaPedagioTarifa();
}

function exibirModalCadastroPracaPedagioTarifa() {
    Global.abrirModal('divModalCadastroPracaPedagioTarifa');
    $("#divModalCadastroPracaPedagioTarifa").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroPracaPedagioTarifa);
    });
}

function fecharModalCadastroPracaPedagioTarifa() {
    Global.fecharModal('divModalCadastroPracaPedagioTarifa');
}

function recarregarGridPracaPedagioTarifa() {
    var lista = obterListaTarifa();
    _gridPracaPedagioTarifa.CarregarGrid(lista);
}

function controlarBotoesCadastroPracaPedagioTarifaHabilitados(isEdicao) {
    _CRUDPracaPedagioTarifa.Adicionar.visible(!isEdicao);
    _CRUDPracaPedagioTarifa.Atualizar.visible(isEdicao);
    _CRUDPracaPedagioTarifa.Excluir.visible(isEdicao);
}

function adicionarPracaPedagioTarifaClick() {
    if (ValidarCamposObrigatorios(_cadastroPracaPedagioTarifa)) {
        var valido = true;
        var lista = obterListaTarifa();
        lista.forEach(function (status, i) {
            if (_cadastroPracaPedagioTarifa.ModeloVeicularCarga.codEntity() == status.CodigoModeloVeicularCarga) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Logistica.PracaPedagio.ModeloVeicularJaAdicionado, Localization.Resources.Logistica.PracaPedagio.NaoEPossivelAdicionarMaisDeUmaTarifaParaMesmoModeloVeicular);
            }
        });
        if (valido) {
            lista.push(obterCadastroPraPedagioTarifaSalvar());
            _pracaPedagioTarifa.Lista.val(lista);
            recarregarGridPracaPedagioTarifa();
            fecharModalCadastroPracaPedagioTarifa();
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.CamposObrigatorios);
    }
}

function atualizarPracaPedagioTarifaClick() {
    if (ValidarCamposObrigatorios(_cadastroPracaPedagioTarifa)) {
        var lista = obterListaTarifa();
        lista.forEach(function (status, i) {
            if (_cadastroPracaPedagioTarifa.Codigo.val() == status.Codigo) {
                lista.splice(i, 1, obterCadastroPraPedagioTarifaSalvar());
            }
        });
        _pracaPedagioTarifa.Lista.val(lista);
        recarregarGridPracaPedagioTarifa();
        fecharModalCadastroPracaPedagioTarifa();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.CamposObrigatorios);
    }
}

function excluirPracaPedagioTarifaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Logistica.PracaPedagio.RealmenteDesejaExcluirTarifaDoModeloVeicular, function () {
        var lista = obterListaTarifa();
        lista.forEach(function (status, i) {
            if (_cadastroPracaPedagioTarifa.Codigo.val() == status.Codigo) {
                lista.splice(i, 1);
            }
        });
        _pracaPedagioTarifa.Lista.val(lista);
        recarregarGridPracaPedagioTarifa();
        fecharModalCadastroPracaPedagioTarifa();
    });
}

function editarPracaPedagioTarifaClick(row) {
    PreencherObjetoKnout(_cadastroPracaPedagioTarifa, { Data: row });
    _cadastroPracaPedagioTarifa.ModeloVeicularCarga.codEntity(row.CodigoModeloVeicularCarga);
    _cadastroPracaPedagioTarifa.ModeloVeicularCarga.val(row.DescricaoModeloVeicularCarga);
    _cadastroPracaPedagioTarifa.ModeloVeicularCarga.entityDescription(row.DescricaoModeloVeicularCarga);
    _cadastroPracaPedagioTarifa.Tarifa.entityDescription(row.Tarifa);
    controlarBotoesCadastroPracaPedagioTarifaHabilitados(true);
    exibirModalCadastroPracaPedagioTarifa();
}

function obterListaTarifa() {
    return _pracaPedagioTarifa.Lista.val();
}

function obterCadastroPraPedagioTarifaSalvar() {
    return {
        Codigo: _cadastroPracaPedagioTarifa.Codigo.val(),
        CodigoModeloVeicularCarga: _cadastroPracaPedagioTarifa.ModeloVeicularCarga.codEntity(),
        DescricaoModeloVeicularCarga: _cadastroPracaPedagioTarifa.ModeloVeicularCarga.val(),
        Data: Global.DataHoraAtual(),
        Tarifa: _cadastroPracaPedagioTarifa.Tarifa.val(),
    };
}

function ordenarGridPracaPedagioTarifa(lista) {
    return lista.sort(function (a, b) {
        var nameA = a.DescricaoModeloVeicularCarga;
        var nameB = b.DescricaoModeloVeicularCarga;
        if (nameA < nameB) {
            return -1;
        }
        if (nameA > nameB) {
            return 1;
        }
        return 0;
    });
}

function preencherPracaPedagioTarifa(dados) {
    if (dados == undefined) dados = [];
    _pracaPedagioTarifa.Lista.val(dados);
    recarregarGridPracaPedagioTarifa();
}

function limparCamposPracaPedagioTarifa() {
    preencherPracaPedagioTarifa([]);
}

function obterPracaPedagioTarifaSalvar() {
    var lista = obterListaTarifa();
    return JSON.stringify(lista);
}