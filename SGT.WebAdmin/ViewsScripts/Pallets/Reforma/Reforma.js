/// <reference path="Envio.js" />
/// <reference path="EnvioQuantidade.js" />
/// <reference path="Etapas.js" />
/// <reference path="NfeRetorno.js" />
/// <reference path="NfeRetornoResumo.js" />
/// <reference path="NfeSaida.js" />
/// <reference path="NfeSaidaResumo.js" />
/// <reference path="NfsRetorno.js" />
/// <reference path="Resumo.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumAbaReformaPallet.js" />
/// <reference path="../../Enumeradores/EnumSituacaoReformaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _abaAtiva;
var _CRUDReformaPallet;
var _gridReformaPallet;
var _isTMS = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS;
var _pesquisaReformaPallet;
var _reformaPallet;

/*
 * Declaração das Classes
 */

var PesquisaReformaPallet = function () {
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Solicitação início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Solicitação limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: !_isTMS });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoReformaPallet.Todas), options: EnumSituacaoReformaPallet.obterOpcoes(), def: EnumSituacaoReformaPallet.Todas, text: "Situação: " });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: _isTMS });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridReformaPallet.CarregarGrid();
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

var ReformaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoReformaPallet.Todas), def: EnumSituacaoReformaPallet.Todas, text: "*Situãção: ", required: true, getType: typesKnockout.int, enable: ko.observable(true) });
}

var CRUDReformaPallet = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Nova", visible: true });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridReformaPallets() {
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
        url: "Reforma/ExportarPesquisa",
        titulo: "Reforma de Pallets"
    };

    _gridReformaPallet = new GridViewExportacao(_pesquisaReformaPallet.Pesquisar.idGrid, "Reforma/Pesquisa", _pesquisaReformaPallet, menuOpcoes, configuracaoExportacao);
    _gridReformaPallet.CarregarGrid();
}

function loadReformaPallet() {
    _reformaPallet = new ReformaPallet();
    HeaderAuditoria("ReformaPallet", _reformaPallet);

    _pesquisaReformaPallet = new PesquisaReformaPallet();
    KoBindings(_pesquisaReformaPallet, "knockoutPesquisaReformaPallet", false, _pesquisaReformaPallet.Pesquisar.id);

    new BuscarFilial(_pesquisaReformaPallet.Filial);
    new BuscarClientes(_pesquisaReformaPallet.Fornecedor);
    new BuscarTransportadores(_pesquisaReformaPallet.Transportador);

    _CRUDReformaPallet = new CRUDReformaPallet();
    KoBindings(_CRUDReformaPallet, "knockoutCRUDCadastroReformaPallet");
    
    loadEtapaReformaPallet();
    loadResumoTransferenciaPallet();
    loadEnvioReformaPallet();
    loadEnvioReformaPalletQuantidade();
    loadNfeSaidaReformaPallet();
    loadNfeSaidaResumoReformaPallet();
    loadNfeRetornoReformaPallet();
    loadNfeRetornoResumoReformaPallet();
    loadNfsRetornoReformaPallet();
    loadGridReformaPallets();
    setarEtapaInicial();
    controlarComponentesHabilitados();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    switch (_abaAtiva) {
        case EnumAbaReformaPallet.Envio:
            adicionarEnvio();
            break;

        case EnumAbaReformaPallet.NfeSaida:
            finalizarNfeSaida();
            break;

        case EnumAbaReformaPallet.Retorno:
            finalizarReforma();
            break;
    }
}

function cancelarClick() {
    if ((_abaAtiva === EnumAbaReformaPallet.NfeSaida) || (_abaAtiva === EnumAbaReformaPallet.Retorno)) {
        exibirConfirmacao("Confirmação", "Realmente deseja cancelar a reforma de pallets?", function () {
            executarReST("Reforma/CancelarPorCodigo", { Codigo: _reformaPallet.Codigo.val() }, function (retorno) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso");

                    novaReforma();
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                }
            }, null);
        });
    }
}

