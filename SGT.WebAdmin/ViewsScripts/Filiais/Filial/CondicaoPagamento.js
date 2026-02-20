/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="../../Enumeradores/EnumTipoPrazoPagamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _diaMes = [
    { text: Localization.Resources.Gerais.Geral.SemConfiguracao, value: "" }
];

for (let i = 1; i <= 31; i++)
    _diaMes.push({ text: i, value: i })

var _cadastroCondicaoPagamento;
var _CRUDcadastroCondicaoPagamento;
var _gridCondicaoPagamento;
var _condicaoPagamento;

/*
 * Declaração das Classes
 */

var CRUDCadastroCondicaoPagamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarCondicaoPagamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarCondicaoPagamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirCondicaoPagamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var CadastroCondicaoPagamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiaEmissaoLimite = PropertyEntity({ val: ko.observable(0), options: _diaMes, def: "", text: Localization.Resources.Filiais.Filial.DiaEmissaoLimite.getFieldDescription(), required: false });
    this.DiaMes = PropertyEntity({ val: ko.observable(0), options: _diaMes, def: "", text: Localization.Resources.Filiais.Filial.DiaMes.getFieldDescription(), required: false });
    this.DiasDePrazoPagamento = PropertyEntity({ text: ko.observable(Localization.Resources.Filiais.Filial.DiasPrazoPagamento.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 4, getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.DiaSemana = PropertyEntity({ val: ko.observable(0), options: EnumDiaSemana.obterOpcoesSemConfiguracao(), def: 0, text: Localization.Resources.Filiais.Filial.DiaSemana.getFieldDescription(), required: false });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Filiais.Filial.TipoCarga.getFieldDescription(), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Filiais.Filial.TipoOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.TipoPrazoPagamento = PropertyEntity({ val: ko.observable(1), options: EnumTipoPrazoPagamento.obterOpcoes(), def: 1, text: Localization.Resources.Filiais.Filial.TipoPrazoPagamento.getFieldDescription(), required: false });
    this.VencimentoForaMes = PropertyEntity({ text: Localization.Resources.Filiais.Filial.VencimentoForaMes, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsiderarDiaUtilVencimento = PropertyEntity({ text: Localization.Resources.Filiais.Filial.ConsiderarDiaUtilVencimento, getType: typesKnockout.bool, val: ko.observable(false), def: false });
}

var CondicaoPagamento = function () {
    this.AtivarCondicao = PropertyEntity({ text: Localization.Resources.Filiais.Filial.AtivarCondicaoPagamento, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ListaCondicaoPagamento = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaCondicaoPagamento.val.subscribe(function () {
        recarregarGridCondicaoPagamento();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCondicaoPagamentoModalClick, type: types.event, text: Localization.Resources.Filiais.Filial.AdicionarCondicaoPagamento });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridCondicaoPagamento() {
    const linhasPorPaginas = 5;
    const ordenacao = { column: 3, dir: orderDir.asc };
    const opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarCondicaoPagamentoClick, icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoCarga", visible: false },
        { data: "CodigoTipoOperacao", visible: false },
        { data: "DiaEmissaoLimite", visible: false },
        { data: "DiaMes", visible: false },
        { data: "DiaSemana", visible: false },
        { data: "TipoPrazoPagamento", visible: false },
        { data: "VencimentoForaMes", visible: false },
        { data: "DiaSemanaDescricao", title: Localization.Resources.Filiais.Filial.DiaSemana, width: "10%", className: "text-align-center", orderable: false },
        { data: "DiaMesDescricao", title: Localization.Resources.Filiais.Filial.DiaMes, width: "10%", className: "text-align-center", orderable: false },
        { data: "DiaEmissaoLimiteDescricao", title: Localization.Resources.Filiais.Filial.DiaEmissaoLimite, width: "10%", className: "text-align-center", orderable: false },
        { data: "VencimentoForaMesDescricao", title: Localization.Resources.Filiais.Filial.VencimentoForaMes, width: "10%", className: "text-align-center", orderable: false },
        { data: "TipoPrazoPagamentoDescricao", title: Localization.Resources.Filiais.Filial.TipoPrazoPagamento, width: "10%", className: "text-align-center", orderable: false },
        { data: "DiasDePrazoPagamento", title: Localization.Resources.Filiais.Filial.DiasPrazoPagamento, width: "10%", className: "text-align-center", orderable: false },
        { data: "DescricaoTipoCarga", title: Localization.Resources.Filiais.Filial.TipoCarga, width: "10%", className: "text-align-left", orderable: false },
        { data: "DescricaoTipoOperacao", title: Localization.Resources.Filiais.Filial.TipoOperacao, width: "10%", className: "text-align-left", orderable: false }
    ];

    _gridCondicaoPagamento = new BasicDataTable(_condicaoPagamento.ListaCondicaoPagamento.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridCondicaoPagamento.CarregarGrid([]);
}

function loadCondicaoPagamento() {
    _condicaoPagamento = new CondicaoPagamento();
    KoBindings(_condicaoPagamento, "knockoutCondicaoPagamento");

    _cadastroCondicaoPagamento = new CadastroCondicaoPagamento();
    KoBindings(_cadastroCondicaoPagamento, "knockoutCadastroCondicaoPagamento");

    _CRUDcadastroCondicaoPagamento = new CRUDCadastroCondicaoPagamento();
    KoBindings(_CRUDcadastroCondicaoPagamento, "knockoutCRUDCadastroCondicaoPagamento");

    BuscarTiposdeCarga(_cadastroCondicaoPagamento.TipoCarga);
    BuscarTiposOperacao(_cadastroCondicaoPagamento.TipoOperacao);

    loadGridCondicaoPagamento();

    if (isUtilizarCondicaoPagamento())
        $("#liTabCondicaoPagamento").show();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarCondicaoPagamentoClick() {
    if (ValidarCamposObrigatorios(_cadastroCondicaoPagamento)) {
        _condicaoPagamento.ListaCondicaoPagamento.val().push(obterCadastroCondicaoPagamentoSalvar());

        recarregarGridCondicaoPagamento();
        fecharModalCadastroCondicaoPagamento();
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function adicionarCondicaoPagamentoModalClick() {
    _cadastroCondicaoPagamento.Codigo.val(guid());

    controlarBotoesCadastroCondicaoPagamentoHabilitados(false);

    exibirModalCadastroCondicaoPagamento();
}

function atualizarCondicaoPagamentoClick() {
    if (ValidarCamposObrigatorios(_cadastroCondicaoPagamento)) {
        const listaCondicaoPagamento = obterListaCondicaoPagamento();

        listaCondicaoPagamento.forEach(function (condicaoPagamento, i) {
            if (_cadastroCondicaoPagamento.Codigo.val() == condicaoPagamento.Codigo) {
                listaCondicaoPagamento.splice(i, 1, obterCadastroCondicaoPagamentoSalvar());
            }
        });

        _condicaoPagamento.ListaCondicaoPagamento.val(listaCondicaoPagamento);

        recarregarGridCondicaoPagamento();
        fecharModalCadastroCondicaoPagamento();
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function editarCondicaoPagamentoClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroCondicaoPagamento, { Data: registroSelecionado });

    _cadastroCondicaoPagamento.TipoCarga.codEntity(registroSelecionado.CodigoTipoCarga);
    _cadastroCondicaoPagamento.TipoCarga.entityDescription(registroSelecionado.DescricaoTipoCarga);
    _cadastroCondicaoPagamento.TipoCarga.val(registroSelecionado.DescricaoTipoCarga);

    _cadastroCondicaoPagamento.TipoOperacao.codEntity(registroSelecionado.CodigoTipoOperacao);
    _cadastroCondicaoPagamento.TipoOperacao.entityDescription(registroSelecionado.DescricaoTipoOperacao);
    _cadastroCondicaoPagamento.TipoOperacao.val(registroSelecionado.DescricaoTipoOperacao);

    controlarBotoesCadastroCondicaoPagamentoHabilitados(true);

    exibirModalCadastroCondicaoPagamento();
}

function excluirCondicaoPagamentoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.DesejaExcluirRegistro, function () {
        removerCondicaoPagamento(_cadastroCondicaoPagamento.Codigo.val());
        fecharModalCadastroCondicaoPagamento();
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposCondicaoPagamento() {
    _condicaoPagamento.AtivarCondicao.val(false);
    _condicaoPagamento.ListaCondicaoPagamento.val([]);
}

function preencherCondicaoPagamento(dadosCondicaoPagamento) {
    if (dadosCondicaoPagamento) {
        _condicaoPagamento.AtivarCondicao.val(dadosCondicaoPagamento.AtivarCondicao);
        _condicaoPagamento.ListaCondicaoPagamento.val(dadosCondicaoPagamento.CondicoesPagamento);
    }
}

function preencherCondicaoPagamentoSalvar(filial) {
    filial["AtivarCondicao"] = _condicaoPagamento.AtivarCondicao.val();
    filial["CondicoesPagamento"] = obterCondicaoPagamentosSalvar();
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroCondicaoPagamentoHabilitados(isEdicao) {
    _CRUDcadastroCondicaoPagamento.Adicionar.visible(!isEdicao);
    _CRUDcadastroCondicaoPagamento.Atualizar.visible(isEdicao);
    _CRUDcadastroCondicaoPagamento.Excluir.visible(isEdicao);
}

function exibirModalCadastroCondicaoPagamento() {
    Global.abrirModal('divModalCadastroCondicaoPagamento');
    $("#divModalCadastroCondicaoPagamento").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroCondicaoPagamento);
    });
}

function fecharModalCadastroCondicaoPagamento() {
    Global.fecharModal('divModalCadastroCondicaoPagamento');
}

function isUtilizarCondicaoPagamento() {
    return (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador);
}

function obterCadastroCondicaoPagamentoSalvar() {
    return {
        Codigo: _cadastroCondicaoPagamento.Codigo.val(),
        CodigoTipoCarga: _cadastroCondicaoPagamento.TipoCarga.codEntity(),
        CodigoTipoOperacao: _cadastroCondicaoPagamento.TipoOperacao.codEntity(),
        DescricaoTipoCarga: _cadastroCondicaoPagamento.TipoCarga.val(),
        DescricaoTipoOperacao: _cadastroCondicaoPagamento.TipoOperacao.val(),
        DiasDePrazoPagamento: _cadastroCondicaoPagamento.DiasDePrazoPagamento.val(),
        DiaEmissaoLimite: _cadastroCondicaoPagamento.DiaEmissaoLimite.val(),
        DiaEmissaoLimiteDescricao: _cadastroCondicaoPagamento.DiaEmissaoLimite.val() > 0 ? _cadastroCondicaoPagamento.DiaEmissaoLimite.val() : Localization.Resources.Gerais.Geral.SemConfiguracao,
        DiaMes: _cadastroCondicaoPagamento.DiaMes.val(),
        DiaMesDescricao: _cadastroCondicaoPagamento.DiaMes.val() > 0 ? _cadastroCondicaoPagamento.DiaMes.val() : Localization.Resources.Gerais.Geral.SemConfiguracao,
        DiaSemana: _cadastroCondicaoPagamento.DiaSemana.val(),
        DiaSemanaDescricao: EnumDiaSemana.obterDescricaoSemConfiguracao(_cadastroCondicaoPagamento.DiaSemana.val()),
        TipoPrazoPagamento: _cadastroCondicaoPagamento.TipoPrazoPagamento.val(),
        TipoPrazoPagamentoDescricao: EnumTipoPrazoPagamento.obterDescricao(_cadastroCondicaoPagamento.TipoPrazoPagamento.val()),
        VencimentoForaMes: _cadastroCondicaoPagamento.VencimentoForaMes.val(),
        VencimentoForaMesDescricao: _cadastroCondicaoPagamento.VencimentoForaMes.val() ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
        ConsiderarDiaUtilVencimento: _cadastroCondicaoPagamento.ConsiderarDiaUtilVencimento.val(),
    };
}

function obterListaCondicaoPagamento() {
    return _condicaoPagamento.ListaCondicaoPagamento.val().slice();
}

function obterCondicaoPagamentosSalvar() {
    const listaCondicaoPagamento = obterListaCondicaoPagamento();

    return JSON.stringify(listaCondicaoPagamento);
}

function recarregarGridCondicaoPagamento() {
    const listaCondicaoPagamento = obterListaCondicaoPagamento();

    _gridCondicaoPagamento.CarregarGrid(listaCondicaoPagamento);
}

function removerCondicaoPagamento(codigo) {
    const listaCondicaoPagamento = obterListaCondicaoPagamento();

    listaCondicaoPagamento.forEach(function (condicaoPagamento, i) {
        if (codigo == condicaoPagamento.Codigo)
            listaCondicaoPagamento.splice(i, 1);
    });

    _condicaoPagamento.ListaCondicaoPagamento.val(listaCondicaoPagamento);
}