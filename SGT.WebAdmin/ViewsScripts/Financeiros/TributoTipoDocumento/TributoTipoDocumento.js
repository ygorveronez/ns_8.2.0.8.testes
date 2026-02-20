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

var _tributoTipoDocumento;
var _pesquisaTributoTipoDocumento;
var _gridTributoTipoDocumento;

var TributoTipoDocumento = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "*Cod. Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });    
    this.TributoTipoImposto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Imposto:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaTributoTipoDocumento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Cód. Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTributoTipoDocumento.CarregarGrid();
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
function loadTributoTipoDocumento() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaTributoTipoDocumento = new PesquisaTributoTipoDocumento();
    KoBindings(_pesquisaTributoTipoDocumento, "knockoutPesquisaTributoTipoDocumento", false, _pesquisaTributoTipoDocumento.Pesquisar.id);

    // Instancia objeto principal
    _tributoTipoDocumento = new TributoTipoDocumento();
    KoBindings(_tributoTipoDocumento, "knockoutTributoTipoDocumento");

    new BuscarTributoTipoImposto(_tributoTipoDocumento.TributoTipoImposto);

    HeaderAuditoria("TributoTipoDocumento", _tributoTipoDocumento);

    // Inicia busca
    buscarTributoTipoDocumento();
}

function adicionarClick(e, sender) {
    Salvar(_tributoTipoDocumento, "TributoTipoDocumento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTributoTipoDocumento.CarregarGrid();
                limparCamposTributoTipoDocumento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tributoTipoDocumento, "TributoTipoDocumento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTributoTipoDocumento.CarregarGrid();
                limparCamposTributoTipoDocumento();
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
        ExcluirPorCodigo(_tributoTipoDocumento, "TributoTipoDocumento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTributoTipoDocumento.CarregarGrid();
                    limparCamposTributoTipoDocumento();
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
    limparCamposTributoTipoDocumento();
}

function editarTributoTipoDocumentoClick(itemGrid) {
    // Limpa os campos
    limparCamposTributoTipoDocumento();

    // Seta o codigo do objeto
    _tributoTipoDocumento.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_tributoTipoDocumento, "TributoTipoDocumento/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaTributoTipoDocumento.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _tributoTipoDocumento.Atualizar.visible(true);
                _tributoTipoDocumento.Excluir.visible(true);
                _tributoTipoDocumento.Cancelar.visible(true);
                _tributoTipoDocumento.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarTributoTipoDocumento() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTributoTipoDocumentoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridTributoTipoDocumento = new GridView(_pesquisaTributoTipoDocumento.Pesquisar.idGrid, "TributoTipoDocumento/Pesquisa", _pesquisaTributoTipoDocumento, menuOpcoes, null);
    _gridTributoTipoDocumento.CarregarGrid();
}

function limparCamposTributoTipoDocumento() {
    _tributoTipoDocumento.Atualizar.visible(false);
    _tributoTipoDocumento.Cancelar.visible(false);
    _tributoTipoDocumento.Excluir.visible(false);
    _tributoTipoDocumento.Adicionar.visible(true);
    LimparCampos(_tributoTipoDocumento);
}