/// <reference path="Configuracao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroSistemaIntegracaoMotorista;
var _configuracaoMotorista;
var _gridConfiguracaoMotoristaSistemaIntegracao;

/*
 * Declaração das Classes
 */

var CadastroSistemaIntegracaoMotorista = function () {
    this.SistemaIntegracaoMotorista = PropertyEntity({ text: "Sistema de integração para validação: ", val: ko.observable(""), options: EnumTipoIntegracao.obterOpcoesValidacaoMotoristaOuVeiculo(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoMotoristaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ConfiguracaoMotorista = function () {
    this.JornadaDiariaMotorista = PropertyEntity({ text: "Jornada Diária:", val: ko.observable(""), def: "", getType: typesKnockout.time });
    this.UtilizarControleJornadaMotorista = PropertyEntity({ text: "Utilizar controle de jornada", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarFichaMotoristaTodos = PropertyEntity({ text: "Habilitar ficha do motorista para todos?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarComissaoPorCargo = PropertyEntity({ text: "Utilizar a comissão por cargo?", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.PercentualComissaoPadrao = PropertyEntity({ text: "% Comissão Padrão:", getType: typesKnockout.decimal, val: ko.observable(0) });
    this.PercentualMediaEquivalente = PropertyEntity({ text: "% Média Equivalente:", getType: typesKnockout.decimal, val: ko.observable(0) });
    this.PercentualEquivaleEquivalente = PropertyEntity({ text: "% Sinistro Equivalente:", getType: typesKnockout.decimal, val: ko.observable(0) });
    this.PercentualAdvertenciaEquivalente = PropertyEntity({ text: "% Advertência Equivalente:", getType: typesKnockout.decimal, val: ko.observable(0) });

    this.ListaSistemaIntegracao = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaSistemaIntegracao.val.subscribe(function () {
        recarregarGridConfiguracaoMotoristaSistemaIntegracao();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoMotoristaModalClick, type: types.event, text: "Adicionar Integração para Validação", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridConfiguracaoMotoristaSistemaIntegracao() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerConfiguracaoSistemaIntegracaoMotorista, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };
    var header = [
        { data: "Tipo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridConfiguracaoMotoristaSistemaIntegracao = new BasicDataTable(_configuracaoMotorista.ListaSistemaIntegracao.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridConfiguracaoMotoristaSistemaIntegracao.CarregarGrid([]);
}

function loadGridMotoristasIgnorar() {
    var excluirMotorista = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            excluirMotoristaIgnorado(data);
        }, tamanho: "10", icone: ""
    };
    var menuOpcoesMotorista = new Object();
    menuOpcoesMotorista.tipo = TypeOptionMenu.link;
    menuOpcoesMotorista.opcoes = new Array();
    menuOpcoesMotorista.opcoes.push(excluirMotorista);

    var headerMotoristaIgnorado = [
        { data: "NomeMotoristaIgnorado", title: "Nome Motorista", width: "70%" }
    ];

    _gridMotoristaBloqueado = new BasicDataTable(_configuracaoEmbarcador.GridMotoristasIgnorados.idGrid, headerMotoristaIgnorado, menuOpcoesMotorista);
    recarregarGridMotoristaIgnorado();
}


function loadConfiguracaoMotorista() {
    $("#container-configuracao-motorista").appendTo("#configuracao-motorista");

    _configuracaoMotorista = new ConfiguracaoMotorista();
    KoBindings(_configuracaoMotorista, "knockoutConfiguracaoMotorista");

    _cadastroSistemaIntegracaoMotorista = new CadastroSistemaIntegracaoMotorista();
    KoBindings(_cadastroSistemaIntegracaoMotorista, "knockoutCadastroSistemaIntegracaoMotorista");

    loadGridConfiguracaoMotoristaSistemaIntegracao();
    loadGridMotoristasIgnorar();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarConfiguracaoMotoristaClick() {
    if (ValidarCamposObrigatorios(_cadastroSistemaIntegracaoMotorista)) {
        if (isConfiguracaoSistemaIntegracaoMotoristaExistente())
            exibirMensagem(tipoMensagem.aviso, "Sistema de integração já existente", "O sistema de integração  " + obterDescricaoSistemaIntegracaoMotorista() + " já está cadastrado.");
        else {
            _configuracaoMotorista.ListaSistemaIntegracao.val().push(obterConfiguracaoSistemaIntegracaoMotoristaSalvar());

            recarregarGridConfiguracaoMotoristaSistemaIntegracao();
            fecharModalCadastroSistemaIntegracaoMotorista();
        }
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function adicionarConfiguracaoMotoristaModalClick() {
    exibirModalCadastroSistemaIntegracaoMotorista();
}

/*
 * Declaração das Funções Públicas
 */

function preencherConfiguracaoMotoristaSalvar(configuracao) {
    configuracao["JornadaDiariaMotorista"] = _configuracaoMotorista.JornadaDiariaMotorista.val();
    configuracao["PercentualComissaoPadrao"] = _configuracaoMotorista.PercentualComissaoPadrao.val();
    configuracao["UtilizarComissaoPorCargo"] = _configuracaoMotorista.UtilizarComissaoPorCargo.val();
    configuracao["PercentualMediaEquivalente"] = _configuracaoMotorista.PercentualMediaEquivalente.val();
    configuracao["PercentualEquivaleEquivalente"] = _configuracaoMotorista.PercentualEquivaleEquivalente.val();
    configuracao["PercentualAdvertenciaEquivalente"] = _configuracaoMotorista.PercentualAdvertenciaEquivalente.val();
    configuracao["UtilizarControleJornadaMotorista"] = _configuracaoMotorista.UtilizarControleJornadaMotorista.val();
    configuracao["HabilitarFichaMotoristaTodos"] = _configuracaoMotorista.HabilitarFichaMotoristaTodos.val();
    configuracao["SistemasIntegracaoMotorista"] = obterListaSistemaIntegracaoMotoristaSalvar();
    configuracao["MotoristasIgnorados"] = obterListaMotoristasIgnorarSalvar();
}

function preencherConfiguracaoMotorista(dadosConfiguracaoMotorista) {
    _configuracaoMotorista.JornadaDiariaMotorista.val(dadosConfiguracaoMotorista.JornadaDiariaMotorista);
    _configuracaoMotorista.UtilizarComissaoPorCargo.val(dadosConfiguracaoMotorista.UtilizarComissaoPorCargo);
    _configuracaoMotorista.PercentualComissaoPadrao.val(dadosConfiguracaoMotorista.PercentualComissaoPadrao);
    _configuracaoMotorista.PercentualMediaEquivalente.val(dadosConfiguracaoMotorista.PercentualMediaEquivalente);
    _configuracaoMotorista.PercentualEquivaleEquivalente.val(dadosConfiguracaoMotorista.PercentualEquivaleEquivalente);
    _configuracaoMotorista.PercentualAdvertenciaEquivalente.val(dadosConfiguracaoMotorista.PercentualAdvertenciaEquivalente);
    _configuracaoMotorista.UtilizarControleJornadaMotorista.val(dadosConfiguracaoMotorista.UtilizarControleJornadaMotorista);
    _configuracaoMotorista.HabilitarFichaMotoristaTodos.val(dadosConfiguracaoMotorista.HabilitarFichaMotoristaTodos);
    _configuracaoMotorista.ListaSistemaIntegracao.val(dadosConfiguracaoMotorista.SistemasIntegracaoMotorista);
}

function preencherConfiguracaoMotoristaIgnorados(dadosConfiguracao) {
    _configuracaoEmbarcador.GridMotoristasIgnorados.list = new Array();
    
    $.each(dadosConfiguracao.MotoristasIgnorados, function (i, dest) {
        var map = new Object();
        map.NomeMotoristaIgnorado = dest.NomeMotoristaIgnorado;
        _configuracaoEmbarcador.GridMotoristasIgnorados.list.push(map);
    });

    recarregarGridMotoristaIgnorado();
}

/*
 * Declaração das Funções
 */

function exibirModalCadastroSistemaIntegracaoMotorista() {
    Global.abrirModal('divModalCadastroSistemaIntegracaoMotorista');
    $("#divModalCadastroSistemaIntegracaoMotorista").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroSistemaIntegracaoMotorista);
    });
}

function fecharModalCadastroSistemaIntegracaoMotorista() {
    Global.fecharModal('divModalCadastroSistemaIntegracaoMotorista');
}

function isConfiguracaoSistemaIntegracaoMotoristaExistente() {
    var listaSistemaIntegracao = obterListaSistemaIntegracaoMotorista();
    var tipo = _cadastroSistemaIntegracaoMotorista.SistemaIntegracaoMotorista.val();

    for (var i = 0; i < listaSistemaIntegracao.length; i++) {
        if (listaSistemaIntegracao[i].Tipo == tipo)
            return true;
    }

    return false;
}

function obterConfiguracaoSistemaIntegracaoMotoristaSalvar() {
    return {
        Tipo: _cadastroSistemaIntegracaoMotorista.SistemaIntegracaoMotorista.val(),
        Descricao: obterDescricaoSistemaIntegracaoMotorista()
    };
}

function obterDescricaoSistemaIntegracaoMotorista() {
    var tipo = _cadastroSistemaIntegracaoMotorista.SistemaIntegracaoMotorista.val();

    for (var i = 0; i < _cadastroSistemaIntegracaoMotorista.SistemaIntegracaoMotorista.options.length; i++) {
        if (_cadastroSistemaIntegracaoMotorista.SistemaIntegracaoMotorista.options[i].value == tipo) {
            return _cadastroSistemaIntegracaoMotorista.SistemaIntegracaoMotorista.options[i].text;
        }
    }

    return "";
}

function obterListaSistemaIntegracaoMotorista() {
    return _configuracaoMotorista.ListaSistemaIntegracao.val().slice();
}

function obterListaSistemaIntegracaoMotoristaSalvar() {
    var listaSistemaIntegracao = obterListaSistemaIntegracaoMotorista();
    var listaSistemaIntegracaoRetornar = new Array();

    listaSistemaIntegracao.forEach(function (sistemaIntegracao) {
        listaSistemaIntegracaoRetornar.push(sistemaIntegracao.Tipo);
    });

    return JSON.stringify(listaSistemaIntegracaoRetornar);
}

function obterListaMotoristasIgnorarSalvar() {
    var listaMotoristaIgnorar = _configuracaoEmbarcador.GridMotoristasIgnorados.list;
    var listaMotoristaIgnorarRetornar = new Array();

    listaMotoristaIgnorar.forEach(function (motoristaIgnorar) {
        listaMotoristaIgnorarRetornar.push(motoristaIgnorar);
    });

    return JSON.stringify(listaMotoristaIgnorarRetornar);
}

function recarregarGridConfiguracaoMotoristaSistemaIntegracao() {
    var listaSistemaIntegracao = obterListaSistemaIntegracaoMotorista();

    _gridConfiguracaoMotoristaSistemaIntegracao.CarregarGrid(listaSistemaIntegracao);
}

function removerConfiguracaoSistemaIntegracaoMotorista(registroSelecionado) {
    var listaSistemaIntegracao = obterListaSistemaIntegracaoMotorista();

    listaSistemaIntegracao.forEach(function (sistemaIntegracao, i) {
        if (registroSelecionado.Tipo == sistemaIntegracao.Tipo) {
            listaSistemaIntegracao.splice(i, 1);
        }
    });

    _configuracaoMotorista.ListaSistemaIntegracao.val(listaSistemaIntegracao);
}

function AdicionarMotoristaIgnoradoClick(e, sender) {
    var tudoCerto = true;
    if (_configuracaoEmbarcador.NomeMotoristaIgnorado.val() === "")
        tudoCerto = false;

    $.each(_configuracaoEmbarcador.GridMotoristasIgnorados.list, function (i, motorist) {
        if (motorist.NomeMotoristaIgnorado === _configuracaoEmbarcador.NomeMotoristaIgnorado.val())
            tudoCerto = false;
    });

    if (tudoCerto) {
        var map = new Object();

        map.NomeMotoristaIgnorado = _configuracaoEmbarcador.NomeMotoristaIgnorado.val();

        _configuracaoEmbarcador.GridMotoristasIgnorados.list.push(map);

        recarregarGridMotoristaIgnorado();
        _configuracaoEmbarcador.NomeMotoristaIgnorado.val("");

        $("#" + _configuracaoEmbarcador.NomeMotoristaIgnorado.id).focus();
    }
}

function recarregarGridMotoristaIgnorado() {
    var data = new Array();
    $.each(_configuracaoEmbarcador.GridMotoristasIgnorados.list, function (i, dest) {
        var obj = new Object();

        obj.NomeMotoristaIgnorado = dest.NomeMotoristaIgnorado;

        data.push(obj);
    });

    _gridMotoristaBloqueado.CarregarGrid(data);
}


function excluirMotoristaIgnorado(e) {

    $.each(_configuracaoEmbarcador.GridMotoristasIgnorados.list, function (i, motorist) {
        if (motorist != null && motorist.NomeMotoristaIgnorado != null && e != null && e.NomeMotoristaIgnorado != null && e.NomeMotoristaIgnorado == motorist.NomeMotoristaIgnorado)
            _configuracaoEmbarcador.GridMotoristasIgnorados.list.splice(i, 1);
    });
    recarregarGridMotoristaIgnorado();
}
