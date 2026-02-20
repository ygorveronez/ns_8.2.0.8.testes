/// <reference path="../../Consultas/ModeloVeiculo.js" />
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
/// <reference path="ModeloVeiculo.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridMarcaVeiculo;
var _marcaVeiculo;
var _pesquisaMarcaVeiculo;

var _TipoMarcaVeiculo = [
    { text: "Tração", value: 1 },
    { text: "Reboque", value: 2 }
];

var _SimNao = [
    { text: "Sim", value: 1 },
    { text: "Não", value: 2 }
];

var PesquisaMarcaVeiculo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Modelo = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: false, text: "Modelo do Veículo:", idBtnSearch: guid(), val: ko.observable("") });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMarcaVeiculo.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var MarcaVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 200 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração: ", maxlength: 50 });
    this.TipoVeiculo = PropertyEntity({ val: ko.observable(1), options: _TipoMarcaVeiculo, eventChange: tipoVeiculoChange, text: "*Tipo do Veículo: ", def: 1, issue: 152 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.Modelos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });

    this.CodigoModelo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescricaoModelo = PropertyEntity({ text: "*Descrição:", required: false, maxlength: 100, val: ko.observable("") });
    this.NumeroEixosModelo = PropertyEntity({ text: ko.observable("*Nº Eixos:"), required: false, getType: typesKnockout.int, maxlength: 2, val: ko.observable(0) });
    this.AtivoModelo = PropertyEntity({ val: ko.observable(true), options: _status, def: ko.observable(true), text: "*Situação:" });
    this.TipoCombustivel = PropertyEntity({ visible: ko.observable(true), type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("*Combustível:"), idBtnSearch: guid(), val: ko.observable("") });
    this.MotorArla = PropertyEntity({ val: ko.observable(1), options: _SimNao, def: ko.observable(1), text: "*Aceita ARLA:", visible: ko.observable(true) });
    this.MediaPadrao = PropertyEntity({ text: ko.observable("*Média Padrão:"), required: false, getType: typesKnockout.decimal, visible: ko.observable(true), configDecimal: { precision: 4, allowZero: true }, maxlength: 8, val: ko.observable(0) });
    this.MediaPadraoVazio = PropertyEntity({ text: ko.observable("*Média Padrão Vazio:"), required: false, getType: typesKnockout.decimal, visible: ko.observable(true), configDecimal: { precision: 4, allowZero: true }, maxlength: 8, val: ko.observable(0) });
    this.AlturaEmMetros = PropertyEntity({ text: ko.observable("Altura em Metros:"), required: false, getType: typesKnockout.decimal, visible: ko.observable(true), configDecimal: { precision: 2, allowZero: true }, maxlength: 8, val: ko.observable(0) });
    this.MediaMinima = PropertyEntity({ text: ko.observable("Média Mínima:"), required: false, getType: typesKnockout.decimal, visible: ko.observable(true), configDecimal: { precision: 4, allowZero: true }, maxlength: 8, val: ko.observable(0) });
    this.MediaMaxima = PropertyEntity({ text: ko.observable("Média Máxima:"), required: false, getType: typesKnockout.decimal, visible: ko.observable(true), configDecimal: { precision: 4, allowZero: true }, maxlength: 8, val: ko.observable(0) });
    this.CodigoFIPE = PropertyEntity({ text: "Código FIPE:", required: false, maxlength: 10, visible: ko.observable(true), val: ko.observable("") });

    this.AdicionarModelo = PropertyEntity({ eventClick: adicionarModeloVeiculoClick, type: types.event, text: ko.observable("Adicionar Modelo"), visible: ko.observable(true) });
};

//*******EVENTOS*******


function loadMarcaVeiculo() {

    _marcaVeiculo = new MarcaVeiculo();
    KoBindings(_marcaVeiculo, "knockoutCadastroMarcaVeiculo");

    _pesquisaMarcaVeiculo = new PesquisaMarcaVeiculo();
    KoBindings(_pesquisaMarcaVeiculo, "knockoutPesquisaMarcaVeiculo", false, _pesquisaMarcaVeiculo.Pesquisar.id);

    HeaderAuditoria("MarcaVeiculo", _marcaVeiculo);

    new BuscarModelosVeiculo(_pesquisaMarcaVeiculo.Modelo);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _marcaVeiculo.NumeroEixosModelo.text("Nº Eixos:");
        _marcaVeiculo.MediaPadrao.text("Média Padrão:");
        _marcaVeiculo.TipoCombustivel.text("Combustível:");
    }

    buscarMarcaVeiculos();
    loadModeloVeiculo();
}

function tipoVeiculoChange(e, sender) {
    if (e.TipoVeiculo.val() == 1) {
        e.TipoCombustivel.visible(true);
        e.MotorArla.visible(true);
        e.MediaPadrao.visible(true);
        e.MediaMinima.visible(true);
        e.MediaMaxima.visible(true);
        e.CodigoFIPE.visible(true);
    } else {
        e.TipoCombustivel.visible(false);
        e.MotorArla.visible(false);
        e.MediaPadrao.visible(false);
        e.MediaMinima.visible(false);
        e.MediaMaxima.visible(false);
        e.CodigoFIPE.visible(false);
    }
}

function adicionarClick(e, sender) {
    resetarTabs();
    Salvar(e, "MarcaVeiculo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridMarcaVeiculo.CarregarGrid();
                limparCamposMarcaVeiculo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    resetarTabs();
    Salvar(e, "MarcaVeiculo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMarcaVeiculo.CarregarGrid();
                limparCamposMarcaVeiculo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a marca " + _marcaVeiculo.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_marcaVeiculo, "MarcaVeiculo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMarcaVeiculo.CarregarGrid();
                    limparCamposMarcaVeiculo();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposMarcaVeiculo();
}

//*******MÉTODOS*******


function buscarMarcaVeiculos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMarcaVeiculo, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridMarcaVeiculo = new GridView(_pesquisaMarcaVeiculo.Pesquisar.idGrid, "MarcaVeiculo/Pesquisa", _pesquisaMarcaVeiculo, menuOpcoes, null);
    _gridMarcaVeiculo.CarregarGrid();
}

function editarMarcaVeiculo(marcaGrid) {
    limparCamposMarcaVeiculo();
    _marcaVeiculo.Codigo.val(marcaGrid.Codigo);
    BuscarPorCodigo(_marcaVeiculo, "MarcaVeiculo/BuscarPorCodigo", function (arg) {
        _pesquisaMarcaVeiculo.ExibirFiltros.visibleFade(false);
        _marcaVeiculo.Atualizar.visible(true);
        _marcaVeiculo.Cancelar.visible(true);
        _marcaVeiculo.Excluir.visible(true);
        _marcaVeiculo.Adicionar.visible(false);
        tipoVeiculoChange(_marcaVeiculo);
        recarregarGridModeloVeiculo();
    }, null);
}

function limparCamposMarcaVeiculo() {
    _marcaVeiculo.Atualizar.visible(false);
    _marcaVeiculo.Cancelar.visible(false);
    _marcaVeiculo.Excluir.visible(false);
    _marcaVeiculo.Adicionar.visible(true);
    LimparCampos(_marcaVeiculo);
    tipoVeiculoChange(_marcaVeiculo);
    recarregarGridModeloVeiculo();
    resetarTabs();
    _marcaVeiculo.AdicionarModelo.text("Adicionar Modelo");
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}
