/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumVerificarTipoDeCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroMonitoramentoEventoTipoDeCarga;
var _CRUDcadastroMonitoramentoEventoTipoDeCarga;
var _gridMonitoramentoEventoTipoDeCarga;
var _monitoramentoEventoTipoDeCarga;

/*
 * Declaração das Classes
 */

var CRUDCadastroMonitoramentoEventoTipoDeCarga = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarMonitoramentoEventoTipoDeCargaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarMonitoramentoEventoTipoDeCargaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirMonitoramentoEventoTipoDeCargaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var CadastroMonitoramentoEventoTipoDeCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoDeCarga = PropertyEntity({ text: "*Tipo de carga:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
}

var MonitoramentoEventoTipoDeCarga = function () {
    this.VerificarTipoDeCarga = PropertyEntity({ text: "Verificar tipo de viagem?", getType: typesKnockout.bool, val: ko.observable(EnumVerificarTipoDeCarga.NaoVerificar), options: EnumVerificarTipoDeCarga.obterOpcoes(), visible: ko.observable(true) });
    this.VerificarTipoDeCarga.val.subscribe(function (val) {
        if (val == EnumVerificarTipoDeCarga.NaoVerificar) {
            _monitoramentoEventoTipoDeCarga.ListaTipoDeCarga.visible(false);
        } else {
            _monitoramentoEventoTipoDeCarga.ListaTipoDeCarga.visible(true);
        }
    });
    _monitoramentoEvento.VerificarTipoDeCarga = this.VerificarTipoDeCarga;

    this.ListaTipoDeCarga = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(false) });
    this.ListaTipoDeCarga.val.subscribe(function () {
        recarregarGridMonitoramentoEventoTipoDeCarga();
    });
    this.Adicionar = PropertyEntity({ eventClick: adicionarMonitoramentoEventoTipoDeCargaModalClick, type: types.event, text: "Adicionar tipo de carga" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMonitoramentoEventoTipoDeCarga() {
    var linhasPorPaginas = 10;
    var ordenacao = { column: 3, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarMonitoramentoEventoTipoDeCargaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoDeCarga", visible: false },
        { data: "DescricaoTipoDeCarga", title: "Descrição", width: "100%", className: "text-align-left", orderable: false }
    ];
    _gridMonitoramentoEventoTipoDeCarga = new BasicDataTable(_monitoramentoEventoTipoDeCarga.ListaTipoDeCarga.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas, null, null, null);
    _gridMonitoramentoEventoTipoDeCarga.CarregarGrid([]);
}

function loadMonitoramentoEventoTipoDeCarga() {
    _monitoramentoEventoTipoDeCarga = new MonitoramentoEventoTipoDeCarga();
    KoBindings(_monitoramentoEventoTipoDeCarga, "knockoutMonitoramentoEventoTipoDeCarga");

    _cadastroMonitoramentoEventoTipoDeCarga = new CadastroMonitoramentoEventoTipoDeCarga();
    KoBindings(_cadastroMonitoramentoEventoTipoDeCarga, "knockoutCadastroMonitoramentoEventoTipoDeCarga");

    _CRUDcadastroMonitoramentoEventoTipoDeCarga = new CRUDCadastroMonitoramentoEventoTipoDeCarga();
    KoBindings(_CRUDcadastroMonitoramentoEventoTipoDeCarga, "knockoutCRUDCadastroMonitoramentoEventoTipoDeCarga");

    new BuscarTiposdeCarga(_cadastroMonitoramentoEventoTipoDeCarga.TipoDeCarga);

    loadGridMonitoramentoEventoTipoDeCarga();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarMonitoramentoEventoTipoDeCargaClick() {
    if (ValidarCamposObrigatorios(_cadastroMonitoramentoEventoTipoDeCarga)) {
        var valido = true;
        var listaTipoDeCarga = obterListaTipoDeCarga();
        listaTipoDeCarga.forEach(function (row, i) {
            if (_cadastroMonitoramentoEventoTipoDeCarga.TipoDeCarga.codEntity() == row.CodigoTipoDeCarga) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Tipo de carga já adicionado", "Não é possível adicionar tipos de carga repetidos.");
            }
        });
        if (valido) {
            _monitoramentoEventoTipoDeCarga.ListaTipoDeCarga.val().push(obterCadastroMonitoramentoEventoTipoDeCargaSalvar());
            recarregarGridMonitoramentoEventoTipoDeCarga();
            fecharModalCadastroMonitoramentoEventoTipoDeCarga();
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function adicionarMonitoramentoEventoTipoDeCargaModalClick() {
    _cadastroMonitoramentoEventoTipoDeCarga.Codigo.val(guid());
    controlarBotoesCadastroMonitoramentoEventoTipoDeCargaHabilitados(false);
    exibirModalCadastroMonitoramentoEventoTipoDeCarga();
}

function atualizarMonitoramentoEventoTipoDeCargaClick() {
    if (ValidarCamposObrigatorios(_cadastroMonitoramentoEventoTipoDeCarga)) {
        var listaTipoDeCarga = obterListaTipoDeCarga();
        listaTipoDeCarga.forEach(function (row, i) {
            if (_cadastroMonitoramentoEventoTipoDeCarga.Codigo.val() == row.Codigo) {
                listaTipoDeCarga.splice(i, 1, obterCadastroMonitoramentoEventoTipoDeCargaSalvar());
            }
        });
        _monitoramentoEventoTipoDeCarga.ListaTipoDeCarga.val(listaTipoDeCarga);
        recarregarGridMonitoramentoEventoTipoDeCarga();
        fecharModalCadastroMonitoramentoEventoTipoDeCarga();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function editarMonitoramentoEventoTipoDeCargaClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroMonitoramentoEventoTipoDeCarga, { Data: registroSelecionado });
    _cadastroMonitoramentoEventoTipoDeCarga.TipoDeCarga.codEntity(registroSelecionado.CodigoTipoDeCarga);
    _cadastroMonitoramentoEventoTipoDeCarga.TipoDeCarga.val(registroSelecionado.DescricaoTipoDeCarga);
    _cadastroMonitoramentoEventoTipoDeCarga.TipoDeCarga.entityDescription(registroSelecionado.DescricaoTipoDeCarga);
    controlarBotoesCadastroMonitoramentoEventoTipoDeCargaHabilitados(true);
    exibirModalCadastroMonitoramentoEventoTipoDeCarga();
}

function excluirMonitoramentoEventoTipoDeCargaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o tipo de carga?", function () {
        removerMonitoramentoEventoTipoDeCarga(_cadastroMonitoramentoEventoTipoDeCarga.Codigo.val());
        fecharModalCadastroMonitoramentoEventoTipoDeCarga();
    });
}

/*
 * Declaração das Funções Públicas
 */

function obterMonitoramentoEventoTipoDeCargaSalvar() {
    var listaTipoDeCarga = obterListaTipoDeCarga();
    return JSON.stringify(listaTipoDeCarga);
}

function preencherMonitoramentoEventoTipoDeCarga(dados) {
    _monitoramentoEventoTipoDeCarga.ListaTipoDeCarga.val(dados);
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroMonitoramentoEventoTipoDeCargaHabilitados(isEdicao) {
    _CRUDcadastroMonitoramentoEventoTipoDeCarga.Adicionar.visible(!isEdicao);
    _CRUDcadastroMonitoramentoEventoTipoDeCarga.Atualizar.visible(isEdicao);
    _CRUDcadastroMonitoramentoEventoTipoDeCarga.Excluir.visible(isEdicao);
}

function exibirModalCadastroMonitoramentoEventoTipoDeCarga() {
    Global.abrirModal('divModalCadastroMonitoramentoEventoTipoDeCarga');
    $("#divModalCadastroMonitoramentoEventoTipoDeCarga").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroMonitoramentoEventoTipoDeCarga);
    });
}

