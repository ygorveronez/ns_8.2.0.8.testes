/// <reference path="../../Consultas/Veiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDPunicaoVeiculo;
var _punicaoVeiculo;
var _pesquisaPunicaoVeiculo;
var _gridPunicaoVeiculo;

var _diasPunicaoOpcoes = [
    { text: "1", value: 1 },
    { text: "2", value: 2 },
    { text: "3", value: 3 },
    { text: "4", value: 4 },
    { text: "5", value: 5 },
    { text: "6", value: 6 },
    { text: "7", value: 7 }
];

/*
 * Declaração das Classes
 */

var CRUDPunicaoVeiculo = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Nova", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PunicaoVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ativo = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), options: _status, def: true });
    this.DiasPunicao = PropertyEntity({ text: "*Dias de Punição: ", val: ko.observable(true), options: _diasPunicaoOpcoes, def: true });
    this.DataInicioPunicao = PropertyEntity({ text: "*Data Início Punição: ", required: true, getType: typesKnockout.dateTime });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Motivo:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Veículo:", idBtnSearch: guid(), enable: ko.observable(true) });
}

var PesquisaPunicaoVeiculo = function () {
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Veículo:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Motivo:", idBtnSearch: guid(), enable: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPunicaoVeiculo, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridPunicaoVeiculo() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "PunicaoVeiculo/ExportarPesquisa", titulo: "Punição de Veículo" };

    _gridPunicaoVeiculo = new GridViewExportacao(_pesquisaPunicaoVeiculo.Pesquisar.idGrid, "PunicaoVeiculo/Pesquisa", _pesquisaPunicaoVeiculo, menuOpcoes, configuracoesExportacao);
    _gridPunicaoVeiculo.CarregarGrid();
}

function loadPunicaoVeiculo() {
    _punicaoVeiculo = new PunicaoVeiculo();
    KoBindings(_punicaoVeiculo, "knockoutPunicaoVeiculo");

    HeaderAuditoria("PunicaoVeiculo", _punicaoVeiculo);

    _CRUDPunicaoVeiculo = new CRUDPunicaoVeiculo();
    KoBindings(_CRUDPunicaoVeiculo, "knockoutCRUDPunicaoVeiculo");

    _pesquisaPunicaoVeiculo = new PesquisaPunicaoVeiculo();
    KoBindings(_pesquisaPunicaoVeiculo, "knockoutPesquisaPunicaoVeiculo", false, _pesquisaPunicaoVeiculo.Pesquisar.id);

    new BuscarVeiculos(_punicaoVeiculo.Veiculo);
    new BuscarVeiculos(_pesquisaPunicaoVeiculo.Veiculo);
    new BuscarMotivoPunicaoVeiculo(_punicaoVeiculo.Motivo);
    new BuscarMotivoPunicaoVeiculo(_pesquisaPunicaoVeiculo.Motivo);

    loadGridPunicaoVeiculo();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_punicaoVeiculo, "PunicaoVeiculo/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridPunicaoVeiculo();
                limparCamposPunicaoVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_punicaoVeiculo, "PunicaoVeiculo/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridPunicaoVeiculo();
                limparCamposPunicaoVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposPunicaoVeiculo();
}

function editarClick(registroSelecionado) {
    limparCamposPunicaoVeiculo();

    _punicaoVeiculo.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_punicaoVeiculo, "PunicaoVeiculo/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaPunicaoVeiculo.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_punicaoVeiculo, "PunicaoVeiculo/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridPunicaoVeiculo();
                    limparCamposPunicaoVeiculo();
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

function controlarBotoesHabilitados(isEdicao) {
    _CRUDPunicaoVeiculo.Atualizar.visible(isEdicao);
    _CRUDPunicaoVeiculo.Excluir.visible(isEdicao);
    _CRUDPunicaoVeiculo.Cancelar.visible(isEdicao);
    _CRUDPunicaoVeiculo.Adicionar.visible(!isEdicao);
}

function limparCamposPunicaoVeiculo() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_punicaoVeiculo);
}

function recarregarGridPunicaoVeiculo() {
    _gridPunicaoVeiculo.CarregarGrid();
}