/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoImportacaoPedido.js" />


var controller = "ImportacaoPedido";
var _gridImportacaoPedido = null, _pesquisaImportacaoPedido = null, _importarPedido = null;
var _gridImportacaoPedidoLinha = null, _gridImportacaoPedidoColuna = null;

function loadImportacaoPedido() {
    _pesquisaImportacaoPedido = new PesquisaImportacaoPedido();
    KoBindings(_pesquisaImportacaoPedido, "knockoutPesquisaImportacaoPedido", false, _pesquisaImportacaoPedido.Pesquisar.id);
    new BuscarFuncionario(_pesquisaImportacaoPedido.Funcionario);

    _importarPedido = new ImportarPedido();
    KoBindings(_importarPedido, "knockoutImportarImportacaoPedido", false, _importarPedido.Importar.id);

    loadGridImportacaoPedido();
}

//*******MAPEAMENTO KNOUCKOUT*******

var PesquisaImportacaoPedido = function () {
    this.Planilha = PropertyEntity({ text: Localization.Resources.Pedidos.ImportacaoPedido.Planilha.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Pedidos.ImportacaoPedido.DataInicial.getFieldDescription(), getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Pedidos.ImportacaoPedido.DataFinal.getFieldDescription(), getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ text: Localization.Resources.Pedidos.ImportacaoPedido.Funcionario.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Pedidos.ImportacaoPedido.Situacao.getFieldDescription(), val: ko.observable(EnumSituacaoImportacaoPedido.Todas), options: EnumSituacaoImportacaoPedido.obterOpcoesPesquisa(), def: true, visible: ko.observable(true) });
    this.Mensagem = PropertyEntity({ text: Localization.Resources.Pedidos.ImportacaoPedido.Mensagem.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ text: Localization.Resources.Pedidos.ImportacaoPedido.Pesquisar, type: types.event,  idGrid: guid(), visible: ko.observable(true), eventClick: pesquisarImportacaoPedidoClick });
    this.ExibirFiltros = PropertyEntity({ text: Localization.Resources.Pedidos.ImportacaoPedido.FiltrosPesquisa, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); } });
};

var ImportarPedido = function () {
    this.Exibir = PropertyEntity({ text: Localization.Resources.Pedidos.ImportacaoPedido.ImportarPlanilha, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.Exibir.visibleFade(!e.Exibir.visibleFade()); } });
    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Pedidos.ImportacaoPedido.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        UrlImportacao: controller + "/Importar",
        UrlConfiguracao: controller + "/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O005_Pedidos,
        CallbackImportacao: function () {
            loadGridImportacaoPedido();
        }
    });
};

function loadGridImportacaoPedido() {
    var linhas = { descricao: Localization.Resources.Pedidos.ImportacaoPedido.VisualizarLinhas, id: guid(), evento: "onclick", metodo: visualizarLinhasImportacaoPedidoClick, tamanho: "10", icone: ""};
    var exportar = { descricao: Localization.Resources.Pedidos.ImportacaoPedido.Exportar, id: guid(), evento: "onclick", metodo: exportarImportacaoPedidoClick, tamanho: "10", icone: "", visibilidade: exportarImportacaoPedidoVisible };
    var reprocessar = { descricao: Localization.Resources.Pedidos.ImportacaoPedido.Reprocessar, id: guid(), evento: "onclick", metodo: reprocessarImportacaoPedidoClick, tamanho: "10", icone: "", visibilidade: reprocessarImportacaoPedidoVisible };
    var cancelar = { descricao: Localization.Resources.Pedidos.ImportacaoPedido.Cancelar, id: guid(), evento: "onclick", metodo: cancelarImportacaoPedidoClick, tamanho: "10", icone: "", visibilidade: cancelarImportacaoPedidoVisible };
    var excluir = { descricao: Localization.Resources.Pedidos.ImportacaoPedido.Excluir, id: guid(), evento: "onclick", metodo: excluirImportacaoPedidoClick, tamanho: "10", icone: "", visibilidade: excluirImportacaoPedidoVisible };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [linhas, exportar, reprocessar, cancelar, excluir], tamanho: 10 };
    _gridImportacaoPedido = new GridView("grid-importacao-pedido", controller + "/Pesquisa", _pesquisaImportacaoPedido, menuOpcoes, null, 10);
    _gridImportacaoPedido.CarregarGrid();
}

