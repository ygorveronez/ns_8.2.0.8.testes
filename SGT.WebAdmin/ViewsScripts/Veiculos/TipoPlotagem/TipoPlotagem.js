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

var _gridTipoPlotagem;
var _tipoPlotagem;
var _pesquisaTipoPlotagem;

var PesquisaTipoPlotagem = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoPlotagem.CarregarGrid();
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

var TipoPlotagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 300 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadTipoPlotagem() {

    _pesquisaTipoPlotagem = new PesquisaTipoPlotagem();
    KoBindings(_pesquisaTipoPlotagem, "knockoutPesquisaTipoPlotagem");

    _tipoPlotagem = new TipoPlotagem();
    KoBindings(_tipoPlotagem, "knockoutCadastroTipoPlotagem");

    HeaderAuditoria("TipoPlotagem", _tipoPlotagem);

    buscarTipoPlotagem();
}

function AdicionarClick(e, sender) {
    Salvar(_tipoPlotagem, "TipoPlotagem/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTipoPlotagem.CarregarGrid();
                LimparCamposTipoPlotagem();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, ExibirCamposObrigatorios);
}

function AtualizarClick(e, sender) {
    Salvar(_tipoPlotagem, "TipoPlotagem/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoPlotagem.CarregarGrid();
                LimparCamposTipoPlotagem();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, ExibirCamposObrigatorios);
}

function ExcluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o segmento do veículo" + _tipoPlotagem.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoPlotagem, "TipoPlotagem/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTipoPlotagem.CarregarGrid();
                    LimparCamposTipoPlotagem();
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
    LimparCamposTipoPlotagem();
}

//*******MÉTODOS*******


function EditarSegmentoVeiculo(marcaEquipamentoGrid) {
    LimparCamposTipoPlotagem();
    _tipoPlotagem.Codigo.val(marcaEquipamentoGrid.Codigo);
    BuscarPorCodigo(_tipoPlotagem, "TipoPlotagem/BuscarPorCodigo", function (arg) {
        _pesquisaTipoPlotagem.ExibirFiltros.visibleFade(false);
        _tipoPlotagem.Atualizar.visible(true);
        _tipoPlotagem.Cancelar.visible(true);
        _tipoPlotagem.Excluir.visible(true);
        _tipoPlotagem.Adicionar.visible(false);
    }, null);
}


function buscarTipoPlotagem() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarSegmentoVeiculo, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoPlotagem = new GridView(_pesquisaTipoPlotagem.Pesquisar.idGrid, "TipoPlotagem/Pesquisa", _pesquisaTipoPlotagem, menuOpcoes, null);
    _gridTipoPlotagem.CarregarGrid();
}


function LimparCamposTipoPlotagem() {
    _tipoPlotagem.Atualizar.visible(false);
    _tipoPlotagem.Cancelar.visible(false);
    _tipoPlotagem.Excluir.visible(false);
    _tipoPlotagem.Adicionar.visible(true);
    LimparCampos(_tipoPlotagem);
}

function ExibirCamposObrigatorios() {
    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios!");
}