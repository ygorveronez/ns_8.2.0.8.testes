/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/Banco.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="Exportacao.js" />
/// <reference path="CentroResultado.js" />
/// <reference path="TipoDespesa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoMovimento;
var _tipoMovimento;
var _pesquisaTipoMovimento;
var _CRUDTipoMovimento;

var _FinalidadeTipoMovimento = [
    { text: "Todas", value: EnumFinalidadeTipoMovimento.Todas },
    { text: "Abastecimento", value: EnumFinalidadeTipoMovimento.Abastecimento },
    { text: "Documento de Entrada", value: EnumFinalidadeTipoMovimento.DocumentoEntrada },
    { text: "Faturamento Mensal", value: EnumFinalidadeTipoMovimento.FaturamentoMensal },
    { text: "Justificativa", value: EnumFinalidadeTipoMovimento.Justificativa },
    { text: "Movimento Financeiro", value: EnumFinalidadeTipoMovimento.MovimentoFinanceiro },
    { text: "Múltiplos Títulos", value: EnumFinalidadeTipoMovimento.MultiploTitulo },
    { text: "Natureza da Operação", value: EnumFinalidadeTipoMovimento.NaturezaOperacao },
    { text: "Pedágio", value: EnumFinalidadeTipoMovimento.Pedagio },
    { text: "Título Financeiro", value: EnumFinalidadeTipoMovimento.TituloFinanceiro }
];

var PesquisaTipoMovimento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid() });
    this.PlanoDebito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Conta de Entrada:", idBtnSearch: guid() });
    this.PlanoCredito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Conta de Saída:", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoMovimento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var TipoMovimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });

    this.PlanoDebito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Conta de Entrada:", idBtnSearch: guid(), required: true, visible: ko.observable(true) });
    this.PlanoCredito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Conta de Saída:", idBtnSearch: guid(), required: true, visible: ko.observable(true) });
    this.PlanoDebitoSintetico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Conta de Entrada:", idBtnSearch: guid(), required: false, visible: ko.observable(false) });
    this.PlanoCreditoSintetico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Conta de Saída:", idBtnSearch: guid(), required: false, visible: ko.observable(false) });
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Banco:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.FinalidadeTipoMovimento = PropertyEntity({ val: ko.observable(EnumFinalidadeTipoMovimento.Todas), options: _FinalidadeTipoMovimento, def: EnumFinalidadeTipoMovimento.Todas, text: "*Finalidade do Tipo Movimento: ", visible: ko.observable(false) });
    this.Finalidades = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, url: "TipoMovimento/ObterFinalidades", text: "Finalidades: ", options: ko.observable(new Array()), visible: ko.observable(false) });

    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 500 });
    this.CodigoFinalidadeTED = PropertyEntity({ text: "Cód. finalidade TED (Pagamento Digital): ", required: false, maxlength: 50 });
    this.CodigoHistorico = PropertyEntity({ text: "Cod. Histórico: ", required: false, maxlength: 50 });
    

    this.Exportar = PropertyEntity({ text: "Exportar ", required: false, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoGerarRateioDeDespesaPorVeiculo = PropertyEntity({ text: "Não gerar rateio de despesas por veículo", required: false, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    //Tab Exportação
    this.ListaConfiguracoesExportacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.ConfiguracoesExportacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.TiposDespesa = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: ", required: false, maxlength: 100 });

    this.Exportar.val.subscribe(function (novoValor) {
        if (novoValor)
            $("#liTabExportacao").removeClass("d-none");
        else
            $("#liTabExportacao").addClass("d-none");
    });
};

var CRUDTipoMovimento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default me-2",

        UrlImportacao: "TipoMovimento/Importar",
        UrlConfiguracao: "TipoMovimento/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O080_ImportacaoTipoMovimentoFinanceiro,
        CallbackImportacao: function () {
            _gridTipoMovimento.CarregarGrid();
        }
    });
};

//*******EVENTOS*******

