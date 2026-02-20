/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivoCancelamentoRetornoCargaColetaBackhaul;
var _motivoCancelamentoRetornoCargaColetaBackhaul;
var _pesquisaMotivoCancelamentoRetornoCargaColetaBackhaul;
var _gridMotivoCancelamentoRetornoCargaColetaBackhaul;

/*
 * Declaração das Classes
 */

var CRUDMotivoCancelamentoRetornoCargaColetaBackhaul = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var MotivoCancelamentoRetornoCargaColetaBackhaul = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.GerarCargaColeta = PropertyEntity({ text: "Gerar carga de coleta?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var PesquisaMotivoCancelamentoRetornoCargaColetaBackhaul = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMotivoCancelamentoRetornoCargaColetaBackhaul, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMotivoCancelamentoRetornoCargaColetaBackhaul() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoCancelamentoRetornoCargaColetaBackhaul/ExportarPesquisa", titulo: "Motivo de Cancelamento da Carga de Coleta Backhaul" };

    _gridMotivoCancelamentoRetornoCargaColetaBackhaul = new GridViewExportacao(_pesquisaMotivoCancelamentoRetornoCargaColetaBackhaul.Pesquisar.idGrid, "MotivoCancelamentoRetornoCargaColetaBackhaul/Pesquisa", _pesquisaMotivoCancelamentoRetornoCargaColetaBackhaul, menuOpcoes, configuracoesExportacao);
    _gridMotivoCancelamentoRetornoCargaColetaBackhaul.CarregarGrid();
}

function loadMotivoCancelamentoRetornoCargaColetaBackhaul() {
    _motivoCancelamentoRetornoCargaColetaBackhaul = new MotivoCancelamentoRetornoCargaColetaBackhaul();
    KoBindings(_motivoCancelamentoRetornoCargaColetaBackhaul, "knockoutMotivoCancelamentoRetornoCargaColetaBackhaul");

    HeaderAuditoria("MotivoCancelamentoRetornoCargaColetaBackhaul", _motivoCancelamentoRetornoCargaColetaBackhaul);

    _CRUDMotivoCancelamentoRetornoCargaColetaBackhaul = new CRUDMotivoCancelamentoRetornoCargaColetaBackhaul();
    KoBindings(_CRUDMotivoCancelamentoRetornoCargaColetaBackhaul, "knockoutCRUDMotivoCancelamentoRetornoCargaColetaBackhaul");

    _pesquisaMotivoCancelamentoRetornoCargaColetaBackhaul = new PesquisaMotivoCancelamentoRetornoCargaColetaBackhaul();
    KoBindings(_pesquisaMotivoCancelamentoRetornoCargaColetaBackhaul, "knockoutPesquisaMotivoCancelamentoRetornoCargaColetaBackhaul", false, _pesquisaMotivoCancelamentoRetornoCargaColetaBackhaul.Pesquisar.id);

    loadGridMotivoCancelamentoRetornoCargaColetaBackhaul();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_motivoCancelamentoRetornoCargaColetaBackhaul, "MotivoCancelamentoRetornoCargaColetaBackhaul/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridMotivoCancelamentoRetornoCargaColetaBackhaul();
                limparCamposMotivoCancelamentoRetornoCargaColetaBackhaul();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoCancelamentoRetornoCargaColetaBackhaul, "MotivoCancelamentoRetornoCargaColetaBackhaul/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridMotivoCancelamentoRetornoCargaColetaBackhaul();
                limparCamposMotivoCancelamentoRetornoCargaColetaBackhaul();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMotivoCancelamentoRetornoCargaColetaBackhaul();
}

function editarClick(registroSelecionado) {
    limparCamposMotivoCancelamentoRetornoCargaColetaBackhaul();

    _motivoCancelamentoRetornoCargaColetaBackhaul.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoCancelamentoRetornoCargaColetaBackhaul, "MotivoCancelamentoRetornoCargaColetaBackhaul/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMotivoCancelamentoRetornoCargaColetaBackhaul.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_motivoCancelamentoRetornoCargaColetaBackhaul, "MotivoCancelamentoRetornoCargaColetaBackhaul/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMotivoCancelamentoRetornoCargaColetaBackhaul();
                    limparCamposMotivoCancelamentoRetornoCargaColetaBackhaul();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções Privadas
 */

function controlarBotoesHabilitados(isEdicao) {
    _CRUDMotivoCancelamentoRetornoCargaColetaBackhaul.Atualizar.visible(isEdicao);
    _CRUDMotivoCancelamentoRetornoCargaColetaBackhaul.Excluir.visible(isEdicao);
    _CRUDMotivoCancelamentoRetornoCargaColetaBackhaul.Cancelar.visible(isEdicao);
    _CRUDMotivoCancelamentoRetornoCargaColetaBackhaul.Adicionar.visible(!isEdicao);
}

function limparCamposMotivoCancelamentoRetornoCargaColetaBackhaul() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_motivoCancelamentoRetornoCargaColetaBackhaul);
}

function recarregarGridMotivoCancelamentoRetornoCargaColetaBackhaul() {
    _gridMotivoCancelamentoRetornoCargaColetaBackhaul.CarregarGrid();
}