/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/PagamentoMotoristaTipo.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="TipoOperacao.js" />

// #region Objetos Globais do Arquivo

var _cadastroSistemaIntegracao;
var _configuracaoIntegracao;
var _gridConfiguracaoSistemaIntegracao;
const _tiposIntegracaoEnvioProgramado = [
    EnumTipoIntegracao.Carrefour,
    EnumTipoIntegracao.Mars,
    EnumTipoIntegracao.GrupoSC,
    EnumTipoIntegracao.ObramaxCTE,
    EnumTipoIntegracao.ObramaxNFE,
    EnumTipoIntegracao.WeberChile
];

// #endregion Objetos Globais do Arquivo

// #region Classes

var CadastroSistemaIntegracao = function () {
    this.SistemaIntegracao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SistemaDeIntegracao.getFieldDescription(), val: ko.observable(""), def: "", options: ko.observable([]), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });

    this.GerarDiariaMotoristaProprio = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarDiariaAutomaticamenteParaOsMotoristasPropriosNaEmissaoDaCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PagamentoMotoristaTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.TipoOperacao.TipoDoPagamento.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.HabilitarIntegracaoAvancoParaEmissao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.HabilitarIntegracaoAvancoParaEmissao, val: ko.observable(false), visible: ko.observable(true), def: false, getType: typesKnockout.bool });
    this.CalcularGerarGNRE = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CalcularGerarGNRE, val: ko.observable(false), visible: ko.observable(true), def: false, getType: typesKnockout.bool });
    this.IntegrarCargasGeradasMultiEmbarcador = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.IntegrarCargasGeradasMultiEmbarcador, val: ko.observable(false), visible: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.IntegrarDadosTransporte = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.IntegrarDadosTransporte, val: ko.observable(false), visible: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.IntegrarDocumentos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.IntegrarDocumentos, val: ko.observable(false), visible: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PossuiTempoEnvioIntegracaoDocumentosCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PossuiTempoEnvioIntegracaoDocumentosCarga, val: ko.observable(false), visible: ko.observable(false), def: false, getType: typesKnockout.bool });
}

var ConfiguracaoIntegracao = function () {
    this.ListaSistemaIntegracao = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaSistemaIntegracao.val.subscribe(function () {
        recarregarGridConfiguracaoSistemaIntegracao();
        ControlarVisibilidadeCamposIntegracao();
    });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridConfiguracaoSistemaIntegracao() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerConfiguracaoSistemaIntegracao, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
    ];

    _gridConfiguracaoSistemaIntegracao = new BasicDataTable(_configuracaoIntegracao.ListaSistemaIntegracao.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridConfiguracaoSistemaIntegracao.CarregarGrid([]);
}

function loadIntegracao() {
    _configuracaoIntegracao = new ConfiguracaoIntegracao();
    KoBindings(_configuracaoIntegracao, "knockoutConfiguracaoIntegracao");

    _cadastroSistemaIntegracao = new CadastroSistemaIntegracao();
    KoBindings(_cadastroSistemaIntegracao, "knockoutCadastroSistemaIntegracao");

    new BuscarPagamentoMotoristaTipo(_cadastroSistemaIntegracao.PagamentoMotoristaTipo);

    _tipoOperacao.GerarDiariaMotoristaProprio = _cadastroSistemaIntegracao.GerarDiariaMotoristaProprio;
    _tipoOperacao.PagamentoMotoristaTipo = _cadastroSistemaIntegracao.PagamentoMotoristaTipo;
    _tipoOperacao.HabilitarIntegracaoAvancoParaEmissao = _cadastroSistemaIntegracao.HabilitarIntegracaoAvancoParaEmissao;
    _tipoOperacao.CalcularGerarGNRE = _cadastroSistemaIntegracao.CalcularGerarGNRE;
    _tipoOperacao.IntegrarCargasGeradasMultiEmbarcador = _cadastroSistemaIntegracao.IntegrarCargasGeradasMultiEmbarcador;
    _tipoOperacao.IntegrarDadosTransporte = _cadastroSistemaIntegracao.IntegrarDadosTransporte;
    _tipoOperacao.IntegrarDocumentos = _cadastroSistemaIntegracao.IntegrarDocumentos;
    _tipoOperacao.PossuiTempoEnvioIntegracaoDocumentosCarga = _cadastroSistemaIntegracao.PossuiTempoEnvioIntegracaoDocumentosCarga;

    loadGridConfiguracaoSistemaIntegracao();
    buscaIntegracoes();
    obterConfiguracoesIntegracoes();
}

