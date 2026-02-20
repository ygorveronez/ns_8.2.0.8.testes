/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoContratoPrestacaoServico.js" />
/// <reference path="ContratoPrestacaoServicoAprovacao.js" />
/// <reference path="ContratoPrestacaoServicoEtapas.js" />
/// <reference path="ContratoPrestacaoServicoFilial.js" />
/// <reference path="ContratoPrestacaoServicoTransportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _abaAtiva;
var _contratoPrestacaoServico;
var _CRUDContratoPrestacaoServico;
var _gridContratoPrestacaoServico;
var _pesquisaContratoPrestacaoServico;

/*
 * Declaração das Classes
 */

var PesquisaContratoPrestacaoServico = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.Situacao = PropertyEntity({ text: "Situação da Aprovação: ", val: ko.observable(EnumSituacaoContratoPrestacaoServico.Todas), options: EnumSituacaoContratoPrestacaoServico.obterOpcoesPesquisa(), def: EnumSituacaoContratoPrestacaoServico.Todas });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContratoPrestacaoServico.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var ContratoPrestacaoServico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataFinal = PropertyEntity({ text: "*Data Final: ", getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "*Data Inicial: ", getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoContratoPrestacaoServico.Todas), options: EnumSituacaoContratoPrestacaoServico.obterOpcoesPesquisa(), def: EnumSituacaoContratoPrestacaoServico.Todas });
    this.Status = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), options: _status, def: true, enable: ko.observable(true) });
    this.ValorTeto = PropertyEntity({ text: "*Valor Teto:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 18, required: true, enable: ko.observable(true) });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
};

var CRUDContratoPrestacaoServico = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Novo", visible: true });
    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridContratoPrestacaoServico() {
    var editarRegistro = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editarRegistro] };
    var configuracaoExportacao = { url: "ContratoPrestacaoServico/ExportarPesquisa", titulo: "Contrato de Prestação de Serviço" };

    _gridContratoPrestacaoServico = new GridViewExportacao(_pesquisaContratoPrestacaoServico.Pesquisar.idGrid, "ContratoPrestacaoServico/Pesquisa", _pesquisaContratoPrestacaoServico, menuOpcoes, configuracaoExportacao);
    _gridContratoPrestacaoServico.CarregarGrid();
}

