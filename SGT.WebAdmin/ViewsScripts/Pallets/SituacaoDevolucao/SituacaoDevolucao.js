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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridSituacaoDevolucao;
var _situacaoDevolucao;
var _pesquisaSituacaoDevolucao;

var PesquisaSituacaoDevolucao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSituacaoDevolucao.CarregarGrid();
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

var SituacaoDevolucao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 150 });
    this.ValorUnitario = PropertyEntity({ text: "Valor Unitário:", maxlength: 10, getType: typesKnockout.decimal });
    this.AcresceSaldo = PropertyEntity({ text: "Acresce no Saldo?", val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.SituacaoPalletAvariado = PropertyEntity({ text: "Situação de Pallet Avariado?", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.SituacaoPalletDescartado = PropertyEntity({ text: "Situação de Pallet Descartado?", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadSituacaoDevolucao() {

    _pesquisaSituacaoDevolucao = new PesquisaSituacaoDevolucao();
    KoBindings(_pesquisaSituacaoDevolucao, "knockoutPesquisaSituacaoDevolucao");

    _situacaoDevolucao = new SituacaoDevolucao();
    KoBindings(_situacaoDevolucao, "knockoutCadastroSituacaoDevolucao");

    HeaderAuditoria("SituacaoDevolucaoPallet", _situacaoDevolucao);

    buscarSituacaoDevolucao();
}

function AdicionarClick(e, sender) {
    Salvar(_situacaoDevolucao, "SituacaoDevolucao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridSituacaoDevolucao.CarregarGrid();
                LimparCamposSituacaoDevolucao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, ExibirCamposObrigatorios);
}

function AtualizarClick(e, sender) {
    Salvar(_situacaoDevolucao, "SituacaoDevolucao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridSituacaoDevolucao.CarregarGrid();
                LimparCamposSituacaoDevolucao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, ExibirCamposObrigatorios);
}

function ExcluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a situação de devolução " + _situacaoDevolucao.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_situacaoDevolucao, "SituacaoDevolucao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridSituacaoDevolucao.CarregarGrid();
                    LimparCamposSituacaoDevolucao();
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
    LimparCamposSituacaoDevolucao();
}

//*******MÉTODOS*******


function EditarSituacaoDevolucao(situacaoDevolucaoGrid) {
    LimparCamposSituacaoDevolucao();
    _situacaoDevolucao.Codigo.val(situacaoDevolucaoGrid.Codigo);
    BuscarPorCodigo(_situacaoDevolucao, "SituacaoDevolucao/BuscarPorCodigo", function (arg) {
        _pesquisaSituacaoDevolucao.ExibirFiltros.visibleFade(false);
        _situacaoDevolucao.Atualizar.visible(true);
        _situacaoDevolucao.Cancelar.visible(true);
        _situacaoDevolucao.Excluir.visible(true);
        _situacaoDevolucao.Adicionar.visible(false);
    }, null);
}


function buscarSituacaoDevolucao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarSituacaoDevolucao, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridSituacaoDevolucao = new GridView(_pesquisaSituacaoDevolucao.Pesquisar.idGrid, "SituacaoDevolucao/Pesquisa", _pesquisaSituacaoDevolucao, menuOpcoes, null);
    _gridSituacaoDevolucao.CarregarGrid();
}


function LimparCamposSituacaoDevolucao() {
    _situacaoDevolucao.Atualizar.visible(false);
    _situacaoDevolucao.Cancelar.visible(false);
    _situacaoDevolucao.Excluir.visible(false);
    _situacaoDevolucao.Adicionar.visible(true);
    LimparCampos(_situacaoDevolucao);
}

function ExibirCamposObrigatorios() {
    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios!");
}