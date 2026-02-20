/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoImportacaoPedido.js" />


var controller = "ImportacaoPrecoCombustivel";
var _gridImportacaoPrecoCombustivel = null, _pesquisaImportacaoPrecoCombustivel = null, _importarPedido = null;
var _gridImportacaoPrecoCombustivelLinha = null, _gridImportacaoPrecoCombustivelColuna = null;

function loadImportacaoPrecoCombustivel() {
    _pesquisaImportacaoPrecoCombustivel = new PesquisaImportacaoPrecoCombustivel();
    KoBindings(_pesquisaImportacaoPrecoCombustivel, "knockoutPesquisaImportacaoPrecoCombustivel", false, _pesquisaImportacaoPrecoCombustivel.Pesquisar.id);
    new BuscarFuncionario(_pesquisaImportacaoPrecoCombustivel.Funcionario);

    _importarPedido = new ImportarPedido();
    KoBindings(_importarPedido, "knockoutImportarImportacaoPrecoCombustivel", false, _importarPedido.Importar.id);

    loadGridImportacaoPrecoCombustivel();
}

//*******MAPEAMENTO KNOUCKOUT*******

var PesquisaImportacaoPrecoCombustivel = function () {
    this.Planilha = PropertyEntity({ text: "Planilha:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data inicial: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data final: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ text: "Funcionário:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoImportacaoPedido.Todas), options: EnumSituacaoImportacaoPedido.obterOpcoesPesquisa(), def: true, visible: ko.observable(true) });
    this.Mensagem = PropertyEntity({ text: "Mensagem:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, idGrid: guid(), visible: ko.observable(true), eventClick: pesquisarImportacaoPrecoCombustivelClick });
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
            loadGridImportacaoPrecoCombustivel();
        }
    });
};

function loadGridImportacaoPrecoCombustivel() {
    var linhas = { descricao: "Visualizar linhas", id: guid(), evento: "onclick", metodo: visualizarLinhasImportacaoPrecoCombustivelClick, tamanho: "10", icone: "" };
    var exportar = { descricao: "Exportar", id: guid(), evento: "onclick", metodo: exportarImportacaoPrecoCombustivelClick, tamanho: "10", icone: "", visibilidade: exportarImportacaoPrecoCombustivelVisible };
    var reprocessar = { descricao: "Reprocessar", id: guid(), evento: "onclick", metodo: reprocessarImportacaoPrecoCombustivelClick, tamanho: "10", icone: "", visibilidade: reprocessarImportacaoPrecoCombustivelVisible };
    var cancelar = { descricao: "Cancelar", id: guid(), evento: "onclick", metodo: cancelarImportacaoPrecoCombustivelClick, tamanho: "10", icone: "", visibilidade: cancelarImportacaoPrecoCombustivelVisible };
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirImportacaoPrecoCombustivelClick, tamanho: "10", icone: "", visibilidade: excluirImportacaoPrecoCombustivelVisible };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [linhas, exportar, reprocessar, cancelar, excluir], tamanho: 10 };
    _gridImportacaoPrecoCombustivel = new GridView("grid-importacao-precocombustivel", controller + "/Pesquisa", _pesquisaImportacaoPrecoCombustivel, menuOpcoes, null, 10);
    _gridImportacaoPrecoCombustivel.CarregarGrid();
}

//*******EVENTOS*******

function exportarImportacaoPrecoCombustivelVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Erro || row.Situacao == EnumSituacaoImportacaoPedido.Sucesso);
}

function reprocessarImportacaoPrecoCombustivelVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Erro || row.Situacao == EnumSituacaoImportacaoPedido.Sucesso);
}

function cancelarImportacaoPrecoCombustivelVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Pendente);
}

function excluirImportacaoPrecoCombustivelVisible(row) {
    return (row.Situacao != EnumSituacaoImportacaoPedido.Processando);
}


function pesquisarImportacaoPrecoCombustivelClick(e, sender) {
    loadGridImportacaoPrecoCombustivel();
}

function visualizarLinhasImportacaoPrecoCombustivelClick(row) {
    Global.abrirModal('divModalImportacaoPrecoCombustivelLinha');
    var linhas = { descricao: "Colunas", id: guid(), evento: "onclick", metodo: visualizarColunasImportacaoPrecoCombustivelClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [linhas], tamanho: 10 };
    var configuracoesExportacao = { url: controller + "/ExportarLinhas?codigo=" + row.Codigo, titulo: "ImportacaoPrecoCombustivelsLinhas" };
    _gridImportacaoPrecoCombustivelLinha = new GridView("grid-importacao-precocombustivel-linha", controller + "/Linhas?codigo=" + row.Codigo,
        null, menuOpcoes, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridImportacaoPrecoCombustivelLinha.CarregarGrid();
}

function exportarImportacaoPrecoCombustivelClick(row) {
    executarDownload(controller + "/Exportar", { codigo: row.Codigo });
}

function visualizarColunasImportacaoPrecoCombustivelClick(row) {
    Global.abrirModal('divModalImportacaoPrecoCombustivelColuna');
    var configuracoesExportacao = { url: controller + "/ExportarColunas?codigo=" + row.Codigo, titulo: "ImportacaoPrecoCombustivelsColunas" };
    _gridImportacaoPrecoCombustivelColuna = new GridView("grid-importacao-precocombustivel-coluna", controller + "/Colunas?codigo=" + row.Codigo,
        null, null, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridImportacaoPrecoCombustivelColuna.CarregarGrid();
}

function reprocessarImportacaoPrecoCombustivelClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja reprocessar a importação dos preços de combustíveis da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Reprocessar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoPrecoCombustivel();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function cancelarImportacaoPrecoCombustivelClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar a importação dos preços de combustíveis da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Cancelar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoPrecoCombustivel();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function excluirImportacaoPrecoCombustivelClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a importação de preços de combustíveis da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Excluir", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoPrecoCombustivel();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}