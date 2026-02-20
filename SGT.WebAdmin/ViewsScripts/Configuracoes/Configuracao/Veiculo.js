/// <reference path="Configuracao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroSistemaIntegracaoVeiculo;
var _configuracaoVeiculo;
var _gridConfiguracaoVeiculoSistemaIntegracao;

/*
 * Declaração das Classes
 */

var CadastroSistemaIntegracaoVeiculo = function () {
    this.SistemaIntegracaoVeiculo = PropertyEntity({ text: "Sistema de integração para validação: ", val: ko.observable(""), options: EnumTipoIntegracao.obterOpcoesValidacaoMotoristaOuVeiculo(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoVeiculoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ConfiguracaoVeiculo = function () {
    this.ListaSistemaIntegracao = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaSistemaIntegracao.val.subscribe(function () {
        recarregarGridConfiguracaoVeiculoSistemaIntegracao();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoVeiculoModalClick, type: types.event, text: "Adicionar Integração para Validação", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridConfiguracaoVeiculoSistemaIntegracao() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerConfiguracaoSistemaIntegracaoVeiculo, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };
    var header = [
        { data: "Tipo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridConfiguracaoVeiculoSistemaIntegracao = new BasicDataTable(_configuracaoVeiculo.ListaSistemaIntegracao.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridConfiguracaoVeiculoSistemaIntegracao.CarregarGrid([]);
}

function loadConfiguracaoVeiculo() {
    $("#container-configuracao-veiculo").appendTo("#configuracao-veiculo");

    _configuracaoVeiculo = new ConfiguracaoVeiculo();
    KoBindings(_configuracaoVeiculo, "knockoutConfiguracaoVeiculo");

    _cadastroSistemaIntegracaoVeiculo = new CadastroSistemaIntegracaoVeiculo();
    KoBindings(_cadastroSistemaIntegracaoVeiculo, "knockoutCadastroSistemaIntegracaoVeiculo");

    loadGridConfiguracaoVeiculoSistemaIntegracao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarConfiguracaoVeiculoClick() {
    if (ValidarCamposObrigatorios(_cadastroSistemaIntegracaoVeiculo)) {
        if (isConfiguracaoSistemaIntegracaoVeiculoExistente())
            exibirMensagem(tipoMensagem.aviso, "Sistema de integração já existente", "O sistema de integração  " + obterDescricaoSistemaIntegracaoVeiculo() + " já está cadastrado.");
        else {
            _configuracaoVeiculo.ListaSistemaIntegracao.val().push(obterConfiguracaoSistemaIntegracaoVeiculoSalvar());

            recarregarGridConfiguracaoVeiculoSistemaIntegracao();
            fecharModalCadastroSistemaIntegracaoVeiculo();
        }
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function adicionarConfiguracaoVeiculoModalClick() {
    exibirModalCadastroSistemaIntegracaoVeiculo();
}

/*
 * Declaração das Funções Públicas
 */

function preencherConfiguracaoVeiculo(dadosConfiguracaoVeiculo) {
    _configuracaoVeiculo.ListaSistemaIntegracao.val(dadosConfiguracaoVeiculo.SistemasIntegracaoVeiculo);
}

function preencherConfiguracaoVeiculoSalvar(configuracao) {
    configuracao["SistemasIntegracaoVeiculo"] = obterListaSistemaIntegracaoVeiculoSalvar();
}

/*
 * Declaração das Funções
 */

function exibirModalCadastroSistemaIntegracaoVeiculo() {
    Global.abrirModal('divModalCadastroSistemaIntegracaoVeiculo');
    $("#divModalCadastroSistemaIntegracaoVeiculo").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroSistemaIntegracaoVeiculo);
    });
}

function fecharModalCadastroSistemaIntegracaoVeiculo() {
    Global.fecharModal('divModalCadastroSistemaIntegracaoVeiculo');
}

function isConfiguracaoSistemaIntegracaoVeiculoExistente() {
    var listaSistemaIntegracao = obterListaSistemaIntegracaoVeiculo();
    var tipo = _cadastroSistemaIntegracaoVeiculo.SistemaIntegracaoVeiculo.val();

    for (var i = 0; i < listaSistemaIntegracao.length; i++) {
        if (listaSistemaIntegracao[i].Tipo == tipo)
            return true;
    }

    return false;
}

function obterConfiguracaoSistemaIntegracaoVeiculoSalvar() {
    return {
        Tipo: _cadastroSistemaIntegracaoVeiculo.SistemaIntegracaoVeiculo.val(),
        Descricao: obterDescricaoSistemaIntegracaoVeiculo()
    };
}

function obterDescricaoSistemaIntegracaoVeiculo() {
    var tipo = _cadastroSistemaIntegracaoVeiculo.SistemaIntegracaoVeiculo.val();

    for (var i = 0; i < _cadastroSistemaIntegracaoVeiculo.SistemaIntegracaoVeiculo.options.length; i++) {
        if (_cadastroSistemaIntegracaoVeiculo.SistemaIntegracaoVeiculo.options[i].value == tipo) {
            return _cadastroSistemaIntegracaoVeiculo.SistemaIntegracaoVeiculo.options[i].text;
        }
    }

    return "";
}

function obterListaSistemaIntegracaoVeiculo() {
    return _configuracaoVeiculo.ListaSistemaIntegracao.val().slice();
}

function obterListaSistemaIntegracaoVeiculoSalvar() {
    var listaSistemaIntegracao = obterListaSistemaIntegracaoVeiculo();
    var listaSistemaIntegracaoRetornar = new Array();

    listaSistemaIntegracao.forEach(function (sistemaIntegracao) {
        listaSistemaIntegracaoRetornar.push(sistemaIntegracao.Tipo);
    });

    return JSON.stringify(listaSistemaIntegracaoRetornar);
}

function recarregarGridConfiguracaoVeiculoSistemaIntegracao() {
    var listaSistemaIntegracao = obterListaSistemaIntegracaoVeiculo();

    _gridConfiguracaoVeiculoSistemaIntegracao.CarregarGrid(listaSistemaIntegracao);
}

function removerConfiguracaoSistemaIntegracaoVeiculo(registroSelecionado) {
    var listaSistemaIntegracao = obterListaSistemaIntegracaoVeiculo();

    listaSistemaIntegracao.forEach(function (sistemaIntegracao, i) {
        if (registroSelecionado.Tipo == sistemaIntegracao.Tipo) {
            listaSistemaIntegracao.splice(i, 1);
        }
    });

    _configuracaoVeiculo.ListaSistemaIntegracao.val(listaSistemaIntegracao);
}