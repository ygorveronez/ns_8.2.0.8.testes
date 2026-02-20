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

var _centroDistribuicao;
var _pesquisaCentroDistribuicao;
var _gridCentroDistribuicao;

var CentroDistribuicao = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaCentroDistribuicao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCentroDistribuicao.CarregarGrid();
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


//*******EVENTOS*******
function loadCentroDistribuicao() {
    _pesquisaCentroDistribuicao = new PesquisaCentroDistribuicao();
    KoBindings(_pesquisaCentroDistribuicao, "knockoutPesquisaCentroDistribuicao", false, _pesquisaCentroDistribuicao.Pesquisar.id);

    _centroDistribuicao = new CentroDistribuicao();
    KoBindings(_centroDistribuicao, "knockoutCentroDistribuicao");

    HeaderAuditoria("CentroDistribuicao", _centroDistribuicao);

    BuscarCentroDistribuicao();
}

function adicionarClick(e, sender) {
    Salvar(_centroDistribuicao, "CentroDistribuicao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridCentroDistribuicao.CarregarGrid();
                LimparCamposCentroDistribuicao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_centroDistribuicao, "CentroDistribuicao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCentroDistribuicao.CarregarGrid();
                LimparCamposCentroDistribuicao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_centroDistribuicao, "CentroDistribuicao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCentroDistribuicao.CarregarGrid();
                    LimparCamposCentroDistribuicao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    LimparCamposCentroDistribuicao();
}

function editarCentroDistribuicaoClick(itemGrid) {
    LimparCamposCentroDistribuicao();
    _centroDistribuicao.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_centroDistribuicao, "CentroDistribuicao/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaCentroDistribuicao.ExibirFiltros.visibleFade(false);

                _centroDistribuicao.Atualizar.visible(true);
                _centroDistribuicao.Excluir.visible(true);
                _centroDistribuicao.Cancelar.visible(true);
                _centroDistribuicao.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function BuscarCentroDistribuicao() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCentroDistribuicaoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridCentroDistribuicao = new GridView(_pesquisaCentroDistribuicao.Pesquisar.idGrid, "CentroDistribuicao/Pesquisa", _pesquisaCentroDistribuicao, menuOpcoes, null);
    _gridCentroDistribuicao.CarregarGrid();
}

function LimparCamposCentroDistribuicao() {
    _centroDistribuicao.Atualizar.visible(false);
    _centroDistribuicao.Cancelar.visible(false);
    _centroDistribuicao.Excluir.visible(false);
    _centroDistribuicao.Adicionar.visible(true);
    LimparCampos(_centroDistribuicao);
}