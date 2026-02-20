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
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoObservacaoCTe.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _observacaoContribuinte;
var _pesquisaObservacaoContribuinte;
var _gridObservacaoContribuinte;

var ObservacaoContribuinte = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Ativo = PropertyEntity({ text: "*Ativo: ", val: ko.observable(true), options: _status, def: true});
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoObservacaoCTe.Contribuinte), options: EnumTipoObservacaoCTe.ObterOpcoes(), text: "*Tipo:", def: EnumTipoObservacaoCTe.Contribuinte, visible: ko.observable(true) });
    this.Identificador = PropertyEntity({ text: "*Identificador:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 20, required: true });
    this.Texto = PropertyEntity({ text: "*Texto:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 160, required: true });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });    
}

var PesquisaObservacaoContribuinte = function () {
    this.Texto = PropertyEntity({ text: "Texto:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Ativo = PropertyEntity({ text: "Ativo: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridObservacaoContribuinte.CarregarGrid();
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
function loadObservacaoContribuinte() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaObservacaoContribuinte = new PesquisaObservacaoContribuinte();
    KoBindings(_pesquisaObservacaoContribuinte, "knockoutPesquisaObservacaoContribuinte", false, _pesquisaObservacaoContribuinte.Pesquisar.id);

    // Instancia objeto principal
    _observacaoContribuinte = new ObservacaoContribuinte();
    KoBindings(_observacaoContribuinte, "knockoutObservacaoContribuinte");

    HeaderAuditoria("ObservacaoContribuinte", _observacaoContribuinte);

    // Inicia busca
    buscarObservacaoContribuinte();
}

function adicionarClick(e, sender) {
    Salvar(_observacaoContribuinte, "ObservacaoContribuinte/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridObservacaoContribuinte.CarregarGrid();
                limparCamposObservacaoContribuinte();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_observacaoContribuinte, "ObservacaoContribuinte/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridObservacaoContribuinte.CarregarGrid();
                limparCamposObservacaoContribuinte();
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
        ExcluirPorCodigo(_observacaoContribuinte, "ObservacaoContribuinte/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridObservacaoContribuinte.CarregarGrid();
                    limparCamposObservacaoContribuinte();
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
    limparCamposObservacaoContribuinte();
}

function editarObservacaoContribuinteClick(itemGrid) {
    // Limpa os campos
    limparCamposObservacaoContribuinte();

    // Seta o codigo do objeto
    _observacaoContribuinte.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_observacaoContribuinte, "ObservacaoContribuinte/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaObservacaoContribuinte.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _observacaoContribuinte.Atualizar.visible(true);
                _observacaoContribuinte.Excluir.visible(true);
                _observacaoContribuinte.Cancelar.visible(true);
                _observacaoContribuinte.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarObservacaoContribuinte() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarObservacaoContribuinteClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridObservacaoContribuinte = new GridView(_pesquisaObservacaoContribuinte.Pesquisar.idGrid, "ObservacaoContribuinte/Pesquisa", _pesquisaObservacaoContribuinte, menuOpcoes, null);
    _gridObservacaoContribuinte.CarregarGrid();
}

function limparCamposObservacaoContribuinte() {
    _observacaoContribuinte.Atualizar.visible(false);
    _observacaoContribuinte.Cancelar.visible(false);
    _observacaoContribuinte.Excluir.visible(false);
    _observacaoContribuinte.Adicionar.visible(true);
    LimparCampos(_observacaoContribuinte);
}