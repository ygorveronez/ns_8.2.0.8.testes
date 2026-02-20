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

var _macro;
var _pesquisaMacro;
var _gridMacro;

var Macro = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "*Código Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", issue: 556, val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaMacro = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", issue: 557, val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMacro.CarregarGrid();
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
function loadMacro() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaMacro = new PesquisaMacro();
    KoBindings(_pesquisaMacro, "knockoutPesquisaMacro", false, _pesquisaMacro.Pesquisar.id);

    // Instancia objeto principal
    _macro = new Macro();
    KoBindings(_macro, "knockoutMacro");

    HeaderAuditoria("Macro", _macro);

    // Instancia buscas

    // Inicia busca
    BuscarMacro();
}

function adicionarClick(e, sender) {
    Salvar(_macro, "Macro/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMacro.CarregarGrid();
                LimparCamposMacro();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_macro, "Macro/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMacro.CarregarGrid();
                LimparCamposMacro();
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
        ExcluirPorCodigo(_macro, "Macro/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMacro.CarregarGrid();
                    LimparCamposMacro();
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
    LimparCamposMacro();
}

function editarMacroClick(itemGrid) {
    // Limpa os campos
    LimparCamposMacro();

    // Seta o codigo do objeto
    _macro.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_macro, "Macro/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaMacro.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _macro.Atualizar.visible(true);
                _macro.Excluir.visible(true);
                _macro.Cancelar.visible(true);
                _macro.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function BuscarMacro() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMacroClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridMacro = new GridView(_pesquisaMacro.Pesquisar.idGrid, "Macro/Pesquisa", _pesquisaMacro, menuOpcoes, null);
    _gridMacro.CarregarGrid();
}

function LimparCamposMacro() {
    _macro.Atualizar.visible(false);
    _macro.Cancelar.visible(false);
    _macro.Excluir.visible(false);
    _macro.Adicionar.visible(true);
    LimparCampos(_macro);
}