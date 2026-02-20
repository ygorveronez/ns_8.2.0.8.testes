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

var _tributoCodigoReceita;
var _pesquisaTributoCodigoReceita;
var _gridTributoCodigoReceita;

var TributoCodigoReceita = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "*Cod. Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });    
    this.TributoVariacaoImposto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Variação:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaTributoCodigoReceita = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Cód. Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTributoCodigoReceita.CarregarGrid();
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
function loadTributoCodigoReceita() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaTributoCodigoReceita = new PesquisaTributoCodigoReceita();
    KoBindings(_pesquisaTributoCodigoReceita, "knockoutPesquisaTributoCodigoReceita", false, _pesquisaTributoCodigoReceita.Pesquisar.id);

    // Instancia objeto principal
    _tributoCodigoReceita = new TributoCodigoReceita();
    KoBindings(_tributoCodigoReceita, "knockoutTributoCodigoReceita");

    new BuscarTributoVariacaoImposto(_tributoCodigoReceita.TributoVariacaoImposto);

    HeaderAuditoria("TributoCodigoReceita", _tributoCodigoReceita);

    // Inicia busca
    buscarTributoCodigoReceita();
}

function adicionarClick(e, sender) {
    Salvar(_tributoCodigoReceita, "TributoCodigoReceita/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTributoCodigoReceita.CarregarGrid();
                limparCamposTributoCodigoReceita();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tributoCodigoReceita, "TributoCodigoReceita/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTributoCodigoReceita.CarregarGrid();
                limparCamposTributoCodigoReceita();
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
        ExcluirPorCodigo(_tributoCodigoReceita, "TributoCodigoReceita/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTributoCodigoReceita.CarregarGrid();
                    limparCamposTributoCodigoReceita();
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
    limparCamposTributoCodigoReceita();
}

function editarTributoCodigoReceitaClick(itemGrid) {
    // Limpa os campos
    limparCamposTributoCodigoReceita();

    // Seta o codigo do objeto
    _tributoCodigoReceita.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_tributoCodigoReceita, "TributoCodigoReceita/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaTributoCodigoReceita.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _tributoCodigoReceita.Atualizar.visible(true);
                _tributoCodigoReceita.Excluir.visible(true);
                _tributoCodigoReceita.Cancelar.visible(true);
                _tributoCodigoReceita.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarTributoCodigoReceita() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTributoCodigoReceitaClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridTributoCodigoReceita = new GridView(_pesquisaTributoCodigoReceita.Pesquisar.idGrid, "TributoCodigoReceita/Pesquisa", _pesquisaTributoCodigoReceita, menuOpcoes, null);
    _gridTributoCodigoReceita.CarregarGrid();
}

function limparCamposTributoCodigoReceita() {
    _tributoCodigoReceita.Atualizar.visible(false);
    _tributoCodigoReceita.Cancelar.visible(false);
    _tributoCodigoReceita.Excluir.visible(false);
    _tributoCodigoReceita.Adicionar.visible(true);
    LimparCampos(_tributoCodigoReceita);
}