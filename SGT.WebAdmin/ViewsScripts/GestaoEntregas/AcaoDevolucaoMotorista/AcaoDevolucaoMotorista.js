/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDAcaoDevolucaoMotorista;
var _gridAcaoDevolucaoMotorista;
var _acaoDevolucaoMotorista;
var _pesquisaAcaoDevolucaoMotorista;

/*
 * Declaração das Classes
 */

var CRUDAcaoDevolucaoMotorista = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var AcaoDevolucaoMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var PesquisaAcaoDevolucaoMotorista = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridAcaoDevolucaoMotorista, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridAcaoDevolucaoMotorista() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "AcaoDevolucaoMotorista/ExportarPesquisa", titulo: "Ação de Devolução para Motorista" };

    _gridAcaoDevolucaoMotorista = new GridViewExportacao(_pesquisaAcaoDevolucaoMotorista.Pesquisar.idGrid, "AcaoDevolucaoMotorista/Pesquisa", _pesquisaAcaoDevolucaoMotorista, menuOpcoes, configuracoesExportacao);
    _gridAcaoDevolucaoMotorista.CarregarGrid();
}

function loadAcaoDevolucaoMotorista() {
    _acaoDevolucaoMotorista = new AcaoDevolucaoMotorista();
    KoBindings(_acaoDevolucaoMotorista, "knockoutAcaoDevolucaoMotorista");

    HeaderAuditoria("AcaoDevolucaoMotorista", _acaoDevolucaoMotorista);

    _CRUDAcaoDevolucaoMotorista = new CRUDAcaoDevolucaoMotorista();
    KoBindings(_CRUDAcaoDevolucaoMotorista, "knockoutCRUDAcaoDevolucaoMotorista");

    _pesquisaAcaoDevolucaoMotorista = new PesquisaAcaoDevolucaoMotorista();
    KoBindings(_pesquisaAcaoDevolucaoMotorista, "knockoutPesquisaAcaoDevolucaoMotorista", false, _pesquisaAcaoDevolucaoMotorista.Pesquisar.id);

    loadGridAcaoDevolucaoMotorista();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_acaoDevolucaoMotorista, "AcaoDevolucaoMotorista/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridAcaoDevolucaoMotorista();
                limparCamposAcaoDevolucaoMotorista();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_acaoDevolucaoMotorista, "AcaoDevolucaoMotorista/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridAcaoDevolucaoMotorista();
                limparCamposAcaoDevolucaoMotorista();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposAcaoDevolucaoMotorista();
}

function editarClick(registroSelecionado) {
    limparCamposAcaoDevolucaoMotorista();

    _acaoDevolucaoMotorista.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_acaoDevolucaoMotorista, "AcaoDevolucaoMotorista/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaAcaoDevolucaoMotorista.ExibirFiltros.visibleFade(false);

                controlarBotoesHabilitados();
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
        ExcluirPorCodigo(_acaoDevolucaoMotorista, "AcaoDevolucaoMotorista/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridAcaoDevolucaoMotorista();
                    limparCamposAcaoDevolucaoMotorista();
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
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _acaoDevolucaoMotorista.Codigo.val() > 0;

    _CRUDAcaoDevolucaoMotorista.Atualizar.visible(isEdicao);
    _CRUDAcaoDevolucaoMotorista.Excluir.visible(isEdicao);
    _CRUDAcaoDevolucaoMotorista.Adicionar.visible(!isEdicao);
}

function limparCamposAcaoDevolucaoMotorista() {
    LimparCampos(_acaoDevolucaoMotorista);
    controlarBotoesHabilitados();
}

function recarregarGridAcaoDevolucaoMotorista() {
    _gridAcaoDevolucaoMotorista.CarregarGrid();
}