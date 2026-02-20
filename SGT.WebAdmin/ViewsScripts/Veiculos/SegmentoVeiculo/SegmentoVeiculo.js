/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridSegmentoVeiculo;
var _segmentoVeiculo;
var _pesquisaSegmentoVeiculo;

var PesquisaSegmentoVeiculo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSegmentoVeiculo.CarregarGrid();
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

var SegmentoVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 300 });    
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.ValorMinimo = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "Valor Mínimo:", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.MetaMensal = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "Meta Mensal:", required: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadSegmentoVeiculo() {

    _pesquisaSegmentoVeiculo = new PesquisaSegmentoVeiculo();
    KoBindings(_pesquisaSegmentoVeiculo, "knockoutPesquisaSegmentoVeiculo");

    _segmentoVeiculo = new SegmentoVeiculo();
    KoBindings(_segmentoVeiculo, "knockoutCadastroSegmentoVeiculo");

    HeaderAuditoria("SegmentoVeiculo", _segmentoVeiculo);

    buscarSegmentosVeiculo();
}

function AdicionarClick(e, sender) {
    Salvar(_segmentoVeiculo, "SegmentoVeiculo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridSegmentoVeiculo.CarregarGrid();
                LimparCamposSegmentoVeiculo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, ExibirCamposObrigatorios);
}

function AtualizarClick(e, sender) {
    Salvar(_segmentoVeiculo, "SegmentoVeiculo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridSegmentoVeiculo.CarregarGrid();
                LimparCamposSegmentoVeiculo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, ExibirCamposObrigatorios);
}

function ExcluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o segmento do veículo" + _segmentoVeiculo.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_segmentoVeiculo, "SegmentoVeiculo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridSegmentoVeiculo.CarregarGrid();
                    LimparCamposSegmentoVeiculo();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function CancelarClick(e) {
    LimparCamposSegmentoVeiculo();
}

//*******MÉTODOS*******


function EditarSegmentoVeiculo(marcaEquipamentoGrid) {
    LimparCamposSegmentoVeiculo();
    _segmentoVeiculo.Codigo.val(marcaEquipamentoGrid.Codigo);
    BuscarPorCodigo(_segmentoVeiculo, "SegmentoVeiculo/BuscarPorCodigo", function (arg) {
        _pesquisaSegmentoVeiculo.ExibirFiltros.visibleFade(false);
        _segmentoVeiculo.Atualizar.visible(true);
        _segmentoVeiculo.Cancelar.visible(true);
        _segmentoVeiculo.Excluir.visible(true);
        _segmentoVeiculo.Adicionar.visible(false);
    }, null);
}


function buscarSegmentosVeiculo() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarSegmentoVeiculo, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridSegmentoVeiculo = new GridView(_pesquisaSegmentoVeiculo.Pesquisar.idGrid, "SegmentoVeiculo/Pesquisa", _pesquisaSegmentoVeiculo, menuOpcoes, null);
    _gridSegmentoVeiculo.CarregarGrid();
}


function LimparCamposSegmentoVeiculo() {
    _segmentoVeiculo.Atualizar.visible(false);
    _segmentoVeiculo.Cancelar.visible(false);
    _segmentoVeiculo.Excluir.visible(false);
    _segmentoVeiculo.Adicionar.visible(true);
    LimparCampos(_segmentoVeiculo);
}

function ExibirCamposObrigatorios() {
    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios!");
}