/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Turno.js" />
/// <reference path="Filial.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _crudSetorFilial;
var _setorFilial;
var _setorFilialBasicTable;
var _setorFilialTurnosBasicTable;

/*
 * Declaração das Classes
 */

var CRUDSetorFilial = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarSetorFilialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarSetorFilialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarSetorFilialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirSetorFilialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var SetorFilial = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });
    this.GridTurnos = PropertyEntity({ type: types.local });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Filiais.Filial.Setor.getRequiredFieldDescription(), maxlength: 200, idBtnSearch: guid() });
    this.Turnos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });

    this.AdicionarTurno = PropertyEntity({ type: types.event, text: Localization.Resources.Filiais.Filial.AdicionarTurno, idBtnSearch: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridSetorFilial() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoSetor", visible: false },
        { data: "DescricaoSetor", title: Localization.Resources.Filiais.Filial.Setor, width: "80%" }
    ];

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        tamanho: 7,
        opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(),
            metodo: editarSetorFilialClick
        }]
    };

    _setorFilialBasicTable = new BasicDataTable(_setorFilial.Grid.id, header, menuOpcoes);
}

function loadGridSetorFilialTurnos() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        tamanho: 7,
        opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir,
            id: guid(),
            metodo: excluirTurnoClick
        }]
    };

    _setorFilialTurnosBasicTable = new BasicDataTable(_setorFilial.GridTurnos.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTurno(_setorFilial.AdicionarTurno, adicionarTurnosCallback, _setorFilialTurnosBasicTable);

    _setorFilialTurnosBasicTable.CarregarGrid([]);
}

