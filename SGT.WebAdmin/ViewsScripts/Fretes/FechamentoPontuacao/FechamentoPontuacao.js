/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoPontuacao.js" />
/// <reference path="FechamentoPontuacaoEtapa.js" />
/// <reference path="FechamentoPontuacaoTransportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDFechamentoPontuacao;
var _fechamentoPontuacao;
var _gridFechamentoPontuacao;
var _pesquisaFechamentoPontuacao;

/*
 * Declaração das Classes
 */

var PesquisaFechamentoPontuacao = function () {
    this.Ano = PropertyEntity({ text: "Ano: ", getType: typesKnockout.int, maxlength: 4, configInt: { precision: 0, allowZero: false, thousands: '' } });
    this.Mes = PropertyEntity({ text: "Mês: ", val: ko.observable(EnumMes.Todos), def: EnumMes.Todos, options: EnumMes.obterOpcoesPesquisa() });
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' }, maxlength: 12 });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoFechamentoPontuacao.Todas), def: EnumSituacaoFechamentoPontuacao.Todas, options: EnumSituacaoFechamentoPontuacao.obterOpcoesPesquisa() });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            recarregarGridFechamentoPontuacao();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var FechamentoPontuacao = function () {
    var anoAtual = (new Date()).getFullYear();
    var mesAtual = (new Date()).getMonth() + 1;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ano = PropertyEntity({ text: "Ano: ", val: ko.observable(anoAtual), def: anoAtual, getType: typesKnockout.int, maxlength: 4, configInt: { precision: 0, allowZero: false, thousands: '' }, enable: ko.observable(true), required: true });
    this.Mes = PropertyEntity({ text: "Mês: ", val: ko.observable(mesAtual), def: mesAtual, options: EnumMes.obterOpcoesPesquisa(), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' }, enable: false });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoFechamentoPontuacao.Todas), options: EnumSituacaoFechamentoPontuacao.obterOpcoesPesquisa(), def: EnumSituacaoFechamentoPontuacao.Todas });
}

var CRUDFechamentoPontuacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Novo" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridFechamentoPontuacao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] }
    var configuracaoExportacao = { url: "FechamentoPontuacao/ExportarPesquisa", titulo: "Fechamentos de Pontuação" };

    _gridFechamentoPontuacao = new GridViewExportacao(_pesquisaFechamentoPontuacao.Pesquisar.idGrid, "FechamentoPontuacao/Pesquisa", _pesquisaFechamentoPontuacao, menuOpcoes, configuracaoExportacao);
    _gridFechamentoPontuacao.CarregarGrid();
}

function loadFechamentoPontuacao() {
    _fechamentoPontuacao = new FechamentoPontuacao();
    KoBindings(_fechamentoPontuacao, "knockoutFechamentoPontuacao");

    HeaderAuditoria("FechamentoPontuacao", _fechamentoPontuacao);

    _pesquisaFechamentoPontuacao = new PesquisaFechamentoPontuacao();
    KoBindings(_pesquisaFechamentoPontuacao, "knockoutPesquisaFechamentoPontuacao", false, _pesquisaFechamentoPontuacao.Pesquisar.id);

    _CRUDFechamentoPontuacao = new CRUDFechamentoPontuacao();
    KoBindings(_CRUDFechamentoPontuacao, "knockoutCRUDCadastroFechamentoPontuacao");

    loadFechamentoPontuacaoEtapa();
    loadFechamentoPontuacaoTransportador();
    setarEtapaInicial();
    controlarComponentesHabilitados();
    loadGridFechamentoPontuacao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    Salvar(_fechamentoPontuacao, "FechamentoPontuacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento de pontuação adicionado com sucesso");
                buscarFechamentoPontuacaoPorCodigo(retorno.Data.Codigo);
                recarregarGridFechamentoPontuacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function cancelarClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja cancelar o fechamento de pontuação?", function () {
        executarReST("FechamentoPontuacao/Cancelar", { Codigo: _fechamentoPontuacao.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento de pontuação cancelado com sucesso");
                    buscarFechamentoPontuacaoPorCodigo(_fechamentoPontuacao.Codigo.val());
                    recarregarGridFechamentoPontuacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function editarClick(registroSelecionado) {
    _pesquisaFechamentoPontuacao.ExibirFiltros.visibleFade(false);

    buscarFechamentoPontuacaoPorCodigo(registroSelecionado.Codigo);
}

function limparClick() {
    limparCamposFechamentoPontuacao();
}

/*
 * Declaração das Funções Privadas
 */

function buscarFechamentoPontuacaoPorCodigo(codigo) {
    limparCamposFechamentoPontuacao();

    executarReST("FechamentoPontuacao/BuscarPorCodigo", { Codigo: codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_fechamentoPontuacao, retorno);
                preencherFechamentoPontuacaoTransportador(retorno.Data);
                setarEtapas();
                controlarComponentesHabilitados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function controlarBotoesHabilitados() {
    _CRUDFechamentoPontuacao.Adicionar.visible(isRegistroNovo());
    _CRUDFechamentoPontuacao.Cancelar.visible(_fechamentoPontuacao.Situacao.val() == EnumSituacaoFechamentoPontuacao.Finalizado);
}

function ControlarCamposHabilitados() {
    var registroNovo = isRegistroNovo();

    _fechamentoPontuacao.Ano.enable(registroNovo);
    _fechamentoPontuacao.Mes.enable(registroNovo);
}

function controlarComponentesHabilitados() {
    controlarBotoesHabilitados();
    ControlarCamposHabilitados();
}

function isRegistroNovo() {
    return (_fechamentoPontuacao.Codigo.val() == 0);
}

function limparCamposFechamentoPontuacao() {
    LimparCampos(_fechamentoPontuacao);
    limparCamposFechamentoPontuacaoTransportador();
    setarEtapaInicial();
    controlarComponentesHabilitados();
}

function recarregarGridFechamentoPontuacao() {
    _gridFechamentoPontuacao.CarregarGrid();
}
