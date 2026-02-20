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

var _moeda;
var _pesquisaMoeda;
var _gridMoeda;

var Moeda = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao, issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Sigla = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Sigla, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoMoeda = PropertyEntity({ text: Localization.Resources.Moedas.Moeda.CodigoMoeda, issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Moedas.Moeda.Status, issue: 556, val: ko.observable(true), options: _status, def: true });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

var PesquisaMoeda = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });

    this.CodigoMoeda = PropertyEntity({ text: Localization.Resources.Moedas.Moeda.CodigoMoeda.getRequiredFieldDescription(), required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), issue: 557, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMoeda.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.ExibirFiltros, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadMoeda() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaMoeda = new PesquisaMoeda();
    KoBindings(_pesquisaMoeda, "knockoutPesquisaMoeda", false, _pesquisaMoeda.Pesquisar.id);

    // Instancia objeto principal
    _moeda = new Moeda();
    KoBindings(_moeda, "knockoutMoeda");

    HeaderAuditoria("Moeda", _moeda);

    // Instancia buscas

    // Inicia busca
    BuscarMoeda();
}

function adicionarClick(e, sender) {
    Salvar(_moeda, "Moeda/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMoeda.CarregarGrid();
                LimparCamposMoeda();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_moeda, "Moeda/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMoeda.CarregarGrid();
                LimparCamposMoeda();
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
        ExcluirPorCodigo(_moeda, "Moeda/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMoeda.CarregarGrid();
                    LimparCamposMoeda();
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
    LimparCamposMoeda();
}

function editarMoedaClick(itemGrid) {
    // Limpa os campos
    LimparCamposMoeda();

    // Seta o codigo do objeto
    _moeda.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_moeda, "Moeda/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaMoeda.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _moeda.Atualizar.visible(true);
                _moeda.Excluir.visible(true);
                _moeda.Cancelar.visible(true);
                _moeda.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function BuscarMoeda() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMoedaClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridMoeda = new GridView(_pesquisaMoeda.Pesquisar.idGrid, "Moeda/Pesquisa", _pesquisaMoeda, menuOpcoes, null);
    _gridMoeda.CarregarGrid();
}

function LimparCamposMoeda() {
    _moeda.Atualizar.visible(false);
    _moeda.Cancelar.visible(false);
    _moeda.Excluir.visible(false);
    _moeda.Adicionar.visible(true);
    LimparCampos(_moeda);
}