function fecharModalCadastroMonitoramentoEventoTipoDeCarga() {
    Global.fecharModal('divModalCadastroMonitoramentoEventoTipoDeCarga');
}

function limparCamposMonitoramentoEventoTipoDeCarga() {
    preencherMonitoramentoEventoTipoDeCarga([]);
}

function obterCadastroMonitoramentoEventoTipoDeCargaSalvar() {
    return {
        Codigo: _cadastroMonitoramentoEventoTipoDeCarga.Codigo.val(),
        CodigoTipoDeCarga: _cadastroMonitoramentoEventoTipoDeCarga.TipoDeCarga.codEntity(),
        DescricaoTipoDeCarga: _cadastroMonitoramentoEventoTipoDeCarga.TipoDeCarga.val()
    };
}

function obterListaTipoDeCarga() {
    return _monitoramentoEventoTipoDeCarga.ListaTipoDeCarga.val().slice();
}

function recarregarGridMonitoramentoEventoTipoDeCarga() {
    var listaTipoDeCarga = obterListaTipoDeCarga();
    _gridMonitoramentoEventoTipoDeCarga.CarregarGrid(listaTipoDeCarga);
}

function removerMonitoramentoEventoTipoDeCarga(codigo) {
    var listaTipoDeCarga = obterListaTipoDeCarga();
    listaTipoDeCarga.forEach(function (row, i) {
        if (codigo == row.Codigo) {
            listaTipoDeCarga.splice(i, 1);
        }
    });
    _monitoramentoEventoTipoDeCarga.ListaTipoDeCarga.val(listaTipoDeCarga);
}

function validarCamposObrigatoriosMonitoramentoEventoTipoDeCarga() {
    if (_monitoramentoEventoTipoDeCarga.VerificarTipoDeCarga.val() != EnumVerificarTipoDeCarga.NaoVerificar) {
        var listaTipoDeCarga = obterListaTipoDeCarga();
        if (listaTipoDeCarga.length == 0) {
            return false;
        }
    }
    return true;
}
