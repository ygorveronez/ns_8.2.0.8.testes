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

var _tributoVariacaoImposto;
var _pesquisaTributoVariacaoImposto;
var _gridTributoVariacaoImposto;

var TributoVariacaoImposto = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "*Cod. Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaTributoVariacaoImposto = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Cód. Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTributoVariacaoImposto.CarregarGrid();
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
function loadTributoVariacaoImposto() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaTributoVariacaoImposto = new PesquisaTributoVariacaoImposto();
    KoBindings(_pesquisaTributoVariacaoImposto, "knockoutPesquisaTributoVariacaoImposto", false, _pesquisaTributoVariacaoImposto.Pesquisar.id);

    // Instancia objeto principal
    _tributoVariacaoImposto = new TributoVariacaoImposto();
    KoBindings(_tributoVariacaoImposto, "knockoutTributoVariacaoImposto");

    HeaderAuditoria("TributoVariacaoImposto", _tributoVariacaoImposto);

    // Inicia busca
    buscarTributoVariacaoImposto();
}

function adicionarClick(e, sender) {
    Salvar(_tributoVariacaoImposto, "TributoVariacaoImposto/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTributoVariacaoImposto.CarregarGrid();
                limparCamposTributoVariacaoImposto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tributoVariacaoImposto, "TributoVariacaoImposto/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTributoVariacaoImposto.CarregarGrid();
                limparCamposTributoVariacaoImposto();
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
        ExcluirPorCodigo(_tributoVariacaoImposto, "TributoVariacaoImposto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTributoVariacaoImposto.CarregarGrid();
                    limparCamposTributoVariacaoImposto();
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
    limparCamposTributoVariacaoImposto();
}

function editarTributoVariacaoImpostoClick(itemGrid) {
    // Limpa os campos
    limparCamposTributoVariacaoImposto();

    // Seta o codigo do objeto
    _tributoVariacaoImposto.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_tributoVariacaoImposto, "TributoVariacaoImposto/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaTributoVariacaoImposto.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _tributoVariacaoImposto.Atualizar.visible(true);
                _tributoVariacaoImposto.Excluir.visible(true);
                _tributoVariacaoImposto.Cancelar.visible(true);
                _tributoVariacaoImposto.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarTributoVariacaoImposto() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTributoVariacaoImpostoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridTributoVariacaoImposto = new GridView(_pesquisaTributoVariacaoImposto.Pesquisar.idGrid, "TributoVariacaoImposto/Pesquisa", _pesquisaTributoVariacaoImposto, menuOpcoes, null);
    _gridTributoVariacaoImposto.CarregarGrid();
}

function limparCamposTributoVariacaoImposto() {
    _tributoVariacaoImposto.Atualizar.visible(false);
    _tributoVariacaoImposto.Cancelar.visible(false);
    _tributoVariacaoImposto.Excluir.visible(false);
    _tributoVariacaoImposto.Adicionar.visible(true);
    LimparCampos(_tributoVariacaoImposto);
}