function editarClick(registroSelecionado) {
    _pesquisaReformaPallet.ExibirFiltros.visibleFade(false);

    editar(registroSelecionado);
}

function limparClick() {
    novaReforma();
}

/*
 * Declaração das Funções
 */

function buscarReformaPalletPorCodigo(codigo) {
    executarReST("Reforma/BuscarPorCodigo", { Codigo: codigo }, function (retorno) {
        if (retorno.Data != null) {
            limparCamposReformaPallet();
            preencherReforma(retorno.Data);
            setarEtapas();
            controlarComponentesHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function editar(reformaPallet) {
    buscarReformaPalletPorCodigo(reformaPallet.Codigo);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function controlarBotoesHabilitados() {
    var botaoAdicionarTexto = "Adicionar";
    var botaoAdicionarVisivel = false;
    var botaoCancelarVisivel = false;

    switch (_reformaPallet.Situacao.val()) {
        case EnumSituacaoReformaPallet.Todas:
            botaoAdicionarTexto = "Enviar";
            botaoAdicionarVisivel = true;
            break;

        case EnumSituacaoReformaPallet.AguardandoNfeSaida:
            if (_abaAtiva === EnumAbaReformaPallet.NfeSaida) {
                botaoAdicionarTexto = "Avançar";
                botaoAdicionarVisivel = true;
                botaoCancelarVisivel = true;
            }
            break;

        case EnumSituacaoReformaPallet.AguardandoRetorno:
            if (_abaAtiva === EnumAbaReformaPallet.Retorno) {
                botaoAdicionarTexto = "Finalizar"
                botaoAdicionarVisivel = true;
                botaoCancelarVisivel = true;
            }
            break;
    }

    _CRUDReformaPallet.Adicionar.text(botaoAdicionarTexto);
    _CRUDReformaPallet.Adicionar.visible(botaoAdicionarVisivel);
    _CRUDReformaPallet.Cancelar.visible(botaoCancelarVisivel);
}

function ControlarCamposHabilitados() {
    controlarCamposEnvioHabilitados();
}

function controlarComponentesHabilitados() {
    controlarBotoesHabilitados();
    ControlarCamposHabilitados();
}

function finalizarReforma() {
    if (_reformaPallet.Situacao.val() === EnumSituacaoReformaPallet.AguardandoRetorno) {
        exibirConfirmacao("Confirmação", "Realmente deseja finalizar a reforma de pallets?", function () {
            if (validarNfeRetornoInformadas() && validarNfsRetornoInformadas()) {
                executarReST("Reforma/Finalizar", { Codigo: _reformaPallet.Codigo.val() }, function (retorno) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Reforma finalizada com sucesso");

                        novaReforma();
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                });
            }
        });
    }
}

function limparCamposReformaPallet() {
    LimparCampos(_reformaPallet);
    limparResumo();
    limparCamposEnvio();
    limparCamposEnvioQuantidade();
    limparNfeSaida();
    limparNfeSaidaResumo();
    limparNfeRetorno();
    limparNfeRetornoResumo();
    limparNfsRetorno();
    setarEtapaInicial();
    setarFocoAbaNfeRetorno();
    controlarComponentesHabilitados();
}

function novaReforma() {
    _gridReformaPallet.CarregarGrid();

    limparCamposReformaPallet();
    buscarSituacoes();
}

function preencherReforma(dados) {
    _reformaPallet.Codigo.val(dados.Codigo);
    _reformaPallet.Situacao.val(dados.Situacao);

    preencherResumo(dados.Resumo);
    preencherEnvio(dados.Envio);
    preencherEnvioQuantidade(dados.QuantidadesEnviadas);
    preencherNfeSaida(dados.ListaNfeSaida);
    preencherNfeRetorno(dados.ListaNfeRetorno);
    preencherNfsRetorno(dados.ListaNfsRetorno);
}

function setarFocoAbaNfeRetorno() {
    $("#abaNfeRetorno").click();
}