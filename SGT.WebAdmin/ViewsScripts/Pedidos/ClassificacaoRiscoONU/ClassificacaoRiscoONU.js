/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/NumeroONU.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridClassificacaoRiscoONU
var _classificacaoRiscoONU
var _pesquisaClassificacaoRiscoONU

var PesquisaClassificacaoRiscoONU = function () {

    this.Descricao = PropertyEntity({ text: "Descrição: ",issue:586});
    this.NumeroONU = PropertyEntity({ text: "Número ONU: ", issue: 1217});
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: ", issue: 556 });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridClassificacaoRiscoONU.CarregarGrid();
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
}

var ClassificacaoRiscoONU = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:",issue: 586, required: true, maxlength: 500 });
    this.NumeroONU = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("*ONU:"),issue: 1217,  idBtnSearch: guid(), enable: ko.observable(true) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue:557 });

    this.ClasseRisco = PropertyEntity({ text: "Classe de Risco:",issue: 1218, required: false, maxlength: 500 });
    this.GrupoEmbarcado = PropertyEntity({ text: "Grupo Embarcador:", issue: 1219, required: false, maxlength: 500 });
    this.NumeroRisco = PropertyEntity({ text: "Número de Risco:", issue: 1220, required: false, maxlength: 500 });
    this.RiscoSubsidiario = PropertyEntity({ text: "Risco Subsidiário:", issue: 1221, required: false, maxlength: 500 });

    this.ProvisoesEspeciais = PropertyEntity({ text: "Provisões Especiais:", issue: 1222, required: false, maxlength: 500 });
    this.LimiteKGVeiculo = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true, allowNegative: false }, maxlength: 15, text: "Qtd. Limitada por Veículo (kg):", issue: 1223, required: false, visible: ko.observable(true) });
    this.LimiteLitroEmbalagemInterna = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: true, allowNegative: false }, maxlength: 15, text: "Qtd. Limitada por Embalagem (Lt):", issue: 1224, required: false, visible: ko.observable(true) });
    this.EmbalagemInstrucao = PropertyEntity({ text: "Instruções para Embalagem:", issue: 1215, required: false, maxlength: 500 });
    this.EmbalagemProvisoesEspeciais = PropertyEntity({ text: "Provisões para Embalagem:", issue: 1215, required: false, maxlength: 500 });
    this.TanqueInstrucao = PropertyEntity({ text: "Instruções para Tanque:", issue: 1215, required: false, maxlength: 500 });
    this.TanqueProvisoesEspeciais = PropertyEntity({ text: "Provisões para Tanque:", issue: 1215, required: false, maxlength: 500 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadClassificacaoRiscoONU() {

    _classificacaoRiscoONU = new ClassificacaoRiscoONU();
    KoBindings(_classificacaoRiscoONU, "knockoutCadastroClassificacaoRiscoONU");

    _pesquisaClassificacaoRiscoONU = new PesquisaClassificacaoRiscoONU();
    KoBindings(_pesquisaClassificacaoRiscoONU, "knockoutPesquisaClassificacaoRiscoONU", false, _pesquisaClassificacaoRiscoONU.Pesquisar.id);

    new BuscarONUS(_classificacaoRiscoONU.NumeroONU, retornoNumeroONU);

    HeaderAuditoria("ClassificacaoRiscoONU", _classificacaoRiscoONU);

    buscarClassificacaoRiscoONUs();
}

function numeroONUExit() {
    if ($("#" + _classificacaoRiscoONU.NumeroONU.id).val() != "") {
        _classificacaoRiscoONU.NumeroONU.codEntity($("#" + _classificacaoRiscoONU.NumeroONU.id).val());
    }
}

function retornoNumeroONU(e, sender) {
    _classificacaoRiscoONU.NumeroONU.val(e.Descricao);
    _classificacaoRiscoONU.NumeroONU.codEntity(e.Descricao);
}

function adicionarClick(e, sender) {
    Salvar(e, "ClassificacaoRiscoONU/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridClassificacaoRiscoONU.CarregarGrid();
                limparCamposClassificacaoRiscoONU();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ClassificacaoRiscoONU/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridClassificacaoRiscoONU.CarregarGrid();
                limparCamposClassificacaoRiscoONU();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o terminal selecionado?", function () {
        ExcluirPorCodigo(_classificacaoRiscoONU, "ClassificacaoRiscoONU/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridClassificacaoRiscoONU.CarregarGrid();
                limparCamposClassificacaoRiscoONU();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposClassificacaoRiscoONU();
}

//*******MÉTODOS*******


function buscarClassificacaoRiscoONUs() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClassificacaoRiscoONU, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridClassificacaoRiscoONU = new GridView(_pesquisaClassificacaoRiscoONU.Pesquisar.idGrid, "ClassificacaoRiscoONU/Pesquisa", _pesquisaClassificacaoRiscoONU, menuOpcoes, null);
    _gridClassificacaoRiscoONU.CarregarGrid();
}

function editarClassificacaoRiscoONU(classificacaoRiscoONUGrid) {
    limparCamposClassificacaoRiscoONU();
    _classificacaoRiscoONU.Codigo.val(classificacaoRiscoONUGrid.Codigo);
    BuscarPorCodigo(_classificacaoRiscoONU, "ClassificacaoRiscoONU/BuscarPorCodigo", function (arg) {
        _pesquisaClassificacaoRiscoONU.ExibirFiltros.visibleFade(false);
        _classificacaoRiscoONU.Atualizar.visible(true);
        _classificacaoRiscoONU.Cancelar.visible(true);
        _classificacaoRiscoONU.Excluir.visible(true);
        _classificacaoRiscoONU.Adicionar.visible(false);
    }, null);
}

function limparCamposClassificacaoRiscoONU() {
    _classificacaoRiscoONU.Atualizar.visible(false);
    _classificacaoRiscoONU.Cancelar.visible(false);
    _classificacaoRiscoONU.Excluir.visible(false);
    _classificacaoRiscoONU.Adicionar.visible(true);
    LimparCampos(_classificacaoRiscoONU);
}
