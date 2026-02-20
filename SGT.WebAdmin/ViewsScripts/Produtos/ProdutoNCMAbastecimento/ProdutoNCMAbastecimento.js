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
/// <reference path="../../Enumeradores/EnumTipoAbastecimento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _produtoNCMAbastecimento;
var _pesquisaProdutoNCMAbastecimento;
var _gridProdutoNCMAbastecimento;

var _tipoAbastecimento = [{ text: "Combustível", value: EnumTipoAbastecimento.Combustivel }, { text: "ARLA", value: EnumTipoAbastecimento.Arla }];
var _pesquisaTipoAbastecimento = [{ text: "Todos", value: EnumTipoAbastecimento.Todos }, { text: "Combustível", value: EnumTipoAbastecimento.Combustivel }, { text: "ARLA", value: EnumTipoAbastecimento.Arla }];

var ProdutoNCMAbastecimento = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NCM = PropertyEntity({ text: "*NCM:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 8 });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.TipoAbastecimento = PropertyEntity({ text: "Tipo Abastecimento: ", val: ko.observable(EnumTipoAbastecimento.Combustivel), options: _tipoAbastecimento, def: EnumTipoAbastecimento.Combustivel });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaProdutoNCMAbastecimento = function () {
    this.NCM = PropertyEntity({ text: "NCM:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 8 });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.TipoAbastecimento = PropertyEntity({ text: "Tipo Abastecimento: ", val: ko.observable(EnumTipoAbastecimento.Todos), options: _pesquisaTipoAbastecimento, def: EnumTipoAbastecimento.Todos });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProdutoNCMAbastecimento.CarregarGrid();
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
function loadProdutoNCMAbastecimento() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaProdutoNCMAbastecimento = new PesquisaProdutoNCMAbastecimento();
    KoBindings(_pesquisaProdutoNCMAbastecimento, "knockoutPesquisaProdutoNCMAbastecimento", false, _pesquisaProdutoNCMAbastecimento.Pesquisar.id);

    // Instancia objeto principal
    _produtoNCMAbastecimento = new ProdutoNCMAbastecimento();
    KoBindings(_produtoNCMAbastecimento, "knockoutProdutoNCMAbastecimento");

    HeaderAuditoria("ProdutoNCMAbastecimento", _produtoNCMAbastecimento);

    // Inicia busca
    buscarProdutoNCMAbastecimento();
}

function adicionarClick(e, sender) {
    Salvar(_produtoNCMAbastecimento, "ProdutoNCMAbastecimento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridProdutoNCMAbastecimento.CarregarGrid();
                limparCamposProdutoNCMAbastecimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_produtoNCMAbastecimento, "ProdutoNCMAbastecimento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProdutoNCMAbastecimento.CarregarGrid();
                limparCamposProdutoNCMAbastecimento();
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
        ExcluirPorCodigo(_produtoNCMAbastecimento, "ProdutoNCMAbastecimento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridProdutoNCMAbastecimento.CarregarGrid();
                    limparCamposProdutoNCMAbastecimento();
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
    limparCamposProdutoNCMAbastecimento();
}

function editarProdutoNCMAbastecimentoClick(itemGrid) {
    // Limpa os campos
    limparCamposProdutoNCMAbastecimento();

    // Seta o codigo do objeto
    _produtoNCMAbastecimento.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_produtoNCMAbastecimento, "ProdutoNCMAbastecimento/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaProdutoNCMAbastecimento.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _produtoNCMAbastecimento.Atualizar.visible(true);
                _produtoNCMAbastecimento.Excluir.visible(true);
                _produtoNCMAbastecimento.Cancelar.visible(true);
                _produtoNCMAbastecimento.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarProdutoNCMAbastecimento() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProdutoNCMAbastecimentoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridProdutoNCMAbastecimento = new GridView(_pesquisaProdutoNCMAbastecimento.Pesquisar.idGrid, "ProdutoNCMAbastecimento/Pesquisa", _pesquisaProdutoNCMAbastecimento, menuOpcoes, null);
    _gridProdutoNCMAbastecimento.CarregarGrid();
}

function limparCamposProdutoNCMAbastecimento() {
    _produtoNCMAbastecimento.Atualizar.visible(false);
    _produtoNCMAbastecimento.Cancelar.visible(false);
    _produtoNCMAbastecimento.Excluir.visible(false);
    _produtoNCMAbastecimento.Adicionar.visible(true);
    LimparCampos(_produtoNCMAbastecimento);
}