function buscaIntegracoes() {
    return new Promise(function (resolve) {
        executarReST("TipoOperacao/BuscarIntegracoes", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                var integracoes = retorno.Data.Integracoes.map(function (d) { return { value: d.Tipo, text: d.Descricao } });
                _cadastroSistemaIntegracao.SistemaIntegracao.options(integracoes);
            }
            resolve();
        });
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarConfiguracaoIntegracaoClick() {
    if (ValidarCamposObrigatorios(_cadastroSistemaIntegracao)) {
        if (isConfiguracaoSistemaIntegracaoExistente())
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pedidos.TipoOperacao.SistemaDeIntegracaoJaExistente, Localization.Resources.Pedidos.TipoOperacao.SistemaIntegracaoJaEstaCadastrado.format(obterDescricaoSistemaIntegracao()));
        else {
            _configuracaoIntegracao.ListaSistemaIntegracao.val().push(obterConfiguracaoSistemaIntegracaoSalvar());

            recarregarGridConfiguracaoSistemaIntegracao();
        }
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function preencherIntegracao(dadosConfiguracaoIntegracao) {
    _configuracaoIntegracao.ListaSistemaIntegracao.val(dadosConfiguracaoIntegracao);
}

function preencherIntegracaoSalvar(tipoOperacao) {
    tipoOperacao["Integracoes"] = obterConfiguracaoIntegracaoSalvar();
}

function limparCamposIntegracao() {
    _configuracaoIntegracao.ListaSistemaIntegracao.val([]);
}

// #endregion Funções Públicas

// #region Funções Privadas

function isConfiguracaoSistemaIntegracaoExistente() {
    var listaSistemaIntegracao = obterListaSistemaIntegracao();
    var tipo = _cadastroSistemaIntegracao.SistemaIntegracao.val();

    for (var i = 0; i < listaSistemaIntegracao.length; i++) {
        if (listaSistemaIntegracao[i].Tipo == tipo)
            return true;
    }

    return false;
}

function obterConfiguracaoIntegracaoSalvar() {
    var listaSistemaIntegracao = obterListaSistemaIntegracao();
    var listaSistemaIntegracaoRetornar = new Array();

    listaSistemaIntegracao.forEach(function (sistemaIntegracao) {
        listaSistemaIntegracaoRetornar.push({
            Codigo: sistemaIntegracao.Codigo,
            Tipo: sistemaIntegracao.Tipo
        });
    });

    return JSON.stringify(listaSistemaIntegracaoRetornar);
}

function obterConfiguracaoSistemaIntegracaoSalvar() {

    if (_cadastroSistemaIntegracao.SistemaIntegracao.val() == EnumTipoIntegracao.Boticario) {
        _cadastroSistemaIntegracao.IntegrarDadosTransporte.visible(true);
        _cadastroSistemaIntegracao.IntegrarDocumentos.visible(true);
    } else if (_cadastroSistemaIntegracao.SistemaIntegracao.val() == EnumTipoIntegracao.ArcelorMittal) {
        _cadastroSistemaIntegracao.IntegrarDadosTransporte.visible(true);
        _cadastroSistemaIntegracao.IntegrarDocumentos.visible(false);
    } else if (SistemaUtilizaIntegracaoProgramada(_cadastroSistemaIntegracao.SistemaIntegracao.val())) { 
        _cadastroSistemaIntegracao.PossuiTempoEnvioIntegracaoDocumentosCarga.visible(true); 
    } else {
        _cadastroSistemaIntegracao.IntegrarDadosTransporte.visible(false);
        _cadastroSistemaIntegracao.IntegrarDocumentos.visible(false);
    }
    return {
        Codigo: guid(),
        Tipo: _cadastroSistemaIntegracao.SistemaIntegracao.val(),
        Descricao: obterDescricaoSistemaIntegracao()
    };
}

function obterDescricaoSistemaIntegracao() {
    var tipo = _cadastroSistemaIntegracao.SistemaIntegracao.val();
    var integracoes = EnumTipoIntegracao.obterOpcoes();
    for (var i = 0; i < integracoes.length; i++) {
        if (integracoes[i].value == tipo) {
            return integracoes[i].text;
        }
    }

    return "";
}

function obterListaSistemaIntegracao() {
    return _configuracaoIntegracao.ListaSistemaIntegracao.val().slice();
}

function recarregarGridConfiguracaoSistemaIntegracao() {
    var listaSistemaIntegracao = obterListaSistemaIntegracao();

    _gridConfiguracaoSistemaIntegracao.CarregarGrid(listaSistemaIntegracao);
}

function removerConfiguracaoSistemaIntegracao(registroSelecionado) {
    var listaSistemaIntegracao = obterListaSistemaIntegracao();

    listaSistemaIntegracao.forEach(function (sistemaIntegracao, i) {
        if (registroSelecionado.Codigo == sistemaIntegracao.Codigo) {
            listaSistemaIntegracao.splice(i, 1);
        }
    });

    _configuracaoIntegracao.ListaSistemaIntegracao.val(listaSistemaIntegracao);
}

function obterConfiguracoesIntegracoes() {
    executarReST("TipoOperacao/ObterConfiguracoesIntegracoes", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _cadastroSistemaIntegracao.HabilitarIntegracaoAvancoParaEmissao.visible(retorno.Data.IntegracaoUnilever);
        }
    });
}

function ControlarVisibilidadeCamposIntegracao() {
    _cadastroSistemaIntegracao.IntegrarCargasGeradasMultiEmbarcador.visible(false);
    _cadastroSistemaIntegracao.PossuiTempoEnvioIntegracaoDocumentosCarga.visible(false);

    var listaSistemaIntegracao = obterListaSistemaIntegracao();
    listaSistemaIntegracao.forEach(function (sistemaIntegracao, i) {
        if (sistemaIntegracao.Tipo == EnumTipoIntegracao.Eship) {
            _cadastroSistemaIntegracao.IntegrarCargasGeradasMultiEmbarcador.visible(true);
        };

        if (sistemaIntegracao.Tipo == EnumTipoIntegracao.Carrefour) {
            _cadastroSistemaIntegracao.PossuiTempoEnvioIntegracaoDocumentosCarga.visible(true);
        };
    });
}

function SistemaUtilizaIntegracaoProgramada(tipoIntegracao) {

    return _tiposIntegracaoEnvioProgramado.includes(tipoIntegracao);
}

// #endregion Funções Privadas
