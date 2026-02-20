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
/// <reference path="../../Enumeradores/EnumTipoBonificacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoBonificacao;
var _tipoBonificacao;
var _pesquisaTipoBonificacao;

var PesquisaTipoBonificacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoBonificacao.CarregarGrid();
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

var TipoBonificacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });    
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadTipoBonificacao() {

    _tipoBonificacao = new TipoBonificacao();
    KoBindings(_tipoBonificacao, "knockoutCadastroTipoBonificacao");

    _pesquisaTipoBonificacao = new PesquisaTipoBonificacao();
    KoBindings(_pesquisaTipoBonificacao, "knockoutPesquisaTipoBonificacao", false, _pesquisaTipoBonificacao.Pesquisar.id);

    HeaderAuditoria("TipoBonificacao", _tipoBonificacao);

    buscarTipoBonificacaos();
}

function adicionarClick(e, sender) {
    Salvar(e, "TipoBonificacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoBonificacao.CarregarGrid();
                limparCamposTipoBonificacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "TipoBonificacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoBonificacao.CarregarGrid();
                limparCamposTipoBonificacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o tipo da bonificação " + _tipoBonificacao.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoBonificacao, "TipoBonificacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTipoBonificacao.CarregarGrid();
                limparCamposTipoBonificacao();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTipoBonificacao();
}

//*******MÉTODOS*******


function buscarTipoBonificacaos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoBonificacao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoBonificacao = new GridView(_pesquisaTipoBonificacao.Pesquisar.idGrid, "TipoBonificacao/Pesquisa", _pesquisaTipoBonificacao, menuOpcoes, null);
    _gridTipoBonificacao.CarregarGrid();
}

function editarTipoBonificacao(tipoBonificacaoGrid) {
    limparCamposTipoBonificacao();
    _tipoBonificacao.Codigo.val(tipoBonificacaoGrid.Codigo);
    BuscarPorCodigo(_tipoBonificacao, "TipoBonificacao/BuscarPorCodigo", function (arg) {
        _pesquisaTipoBonificacao.ExibirFiltros.visibleFade(false);
        _tipoBonificacao.Atualizar.visible(true);
        _tipoBonificacao.Cancelar.visible(true);
        _tipoBonificacao.Excluir.visible(true);
        _tipoBonificacao.Adicionar.visible(false);
    }, null);
}

function limparCamposTipoBonificacao() {
    _tipoBonificacao.Atualizar.visible(false);
    _tipoBonificacao.Cancelar.visible(false);
    _tipoBonificacao.Excluir.visible(false);
    _tipoBonificacao.Adicionar.visible(true);
    LimparCampos(_tipoBonificacao);
}
