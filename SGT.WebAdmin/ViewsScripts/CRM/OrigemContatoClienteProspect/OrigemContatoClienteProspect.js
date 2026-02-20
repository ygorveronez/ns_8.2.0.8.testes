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

var _origemContatoClienteProspect;
var _pesquisaOrigemContatoClienteProspect;
var _gridOrigemContatoClienteProspect;

var OrigemContatoClienteProspect = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    
    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaOrigemContatoClienteProspect = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridOrigemContatoClienteProspect.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadOrigemContatoClienteProspect() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaOrigemContatoClienteProspect = new PesquisaOrigemContatoClienteProspect();
    KoBindings(_pesquisaOrigemContatoClienteProspect, "knockoutPesquisaOrigemContatoClienteProspect", false, _pesquisaOrigemContatoClienteProspect.Pesquisar.id);

    // Instancia objeto principal
    _origemContatoClienteProspect = new OrigemContatoClienteProspect();
    KoBindings(_origemContatoClienteProspect, "knockoutOrigemContatoClienteProspect");

    HeaderAuditoria("OrigemContatoClienteProspect", _origemContatoClienteProspect);

    // Inicia busca
    buscarOrigemContatoClienteProspect();
}

function adicionarClick(e, sender) {
    Salvar(_origemContatoClienteProspect, "OrigemContatoClienteProspect/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridOrigemContatoClienteProspect.CarregarGrid();
                limparCamposOrigemContatoClienteProspect();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_origemContatoClienteProspect, "OrigemContatoClienteProspect/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridOrigemContatoClienteProspect.CarregarGrid();
                limparCamposOrigemContatoClienteProspect();
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
        ExcluirPorCodigo(_origemContatoClienteProspect, "OrigemContatoClienteProspect/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridOrigemContatoClienteProspect.CarregarGrid();
                    limparCamposOrigemContatoClienteProspect();
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
    limparCamposOrigemContatoClienteProspect();
}

function editarOrigemContatoClienteProspectClick(itemGrid) {
    // Limpa os campos
    limparCamposOrigemContatoClienteProspect();

    // Seta o codigo do objeto
    _origemContatoClienteProspect.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_origemContatoClienteProspect, "OrigemContatoClienteProspect/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaOrigemContatoClienteProspect.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _origemContatoClienteProspect.Atualizar.visible(true);
                _origemContatoClienteProspect.Excluir.visible(true);
                _origemContatoClienteProspect.Cancelar.visible(true);
                _origemContatoClienteProspect.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarOrigemContatoClienteProspect() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarOrigemContatoClienteProspectClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridOrigemContatoClienteProspect = new GridView(_pesquisaOrigemContatoClienteProspect.Pesquisar.idGrid, "OrigemContatoClienteProspect/Pesquisa", _pesquisaOrigemContatoClienteProspect, menuOpcoes, null);
    _gridOrigemContatoClienteProspect.CarregarGrid();
}

function limparCamposOrigemContatoClienteProspect() {
    _origemContatoClienteProspect.Atualizar.visible(false);
    _origemContatoClienteProspect.Cancelar.visible(false);
    _origemContatoClienteProspect.Excluir.visible(false);
    _origemContatoClienteProspect.Adicionar.visible(true);
    LimparCampos(_origemContatoClienteProspect);
}