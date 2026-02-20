/// <reference path="GrupoPessoas.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridGrupoPessoaObservacaoNfeFormula;
var _grupoPessoaObservacaoNfeFormula;
var _grupoPessoaObservacaoNfe;

/*
 * Declaração das Classes
 */

var GrupoPessoaObservacaoNfe = function () {
    this.Observacao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Observacao.getFieldDescription(), maxlength: 2000, enable: ko.observable(true) });
    this.ListaFormula = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), enable: ko.observable(true)  });

    this.ListaFormula.val.subscribe(function () {
        recarregarGridObservacaoNfeFormula();
    });

    this.Observacao.val.subscribe(function (observacao) {
        _grupoPessoas.ObservacaoNfe.val(observacao);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarObservacaoNfeFormulaModalClick, type: types.event, text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.AdicionarFormula), visible: ko.observable(true), enable: ko.observable(true)  });
}

var GrupoPessoaObservacaoNfeFormula = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.string });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Descricao.getRequiredFieldDescription(), required: true, maxlength: 200, enable: ko.observable(true) });
    this.IdentificadorInicio = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificadorInicio.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.IdentificadorFim = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificadorFim.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.QtdMinimoDigitos = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.QuantidadeMinimoDigitos.getFieldDescription(), maxlength: 100, getType: typesKnockout.int, enable: ko.observable(true) });
    this.QtdMaximoDigitos = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.QuantidadeMaximoDigitos.getFieldDescription(), maxlength: 100, getType: typesKnockout.int, enable: ko.observable(true) });
    this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.NumeroPedido.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.Tag = PropertyEntity({ text: "*Tag: ", required: true, maxlength: 100, enable: ko.observable(true) });
    this.NumeroContainer = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Container.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.TaraContainer = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.TaraContainer.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.LacreContainerUm = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.LacreContainer1.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.LacreContainerDois = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.LacreContainer2.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.LacreContainerTres = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.LacreContainer3.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.NumeroControleCliente = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.NumeroControleCliente.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.NumeroControlePedido = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.NumeroControlePedido.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.NumeroReferenciaEDI = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.NumeroReferenciaEDI.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.SubstituicaoTributaria = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.SubstituicaoTributaria.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarObservacaoNfeFormulaClick, type: types.event, text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.Adicionar), enable: ko.observable(true), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarObservacaoNfeFormulaClick, type: types.event, text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.Atualizar), visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridGrupoPessoaObservacaoNfeFormula() {
    var linhasPorPaginas = 5;
    var opcaoAdicionarTagObservacao = { descricao: Localization.Resources.Pessoas.GrupoPessoas.AdicionarNaObservacao, id: guid(), metodo: adicionarTagObservacaoNfeClick, icone: "" };
    var opcaoRemover = { descricao: Localization.Resources.Pessoas.GrupoPessoas.Remover, id: guid(), metodo: removerObservacaoNfeFormulaClick, icone: "" };
    var opcaoEditar = { descricao: Localization.Resources.Pessoas.GrupoPessoas.Editar, id: guid(), metodo: editarObservacaoNfeFormulaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Pessoas.GrupoPessoas.Opcoes, tamanho: 15, opcoes: [opcaoAdicionarTagObservacao, opcaoEditar, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "NumeroPedido", visible: false },
        { data: "NumeroContainer", visible: false },
        { data: "TaraContainer", visible: false },
        { data: "LacreContainerUm", visible: false },
        { data: "LacreContainerDois", visible: false },
        { data: "LacreContainerTres", visible: false },
        { data: "NumeroControleCliente", visible: false },
        { data: "NumeroControlePedido", visible: false },
        { data: "NumeroReferenciaEDI", visible: false },
        { data: "SubstituicaoTributaria", visible: false },
        { data: "QtdMinimoDigitos", visible: false },
        { data: "QtdMaximoDigitos", visible: false },
        { data: "Tag", title: "Tag", width: "20%", className: "text-align-left" },
        { data: "Descricao", title: Localization.Resources.Pessoas.GrupoPessoas.Descricao, width: "30%", className: "text-align-left" },
        { data: "IdentificadorInicio", title: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoInicio, width: "25%", className: "text-align-left" },
        { data: "IdentificadorFim", title: Localization.Resources.Pessoas.GrupoPessoas.IdentificadorFim, width: "25%", className: "text-align-left" }
    ];

    _gridGrupoPessoaObservacaoNfeFormula = new BasicDataTable(_grupoPessoaObservacaoNfe.ListaFormula.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridGrupoPessoaObservacaoNfeFormula.CarregarGrid([]);
}

function loadGrupoPessoaObservacaoNfe() {
    _grupoPessoaObservacaoNfe = new GrupoPessoaObservacaoNfe();
    KoBindings(_grupoPessoaObservacaoNfe, "knockoutObservacaoNfe");

    _grupoPessoaObservacaoNfeFormula = new GrupoPessoaObservacaoNfeFormula();
    KoBindings(_grupoPessoaObservacaoNfeFormula, "knockoutObservacaoNfeFormula");

    loadGridGrupoPessoaObservacaoNfeFormula();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarObservacaoNfeFormulaClick() {
    adicionarObservacaoNfeFormula();
}

function atualizarObservacaoNfeFormulaClick() {
    atualizarObservacaoNfeFormula();
}

function adicionarObservacaoNfeFormulaModalClick() {
    _grupoPessoaObservacaoNfeFormula.Codigo.val(guid());

    exibirModalObservacaoNfeFormula();
}

function adicionarTagObservacaoNfeClick(registroSelecionado) {
    _grupoPessoaObservacaoNfe.Observacao.val(_grupoPessoaObservacaoNfe.Observacao.val() + "#" + registroSelecionado.Tag);
}

function removerObservacaoNfeFormulaClick(registroSelecionado) {
    var listaFormula = obterListaObservacaoNfeFormula();

    listaFormula.forEach(function (formula, i) {
        if (registroSelecionado.Codigo == formula.Codigo) {
            listaFormula.splice(i, 1);
        }
    });

    _grupoPessoaObservacaoNfe.ListaFormula.val(listaFormula);
}

function editarObservacaoNfeFormulaClick(registroSelecionado) {
    PreencherObjetoKnout(_grupoPessoaObservacaoNfeFormula, { Data: registroSelecionado });
    _grupoPessoaObservacaoNfeFormula.Atualizar.visible(true);
    _grupoPessoaObservacaoNfeFormula.Adicionar.visible(false);
    exibirModalObservacaoNfeFormula();
}

/*
 * Declaração das Funções
 */

function adicionarObservacaoNfeFormula() {
    if (ValidarCamposObrigatorios(_grupoPessoaObservacaoNfeFormula)) {
        var listaFormula = obterListaObservacaoNfeFormula();
        var formulaAdicionar = RetornarObjetoPesquisa(_grupoPessoaObservacaoNfeFormula);

        if (isExisteCaracterEspacoTagInformada(formulaAdicionar))
            exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.TagInvalida, Localization.Resources.Pessoas.GrupoPessoas.TagNaoPodeConterEspacos);
        else if (isExisteFormulaCadastrada(listaFormula, formulaAdicionar))
            exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.FormulaExistente, Localization.Resources.Pessoas.GrupoPessoas.FormulaTagJaCadastrada);
        else {
            listaFormula.push(formulaAdicionar);

            _grupoPessoaObservacaoNfe.ListaFormula.val(listaFormula);

            fecharModalObservacaoNfeFormula();
        }
    }
    else
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
}

