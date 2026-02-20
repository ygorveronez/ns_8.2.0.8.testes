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

var _motivoCancelamentoPagamento;
var _pesquisaMotivoCancelamentoPagamento;
var _gridMotivoCancelamentoPagamento;

var MotivoCancelamentoPagamento = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaMotivoCancelamentoPagamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoCancelamentoPagamento.CarregarGrid();
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
function loadMotivoCancelamentoPagamento() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaMotivoCancelamentoPagamento = new PesquisaMotivoCancelamentoPagamento();
    KoBindings(_pesquisaMotivoCancelamentoPagamento, "knockoutPesquisaMotivoCancelamentoPagamento", false, _pesquisaMotivoCancelamentoPagamento.Pesquisar.id);

    // Instancia ProdutoAvaria
    _motivoCancelamentoPagamento = new MotivoCancelamentoPagamento();
    KoBindings(_motivoCancelamentoPagamento, "knockoutMotivoCancelamentoPagamento");

    HeaderAuditoria("MotivoCancelamentoPagamento", _motivoCancelamentoPagamento);

    // Inicia busca
    buscarMotivoCancelamentoPagamento();
}

function adicionarClick(e, sender) {
    Salvar(_motivoCancelamentoPagamento, "MotivoCancelamentoPagamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoCancelamentoPagamento.CarregarGrid();
                limparCamposMotivoCancelamentoPagamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoCancelamentoPagamento, "MotivoCancelamentoPagamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoCancelamentoPagamento.CarregarGrid();
                limparCamposMotivoCancelamentoPagamento();
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
        ExcluirPorCodigo(_motivoCancelamentoPagamento, "MotivoCancelamentoPagamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoCancelamentoPagamento.CarregarGrid();
                    limparCamposMotivoCancelamentoPagamento();
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
    limparCamposMotivoCancelamentoPagamento();
}

function editarMotivoCancelamentoPagamentoClick(itemGrid) {
    // Limpa os campos
    limparCamposMotivoCancelamentoPagamento();

    // Seta o codigo do ProdutoAvaria
    _motivoCancelamentoPagamento.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_motivoCancelamentoPagamento, "MotivoCancelamentoPagamento/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaMotivoCancelamentoPagamento.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _motivoCancelamentoPagamento.Atualizar.visible(true);
                _motivoCancelamentoPagamento.Excluir.visible(true);
                _motivoCancelamentoPagamento.Cancelar.visible(true);
                _motivoCancelamentoPagamento.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarMotivoCancelamentoPagamento() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoCancelamentoPagamentoClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "MotivoCancelamentoPagamento/ExportarPesquisa",
        titulo: "Motivo Avaria"
    };


    // Inicia Grid de busca
    _gridMotivoCancelamentoPagamento = new GridViewExportacao(_pesquisaMotivoCancelamentoPagamento.Pesquisar.idGrid, "MotivoCancelamentoPagamento/Pesquisa", _pesquisaMotivoCancelamentoPagamento, menuOpcoes, configExportacao);
    _gridMotivoCancelamentoPagamento.CarregarGrid();
}

function limparCamposMotivoCancelamentoPagamento() {
    _motivoCancelamentoPagamento.Atualizar.visible(false);
    _motivoCancelamentoPagamento.Cancelar.visible(false);
    _motivoCancelamentoPagamento.Excluir.visible(false);
    _motivoCancelamentoPagamento.Adicionar.visible(true);
    LimparCampos(_motivoCancelamentoPagamento);
}