function loadContratoPrestacaoServico() {
    _contratoPrestacaoServico = new ContratoPrestacaoServico();
    KoBindings(_contratoPrestacaoServico, "knockoutContratoPrestacaoServico");

    HeaderAuditoria("ContratoPrestacaoServico", _contratoPrestacaoServico);

    _pesquisaContratoPrestacaoServico = new PesquisaContratoPrestacaoServico();
    KoBindings(_pesquisaContratoPrestacaoServico, "knockoutPesquisaContratoPrestacaoServico", false, _pesquisaContratoPrestacaoServico.Pesquisar.id);

    _CRUDContratoPrestacaoServico = new CRUDContratoPrestacaoServico();
    KoBindings(_CRUDContratoPrestacaoServico, "knockoutCRUDCadastroContratoPrestacaoServico");

    loadContratoPrestacaoServicoEtapa();
    loadContratoPrestacaoServicoTransportador();
    loadContratoPrestacaoServicoFilial();
    loadContratoPrestacaoServicoAprovacao();
    loadGridContratoPrestacaoServico();
    setarEtapaInicial();
    controlarComponentesHabilitados();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    if (validarCamposContratoPrestacaoServico()) {
        executarReST("ContratoPrestacaoServico/Adicionar", obterContratoPrestacaoServicoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionado com sucesso");

                    recarregarGridContratoPrestacaoServico();
                    limparCamposContratoPrestacaoServico();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function atualizarClick() {
    if (validarCamposContratoPrestacaoServico()) {
        executarReST("ContratoPrestacaoServico/Atualizar", obterContratoPrestacaoServicoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                    recarregarGridContratoPrestacaoServico();
                    limparCamposContratoPrestacaoServico();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function editarClick(ContratoPrestacaoServicoSelecionado) {
    _pesquisaContratoPrestacaoServico.ExibirFiltros.visibleFade(false);

    buscarContratoPrestacaoServicoPorCodigo(ContratoPrestacaoServicoSelecionado.Codigo);
}

function limparClick() {
    limparCamposContratoPrestacaoServico();
}

function reprocessarRegrasClick() {
    executarReST("ContratoPrestacaoServico/ReprocessarRegras", { Codigo: _contratoPrestacaoServico.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Regras de aprovação reprocessadas com sucesso.");
                buscarContratoPrestacaoServicoPorCodigo(_contratoPrestacaoServico.Codigo.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar o contrato de prestação de serviço.");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

/*
 * Declaração das Funções
 */

function buscarContratoPrestacaoServicoPorCodigo(codigo) {
    limparCamposContratoPrestacaoServico();

    executarReST("ContratoPrestacaoServico/BuscarPorCodigo", { Codigo: codigo }, function (retorno) {
        if (retorno.Data != null) {
            PreencherObjetoKnout(_contratoPrestacaoServico, { Data: retorno.Data.DadosContratoPrestacaoServico });
            preencherAprovacao(retorno.Data.ResumoAprovacao);
            preencherContratoPrestacaoServicoTransportador(retorno.Data.Transportadores);
            preencherContratoPrestacaoServicoFilial(retorno.Data.Filiais);
            setarEtapas(retorno.Data.DadosContratoPrestacaoServico.Situacao);
            controlarComponentesHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function controlarBotoesHabilitados() {
    var botaoAdicionarVisivel = false;
    var botaoAtualizarVisivel = false;
    var botaoReprocessarRegrasVisivel = false;

    switch (_contratoPrestacaoServico.Situacao.val()) {
        case EnumSituacaoContratoPrestacaoServico.Aprovado:
        case EnumSituacaoContratoPrestacaoServico.AprovacaoRejeitada:
            botaoAtualizarVisivel = (_abaAtiva === EnumAbaContratoPrestacaoServico.Dados);
            break;

        case EnumSituacaoContratoPrestacaoServico.Todas:
            botaoAdicionarVisivel = true;
            break;

        case EnumSituacaoContratoPrestacaoServico.SemRegraAprovacao:
            botaoAtualizarVisivel = (_abaAtiva === EnumAbaContratoPrestacaoServico.Dados);
            botaoReprocessarRegrasVisivel = (_abaAtiva === EnumAbaContratoPrestacaoServico.Aprovacao);
            break;
    }

    _CRUDContratoPrestacaoServico.Adicionar.visible(botaoAdicionarVisivel);
    _CRUDContratoPrestacaoServico.Atualizar.visible(botaoAtualizarVisivel);
    _CRUDContratoPrestacaoServico.ReprocessarRegras.visible(botaoReprocessarRegrasVisivel);
}

function ControlarCamposHabilitados() {
    var habilitarCampos = EnumSituacaoContratoPrestacaoServico.isPermiteAtualizar(_contratoPrestacaoServico.Situacao.val());

    _contratoPrestacaoServico.DataFinal.enable(habilitarCampos);
    _contratoPrestacaoServico.DataInicial.enable(habilitarCampos);
    _contratoPrestacaoServico.Descricao.enable(habilitarCampos);
    _contratoPrestacaoServico.Status.enable(habilitarCampos);
    _contratoPrestacaoServico.ValorTeto.enable(habilitarCampos);

    controlarCamposContratoPrestacaoServicoTransportador(habilitarCampos);
    controlarCamposContratoPrestacaoServicoFilial(habilitarCampos);
}

function controlarComponentesHabilitados() {
    controlarBotoesHabilitados();
    ControlarCamposHabilitados();
}

function limparCamposContratoPrestacaoServico() {
    LimparCampos(_contratoPrestacaoServico);
    limparCamposContratoPrestacaoServicoTransportador();
    limparCamposContratoPrestacaoServicoFilial();
    limparCamposAprovacao();
    setarEtapaInicial();
    controlarComponentesHabilitados();

    $("#tabContratoPrestacaoServicoDados").click();
}

function obterContratoPrestacaoServicoSalvar() {
    var contratoPrestacaoServico = RetornarObjetoPesquisa(_contratoPrestacaoServico);

    contratoPrestacaoServico["Transportadores"] = obterContratoPrestacaoServicoTransportadorSalvar();
    contratoPrestacaoServico["Filiais"] = obterContratoPrestacaoServicoFilialSalvar();

    return contratoPrestacaoServico;
}

function recarregarGridContratoPrestacaoServico() {
    _gridContratoPrestacaoServico.CarregarGrid();
}

function validarCamposContratoPrestacaoServico() {
    if (!ValidarCamposObrigatorios(_contratoPrestacaoServico)) {
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return false;
    }

    if (!isContratoPrestacaoServicoTransportadorInformado()) {
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe pelo menos um transportador");
        return false;
    }

    return true;
}