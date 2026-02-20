/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Regiao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroGrupoBonificacaoMeta;
var _CRUDcadastroGrupoBonificacaoMeta;
var _gridGrupoBonificacaoMeta;
var _grupoBonificacaoMeta;

/*
 * Declaração das Classes
 */

var CRUDCadastroGrupoBonificacaoMeta = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarGrupoBonificacaoMetaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarGrupoBonificacaoMetaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirGrupoBonificacaoMetaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var CadastroGrupoBonificacaoMeta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regiao = PropertyEntity({ text: "Região", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.QuantidadeCargasIdaPrevista = PropertyEntity({ text: "Quantidade Cargas Ida Prevista", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: true, thousands: "" }, required: true, val: ko.observable(0), def: 0 });
    this.QuantidadeCargasIdaRealizada = PropertyEntity({ text: "Quantidade Cargas Ida Realizada", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: true, thousands: "" }, required: true, val: ko.observable(0), def: 0 });
    this.QuantidadeCargasRetornoPrevista = PropertyEntity({ text: "Quantidade Cargas Retorno Prevista", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: true, thousands: "" }, required: true, val: ko.observable(0), def: 0 });
    this.QuantidadeCargasRetornoRealizada = PropertyEntity({ text: "Quantidade Cargas Retorno Realizada", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: true, thousands: "" }, required: true, val: ko.observable(0), def: 0 });
}

