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

var _tipoEmbalagem;
var _pesquisaTipoEmbalagem;
var _gridTipoEmbalagem;

var TipoEmbalagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CodigoIntegracao = PropertyEntity({ text: "*Código Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação", required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaTipoEmbalagem = function () {
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:",  required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", issue: 557, val: ko.observable(true), options: _statusPesquisa, def: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoEmbalagem.CarregarGrid();
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
function loadTipoEmbalagem() {
    _pesquisaTipoEmbalagem = new PesquisaTipoEmbalagem();
    KoBindings(_pesquisaTipoEmbalagem, "knockoutPesquisaTipoEmbalagem", false, _pesquisaTipoEmbalagem.Pesquisar.id);

    _tipoEmbalagem = new TipoEmbalagem();
    KoBindings(_tipoEmbalagem, "knockoutTipoEmbalagem");

    HeaderAuditoria("TipoEmbalagem", _tipoEmbalagem);

    buscarTipoEmbalagem();
}

function adicionarClick(e, sender) {
    Salvar(_tipoEmbalagem, "TipoEmbalagem/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTipoEmbalagem.CarregarGrid();
                limparCamposTipoEmbalagem();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tipoEmbalagem, "TipoEmbalagem/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoEmbalagem.CarregarGrid();
                limparCamposTipoEmbalagem();
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
        ExcluirPorCodigo(_tipoEmbalagem, "TipoEmbalagem/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTipoEmbalagem.CarregarGrid();
                    limparCamposTipoEmbalagem();
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
    limparCamposTipoEmbalagem();
}

function editarTipoEmbalagemClick(itemGrid) {
    // Limpa os campos
    limparCamposTipoEmbalagem();

    _tipoEmbalagem.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_tipoEmbalagem, "TipoEmbalagem/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaTipoEmbalagem.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _tipoEmbalagem.Atualizar.visible(true);
                _tipoEmbalagem.Excluir.visible(true);
                _tipoEmbalagem.Cancelar.visible(true);
                _tipoEmbalagem.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarTipoEmbalagem() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoEmbalagemClick, tamanho: "7", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "TipoEmbalagem/ExportarPesquisa",
        titulo: "Tipo Embalagem"
    };


    // Inicia Grid de busca
    _gridTipoEmbalagem = new GridViewExportacao(_pesquisaTipoEmbalagem.Pesquisar.idGrid, "TipoEmbalagem/Pesquisa", _pesquisaTipoEmbalagem, menuOpcoes, configExportacao);
    _gridTipoEmbalagem.CarregarGrid();
}

function limparCamposTipoEmbalagem() {
    _tipoEmbalagem.Atualizar.visible(false);
    _tipoEmbalagem.Cancelar.visible(false);
    _tipoEmbalagem.Excluir.visible(false);
    _tipoEmbalagem.Adicionar.visible(true);
    LimparCampos(_tipoEmbalagem);
}