function atualizarObservacaoNfeFormula() {
    if (ValidarCamposObrigatorios(_grupoPessoaObservacaoNfeFormula)) {
        var formulaAdicionar = RetornarObjetoPesquisa(_grupoPessoaObservacaoNfeFormula);
        var listaFormula = obterListaObservacaoNfeFormula();

        if (isExisteCaracterEspacoTagInformada(formulaAdicionar))
            exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.TagInvalida, Localization.Resources.Pessoas.GrupoPessoas.TagNaoPodeConterEspacos);
        else {
            for (var i = 0; i < listaFormula.length; i++) {
                formulaSelecionada = listaFormula[i];
                if (formulaSelecionada.Codigo == _grupoPessoaObservacaoNfeFormula.Codigo.val()) {
                    _grupoPessoaObservacaoNfe.ListaFormula.val()[i] = formulaAdicionar;
                }
            }

            listaFormula = obterListaObservacaoNfeFormula();
            _grupoPessoaObservacaoNfe.ListaFormula.val(listaFormula);

            fecharModalObservacaoNfeFormula();
        }
    }
    else
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
}

function exibirModalObservacaoNfeFormula() {
    Global.abrirModal('divModalObservacaoNfeFormula');
    $("#divModalObservacaoNfeFormula").one('hidden.bs.modal', function () {
        LimparCampos(_grupoPessoaObservacaoNfeFormula);
        _grupoPessoaObservacaoNfeFormula.Atualizar.visible(false);
        _grupoPessoaObservacaoNfeFormula.Adicionar.visible(true);
    });
}

function fecharModalObservacaoNfeFormula() {
    Global.fecharModal('divModalObservacaoNfeFormula');
}

function isExisteCaracterEspacoTagInformada(formulaAdicionar) {
    return formulaAdicionar.Tag.indexOf(" ") !== -1
}

function isExisteFormulaCadastrada(listaFormula, formulaAdicionar) {
    for (var i = 0; i < listaFormula.length; i++) {
        var formula = listaFormula[i];

        if (formulaAdicionar.Tag == formula.Tag)
            return true;
    }

    return false;
}

function limparCamposObservacaoNfe() {
    LimparCampos(_grupoPessoaObservacaoNfe);

    _grupoPessoaObservacaoNfe.ListaFormula.val(new Array());
}

function obterListaObservacaoNfeFormula() {
    return _grupoPessoaObservacaoNfe.ListaFormula.val().slice();
}

function obterListaObservacaoNfeFormulaSalvar() {
    var listaFormula = obterListaObservacaoNfeFormula();

    return JSON.stringify(listaFormula);
}

function preencherCampoObservacaoNfe() {
    _grupoPessoaObservacaoNfe.Observacao.val(_grupoPessoas.ObservacaoNfe.val());
    _grupoPessoaObservacaoNfe.ListaFormula.val(_grupoPessoas.FormulasObservacaoNfe.val());    
}

function recarregarGridObservacaoNfeFormula() {
    var listaFormula = obterListaObservacaoNfeFormula();

    _gridGrupoPessoaObservacaoNfeFormula.CarregarGrid(listaFormula);
}
