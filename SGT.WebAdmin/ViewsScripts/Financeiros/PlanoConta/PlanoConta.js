/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
/// <reference path="../../Enumeradores/EnumReceitaDespesa.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumGrupoDeResultado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPlanoConta;
var _planoConta;
var _planoContaBotoes;
var _pesquisaPlanoConta;

var _AnaliticoSinteticoPesquisa = [
    { text: "Todos", value: 0 },
    { text: "Analitico", value: EnumAnaliticoSintetico.Analitico },
    { text: "Sintético", value: EnumAnaliticoSintetico.Sintetico }
];

var _AnaliticoSintetico = [
    { text: "Analitico", value: EnumAnaliticoSintetico.Analitico },
    { text: "Sintético", value: EnumAnaliticoSintetico.Sintetico }
];

var _PesquisaReceitaDespesa = [
    { text: "Todos", value: 0 },
    { text: "Receita", value: EnumReceitaDespesa.Receita },
    { text: "Despesa", value: EnumReceitaDespesa.Despesa },
    { text: "Outros", value: EnumReceitaDespesa.Outros }
];

var _ReceitaDespesa = [
    { text: "Receita", value: EnumReceitaDespesa.Receita },
    { text: "Despesa", value: EnumReceitaDespesa.Despesa },
    { text: "Outros", value: EnumReceitaDespesa.Outros }
];

var _PesquisaGrupoDeResultado = [
    { text: "Todos", value: 0 },
    { text: "Receita Operacional Bruta", value: EnumGrupoDeResultado.ReceitaOperacionalBruta },
    { text: "Dedução da Receita Bruta", value: EnumGrupoDeResultado.DeducaoReceitaBruta },
    { text: "Custo das Vendas", value: EnumGrupoDeResultado.CustoVenda },
    { text: "Despesas Operacionais", value: EnumGrupoDeResultado.DespesaOperacional },
    { text: "Resultado Financeiro", value: EnumGrupoDeResultado.ResultadoFinanceiro },
    { text: "Resultado Não Operacional", value: EnumGrupoDeResultado.ResultadoNaoOperacional },
    { text: "IRPJ/CSLL", value: EnumGrupoDeResultado.IrpjCsll },
    { text: "Investimentos", value: EnumGrupoDeResultado.Investimento }
];

var _GrupoDeResultado = [
    { text: "Nenhum", value: EnumGrupoDeResultado.Nenhum },
    { text: "Receita Operacional Bruta", value: EnumGrupoDeResultado.ReceitaOperacionalBruta },
    { text: "Dedução da Receita Bruta", value: EnumGrupoDeResultado.DeducaoReceitaBruta },
    { text: "Custo das Vendas", value: EnumGrupoDeResultado.CustoVenda },
    { text: "Despesas Operacionais", value: EnumGrupoDeResultado.DespesaOperacional },
    { text: "Resultado Financeiro", value: EnumGrupoDeResultado.ResultadoFinanceiro },
    { text: "Resultado Não Operacional", value: EnumGrupoDeResultado.ResultadoNaoOperacional },
    { text: "IRPJ/CSLL", value: EnumGrupoDeResultado.IrpjCsll },
    { text: "Investimentos", value: EnumGrupoDeResultado.Investimento }
];

