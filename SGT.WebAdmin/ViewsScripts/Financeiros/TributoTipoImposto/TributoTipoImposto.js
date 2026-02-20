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

var _tributoTipoImposto;
var _pesquisaTributoTipoImposto;
var _gridTributoTipoImposto;

var TributoTipoImposto = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "*Cod. Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });    
    this.TributoCodigoReceita = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Cód. da receita:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });    

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaTributoTipoImposto = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Cód. Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTributoTipoImposto.CarregarGrid();
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
function loadTributoTipoImposto() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaTributoTipoImposto = new PesquisaTributoTipoImposto();
    KoBindings(_pesquisaTributoTipoImposto, "knockoutPesquisaTributoTipoImposto", false, _pesquisaTributoTipoImposto.Pesquisar.id);

    // Instancia objeto principal
    _tributoTipoImposto = new TributoTipoImposto();
    KoBindings(_tributoTipoImposto, "knockoutTributoTipoImposto");
        
    new BuscarTributoCodigoReceita(_tributoTipoImposto.TributoCodigoReceita);

    HeaderAuditoria("TributoTipoImposto", _tributoTipoImposto);

    // Inicia busca
    buscarTributoTipoImposto();
}

function adicionarClick(e, sender) {
    Salvar(_tributoTipoImposto, "TributoTipoImposto/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTributoTipoImposto.CarregarGrid();
                limparCamposTributoTipoImposto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tributoTipoImposto, "TributoTipoImposto/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTributoTipoImposto.CarregarGrid();
                limparCamposTributoTipoImposto();
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
        ExcluirPorCodigo(_tributoTipoImposto, "TributoTipoImposto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTributoTipoImposto.CarregarGrid();
                    limparCamposTributoTipoImposto();
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
    limparCamposTributoTipoImposto();
}

function editarTributoTipoImpostoClick(itemGrid) {
    // Limpa os campos
    limparCamposTributoTipoImposto();

    // Seta o codigo do objeto
    _tributoTipoImposto.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_tributoTipoImposto, "TributoTipoImposto/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaTributoTipoImposto.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _tributoTipoImposto.Atualizar.visible(true);
                _tributoTipoImposto.Excluir.visible(true);
                _tributoTipoImposto.Cancelar.visible(true);
                _tributoTipoImposto.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarTributoTipoImposto() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTributoTipoImpostoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridTributoTipoImposto = new GridView(_pesquisaTributoTipoImposto.Pesquisar.idGrid, "TributoTipoImposto/Pesquisa", _pesquisaTributoTipoImposto, menuOpcoes, null);
    _gridTributoTipoImposto.CarregarGrid();
}

function limparCamposTributoTipoImposto() {
    _tributoTipoImposto.Atualizar.visible(false);
    _tributoTipoImposto.Cancelar.visible(false);
    _tributoTipoImposto.Excluir.visible(false);
    _tributoTipoImposto.Adicionar.visible(true);
    LimparCampos(_tributoTipoImposto);
}