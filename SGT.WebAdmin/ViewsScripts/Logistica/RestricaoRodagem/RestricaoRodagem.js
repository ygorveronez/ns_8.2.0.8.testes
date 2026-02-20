/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="RestricaoRodagemClienteDestino.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRestricaoRodagem;
var _restricaoRodagem;
var _pesquisaRestricaoRodagem;
var _gridRestricaoRodagem;
var _finalPlaca = [
    { text: "0", value: 0 },
    { text: "1", value: 1 },
    { text: "2", value: 2 },
    { text: "3", value: 3 },
    { text: "4", value: 4 },
    { text: "5", value: 5 },
    { text: "6", value: 6 },
    { text: "7", value: 7 },
    { text: "8", value: 8 },
    { text: "9", value: 9 }
];

/*
 * Declaração das Classes
 */

var CRUDRestricaoRodagem = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var RestricaoRodagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Carregamento:", idBtnSearch: guid(), required: true });
    this.DiaSemana = PropertyEntity({ type: types.map, val: ko.observable(EnumDiaSemana.Segunda), options: ko.observable(EnumDiaSemana.obterOpcoes()), text: "*Dia da Semana:", def: EnumDiaSemana.Segunda, required: true });
    this.FinalPlaca = PropertyEntity({ type: types.map, val: ko.observable(0), options: ko.observable(_finalPlaca), text: "*Final da Placa:", def: 0, required: true });
    this.HoraFinal = PropertyEntity({ text: "*Fim da Restrição: ", required: true, getType: typesKnockout.time });
    this.HoraInicial = PropertyEntity({ text: "*Início da Restrição: ", required: true, getType: typesKnockout.time });
}

var PesquisaRestricaoRodagem = function () {
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", idBtnSearch: guid() });
    this.DiaSemana = PropertyEntity({ type: types.map, val: ko.observable(EnumDiaSemana.Todos), options: ko.observable(EnumDiaSemana.obterOpcoesPesquisa()), text: "Dia da Semana:", def: EnumDiaSemana.Todos });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridRestricaoRodagem, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridRestricaoRodagem() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "RestricaoRodagem/ExportarPesquisa", titulo: "Restrição de Rodagem" };

    _gridRestricaoRodagem = new GridViewExportacao(_pesquisaRestricaoRodagem.Pesquisar.idGrid, "RestricaoRodagem/Pesquisa", _pesquisaRestricaoRodagem, menuOpcoes, configuracoesExportacao);
    _gridRestricaoRodagem.CarregarGrid();
}

function loadRestricaoRodagem() {
    _restricaoRodagem = new RestricaoRodagem();
    KoBindings(_restricaoRodagem, "knockoutRestricaoRodagem");

    HeaderAuditoria("RestricaoRodagem", _restricaoRodagem);

    _CRUDRestricaoRodagem = new CRUDRestricaoRodagem();
    KoBindings(_CRUDRestricaoRodagem, "knockoutCRUDRestricaoRodagem");

    _pesquisaRestricaoRodagem = new PesquisaRestricaoRodagem();
    KoBindings(_pesquisaRestricaoRodagem, "knockoutPesquisaRestricaoRodagem", false, _pesquisaRestricaoRodagem.Pesquisar.id);

    new BuscarCentrosCarregamento(_restricaoRodagem.CentroCarregamento);
    new BuscarCentrosCarregamento(_pesquisaRestricaoRodagem.CentroCarregamento);

    loadRestricaoRodagemClienteDestino();
    loadGridRestricaoRodagem();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    if (ValidarCamposObrigatorios(_restricaoRodagem)) {
        executarReST("RestricaoRodagem/Adicionar", obterRestricaoRodagemSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                    recarregarGridRestricaoRodagem();
                    limparCamposRestricaoRodagem();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function atualizarClick() {
    if (ValidarCamposObrigatorios(_restricaoRodagem)) {
        executarReST("RestricaoRodagem/Atualizar", obterRestricaoRodagemSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");

                    recarregarGridRestricaoRodagem();
                    limparCamposRestricaoRodagem();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function cancelarClick() {
    limparCamposRestricaoRodagem();
}

function editarClick(registroSelecionado) {
    limparCamposRestricaoRodagem();

    executarReST("RestricaoRodagem/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaRestricaoRodagem.ExibirFiltros.visibleFade(false);

                PreencherObjetoKnout(_restricaoRodagem, { Data: retorno.Data.Dados });
                preencherRestricaoRodagemClienteDestino(retorno.Data.ClientesDestino);
                controlarBotoesHabilitados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_restricaoRodagem, "RestricaoRodagem/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridRestricaoRodagem();
                    limparCamposRestricaoRodagem();
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
    var isEdicao = _restricaoRodagem.Codigo.val();

    _CRUDRestricaoRodagem.Atualizar.visible(isEdicao);
    _CRUDRestricaoRodagem.Excluir.visible(isEdicao);
    _CRUDRestricaoRodagem.Cancelar.visible(isEdicao);
    _CRUDRestricaoRodagem.Adicionar.visible(!isEdicao);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function limparCamposRestricaoRodagem() {
    LimparCampos(_restricaoRodagem);
    limparCamposRestricaoRodagemClienteDestino();
    controlarBotoesHabilitados();

    $("#tabRestricaoRodagemDados").click();
}

function obterRestricaoRodagemSalvar() {
    var restricaoRodagem = RetornarObjetoPesquisa(_restricaoRodagem);

    restricaoRodagem["ClientesDestino"] = obterRestricaoRodagemClienteDestinoSalvar();

    return restricaoRodagem;
}

function recarregarGridRestricaoRodagem() {
    _gridRestricaoRodagem.CarregarGrid();
}