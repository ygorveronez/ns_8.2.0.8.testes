/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumStatusLicenca.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroGrupoBonificacaoVigencia;
var _CRUDcadastroGrupoBonificacaoVigencia;
var _gridGrupoBonificacaoVigencia;
var _grupoBonificacaoVigencia;

/*
 * Declaração das Classes
 */

var CRUDCadastroGrupoBonificacaoVigencia = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarGrupoBonificacaoVigenciaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarGrupoBonificacaoVigenciaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirGrupoBonificacaoVigenciaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var CadastroGrupoBonificacaoVigencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "*Data Inicial", getType: typesKnockout.date, required: true, val: ko.observable("") });
    this.DataFinal = PropertyEntity({ text: "*Data Final", getType: typesKnockout.date, required: true, val: ko.observable("") });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Situacao = PropertyEntity({ text: "*Situação", options: EnumStatusLicenca.obterOpcoes(), val: ko.observable(EnumStatusLicenca.Vigente), required: true, def: EnumStatusLicenca.Vigente });
}

var GrupoBonificacaoVigencia = function () {
    this.Vigencia = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.Vigencia.val.subscribe(function () {
        recarregarGridGrupoBonificacaoVigencia();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarGrupoBonificacaoVigenciaModalClick, type: types.event, text: "Adicionar" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridGrupoBonificacaoVigencia() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 2, dir: orderDir.asc };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarGrupoBonificacaoVigenciaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "DataInicial", title: "Data Inicial", width: "30%", className: "text-align-left", orderable: false },
        { data: "DataFinal", title: "Data Final", width: "30%", className: "text-align-left", orderable: false },
        { data: "Situacao", title: "Situação", width: "30%", className: "text-align-left", orderable: false },
    ];

    _gridGrupoBonificacaoVigencia = new BasicDataTable(_grupoBonificacaoVigencia.Vigencia.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridGrupoBonificacaoVigencia.CarregarGrid([]);
}

