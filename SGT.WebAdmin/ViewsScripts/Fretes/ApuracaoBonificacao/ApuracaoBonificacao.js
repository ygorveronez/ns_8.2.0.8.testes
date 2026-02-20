/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoPontuacao.js" />
/// <reference path="ApuracaoBonificacaoEtapa.js" />
/// <reference path="ApuracaoBonificacaoApuracoes.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDApuracaoBonificacao;
var _apuracaoBonificacao;
var _gridApuracaoBonificacao;
var _pesquisaApuracaoBonificacao;

/*
 * Declaração das Classes
 */

var PesquisaApuracaoBonificacao = function () {
    this.Ano = PropertyEntity({ text: "Ano: ", getType: typesKnockout.int, maxlength: 4, configInt: { precision: 0, allowZero: false, thousands: '' } });
    this.Mes = PropertyEntity({ text: "Mês: ", val: ko.observable(EnumMes.Todos), def: EnumMes.Todos, options: EnumMes.obterOpcoesPesquisa() });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoApuracaoBonificacao.Todas), def: EnumSituacaoApuracaoBonificacao.Todas, options: EnumSituacaoApuracaoBonificacao.obterOpcoesPesquisa() });
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' }, maxlength: 12 });
    this.RegraApuracao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Regra apuração: ", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            recarregarGridApuracaoBonificacao();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var ApuracaoBonificacao = function () {
    const anoAtual = (new Date()).getFullYear();
    const mesAtual = (new Date()).getMonth() + 1;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TotalAcrescimo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.TotalDesconto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.Ano = PropertyEntity({ text: "Ano: ", val: ko.observable(anoAtual), def: anoAtual, getType: typesKnockout.int, maxlength: 4, configInt: { precision: 0, allowZero: false, thousands: '' }, enable: ko.observable(true), required: true });
    this.Mes = PropertyEntity({ text: "Mês: ", val: ko.observable(mesAtual), def: mesAtual, options: EnumMes.obterOpcoes(), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' }, enable: false });
    this.RegraApuracao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "*Regra apuração: ", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoApuracaoBonificacao.Todas), options: EnumSituacaoApuracaoBonificacao.obterOpcoesPesquisa(), def: EnumSituacaoApuracaoBonificacao.Todas });
}

var CRUDApuracaoBonificacao = function () {
    this.Processar = PropertyEntity({ eventClick: processarClick, type: types.event, text: "Processar", idGrid: guid(), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Novo" });
    this.GerarOcorrencias = PropertyEntity({ eventClick: gerarOcorrenciasPGT, type: types.event, text: "Gerar Ocorrência(s)", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridApuracaoBonificacao() {
    const opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "15", icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] }
    const configuracaoExportacao = { url: "ApuracaoBonificacao/ExportarPesquisa", titulo: "Apurações de Bonificação" };

    _gridApuracaoBonificacao = new GridViewExportacao(_pesquisaApuracaoBonificacao.Pesquisar.idGrid, "ApuracaoBonificacao/Pesquisa", _pesquisaApuracaoBonificacao, menuOpcoes, configuracaoExportacao);
    _gridApuracaoBonificacao.CarregarGrid();
}

function loadApuracaoBonificacao() {
    _apuracaoBonificacao = new ApuracaoBonificacao();
    KoBindings(_apuracaoBonificacao, "knockoutApuracaoBonificacao");

    _pesquisaApuracaoBonificacao = new PesquisaApuracaoBonificacao();
    KoBindings(_pesquisaApuracaoBonificacao, "knockoutPesquisaApuracaoBonificacao", false, _pesquisaApuracaoBonificacao.Pesquisar.id);

    _CRUDApuracaoBonificacao = new CRUDApuracaoBonificacao();
    KoBindings(_CRUDApuracaoBonificacao, "knockoutCRUDCadastroApuracaoBonificacao");

    BuscarBonificacaoTransportador(_apuracaoBonificacao.RegraApuracao);
    BuscarBonificacaoTransportador(_pesquisaApuracaoBonificacao.RegraApuracao);

    loadApuracaoBonificacaoEtapa();
    loadApuracaoBonificacaoApuracoes();
    setarEtapaInicial();
    controlarComponentesHabilitados();
    loadGridApuracaoBonificacao();
    loadGridApuracaoFechamento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function processarClick() {
    Salvar(_apuracaoBonificacao, "ApuracaoBonificacao/Processar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Apuração de bonificaçãoo adicionada com sucesso");
                buscarApuracaoBonificacaoPorCodigo(retorno.Data.Codigo);
                recarregarGridApuracaoBonificacao();
                recarregarGridApuracaoFechamento(retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function cancelarClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja cancelar a apuração de bonificação?", function () {
        executarReST("ApuracaoBonificacao/Cancelar", { Codigo: _apuracaoBonificacao.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento de pontuação cancelado com sucesso");
                    buscarApuracaoBonificacaoPorCodigo(_apuracaoBonificacao.Codigo.val());
                    recarregarGridApuracaoBonificacao();
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
    _pesquisaApuracaoBonificacao.ExibirFiltros.visibleFade(false);

    buscarApuracaoBonificacaoPorCodigo(registroSelecionado.Codigo);
}

function limparClick() {
    limparCamposFechamentoPontuacao();
}

/*
 * Declaração das Funções Privadas
 */

function buscarApuracaoBonificacaoPorCodigo(codigo) {
    limparCamposFechamentoPontuacao();
    _apuracaoBonificacao.Codigo.val(codigo);
    BuscarPorCodigo(_apuracaoBonificacao, "ApuracaoBonificacao/BuscarPorCodigo", function (arg) {
        recarregarGridApuracaoFechamento(arg.Data);
        setarEtapas();
        controlarComponentesHabilitados();
    }, null);
}

function controlarBotoesHabilitados(situacao) {
    _CRUDApuracaoBonificacao.Processar.visible(isRegistroNovo());
    _CRUDApuracaoBonificacao.Cancelar.visible(!isRegistroNovo() && _apuracaoBonificacao.Situacao.val() == EnumSituacaoApuracaoBonificacao.AguardandoGeracaoOcorrencia);
    _CRUDApuracaoBonificacao.GerarOcorrencias.visible(!isRegistroNovo() && _apuracaoBonificacao.Situacao.val() == EnumSituacaoApuracaoBonificacao.AguardandoGeracaoOcorrencia);
}

function ControlarCamposHabilitados() {
    const registroNovo = isRegistroNovo();

    _apuracaoBonificacao.Ano.enable(registroNovo);
    _apuracaoBonificacao.Mes.enable(registroNovo);
    _apuracaoBonificacao.RegraApuracao.enable(registroNovo);
}

function controlarComponentesHabilitados() {
    controlarBotoesHabilitados();
    ControlarCamposHabilitados();
}

function isRegistroNovo() {
    return (_apuracaoBonificacao.Codigo.val() == 0);
}

function limparCamposFechamentoPontuacao() {
    LimparCampos(_apuracaoBonificacao);
    setarEtapaInicial();
    controlarComponentesHabilitados();
}

function recarregarGridApuracaoBonificacao() {
    _gridApuracaoBonificacao.CarregarGrid();
}