/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Globais.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="GrupoTransportador.js" />

// #region Objetos Globais do Arquivo

var _cadastroSistemaIntegracao;
var _configuracaoIntegracao;
var _gridConfiguracaoSistemaIntegracao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CadastroSistemaIntegracao = function () {
    this.SistemaIntegracao = PropertyEntity({ text: "Sistema de Integração", val: ko.observable(""), def: "", options: ko.observable([]), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoIntegracaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ConfiguracaoIntegracao = function () {
    this.ListaSistemaIntegracao = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaSistemaIntegracao.val.subscribe(function () {
        recarregarGridConfiguracaoSistemaIntegracao();
    });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridConfiguracaoSistemaIntegracao() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerConfiguracaoSistemaIntegracao, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };
    var header = [
        { data: "Tipo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
    ];

    _gridConfiguracaoSistemaIntegracao = new BasicDataTable(_configuracaoIntegracao.ListaSistemaIntegracao.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridConfiguracaoSistemaIntegracao.CarregarGrid([]);
}

function loadIntegracaoGrupoTransportador() {
    _configuracaoIntegracao = new ConfiguracaoIntegracao();
    KoBindings(_configuracaoIntegracao, "knockoutConfiguracaoIntegracao");

    _cadastroSistemaIntegracao = new CadastroSistemaIntegracao();
    KoBindings(_cadastroSistemaIntegracao, "knockoutCadastroSistemaIntegracao");

    loadGridConfiguracaoSistemaIntegracao();
    buscarIntegracoes();
}

function buscarIntegracoes() {
    return new Promise(function (resolve) {
        executarReST("GrupoTransportador/BuscarTiposIntegracoes", {}, function (retorno) {
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
            exibirMensagem(tipoMensagem.aviso, `Sistema de integração: ${obterDescricaoSistemaIntegracao()} já cadastrado.`)
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

function limparCamposIntegracao() {
    _configuracaoIntegracao.ListaSistemaIntegracao.val([]);
    _gridConfiguracaoSistemaIntegracao.CarregarGrid([]);
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
    var listaSistemaIntegracaoRetornar = [];

    listaSistemaIntegracao.forEach(function (sistemaIntegracao) {
        listaSistemaIntegracaoRetornar.push({
            Codigo: sistemaIntegracao.Codigo,
            Tipo: sistemaIntegracao.Tipo
        });
    });

    return JSON.stringify(listaSistemaIntegracaoRetornar);
}

function obterConfiguracaoSistemaIntegracaoSalvar() {
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

// #endregion Funções Privadas