//*******EVENTOS*******

function exportarImportacaoPedidoVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Erro || row.Situacao == EnumSituacaoImportacaoPedido.Sucesso);
}

function reprocessarImportacaoPedidoVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Erro || row.Situacao == EnumSituacaoImportacaoPedido.Sucesso);
}

function cancelarImportacaoPedidoVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Pendente);
}

function excluirImportacaoPedidoVisible(row) {
    return (row.Situacao != EnumSituacaoImportacaoPedido.Processando);
}


function pesquisarImportacaoPedidoClick(e, sender) {
    loadGridImportacaoPedido();
}

function visualizarLinhasImportacaoPedidoClick(row) {
    Global.abrirModal('divModalImportacaoPedidoLinha');
    var linhas = { descricao: Localization.Resources.Pedidos.ImportacaoPedido.Colunas, id: guid(), evento: "onclick", metodo: visualizarColunasImportacaoPedidoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [linhas], tamanho: 10 };
    var configuracoesExportacao = { url: controller + "/ExportarLinhas?codigo=" + row.Codigo, titulo: "ImportacaoPedidosLinhas" };
    _gridImportacaoPedidoLinha = new GridView("grid-importacao-pedido-linha", controller + "/Linhas?codigo=" + row.Codigo,
        null, menuOpcoes, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridImportacaoPedidoLinha.CarregarGrid();
}

function exportarImportacaoPedidoClick(row) {
    executarDownload(controller + "/Exportar", { codigo: row.Codigo});
}

function visualizarColunasImportacaoPedidoClick(row) {
    Global.abrirModal('divModalImportacaoPedidoColuna');
    var configuracoesExportacao = { url: controller + "/ExportarColunas?codigo=" + row.Codigo, titulo: "ImportacaoPedidosColunas" };
    _gridImportacaoPedidoColuna = new GridView("grid-importacao-pedido-coluna", controller + "/Colunas?codigo=" + row.Codigo,
        null, null, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridImportacaoPedidoColuna.CarregarGrid();
}

function reprocessarImportacaoPedidoClick(row) {
    exibirConfirmacao(Localization.Resources.Pedidos.ImportacaoPedido.Confirmacao, Localization.Resources.Pedidos.ImportacaoPedido.RealmenteDesejaReprocessarInformacao.format(row.Planilha), function () {
        executarReST(controller + "/Reprocessar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pedidos.ImportacaoPedido.Sucesso, arg.Msg);
                loadGridImportacaoPedido();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Pedidos.ImportacaoPedido.Falha, arg.Msg);
            }
        });
    });
}

function cancelarImportacaoPedidoClick(row) {
    exibirConfirmacao(Localization.Resources.Pedidos.ImportacaoPedido.Confirmacao, Localization.Resources.Pedidos.ImportacaoPedido.RealmenteDesejaCancelarImportacao.format(row.Planilha), function () {
        executarReST(controller + "/Cancelar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pedidos.ImportacaoPedido.Sucesso, arg.Msg);
                loadGridImportacaoPedido();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Pedidos.ImportacaoPedido.Falha, arg.Msg);
            }
        });
    });
}

function excluirImportacaoPedidoClick(row) {
    exibirConfirmacao(Localization.Resources.Pedidos.ImportacaoPedido.Confirmacao, Localization.Resources.Pedidos.ImportacaoPedido.RealmenteDesejaExcluirImportacaoPedidos.format(row.Planilha), function () {
        executarReST(controller + "/Excluir", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pedidos.ImportacaoPedido.Sucesso, arg.Msg);
                loadGridImportacaoPedido();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Pedidos.ImportacaoPedido.Falha, arg.Msg);
            }
        });
    });
}

//*******MÉTODOS*******