function LoadGrupoBonificacaoVigencia() {
    _grupoBonificacaoVigencia = new GrupoBonificacaoVigencia();
    KoBindings(_grupoBonificacaoVigencia, "knockoutVigencia");

    _cadastroGrupoBonificacaoVigencia = new CadastroGrupoBonificacaoVigencia();
    KoBindings(_cadastroGrupoBonificacaoVigencia, "knockoutCadastroGrupoBonificacaoVigencia");

    _CRUDcadastroGrupoBonificacaoVigencia = new CRUDCadastroGrupoBonificacaoVigencia();
    KoBindings(_CRUDcadastroGrupoBonificacaoVigencia, "knockoutCRUDCadastroGrupoBonificacaoVigencia");

    loadGridGrupoBonificacaoVigencia();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarGrupoBonificacaoVigenciaClick() {
    if (ValidarCamposObrigatorios(_cadastroGrupoBonificacaoVigencia)) {
        if (validarVigencia()) {
            _grupoBonificacaoVigencia.Vigencia.val().push(obterCadastroGrupoBonificacaoVigenciaSalvar());

            recarregarGridGrupoBonificacaoVigencia();
            fecharModalCadastroGrupoBonificacaoVigencia();
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Atenção", "Uma vigênca já foi cadastrada com estas datas.");
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios");
}

function adicionarGrupoBonificacaoVigenciaModalClick() {
    _cadastroGrupoBonificacaoVigencia.Codigo.val(guid());

    controlarBotoesCadastroGrupoBonificacaoVigenciaHabilitados(false);
    controlarCamposCadastroGrupoBonificacaoVigenciaHabilitados(false);

    exibirModalCadastroGrupoBonificacaoVigencia();
}

function atualizarGrupoBonificacaoVigenciaClick() {
    if (ValidarCamposObrigatorios(_cadastroGrupoBonificacaoVigencia)) {
        if (validarVigencia()) {
            var vigencias = obterVigencia();

            vigencias.forEach(function (vigencia, i) {
                if (_cadastroGrupoBonificacaoVigencia.Codigo.val() == vigencia.Codigo) {
                    vigencias.splice(i, 1, obterCadastroGrupoBonificacaoVigenciaSalvar());
                }
            });

            _grupoBonificacaoVigencia.Vigencia.val(vigencias);

            recarregarGridGrupoBonificacaoVigencia();
            fecharModalCadastroGrupoBonificacaoVigencia();
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Atenção", "Uma vigênca já foi cadastrada com estas datas.");
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios");
}

function editarGrupoBonificacaoVigenciaClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroGrupoBonificacaoVigencia, { Data: registroSelecionado });

    _cadastroGrupoBonificacaoVigencia.DataInicial.val(registroSelecionado.DataInicial);
    _cadastroGrupoBonificacaoVigencia.DataFinal.val(registroSelecionado.DataFinal);
    _cadastroGrupoBonificacaoVigencia.Situacao.val(registroSelecionado.Situacao);

    controlarBotoesCadastroGrupoBonificacaoVigenciaHabilitados(true);
    controlarCamposCadastroGrupoBonificacaoVigenciaHabilitados(true);

    exibirModalCadastroGrupoBonificacaoVigencia();
}

function excluirGrupoBonificacaoVigenciaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "", function () {
        removerGrupoBonificacaoVigencia(_cadastroGrupoBonificacaoVigencia.Codigo.val());
        fecharModalCadastroGrupoBonificacaoVigencia();
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposGrupoBonificacaoVigencia() {
    LimparCampos(_grupoBonificacaoVigencia);

    _grupoBonificacaoVigencia.Vigencia.val([]);
}

function preencherGrupoBonificacaoVigenciaSalvar(_grupoBonificacao) {
    _grupoBonificacao["Vigencia"] = JSON.stringify(_grupoBonificacaoVigencia.Vigencia.val().slice());
}

function preencherGrupoBonificacaoVigencia(dadosVigencia) {
    PreencherObjetoKnout(_grupoBonificacaoVigencia, { Data: dadosVigencia });
    _grupoBonificacaoVigencia.Vigencia.val(dadosVigencia);
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroGrupoBonificacaoVigenciaHabilitados(isEdicao) {
    _CRUDcadastroGrupoBonificacaoVigencia.Adicionar.visible(!isEdicao);
    _CRUDcadastroGrupoBonificacaoVigencia.Atualizar.visible(isEdicao);
    _CRUDcadastroGrupoBonificacaoVigencia.Excluir.visible(isEdicao);
}

function controlarCamposCadastroGrupoBonificacaoVigenciaHabilitados(isEdicao) {
    //_cadastroGrupoBonificacaoVigencia.Regiao.enable(!isEdicao);
}

function exibirModalCadastroGrupoBonificacaoVigencia() {
    Global.abrirModal('divModalCadastroGrupoBonificacaoVigencia');
    $("#divModalCadastroGrupoBonificacaoVigencia").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroGrupoBonificacaoVigencia);
    });
}

function fecharModalCadastroGrupoBonificacaoVigencia() {
    Global.fecharModal('divModalCadastroGrupoBonificacaoVigencia');
}

function obterCadastroGrupoBonificacaoVigenciaSalvar() {
    return {
        Codigo: _cadastroGrupoBonificacaoVigencia.Codigo.val(),
        DataInicial: _cadastroGrupoBonificacaoVigencia.DataInicial.val(),
        DataFinal: _cadastroGrupoBonificacaoVigencia.DataFinal.val(),
        Situacao: _cadastroGrupoBonificacaoVigencia.Situacao.val()
    };
}

function obterVigencia() {
    return _grupoBonificacaoVigencia.Vigencia.val().slice();
}

function obterVigenciaSalvar() {
    var vigencia = obterVigencia();

    return JSON.stringify(vigencia);
}

function recarregarGridGrupoBonificacaoVigencia() {
    var vigencias = new Array();

    for (var i = 0; i < _grupoBonificacaoVigencia.Vigencia.val().length; i++) {
        var vigencia = _grupoBonificacaoVigencia.Vigencia.val()[i];
        vigencias.push({
            Codigo: vigencia.Codigo,
            DataInicial: vigencia.DataInicial,
            DataFinal: vigencia.DataFinal,
            Situacao: EnumStatusLicenca.obterDescricao(vigencia.Situacao)
        });
    }

    _gridGrupoBonificacaoVigencia.CarregarGrid(vigencias);
}

function removerGrupoBonificacaoVigencia(codigo) {
    var vigencias = obterVigencia();
    vigencias.forEach(function (vigencia, i) {
        if (codigo == vigencia.Codigo)
            vigencias.splice(i, 1);
    });

    _grupoBonificacaoVigencia.Vigencia.val(vigencias);
}

function validarVigencia() {
    var vigencias = obterVigencia();

    var dataInicial = moment(_cadastroGrupoBonificacaoVigencia.DataInicial.val(), "DD/MM/YYYY");
    var dataFinal = _cadastroGrupoBonificacaoVigencia.DataFinal.val() != "" ? moment(_cadastroGrupoBonificacaoVigencia.DataFinal.val(), "DD/MM/YYYY") : "";

    for (var i = 0; i < vigencias.length; i++) {
        var vigencia = vigencias[i];

        if (vigencia.Codigo != _cadastroGrupoBonificacaoVigencia.Codigo.val()) {
            var dataInicialCadastrada = moment(vigencia.DataInicial, "DD/MM/YYYY");
            var dataFinalCadastrada = vigencia.DataFinal ? moment(vigencia.DataFinal, "DD/MM/YYYY") : "";

            if (!validarDatasVigencia(dataInicial, dataFinal, dataInicialCadastrada, dataFinalCadastrada)) {
                return false;
            }
        }
    }

    return true;
}

function validarDatasVigencia(dataInicial, dataFinal, dataInicialCadastrada, dataFinalCadastrada) {
    if (dataFinal && dataFinalCadastrada) {
        return !(
            dataInicial.isBetween(dataInicialCadastrada, dataFinalCadastrada) ||
            dataFinal.isBetween(dataInicialCadastrada, dataFinalCadastrada) ||
            dataInicialCadastrada.isBetween(dataInicial, dataFinal) ||
            dataFinalCadastrada.isBetween(dataInicial, dataFinal) ||
            dataInicial.isSame(dataInicialCadastrada) ||
            dataInicial.isSame(dataFinalCadastrada) ||
            dataFinal.isSame(dataInicialCadastrada) ||
            dataFinal.isSame(dataFinalCadastrada)
        );
    }

    if (!dataFinal && !dataFinalCadastrada)
        return false;

    if (!dataFinal && dataInicialCadastrada.isAfter(dataInicial))
        return false;

    if (!dataFinalCadastrada && dataInicial.isAfter(dataInicialCadastrada))
        return false;

    return true;
}