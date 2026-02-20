/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoImportacaoCTeEmitidoForaEmbarcador.js" />

var controller = "ImportacaoCTeEmitidoForaEmbarcador";
var _gridImportacaoCTeEmitidoForaEmbarcador = null, _pesquisaImportacaoCTeEmitidoForaEmbarcador = null, _importacaoCTeEmitidoForaEmbarcador = null;
var _gridImportacaoCTeEmitidoForaEmbarcadorLinha = null, _gridImportacaoCTeEmitidoForaEmbarcadorColuna = null;

function loadImportacaoCTeEmitidoForaEmbarcador() {
    _pesquisaImportacaoCTeEmitidoForaEmbarcador = new PesquisaImportacaoCTeEmitidoForaEmbarcador();
    KoBindings(_pesquisaImportacaoCTeEmitidoForaEmbarcador, "knockoutPesquisaImportacaoCTeEmitidoForaEmbarcador", false, _pesquisaImportacaoCTeEmitidoForaEmbarcador.Pesquisar.id);
    BuscarFuncionario(_pesquisaImportacaoCTeEmitidoForaEmbarcador.Funcionario);

    _importacaoCTeEmitidoForaEmbarcador = new ImportacaoPlanilhaImportacaoCTeEmitidoForaEmbarcador();
    KoBindings(_importacaoCTeEmitidoForaEmbarcador, "knockoutImportacaoCTeEmitidoForaEmbarcador", false, _importacaoCTeEmitidoForaEmbarcador.Importar.id);

    loadGridImportacaoCTeEmitidoForaEmbarcador();
}

//*******MAPEAMENTO KNOUCKOUT*******

var PesquisaImportacaoCTeEmitidoForaEmbarcador = function () {
    this.Planilha = PropertyEntity({ text: "Planilha: ", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data inicial: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ text: "Funcionário: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoImportacaoCTeEmitidoForaEmbarcador.Todas), options: EnumSituacaoImportacaoCTeEmitidoForaEmbarcador.obterOpcoesPesquisa(), def: EnumSituacaoImportacaoCTeEmitidoForaEmbarcador.Todas, visible: ko.observable(true) });
    this.Mensagem = PropertyEntity({ text: "Mensagem: ", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, idGrid: guid(), visible: ko.observable(true), eventClick: pesquisarImportacaoCTeEmitidoForaEmbarcadorClick });
    this.ExibirFiltros = PropertyEntity({ text: "Exibir Filtros", type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); } });
};

var ImportacaoPlanilhaImportacaoCTeEmitidoForaEmbarcador = function () {
    this.Exibir = PropertyEntity({ text: "Importar Planilha", type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.Exibir.visibleFade(!e.Exibir.visibleFade()); } });
    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        UrlImportacao: controller + "/Importar",
        UrlConfiguracao: controller + "/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O070_ImportacaoCTeEmitidoForaEmbarcador,
        CallbackImportacao: function () {
            loadGridImportacaoCTeEmitidoForaEmbarcador();
        }
    });
};

function loadGridImportacaoCTeEmitidoForaEmbarcador() {
    let linhas = { descricao: "Visualizar Linhas", id: guid(), evento: "onclick", metodo: visualizarLinhasImportacaoCTeEmitidoForaEmbarcadorClick, tamanho: "10", icone: "" };
    let exportar = { descricao: "Exportar", id: guid(), evento: "onclick", metodo: exportarImportacaoCTeEmitidoForaEmbarcadorClick, tamanho: "10", icone: "", visibilidade: exportarImportacaoCTeEmitidoForaEmbarcadorVisible };
    let reprocessar = { descricao: "Reprocessar", id: guid(), evento: "onclick", metodo: reprocessarImportacaoCTeEmitidoForaEmbarcadorClick, tamanho: "10", icone: "", visibilidade: reprocessarImportacaoCTeEmitidoForaEmbarcadorVisible };
    let cancelar = { descricao: "Cancelar", id: guid(), evento: "onclick", metodo: cancelarImportacaoCTeEmitidoForaEmbarcadorClick, tamanho: "10", icone: "", visibilidade: cancelarImportacaoCTeEmitidoForaEmbarcadorVisible };
    let excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirImportacaoCTeEmitidoForaEmbarcadorClick, tamanho: "10", icone: "", visibilidade: excluirImportacaoCTeEmitidoForaEmbarcadorVisible };
    let menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [linhas, exportar, reprocessar, cancelar, excluir], tamanho: 10 };
    _gridImportacaoCTeEmitidoForaEmbarcador = new GridView("grid-importacao-cte-emitido-fora-embarcador", controller + "/Pesquisa", _pesquisaImportacaoCTeEmitidoForaEmbarcador, menuOpcoes, null, 10);
    _gridImportacaoCTeEmitidoForaEmbarcador.CarregarGrid();
}