function loadTipoMovimento() {

    _tipoMovimento = new TipoMovimento();
    KoBindings(_tipoMovimento, "knockoutCadastroTipoMovimento");

    _CRUDTipoMovimento = new CRUDTipoMovimento();
    KoBindings(_CRUDTipoMovimento, "knockoutCRUDTipoMovimento");

    HeaderAuditoria("TipoMovimento", _tipoMovimento);

    _pesquisaTipoMovimento = new PesquisaTipoMovimento();
    KoBindings(_pesquisaTipoMovimento, "knockoutPesquisaTipoMovimento", false, _pesquisaTipoMovimento.Pesquisar.id);

    new BuscarPlanoConta(_tipoMovimento.PlanoDebito, "Selecione a Conta Analítica de Entrada", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarPlanoConta(_tipoMovimento.PlanoCredito, "Selecione a Conta Analítica de Saída", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarBanco(_tipoMovimento.Banco);

    new BuscarCentroResultado(_pesquisaTipoMovimento.CentroResultado, "Selecione as Contas Analiticas", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarPlanoConta(_pesquisaTipoMovimento.PlanoDebito, "Selecione a Conta Analítica de Entrada", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarPlanoConta(_pesquisaTipoMovimento.PlanoCredito, "Selecione a Conta Analítica de Saída", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);

    buscarTipoMovimentos();
    loadCentroResultado();
    loadTipoDespesa();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
        _tipoMovimento.Finalidades.visible(true);

    $('.nav-tabs a').click(function (e) {
        e.preventDefault();
        $('#tabsTipoMovimento .tab-content').each(function (i, tabContent) {
            $(tabContent).children().each(function (z, el) {
                $(el).removeClass('active');
            });
        });
        $(this).tab('show');
    });

    LoadConfiguracaoExportacao();
}

function adicionarClick(e, sender) {
    preencherListasSelecaoTipoMovimento();

    Salvar(_tipoMovimento, "TipoMovimento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoMovimento.CarregarGrid();
                limparCamposTipoMovimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListasSelecaoTipoMovimento();

    Salvar(_tipoMovimento, "TipoMovimento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoMovimento.CarregarGrid();
                limparCamposTipoMovimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o tipo de movimento " + _tipoMovimento.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoMovimento, "TipoMovimento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTipoMovimento.CarregarGrid();
                    limparCamposTipoMovimento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTipoMovimento();
}

function preencherListasSelecaoTipoMovimento() {
    _tipoMovimento.ListaConfiguracoesExportacao.val(JSON.stringify(_tipoMovimento.ConfiguracoesExportacao.val()));
    _tipoMovimento.TiposDespesa.val(obterTiposDespesaSalvar());
}

//*******MÉTODOS*******

function buscarTipoMovimentos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoMovimento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoMovimento = new GridView(_pesquisaTipoMovimento.Pesquisar.idGrid, "TipoMovimento/Pesquisa", _pesquisaTipoMovimento, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridTipoMovimento.CarregarGrid();
}

function editarTipoMovimento(TipoMovimentoGrid) {
    limparCamposTipoMovimento();
    _tipoMovimento.Codigo.val(TipoMovimentoGrid.Codigo);
    BuscarPorCodigo(_tipoMovimento, "TipoMovimento/BuscarPorCodigo", function (arg) {
        _pesquisaTipoMovimento.ExibirFiltros.visibleFade(false);
        _CRUDTipoMovimento.Atualizar.visible(true);
        _CRUDTipoMovimento.Cancelar.visible(true);
        _CRUDTipoMovimento.Excluir.visible(true);
        _CRUDTipoMovimento.Adicionar.visible(false);
        _CRUDTipoMovimento.Importar.visible(false);

        recarregarGridCentroResultado();
        RecarregarGridConfiguracaoExportacao();
        preencherListaTiposDespesa(arg.Data);
    }, null);
}

function limparCamposTipoMovimento() {
    _CRUDTipoMovimento.Atualizar.visible(false);
    _CRUDTipoMovimento.Cancelar.visible(false);
    _CRUDTipoMovimento.Excluir.visible(false);
    _CRUDTipoMovimento.Adicionar.visible(true);

    LimparCampos(_tipoMovimento);
    recarregarGridCentroResultado();
    resetarTabs();

    RecarregarGridConfiguracaoExportacao();
    LimparCamposConfiguracaoExportacao();
    limparListaTiposDespesa();
}

function resetarTabs() {
    Global.ResetarMultiplasAbas();
}
