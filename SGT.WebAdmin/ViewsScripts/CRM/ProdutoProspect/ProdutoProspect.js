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

var _produtoProspect;
var _pesquisaProdutoProspect;
var _gridProdutoProspect;

var ProdutoProspect = function () {
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

var PesquisaProdutoProspect = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProdutoProspect.CarregarGrid();
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
function loadProdutoProspect() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaProdutoProspect = new PesquisaProdutoProspect();
    KoBindings(_pesquisaProdutoProspect, "knockoutPesquisaProdutoProspect", false, _pesquisaProdutoProspect.Pesquisar.id);

    // Instancia objeto principal
    _produtoProspect = new ProdutoProspect();
    KoBindings(_produtoProspect, "knockoutProdutoProspect");

    HeaderAuditoria("ProdutoProspect", _produtoProspect);

    // Inicia busca
    buscarProdutoProspect();
}

function adicionarClick(e, sender) {
    Salvar(_produtoProspect, "ProdutoProspect/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridProdutoProspect.CarregarGrid();
                limparCamposProdutoProspect();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_produtoProspect, "ProdutoProspect/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProdutoProspect.CarregarGrid();
                limparCamposProdutoProspect();
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
        ExcluirPorCodigo(_produtoProspect, "ProdutoProspect/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridProdutoProspect.CarregarGrid();
                    limparCamposProdutoProspect();
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
    limparCamposProdutoProspect();
}

function editarProdutoProspectClick(itemGrid) {
    // Limpa os campos
    limparCamposProdutoProspect();

    // Seta o codigo do objeto
    _produtoProspect.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_produtoProspect, "ProdutoProspect/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaProdutoProspect.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _produtoProspect.Atualizar.visible(true);
                _produtoProspect.Excluir.visible(true);
                _produtoProspect.Cancelar.visible(true);
                _produtoProspect.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarProdutoProspect() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProdutoProspectClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridProdutoProspect = new GridView(_pesquisaProdutoProspect.Pesquisar.idGrid, "ProdutoProspect/Pesquisa", _pesquisaProdutoProspect, menuOpcoes, null);
    _gridProdutoProspect.CarregarGrid();
}

function limparCamposProdutoProspect() {
    _produtoProspect.Atualizar.visible(false);
    _produtoProspect.Cancelar.visible(false);
    _produtoProspect.Excluir.visible(false);
    _produtoProspect.Adicionar.visible(true);
    LimparCampos(_produtoProspect);
}