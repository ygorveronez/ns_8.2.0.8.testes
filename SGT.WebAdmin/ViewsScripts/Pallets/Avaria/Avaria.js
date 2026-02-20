/// <reference path="Aprovacao.js" />
/// <reference path="DadosAvaria.js" />
/// <reference path="DadosAvariaQuantidade.js" />
/// <reference path="Etapas.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/MotivoAvariaPallet.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumAbaAvariaPallet.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAvariaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _abaAtiva;
var _avariaPallet;
var _CRUDAvariaPallet;
var _gridAvariaPallet;
var _pesquisaAvariaPallet;

/*
 * Declaração das Classes
 */

var PesquisaAvariaPallet = function () {
    var isTMS = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS;

    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Solicitação início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Solicitação limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: !isTMS });
    this.MotivoAvaria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo Avaria:", idBtnSearch: guid() });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Setor:", idBtnSearch: guid(), visible: !isTMS });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAvariaPallet.Todas), options: EnumSituacaoAvariaPallet.obterOpcoes(), def: EnumSituacaoAvariaPallet.Todas, text: "Situação: " });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: isTMS });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAvariaPallet.CarregarGrid();
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

var AvariaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAvariaPallet.Todas), def: EnumSituacaoAvariaPallet.Todas, text: "*Situação: ", required: true, getType: typesKnockout.int, enable: ko.observable(true) });
}

var CRUDAvariaPallet = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Nova", visible: true });
    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridAvariaPallets() {
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
        url: "Avaria/ExportarPesquisa",
        titulo: "Avaria de Pallets"
    };

    _gridAvariaPallet = new GridViewExportacao(_pesquisaAvariaPallet.Pesquisar.idGrid, "Avaria/Pesquisa", _pesquisaAvariaPallet, menuOpcoes, configuracaoExportacao);
    _gridAvariaPallet.CarregarGrid();
}

function loadAvariaPallet() {
    _avariaPallet = new AvariaPallet();
    HeaderAuditoria("AvariaPallet", _avariaPallet);

    _pesquisaAvariaPallet = new PesquisaAvariaPallet();
    KoBindings(_pesquisaAvariaPallet, "knockoutPesquisaAvariaPallet", false, _pesquisaAvariaPallet.Pesquisar.id);

    new BuscarFilial(_pesquisaAvariaPallet.Filial);
    new BuscarMotivoAvariaPallet(_pesquisaAvariaPallet.MotivoAvaria);
    new BuscarSetorFuncionario(_pesquisaAvariaPallet.Setor);
    new BuscarTransportadores(_pesquisaAvariaPallet.Transportador);

    _CRUDAvariaPallet = new CRUDAvariaPallet();
    KoBindings(_CRUDAvariaPallet, "knockoutCRUDCadastroAvariaPallet");
    
    loadEtapaAvariaPallet();
    loadDadosAvariaPallet();
    loadDadosAvariaPalletQuantidade();
    loadAnexo();
    loadAprovacaoAvariaPallet();
    loadGridAvariaPallets();
    setarEtapaInicial();
    controlarComponentesHabilitados();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    adicionarDadosAvaria();
}

function editarClick(avariaPalletSelecionada) {
    _pesquisaAvariaPallet.ExibirFiltros.visibleFade(false);

    editar(avariaPalletSelecionada);
}

function limparClick() {
    limparCamposAvariaPallet();
    buscarSituacoes();
}

function reprocessarRegrasClick() {
    executarReST("Avaria/ReprocessarRegras", { Codigo: _avariaPallet.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Regras de aprovação reprocessadas com sucesso.");
                buscarAvariaPalletPorCodigo(_avariaPallet.Codigo.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar a avaria de Pallets.");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

/*
 * Declaração das Funções
 */

function buscarAvariaPalletPorCodigo(codigo) {
    executarReST("Avaria/BuscarPorCodigo", { Codigo: codigo }, function (retorno) {
        if (retorno.Data != null) {
            limparCamposAvariaPallet();
            preencherAvaria(retorno.Data);
            setarEtapas();
            controlarComponentesHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function editar(avariaPallet) {
    buscarAvariaPalletPorCodigo(avariaPallet.Codigo);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function controlarBotoesHabilitados() {
    var botaoAdicionarVisivel = false;
    var botaoReprocessarRegrasVisivel = false;

    switch (_avariaPallet.Situacao.val()) {
        case EnumSituacaoAvariaPallet.Todas:
            botaoAdicionarVisivel = true;
            break;

        case EnumSituacaoAvariaPallet.SemRegraAprovacao:
            botaoReprocessarRegrasVisivel = (_abaAtiva === EnumAbaAvariaPallet.Aprovacao);
            break;
    }

    _CRUDAvariaPallet.Adicionar.visible(botaoAdicionarVisivel);
    _CRUDAvariaPallet.ReprocessarRegras.visible(botaoReprocessarRegrasVisivel);
}

function ControlarCamposHabilitados() {
    controlarCamposDadosAvariaHabilitados();
}

function controlarComponentesHabilitados() {
    controlarBotoesHabilitados();
    ControlarCamposHabilitados();
}

function limparCamposAvariaPallet() {
    LimparCampos(_avariaPallet);
    limparCamposDadosAvaria();
    limparCamposDadosAvariaQuantidade();
    limparAnexos();
    limparCamposAprovacao();
    setarEtapaInicial();
    controlarComponentesHabilitados();
}

function preencherAvaria(dados) {
    _avariaPallet.Codigo.val(dados.Codigo);
    _avariaPallet.Situacao.val(dados.Situacao);

    preencherDadosAvaria(dados.DadosAvaria);
    preencherDadosAvariaQuantidade(dados.QuantidadesAvariadas);
    preencherAnexos(dados.Anexos);
    preencherAprovacao(dados.ResumoAprovacao, dados.Codigo, dados.Situacao);
}