var GrupoBonificacaoMeta = function () {
    this.Metas = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.Metas.val.subscribe(function () {
        recarregarGridGrupoBonificacaoMeta();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarGrupoBonificacaoMetaModalClick, type: types.event, text: "Adicionar" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridGrupoBonificacaoMeta() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 2, dir: orderDir.asc };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarGrupoBonificacaoMetaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoRegiao", visible: false },
        { data: "Regiao", title: "Região", width: "40%", className: "text-align-left", orderable: false },
        { data: "QuantidadeCargasIdaPrevista", title: "Qtd. Cargas Ida Prevista", width: "15%", className: "text-align-left", orderable: false },
        { data: "QuantidadeCargasIdaRealizada", title: "Qtd. Cargas Ida Realizada", width: "15%", className: "text-align-left", orderable: false },
        { data: "QuantidadeCargasRetornoPrevista", title: "Qtd. Cargas Retorno Prevista", width: "15%", className: "text-align-left", orderable: false },
        { data: "QuantidadeCargasRetornoRealizada", title: "Qtd. Cargas Retorno Realizada", width: "15%", className: "text-align-left", orderable: false }
    ];

    _gridGrupoBonificacaoMeta = new BasicDataTable(_grupoBonificacaoMeta.Metas.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridGrupoBonificacaoMeta.CarregarGrid([]);
}

function LoadGrupoBonificacaoMeta() {
    _grupoBonificacaoMeta = new GrupoBonificacaoMeta();
    KoBindings(_grupoBonificacaoMeta, "knockoutMetas");

    _cadastroGrupoBonificacaoMeta = new CadastroGrupoBonificacaoMeta();
    KoBindings(_cadastroGrupoBonificacaoMeta, "knockoutCadastroGrupoBonificacaoMeta");

    _CRUDcadastroGrupoBonificacaoMeta = new CRUDCadastroGrupoBonificacaoMeta();
    KoBindings(_CRUDcadastroGrupoBonificacaoMeta, "knockoutCRUDCadastroGrupoBonificacaoMeta");

    new BuscarRegioes(_cadastroGrupoBonificacaoMeta.Regiao);

    loadGridGrupoBonificacaoMeta();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarGrupoBonificacaoMetaClick() {
    if (ValidarCamposObrigatorios(_cadastroGrupoBonificacaoMeta)) {
        if (!isMetaCadastrada()) {
            _grupoBonificacaoMeta.Metas.val().push(obterCadastroGrupoBonificacaoMetaSalvar());

            recarregarGridGrupoBonificacaoMeta();
            fecharModalCadastroGrupoBonificacaoMeta();
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Atenção", "A meta desta região já está cadastrada.");
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios");
}

function adicionarGrupoBonificacaoMetaModalClick() {
    _cadastroGrupoBonificacaoMeta.Codigo.val(guid());

    controlarBotoesCadastroGrupoBonificacaoMetaHabilitados(false);
    controlarCamposCadastroGrupoBonificacaoMetaHabilitados(false);

    exibirModalCadastroGrupoBonificacaoMeta();
}

function atualizarGrupoBonificacaoMetaClick() {
    if (ValidarCamposObrigatorios(_cadastroGrupoBonificacaoMeta)) {
        if (!isMetaCadastrada()) {
            var metas = obterMetas();

            metas.forEach(function (meta, i) {
                if (_cadastroGrupoBonificacaoMeta.Codigo.val() == meta.Codigo) {
                    metas.splice(i, 1, obterCadastroGrupoBonificacaoMetaSalvar());
                }
            });

            _grupoBonificacaoMeta.Metas.val(metas);

            recarregarGridGrupoBonificacaoMeta();
            fecharModalCadastroGrupoBonificacaoMeta();
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Atenção", "A meta desta região já está cadastrada.");
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios");
}

function editarGrupoBonificacaoMetaClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroGrupoBonificacaoMeta, { Data: registroSelecionado });

    _cadastroGrupoBonificacaoMeta.Regiao.codEntity(registroSelecionado.CodigoRegiao);
    _cadastroGrupoBonificacaoMeta.Regiao.val(registroSelecionado.Regiao);
    _cadastroGrupoBonificacaoMeta.Regiao.entityDescription(registroSelecionado.Regiao);
    _cadastroGrupoBonificacaoMeta.QuantidadeCargasIdaPrevista.val(registroSelecionado.QuantidadeCargasIdaPrevista);
    _cadastroGrupoBonificacaoMeta.QuantidadeCargasIdaRealizada.val(registroSelecionado.QuantidadeCargasIdaRealizada);
    _cadastroGrupoBonificacaoMeta.QuantidadeCargasRetornoPrevista.val(registroSelecionado.QuantidadeCargasRetornoPrevista);
    _cadastroGrupoBonificacaoMeta.QuantidadeCargasRetornoRealizada.val(registroSelecionado.QuantidadeCargasRetornoRealizada);

    controlarBotoesCadastroGrupoBonificacaoMetaHabilitados(true);
    controlarCamposCadastroGrupoBonificacaoMetaHabilitados(true);

    exibirModalCadastroGrupoBonificacaoMeta();
}

function excluirGrupoBonificacaoMetaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "", function () {
        removerGrupoBonificacaoMeta(_cadastroGrupoBonificacaoMeta.Codigo.val());
        fecharModalCadastroGrupoBonificacaoMeta();
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposGrupoBonificacaoMeta() {
    LimparCampos(_grupoBonificacaoMeta);

    _grupoBonificacaoMeta.Metas.val([]);
}

function preencherGrupoBonificacaoMetaSalvar(_grupoBonificacao) {
    _grupoBonificacao["Metas"] = JSON.stringify(_grupoBonificacaoMeta.Metas.val().slice());
}

function preencherGrupoBonificacaoMeta(dadosMeta) {
    PreencherObjetoKnout(_grupoBonificacaoMeta, { Data: dadosMeta });
    _grupoBonificacaoMeta.Metas.val(dadosMeta);
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroGrupoBonificacaoMetaHabilitados(isEdicao) {
    _CRUDcadastroGrupoBonificacaoMeta.Adicionar.visible(!isEdicao);
    _CRUDcadastroGrupoBonificacaoMeta.Atualizar.visible(isEdicao);
    _CRUDcadastroGrupoBonificacaoMeta.Excluir.visible(isEdicao);
}

function controlarCamposCadastroGrupoBonificacaoMetaHabilitados(isEdicao) {
    //_cadastroGrupoBonificacaoMeta.Regiao.enable(!isEdicao);
}

function exibirModalCadastroGrupoBonificacaoMeta() {
    Global.abrirModal('divModalCadastroGrupoBonificacaoMeta');
    $("#divModalCadastroGrupoBonificacaoMeta").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroGrupoBonificacaoMeta);
    });
}

function fecharModalCadastroGrupoBonificacaoMeta() {
    Global.fecharModal('divModalCadastroGrupoBonificacaoMeta');
}

function obterCadastroGrupoBonificacaoMetaSalvar() {
    return {
        Codigo: _cadastroGrupoBonificacaoMeta.Codigo.val(),
        CodigoRegiao: _cadastroGrupoBonificacaoMeta.Regiao.codEntity(),
        Regiao: _cadastroGrupoBonificacaoMeta.Regiao.val(),
        QuantidadeCargasIdaPrevista: _cadastroGrupoBonificacaoMeta.QuantidadeCargasIdaPrevista.val(),
        QuantidadeCargasIdaRealizada: _cadastroGrupoBonificacaoMeta.QuantidadeCargasIdaRealizada.val(),
        QuantidadeCargasRetornoPrevista: _cadastroGrupoBonificacaoMeta.QuantidadeCargasRetornoPrevista.val(),
        QuantidadeCargasRetornoRealizada: _cadastroGrupoBonificacaoMeta.QuantidadeCargasRetornoRealizada.val()
    };
}

function obterMetas() {
    return _grupoBonificacaoMeta.Metas.val().slice();
}

function obterMetaSalvar() {
    var metas = obterMetas();

    return JSON.stringify(metas);
}

function recarregarGridGrupoBonificacaoMeta() {
    var metas = obterMetas();

    _gridGrupoBonificacaoMeta.CarregarGrid(metas);
}

function removerGrupoBonificacaoMeta(codigo) {
    var metas = obterMetas();

    metas.forEach(function (meta, i) {
        if (codigo == meta.Codigo)
            metas.splice(i, 1);
    });

    _grupoBonificacaoMeta.Metas.val(metas);
}

function isMetaCadastrada() {
    var metas = obterMetas();
    var metaCadastrada = false;

    metas.forEach(function (meta, i) {
        if ((_cadastroGrupoBonificacaoMeta.Codigo.val() != meta.Codigo) && (_cadastroGrupoBonificacaoMeta.Regiao.codEntity() == meta.CodigoRegiao)) {
            metaCadastrada = true;
            return false;
        }
    });

    return metaCadastrada;
}