//*******EVENTOS*******

function exportarImportacaoCTeEmitidoForaEmbarcadorVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoCTeEmitidoForaEmbarcador.Erro || row.Situacao == EnumSituacaoImportacaoCTeEmitidoForaEmbarcador.Sucesso);
}

function reprocessarImportacaoCTeEmitidoForaEmbarcadorVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoCTeEmitidoForaEmbarcador.Erro || row.Situacao == EnumSituacaoImportacaoCTeEmitidoForaEmbarcador.Sucesso);
}

function cancelarImportacaoCTeEmitidoForaEmbarcadorVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoTeEmitidoForaEmbarcador.Pendente);
}

function excluirImportacaoCTeEmitidoForaEmbarcadorVisible(row) {
    return (row.Situacao != EnumSituacaoImportacaoCTeEmitidoForaEmbarcador.Processando);
}


function pesquisarImportacaoCTeEmitidoForaEmbarcadorClick(e, sender) {
    loadGridImportacaoCTeEmitidoForaEmbarcador();
}

function visualizarLinhasImportacaoCTeEmitidoForaEmbarcadorClick(row) {
    Global.abrirModal('divModalImportacaoCTeEmitidoForaEmbarcadorLinha');
    let linhas = { descricao: "Colunas", id: guid(), evento: "onclick", metodo: visualizarColunasImportacaoCTeEmitidoForaEmbarcadorClick, tamanho: "10", icone: "" };
    let menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [linhas], tamanho: 10 };
    let configuracoesExportacao = { url: controller + "/ExportarLinhas?codigo=" + row.Codigo, titulo: "ImportacaoCTeEmitidoForaEmbarcadorLinhas" };
    _gridImportacaoCTeEmitidoForaEmbarcadorLinha = new GridView("grid-importacao-cte-emitido-fora-embarcador-linha", controller + "/Linhas?codigo=" + row.Codigo,
        null, menuOpcoes, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridImportacaoCTeEmitidoForaEmbarcadorLinha.CarregarGrid();
}

function exportarImportacaoCTeEmitidoForaEmbarcadorClick(row) {
    executarDownload(controller + "/Exportar", { codigo: row.Codigo });
}

function visualizarColunasImportacaoCTeEmitidoForaEmbarcadorClick(row) {
    Global.abrirModal('divModalImportacaoCTeEmitidoForaEmbarcadorColuna');
    let configuracoesExportacao = { url: controller + "/ExportarColunas?codigo=" + row.Codigo, titulo: "ImportacaoCTeEmitidoForaEmbarcadorColunas" };
    _gridImportacaoCTeEmitidoForaEmbarcadorColuna = new GridView("grid-importacao-cte-emitido-fora-embarcador-coluna", controller + "/Colunas?codigo=" + row.Codigo,
        null, null, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridImportacaoCTeEmitidoForaEmbarcadorColuna.CarregarGrid();
}

function reprocessarImportacaoCTeEmitidoForaEmbarcadorClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja reprocessar importação. ".format(row.Planilha), function () {
        executarReST(controller + "/Reprocessar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoCTeEmitidoForaEmbarcador();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function cancelarImportacaoCTeEmitidoForaEmbarcadorClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar importação. ".format(row.Planilha), function () {
        executarReST(controller + "/Cancelar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoCTeEmitidoForaEmbarcador();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function excluirImportacaoCTeEmitidoForaEmbarcadorClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir importação de CTes Terceiros".format(row.Planilha), function () {
        executarReST(controller + "/Excluir", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoCTeEmitidoForaEmbarcador();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}