var PesquisaPlanoConta = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true } });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: " });
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Plano = PropertyEntity({ text: "Número conta gerencial: " });
    this.ReceitaDespesa = PropertyEntity({ val: ko.observable(0), options: _PesquisaReceitaDespesa, text: "Receita / Despesa: ", def: 0 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Tipo = PropertyEntity({ val: ko.observable(0), options: _AnaliticoSinteticoPesquisa, def: 0, text: "Tipo de conta: " });
    this.GrupoDeResultado = PropertyEntity({ val: ko.observable(0), options: _PesquisaGrupoDeResultado, def: 0, text: "Grupo de Resultado: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPlanoConta.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var PlanoConta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.Plano = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), validaEscritaBusca: false, text: "*Número conta gerencial:", idBtnSearch: guid(), required: true, val: ko.observable("") });
    this.PlanoContaSintetico = PropertyEntity({ text: "Plano de conta gerencial sintético: ", required: false, enable: false, issue: 359 });
    this.PlanoContabilidade = PropertyEntity({ text: "Código Integração: ", required: false, issue: 358 });
    this.AnaliticoSintetico = PropertyEntity({ val: ko.observable(EnumAnaliticoSintetico.Analitico), options: _AnaliticoSintetico, text: "*Tipo Conta: ", def: EnumAnaliticoSintetico.Analitico });
    this.ReceitaDespesa = PropertyEntity({ val: ko.observable(EnumReceitaDespesa.Outros), options: _ReceitaDespesa, text: "*Receita / Despesa: ", def: EnumReceitaDespesa.Outros, issue: 1050 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.GrupoDeResultado = PropertyEntity({ val: ko.observable(EnumGrupoDeResultado.Nenhum), options: _GrupoDeResultado, def: EnumGrupoDeResultado.Nenhum, text: "*Grupo de Resultado: " });

    this.SaldoInicialConciliacaoBancaria = PropertyEntity({ text: "Saldo Inicial da Conciliação Bancária: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.SaldoFinalConciliacaoBancaria = PropertyEntity({ text: "Saldo Final da Conciliação Bancária: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true) });

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
        UrlImportacao: "PlanoConta/Importar",
        UrlConfiguracao: "PlanoConta/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O083_PlanoConta,
        CallbackImportacao: function () {
            _gridPlanoConta.CarregarGrid();
        }
    });
}

//*******EVENTOS*******


function loadPlanoConta() {

    _planoConta = new PlanoConta();
    KoBindings(_planoConta, "knockoutCadastroPlanoConta");

    HeaderAuditoria("PlanoConta", _planoConta);

    _pesquisaPlanoConta = new PesquisaPlanoConta();
    KoBindings(_pesquisaPlanoConta, "knockoutPesquisaPlanoConta", false, _pesquisaPlanoConta.Pesquisar.id);

    new BuscarPlanoConta(_planoConta.Plano, "Selecione a Conta Sintética", "Contas Sintéticas", RetornoSelecaoContaSintetica, EnumAnaliticoSintetico.Sintetico);

    buscarPlanoContas();
    
}

function RetornoSelecaoContaSintetica(data) {
    if (data != null) {
        var data =
            {
                Codigo: data.Codigo,
                Plano: data.Plano
            }
        executarReST("PlanoConta/ProximaNumeracao", data, function (e) {
            if (e.Success) {
                _planoConta.Plano.val(e.Data.Plano);
                _planoConta.Plano.codEntity(e.Data.Plano);
                _planoConta.PlanoContaSintetico.val(e.Data.PlanoContaSintetico);
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
        });
    }
}

function adicionarClick(e, sender) {
    //if (e.Plano.val() != "" && e.Plano.codEntity() == "")
    e.Plano.codEntity(e.Plano.val());
    Salvar(e, "PlanoConta/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridPlanoConta.CarregarGrid();
                limparCamposPlanoConta();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            LimparCampoEntity(_planoConta.Plano);
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    e.Plano.codEntity(e.Plano.val());
    Salvar(e, "PlanoConta/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPlanoConta.CarregarGrid();
                limparCamposPlanoConta();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o plano de contas " + _planoConta.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_planoConta, "PlanoConta/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridPlanoConta.CarregarGrid();
                limparCamposPlanoConta();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposPlanoConta();
}

function ConsultaPlanoClick(e, sender) {

}

//*******MÉTODOS*******


function buscarPlanoContas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPlanoConta, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "PlanoConta/ExportarPesquisa",
        titulo: "Plano de Contar"
    };

    _gridPlanoConta = new GridViewExportacao(_pesquisaPlanoConta.Pesquisar.idGrid, "PlanoConta/Pesquisa", _pesquisaPlanoConta, menuOpcoes, configExportacao, { column: 2, dir: orderDir.asc });
    _gridPlanoConta.CarregarGrid();
}

function editarPlanoConta(planoContaGrid) {
    limparCamposPlanoConta();
    _planoConta.Codigo.val(planoContaGrid.Codigo);
    BuscarPorCodigo(_planoConta, "PlanoConta/BuscarPorCodigo", function (arg) {
        _pesquisaPlanoConta.ExibirFiltros.visibleFade(false);
        _planoConta.Atualizar.visible(true);
        _planoConta.Cancelar.visible(true);
        _planoConta.Excluir.visible(true);
        _planoConta.Adicionar.visible(false);
    }, null);
}

function limparCamposPlanoConta() {
    _planoConta.Atualizar.visible(false);
    _planoConta.Cancelar.visible(false);
    _planoConta.Excluir.visible(false);
    _planoConta.Adicionar.visible(true);
    LimparCampos(_planoConta);
}
