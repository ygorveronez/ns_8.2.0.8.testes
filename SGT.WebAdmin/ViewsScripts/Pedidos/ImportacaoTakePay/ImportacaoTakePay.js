/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoImportacaoPedido.js" />


var controller = "ImportacaoTakePay";
var _gridImportacaoTakePay = null, _pesquisaImportacaoTakePay = null, _importarPedido = null;
var _gridImportacaoTakePayLinha = null, _gridImportacaoTakePayColuna = null;

function loadImportacaoTakePay() {
    _pesquisaImportacaoTakePay = new PesquisaImportacaoTakePay();
    KoBindings(_pesquisaImportacaoTakePay, "knockoutPesquisaImportacaoTakePay", false, _pesquisaImportacaoTakePay.Pesquisar.id);
    new BuscarFuncionario(_pesquisaImportacaoTakePay.Funcionario);

    _importarPedido = new ImportarPedido();
    KoBindings(_importarPedido, "knockoutImportarImportacaoTakePay", false, _importarPedido.Importar.id);

    loadGridImportacaoTakePay();
}

//*******MAPEAMENTO KNOUCKOUT*******

var PesquisaImportacaoTakePay = function () {
    this.Planilha = PropertyEntity({ text: "Planilha:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data inicial: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data final: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ text: "Funcionário:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoImportacaoPedido.Todas), options: EnumSituacaoImportacaoPedido.obterOpcoesPesquisa(), def: true, visible: ko.observable(true) });
    this.Mensagem = PropertyEntity({ text: "Mensagem:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, idGrid: guid(), visible: ko.observable(true), eventClick: pesquisarImportacaoTakePayClick });
    this.ExibirFiltros = PropertyEntity({ text: "Filtros de Pesquisa", type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); } });
};

var ImportarPedido = function () {
    this.Exibir = PropertyEntity({ text: "Importar planilha", type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.Exibir.visibleFade(!e.Exibir.visibleFade()); } });
    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        UrlImportacao: controller + "/Importar",
        UrlConfiguracao: controller + "/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O005_Pedidos,
        CallbackImportacao: function () {
            loadGridImportacaoTakePay();
        }
    });
};

function loadGridImportacaoTakePay() {
    var linhas = { descricao: "Visualizar linhas", id: guid(), evento: "onclick", metodo: visualizarLinhasImportacaoTakePayClick, tamanho: "10", icone: "" };
    var exportar = { descricao: "Exportar", id: guid(), evento: "onclick", metodo: exportarImportacaoTakePayClick, tamanho: "10", icone: "", visibilidade: exportarImportacaoTakePayVisible };
    var reprocessar = { descricao: "Reprocessar", id: guid(), evento: "onclick", metodo: reprocessarImportacaoTakePayClick, tamanho: "10", icone: "", visibilidade: reprocessarImportacaoTakePayVisible };
    var cancelar = { descricao: "Cancelar", id: guid(), evento: "onclick", metodo: cancelarImportacaoTakePayClick, tamanho: "10", icone: "", visibilidade: cancelarImportacaoTakePayVisible };
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirImportacaoTakePayClick, tamanho: "10", icone: "", visibilidade: excluirImportacaoTakePayVisible };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [linhas, exportar, reprocessar, cancelar, excluir], tamanho: 10 };
    _gridImportacaoTakePay = new GridView("grid-importacao-takepay", controller + "/Pesquisa", _pesquisaImportacaoTakePay, menuOpcoes, null, 10);
    _gridImportacaoTakePay.CarregarGrid();
}

//*******EVENTOS*******

function exportarImportacaoTakePayVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Erro || row.Situacao == EnumSituacaoImportacaoPedido.Sucesso);
}

function reprocessarImportacaoTakePayVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Erro || row.Situacao == EnumSituacaoImportacaoPedido.Sucesso);
}

function cancelarImportacaoTakePayVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Pendente);
}

function excluirImportacaoTakePayVisible(row) {
    return (row.Situacao != EnumSituacaoImportacaoPedido.Processando);
}


function pesquisarImportacaoTakePayClick(e, sender) {
    loadGridImportacaoTakePay();
}

function visualizarLinhasImportacaoTakePayClick(row) {
    Global.abrirModal('divModalImportacaoTakePayLinha');
    var linhas = { descricao: "Colunas", id: guid(), evento: "onclick", metodo: visualizarColunasImportacaoTakePayClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [linhas], tamanho: 10 };
    var configuracoesExportacao = { url: controller + "/ExportarLinhas?codigo=" + row.Codigo, titulo: "ImportacaoTakePaysLinhas" };
    _gridImportacaoTakePayLinha = new GridView("grid-importacao-takepay-linha", controller + "/Linhas?codigo=" + row.Codigo,
        null, menuOpcoes, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridImportacaoTakePayLinha.CarregarGrid();
}

function exportarImportacaoTakePayClick(row) {
    executarDownload(controller + "/Exportar", { codigo: row.Codigo });
}

function visualizarColunasImportacaoTakePayClick(row) {
    Global.abrirModal('divModalImportacaoTakePayColuna');
    var configuracoesExportacao = { url: controller + "/ExportarColunas?codigo=" + row.Codigo, titulo: "ImportacaoTakePaysColunas" };
    _gridImportacaoTakePayColuna = new GridView("grid-importacao-takepay-coluna", controller + "/Colunas?codigo=" + row.Codigo,
        null, null, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridImportacaoTakePayColuna.CarregarGrid();
}

function reprocessarImportacaoTakePayClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja reprocessar a importação dos pedidos da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Reprocessar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoTakePay();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function cancelarImportacaoTakePayClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar a importação dos pedidos da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Cancelar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoTakePay();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function excluirImportacaoTakePayClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a importação de pedidos da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Excluir", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoTakePay();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}