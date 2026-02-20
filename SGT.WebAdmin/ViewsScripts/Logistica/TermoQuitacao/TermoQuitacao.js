/// <reference path="Anexo.js" />
/// <reference path="AnexoTransportador.js" />
/// <reference path="Aprovacao.js" />
/// <reference path="DadosTermoQuitacao.js" />
/// <reference path="DadosAceiteTransportador.js" />
/// <reference path="Etapas.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumAbaTermoQuitacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTermoQuitacao.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _abaAtiva;
var _termoQuitacao;
var _CRUDTermoQuitacao;
var _gridTermoQuitacao;
var _pesquisaTermoQuitacao;

/*
 * Declaração das Classes
 */

var PesquisaTermoQuitacao = function () {
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
    this.DataBaseInicial = PropertyEntity({ text: "Data Base Inicial: ", getType: typesKnockout.date });
    this.DataBaseLimite = PropertyEntity({ text: "Data Base Limite: ", dateRangeInit: this.DataBaseInicial, getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoTermoQuitacao.Todas), options: EnumSituacaoTermoQuitacao.obterOpcoesPesquisa(), text: "Situação: " });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: !isAcessoTransportador() });

    this.DataBaseInicial.dateRangeLimit = this.DataBaseLimite;
    this.DataBaseLimite.dateRangeInit = this.DataBaseInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTermoQuitacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(isAcessoTransportador()), visible: ko.observable(true)
    });
}

var TermoQuitacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoTermoQuitacao.Todas), def: EnumSituacaoTermoQuitacao.Todas, text: "*Situação: ", required: true, getType: typesKnockout.int, enable: ko.observable(true) });
}

var CRUDTermoQuitacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Aprovar = PropertyEntity({ eventClick: aprovarClick, type: types.event, text: "Aprovar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Novo", visible: !isAcessoTransportador() });
    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });
    this.Reprovar = PropertyEntity({ eventClick: reprovarClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTermoQuitacao() {
    var editarRegistro = {
        descricao: "Editar",
        id: "clasEditar",
        evento: "onclick",
        metodo: editarClick,
        tamanho: "15",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editarRegistro]
    }

    var configuracaoExportacao = {
        url: "TermoQuitacao/ExportarPesquisa",
        titulo: "Termos de Quitação"
    };

    _gridTermoQuitacao = new GridViewExportacao(_pesquisaTermoQuitacao.Pesquisar.idGrid, "TermoQuitacao/Pesquisa", _pesquisaTermoQuitacao, menuOpcoes, configuracaoExportacao);
    _gridTermoQuitacao.CarregarGrid();
}

function loadTermoQuitacao() {
    if (!isAcessoTransportador())
        $("#formulario-cadastro-termo-quitacao").show();

    _termoQuitacao = new TermoQuitacao();
    HeaderAuditoria("TermoQuitacao", _termoQuitacao);

    _pesquisaTermoQuitacao = new PesquisaTermoQuitacao();
    KoBindings(_pesquisaTermoQuitacao, "knockoutPesquisaTermoQuitacao", false, _pesquisaTermoQuitacao.Pesquisar.id);

    _CRUDTermoQuitacao = new CRUDTermoQuitacao();
    KoBindings(_CRUDTermoQuitacao, "knockoutCRUDCadastroTermoQuitacao");

    new BuscarTransportadores(_pesquisaTermoQuitacao.Transportador);

    loadEtapaTermoQuitacao();
    loadDadosTermoQuitacao();
    loadAnexo();
    loadDadosAceiteTransportador();
    loadAnexoTransportador();
    loadAprovacaoTermoQuitacao();
    loadGridTermoQuitacao();
    setarEtapaInicial();
    controlarComponentesHabilitados();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    adicionarDadosTermoQuitacao();
}

function aprovarClick() {
    aprovarTermoQuitacao();
}

function atualizarClick() {
    atualizarDadosTermoQuitacao();
}

function editarClick(termoQuitacaoSelecionada) {
    buscarTermoQuitacaoPorCodigo(termoQuitacaoSelecionada.Codigo, function () {
        _pesquisaTermoQuitacao.ExibirFiltros.visibleFade(false);

        $("#formulario-cadastro-termo-quitacao").show();
    });
}

function limparClick() {
    if (!isAcessoTransportador())
        limparCamposTermoQuitacao();
}

function reprocessarRegrasClick() {
    executarReST("TermoQuitacao/ReprocessarRegras", { Codigo: _termoQuitacao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Regras de aprovação reprocessadas com sucesso.");
                buscarTermoQuitacaoPorCodigo(_termoQuitacao.Codigo.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar o termo de quitação.");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function reprovarClick() {
    reprovarTermoQuitacao();
}

/*
 * Declaração das Funções
 */

function buscarTermoQuitacaoPorCodigo(codigo, callback) {
    executarReST("TermoQuitacao/BuscarPorCodigo", { Codigo: codigo }, function (retorno) {
        if (retorno.Data != null) {
            if (callback instanceof Function)
                callback();

            limparCamposTermoQuitacao();
            preencherTermoQuitacao(retorno.Data);
            setarEtapas();
            controlarComponentesHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function controlarBotoesHabilitados() {
    var acessoTransportador = isAcessoTransportador();
    var botaoAdicionarVisivel = false;
    var botaoAprovarVisivel = false;
    var botaoAtualizarVisivel = false;
    var botaoReprocessarRegrasVisivel = false;
    var botaoReprovarVisivel = false;

    switch (_termoQuitacao.Situacao.val()) {
        case EnumSituacaoTermoQuitacao.Todas:
            botaoAdicionarVisivel = ((_abaAtiva === EnumAbaTermoQuitacao.DadosTermo) && !acessoTransportador);
            break;

        case EnumSituacaoTermoQuitacao.AceiteTransportadorRejeitado:
            botaoAtualizarVisivel = ((_abaAtiva === EnumAbaTermoQuitacao.DadosTermo) && !acessoTransportador);
            break;

        case EnumSituacaoTermoQuitacao.AguardandoAceiteTransportador:
        case EnumSituacaoTermoQuitacao.AprovacaoRejeitada:
            if ((_abaAtiva === EnumAbaTermoQuitacao.DadosAceiteTransportador) && acessoTransportador) {
                botaoAprovarVisivel = true;
                botaoReprovarVisivel = true;
            }
            break;

        case EnumSituacaoTermoQuitacao.SemRegraAprovacao:
            botaoReprocessarRegrasVisivel = (_abaAtiva === EnumAbaTermoQuitacao.Aprovacao);
            break;
    }

    _CRUDTermoQuitacao.Adicionar.visible(botaoAdicionarVisivel);
    _CRUDTermoQuitacao.Aprovar.visible(botaoAprovarVisivel);
    _CRUDTermoQuitacao.Atualizar.visible(botaoAtualizarVisivel);
    _CRUDTermoQuitacao.ReprocessarRegras.visible(botaoReprocessarRegrasVisivel);
    _CRUDTermoQuitacao.Reprovar.visible(botaoReprovarVisivel);
}

function ControlarCamposHabilitados() {
    controlarCamposDadosTermoQuitacaoHabilitados();
    controlarCamposDadosAceiteTransportadorHabilitados();
}

function controlarComponentesHabilitados() {
    controlarBotoesHabilitados();
    ControlarCamposHabilitados();
}

function isAcessoTransportador() {
    return (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe);
}

function limparCamposTermoQuitacao() {
    LimparCampos(_termoQuitacao);
    limparCamposDadosTermoQuitacao();
    limparAnexo();
    limparCamposDadosAceiteTransportador();
    limparAnexoTransportador();
    limparCamposAprovacao();
    setarEtapaInicial();
    controlarComponentesHabilitados();
}

function preencherTermoQuitacao(dados) {
    _termoQuitacao.Codigo.val(dados.Codigo);
    _termoQuitacao.Situacao.val(dados.Situacao);

    preencherDadosTermoQuitacao(dados.DadosTermoQuitacao);
    preencherAnexo(dados.Anexos);
    preencherDadosAceiteTransportador(dados.DadosAceiteTransportador);
    preencherAnexoTransportador(dados.AnexosTransportador);
    preencherAprovacao(dados.ResumoAprovacao, dados.Situacao);
}