function loadSetorFilial() {
    _setorFilial = new SetorFilial();
    KoBindings(_setorFilial, "knockoutSetoresFilial");

    _crudSetorFilial = new CRUDSetorFilial();
    KoBindings(_crudSetorFilial, "knockoutCRUDSetoresFilial");

    loadGridSetorFilial();
    loadGridSetorFilialTurnos();

    new BuscarSetorFuncionario(_setorFilial.Setor, null);

    recarregarGridSetoresFilial();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarSetorFilialClick(e, sender) {
    if (ValidarCamposObrigatorios(_setorFilial)) {
        if (isSetorFilialExistente())
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Gerais.Geral.RegistroDuplicadoMensagem);
        else {
            _setorFilial.Codigo.val(guid());

            _filial.SetoresFilial.val().push(obterSetorFilialSalvar());

            limparCamposSetorFilial();
            recarregarGridSetoresFilial();
            setarFocoPrimeiroCampoCadastroSetorFilial();
        }
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function adicionarTurnosCallback(registrosSelecionados) {
    for (var i = 0; i < registrosSelecionados.length; i++) {
        _setorFilial.Turnos.val().push({
            Codigo: registrosSelecionados[i].Codigo,
            Descricao: registrosSelecionados[i].Descricao
        });
    }

    recarregarGridSetoresFilialTurnos();
}

function atualizarSetorFilialClick(e, sender) {
    if (ValidarCamposObrigatorios(_setorFilial)) {
        for (var i = 0; i < _filial.SetoresFilial.val().length; i++) {
            if (_setorFilial.Codigo.val() == _filial.SetoresFilial.val()[i].Codigo) {
                if (isSetorFilialExistente())
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Gerais.Geral.RegistroDuplicadoMensagem);
                else {
                    _filial.SetoresFilial.val()[i] = obterSetorFilialSalvar();

                    limparCamposSetorFilial();
                    recarregarGridSetoresFilial();
                    setarFocoPrimeiroCampoCadastroSetorFilial();

                    break;
                }
            }
        }


    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function cancelarSetorFilialClick(e) {
    limparCamposSetorFilial();
    setarFocoPrimeiroCampoCadastroSetorFilial();
}

function editarSetorFilialClick(registroSelecionado) {
    for (var i = 0; i < _filial.SetoresFilial.val().length; i++) {
        var setorFilial = _filial.SetoresFilial.val()[i];

        if (registroSelecionado.Codigo == setorFilial.Codigo) {
            var turnos = new Array();

            for (var j = 0; j < setorFilial.Turnos.length; j++) {
                turnos.push({
                    Codigo: setorFilial.Turnos[j].Codigo,
                    Descricao: setorFilial.Turnos[j].Descricao
                });
            }

            _setorFilial.Codigo.val(setorFilial.Codigo);
            _setorFilial.Setor.codEntity(setorFilial.Setor.Codigo);
            _setorFilial.Setor.val(setorFilial.Setor.Descricao);
            _setorFilial.Turnos.val(turnos);

            break;
        }
    }

    recarregarGridSetoresFilialTurnos();

    var isEdicao = true;

    controlarBotoesHabilitados(isEdicao);
}

function excluirSetorFilialClick(e, sender) {
    for (var i = 0; i < _filial.SetoresFilial.val().length; i++) {
        if (_setorFilial.Codigo.val() == _filial.SetoresFilial.val()[i].Codigo) {
            _filial.SetoresFilial.val().splice(i, 1);
            break;
        }
    }

    limparCamposSetorFilial();
    recarregarGridSetoresFilial();
    setarFocoPrimeiroCampoCadastroSetorFilial();
}

function excluirTurnoClick(registroSelecionado) {
    for (var i = 0; i < _setorFilial.Turnos.val().length; i++) {
        if (registroSelecionado.Codigo == _setorFilial.Turnos.val()[i].Codigo) {
            _setorFilial.Turnos.val().splice(i, 1);
            break;
        }
    }

    recarregarGridSetoresFilialTurnos();
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados(isEdicao) {
    _crudSetorFilial.Atualizar.visible(isEdicao);
    _crudSetorFilial.Excluir.visible(isEdicao);
    _crudSetorFilial.Cancelar.visible(isEdicao);
    _crudSetorFilial.Adicionar.visible(!isEdicao);
}

function isSetorFilialExistente() {
    for (var i = 0; i < _filial.SetoresFilial.val().length; i++) {
        if ((_setorFilial.Codigo.val() != _filial.SetoresFilial.val()[i].Codigo) && (_setorFilial.Setor.codEntity() == _filial.SetoresFilial.val()[i].Setor.Codigo))
            return true;
    }

    return false;
}

function limparCamposSetorFilial() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_setorFilial);
    limparGridSetorFilialTurnos();
}

function limparGridSetorFilialTurnos() {
    _setorFilialTurnosBasicTable.CarregarGrid(new Array());
}

function obterSetorFilialSalvar() {
    var turnos = new Array();

    for (var i = 0; i < _setorFilial.Turnos.val().length; i++) {
        turnos.push({
            Codigo: _setorFilial.Turnos.val()[i].Codigo,
            Descricao: _setorFilial.Turnos.val()[i].Descricao
        });
    }

    return {
        Codigo: _setorFilial.Codigo.val(),
        Setor: {
            Codigo: _setorFilial.Setor.codEntity(),
            Descricao: _setorFilial.Setor.val()
        },
        Turnos: turnos
    };
}

function preencherListaSetorFilialSalvar() {
    try {
        _filial.SetoresFilial.val(JSON.parse(_filial.SetoresFilial.val()));
    }
    catch (e) {
    }

    _filial.SetoresFilial.val(JSON.stringify(_filial.SetoresFilial.val()));
}

function recarregarGridSetoresFilial() {
    var listaSetoresFilialAdicionados = new Array();

    for (var i = 0; i < _filial.SetoresFilial.val().length; i++) {
        listaSetoresFilialAdicionados.push({
            Codigo: _filial.SetoresFilial.val()[i].Codigo,
            CodigoSetor: _filial.SetoresFilial.val()[i].Setor.Codigo,
            DescricaoSetor: _filial.SetoresFilial.val()[i].Setor.Descricao
        });
    }

    _setorFilialBasicTable.CarregarGrid(listaSetoresFilialAdicionados);
}

function recarregarGridSetoresFilialTurnos() {
    var turnos = new Array();

    for (var i = 0; i < _setorFilial.Turnos.val().length; i++) {
        turnos.push({
            Codigo: _setorFilial.Turnos.val()[i].Codigo,
            Descricao: _setorFilial.Turnos.val()[i].Descricao
        });
    }

    _setorFilialTurnosBasicTable.CarregarGrid(turnos);
}

function setarFocoPrimeiroCampoCadastroSetorFilial() {
    $("#" + _setorFilial.Setor.id).focus();
}