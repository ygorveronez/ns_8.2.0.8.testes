/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _extratoBancarioTipoLancamento;
var _pesquisaExtratoBancarioTipoLancamento;
var _gridExtratoBancarioTipoLancamento;

var ExtratoBancarioTipoLancamento = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "*Cód. Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.NaoImportarRegistroAoEstrato = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Não importar este registro ao Extrato Bancário ao importar arquivo bancário?", def: false, visible: ko.observable(true) });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaExtratoBancarioTipoLancamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Cód. Integração:", getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridExtratoBancarioTipoLancamento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadExtratoBancarioTipoLancamento() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaExtratoBancarioTipoLancamento = new PesquisaExtratoBancarioTipoLancamento();
    KoBindings(_pesquisaExtratoBancarioTipoLancamento, "knockoutPesquisaExtratoBancarioTipoLancamento", false, _pesquisaExtratoBancarioTipoLancamento.Pesquisar.id);

    // Instancia objeto principal
    _extratoBancarioTipoLancamento = new ExtratoBancarioTipoLancamento();
    KoBindings(_extratoBancarioTipoLancamento, "knockoutExtratoBancarioTipoLancamento");

    HeaderAuditoria("ExtratoBancarioTipoLancamento", _extratoBancarioTipoLancamento);

    // Inicia busca
    buscarExtratoBancarioTipoLancamento();
}

function adicionarClick(e, sender) {
    Salvar(_extratoBancarioTipoLancamento, "ExtratoBancarioTipoLancamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridExtratoBancarioTipoLancamento.CarregarGrid();
                limparCamposExtratoBancarioTipoLancamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_extratoBancarioTipoLancamento, "ExtratoBancarioTipoLancamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridExtratoBancarioTipoLancamento.CarregarGrid();
                limparCamposExtratoBancarioTipoLancamento();
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
        ExcluirPorCodigo(_extratoBancarioTipoLancamento, "ExtratoBancarioTipoLancamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridExtratoBancarioTipoLancamento.CarregarGrid();
                    limparCamposExtratoBancarioTipoLancamento();
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
    limparCamposExtratoBancarioTipoLancamento();
}

function editarExtratoBancarioTipoLancamentoClick(itemGrid) {
    // Limpa os campos
    limparCamposExtratoBancarioTipoLancamento();

    // Seta o codigo do objeto
    _extratoBancarioTipoLancamento.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_extratoBancarioTipoLancamento, "ExtratoBancarioTipoLancamento/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaExtratoBancarioTipoLancamento.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _extratoBancarioTipoLancamento.Atualizar.visible(true);
                _extratoBancarioTipoLancamento.Excluir.visible(true);
                _extratoBancarioTipoLancamento.Cancelar.visible(true);
                _extratoBancarioTipoLancamento.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function buscarExtratoBancarioTipoLancamento() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarExtratoBancarioTipoLancamentoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridExtratoBancarioTipoLancamento = new GridView(_pesquisaExtratoBancarioTipoLancamento.Pesquisar.idGrid, "ExtratoBancarioTipoLancamento/Pesquisa", _pesquisaExtratoBancarioTipoLancamento, menuOpcoes, null);
    _gridExtratoBancarioTipoLancamento.CarregarGrid();
}

function limparCamposExtratoBancarioTipoLancamento() {
    _extratoBancarioTipoLancamento.Atualizar.visible(false);
    _extratoBancarioTipoLancamento.Cancelar.visible(false);
    _extratoBancarioTipoLancamento.Excluir.visible(false);
    _extratoBancarioTipoLancamento.Adicionar.visible(true);
    LimparCampos(_extratoBancarioTipoLancamento);
}