/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivoPunicaoVeiculo;
var _motivoPunicaoVeiculo;
var _pesquisaMotivoPunicaoVeiculo;
var _gridMotivoPunicaoVeiculo;

/*
 * Declaração das Classes
 */

var CRUDMotivoPunicaoVeiculo = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var MotivoPunicaoVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var PesquisaMotivoPunicaoVeiculo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMotivoPunicaoVeiculo, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMotivoPunicaoVeiculo() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoPunicaoVeiculo/ExportarPesquisa", titulo: "Motivo Punição de Veículo" };

    _gridMotivoPunicaoVeiculo = new GridViewExportacao(_pesquisaMotivoPunicaoVeiculo.Pesquisar.idGrid, "MotivoPunicaoVeiculo/Pesquisa", _pesquisaMotivoPunicaoVeiculo, menuOpcoes, configuracoesExportacao);
    _gridMotivoPunicaoVeiculo.CarregarGrid();
}

function loadMotivoPunicaoVeiculo() {
    _motivoPunicaoVeiculo = new MotivoPunicaoVeiculo();
    KoBindings(_motivoPunicaoVeiculo, "knockoutMotivoPunicaoVeiculo");

    HeaderAuditoria("MotivoPunicaoVeiculo", _motivoPunicaoVeiculo);

    _CRUDMotivoPunicaoVeiculo = new CRUDMotivoPunicaoVeiculo();
    KoBindings(_CRUDMotivoPunicaoVeiculo, "knockoutCRUDMotivoPunicaoVeiculo");

    _pesquisaMotivoPunicaoVeiculo = new PesquisaMotivoPunicaoVeiculo();
    KoBindings(_pesquisaMotivoPunicaoVeiculo, "knockoutPesquisaMotivoPunicaoVeiculo", false, _pesquisaMotivoPunicaoVeiculo.Pesquisar.id);

    loadGridMotivoPunicaoVeiculo();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_motivoPunicaoVeiculo, "MotivoPunicaoVeiculo/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridMotivoPunicaoVeiculo();
                limparCamposMotivoPunicaoVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoPunicaoVeiculo, "MotivoPunicaoVeiculo/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridMotivoPunicaoVeiculo();
                limparCamposMotivoPunicaoVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMotivoPunicaoVeiculo();
}

function editarClick(registroSelecionado) {
    limparCamposMotivoPunicaoVeiculo();

    _motivoPunicaoVeiculo.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoPunicaoVeiculo, "MotivoPunicaoVeiculo/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMotivoPunicaoVeiculo.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_motivoPunicaoVeiculo, "MotivoPunicaoVeiculo/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMotivoPunicaoVeiculo();
                    limparCamposMotivoPunicaoVeiculo();
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
    _CRUDMotivoPunicaoVeiculo.Atualizar.visible(isEdicao);
    _CRUDMotivoPunicaoVeiculo.Excluir.visible(isEdicao);
    _CRUDMotivoPunicaoVeiculo.Cancelar.visible(isEdicao);
    _CRUDMotivoPunicaoVeiculo.Adicionar.visible(!isEdicao);
}

function limparCamposMotivoPunicaoVeiculo() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_motivoPunicaoVeiculo);
}

function recarregarGridMotivoPunicaoVeiculo() {
    _gridMotivoPunicaoVeiculo.CarregarGrid();
}