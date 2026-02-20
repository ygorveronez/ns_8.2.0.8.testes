/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoImportacaoPedido.js" />


var controller = "NotaDeDebito";
var _gridImportacaoNotaDebito = null, _pesquisaImportacaoNotaDebito = null, _importarPedido = null;
var _gridImportacaoNotaDebitoLinha = null, _gridImportacaoNotaDebitoColuna = null;

function loadImportacaoNotaDebito() {
    _pesquisaImportacaoNotaDebito = new PesquisaImportacaoNotaDebito();
    KoBindings(_pesquisaImportacaoNotaDebito, "knockoutPesquisaImportacaoNotaDebito", false, _pesquisaImportacaoNotaDebito.Pesquisar.id);
    new BuscarFuncionario(_pesquisaImportacaoNotaDebito.Funcionario);

    _importarPedido = new ImportarPedido();
    KoBindings(_importarPedido, "knockoutImportarImportacaoNotaDebito", false, _importarPedido.Importar.id);

    loadGridImportacaoNotaDebito();
}

//*******MAPEAMENTO KNOUCKOUT*******

var PesquisaImportacaoNotaDebito = function () {
    this.Planilha = PropertyEntity({ text: "Planilha:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data inicial: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data final: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ text: "Funcionário:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoImportacaoPedido.Todas), options: EnumSituacaoImportacaoPedido.obterOpcoesPesquisa(), def: true, visible: ko.observable(true) });
    this.Mensagem = PropertyEntity({ text: "Mensagem:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, idGrid: guid(), visible: ko.observable(true), eventClick: pesquisarImportacaoNotaDebitoClick });
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
            loadGridImportacaoNotaDebito();
        }
    });
};

function loadGridImportacaoNotaDebito() {
    var linhas = { descricao: "Visualizar linhas", id: guid(), evento: "onclick", metodo: visualizarLinhasImportacaoNotaDebitoClick, tamanho: "10", icone: "" };
    var exportar = { descricao: "Exportar", id: guid(), evento: "onclick", metodo: exportarImportacaoNotaDebitoClick, tamanho: "10", icone: "", visibilidade: exportarImportacaoNotaDebitoVisible };
    var reprocessar = { descricao: "Reprocessar", id: guid(), evento: "onclick", metodo: reprocessarImportacaoNotaDebitoClick, tamanho: "10", icone: "", visibilidade: reprocessarImportacaoNotaDebitoVisible };
    var cancelar = { descricao: "Cancelar", id: guid(), evento: "onclick", metodo: cancelarImportacaoNotaDebitoClick, tamanho: "10", icone: "", visibilidade: cancelarImportacaoNotaDebitoVisible };
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirImportacaoNotaDebitoClick, tamanho: "10", icone: "", visibilidade: excluirImportacaoNotaDebitoVisible };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [linhas, exportar, reprocessar, cancelar, excluir], tamanho: 10 };
    _gridImportacaoNotaDebito = new GridView("grid-nota-debito", controller + "/Pesquisa", _pesquisaImportacaoNotaDebito, menuOpcoes, null, 10);
    _gridImportacaoNotaDebito.CarregarGrid();
}

//*******EVENTOS*******

function exportarImportacaoNotaDebitoVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Erro || row.Situacao == EnumSituacaoImportacaoPedido.Sucesso);
}

function reprocessarImportacaoNotaDebitoVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Erro || row.Situacao == EnumSituacaoImportacaoPedido.Sucesso);
}

function cancelarImportacaoNotaDebitoVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Pendente);
}

function excluirImportacaoNotaDebitoVisible(row) {
    return (row.Situacao != EnumSituacaoImportacaoPedido.Processando);
}


function pesquisarImportacaoNotaDebitoClick(e, sender) {
    loadGridImportacaoNotaDebito();
}

function visualizarLinhasImportacaoNotaDebitoClick(row) {
    Global.abrirModal('divModalImportacaoNotaDebitoLinha');
    var linhas = { descricao: "Colunas", id: guid(), evento: "onclick", metodo: visualizarColunasImportacaoNotaDebitoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [linhas], tamanho: 10 };
    var configuracoesExportacao = { url: controller + "/ExportarLinhas?codigo=" + row.Codigo, titulo: "ImportacaoNotaDebitoLinhas" };
    _gridImportacaoNotaDebitoLinha = new GridView("grid-nota-debito-linha", controller + "/Linhas?codigo=" + row.Codigo,
        null, menuOpcoes, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridImportacaoNotaDebitoLinha.CarregarGrid();
}

function exportarImportacaoNotaDebitoClick(row) {
    executarDownload(controller + "/Exportar", { codigo: row.Codigo });
}

function visualizarColunasImportacaoNotaDebitoClick(row) {
    Global.abrirModal('divModalImportacaoNotaDebitoColuna');
    var configuracoesExportacao = { url: controller + "/ExportarColunas?codigo=" + row.Codigo, titulo: "ImportacaoNotaDebitoColunas" };
    _gridImportacaoNotaDebitoColuna = new GridView("grid-nota-debito-coluna", controller + "/Colunas?codigo=" + row.Codigo,
        null, null, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridImportacaoNotaDebitoColuna.CarregarGrid();
}

function reprocessarImportacaoNotaDebitoClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja reprocessar a importação dos pedidos da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Reprocessar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoNotaDebito();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function cancelarImportacaoNotaDebitoClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar a importação dos pedidos da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Cancelar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoNotaDebito();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function excluirImportacaoNotaDebitoClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a importação de pedidos da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Excluir", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoNotaDebito();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}