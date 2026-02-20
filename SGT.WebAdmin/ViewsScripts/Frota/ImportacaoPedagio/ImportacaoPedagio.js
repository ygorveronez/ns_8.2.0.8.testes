/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoImportacaoPedagio.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />

var controller = "ImportacaoPedagio";

var _gridImportacaoPedagio,
    _pesquisaImportacaoPedagio,
    _importarPedagio,
    _informacaoImportacaoSelecionada;

var _gridImportacaoPedagioLinha,
    _gridImportacaoPedagioColuna;

var PesquisaImportacaoPedagio = function () {
    this.Planilha = PropertyEntity({ text: "Planilha:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data inicial: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data final: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ text: "Funcionário:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoImportacaoPedagio.Todas), options: EnumSituacaoImportacaoPedagio.obterOpcoesPesquisa(), def: true, visible: ko.observable(true) });
    this.Mensagem = PropertyEntity({ text: "Mensagem:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, idGrid: guid(), visible: ko.observable(true), eventClick: pesquisarImportacaoPedagioClick });
    this.ExibirFiltros = PropertyEntity({ text: "Filtros de Pesquisa", type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); } });
};

var InformacaoImportacaoSelecionada = function () {
    this.CodigoImportacao = PropertyEntity({ getType: typesKnockout.int, def: 0, val: ko.observable(0) });
    this.CodigoLinha = PropertyEntity({ getType: typesKnockout.int, def: 0, val: ko.observable(0) });
}

var ImportarPedagio = function () {
    this.Exibir = PropertyEntity({ text: "Importar planilha", type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.Exibir.visibleFade(!e.Exibir.visibleFade()); } });
    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        UrlImportacao: controller + "/Importar",
        UrlConfiguracao: controller + "/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O047_ImportacaoPedagio,
        CallbackImportacao: function () {
            _gridImportacaoPedagio.CarregarGrid();
        }
    });
};

function loadImportacaoPedagio() {
    _pesquisaImportacaoPedagio = new PesquisaImportacaoPedagio();
    KoBindings(_pesquisaImportacaoPedagio, "knockoutPesquisaImportacaoPedagio", false, _pesquisaImportacaoPedagio.Pesquisar.id);
    new BuscarFuncionario(_pesquisaImportacaoPedagio.Funcionario);

    _importarPedagio = new ImportarPedagio();
    KoBindings(_importarPedagio, "knockoutImportarImportacaoPedagio", false, _importarPedagio.Importar.id);

    _informacaoImportacaoSelecionada = new InformacaoImportacaoSelecionada();

    loadGridImportacaoPedagio();
    loadGridImportacaoPedagioLinhas();
    loadGridImportacaoPedagioColunas();
}

function loadGridImportacaoPedagio() {
    var linhas = { descricao: "Visualizar linhas", id: guid(), evento: "onclick", metodo: visualizarLinhasImportacaoPedagioClick, tamanho: "10", icone: "" };
    var exportar = { descricao: "Exportar", id: guid(), evento: "onclick", metodo: exportarImportacaoPedagioClick, tamanho: "10", icone: "", visibilidade: exportarImportacaoPedagioVisible };
    var reprocessar = { descricao: "Reprocessar", id: guid(), evento: "onclick", metodo: reprocessarImportacaoPedagioClick, tamanho: "10", icone: "", visibilidade: reprocessarImportacaoPedagioVisible };
    var cancelar = { descricao: "Cancelar", id: guid(), evento: "onclick", metodo: cancelarImportacaoPedagioClick, tamanho: "10", icone: "", visibilidade: cancelarImportacaoPedagioVisible };
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirImportacaoPedagioClick, tamanho: "10", icone: "", visibilidade: excluirImportacaoPedagioVisible };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [linhas, exportar, reprocessar, cancelar, excluir], tamanho: 10 };

    _gridImportacaoPedagio = new GridView("grid-importacao-pedagio", controller + "/Pesquisa", _pesquisaImportacaoPedagio, menuOpcoes, null, 10);
    _gridImportacaoPedagio.CarregarGrid();
}

function loadGridImportacaoPedagioLinhas() {
    var linhas = { descricao: "Colunas", id: guid(), evento: "onclick", metodo: visualizarColunasImportacaoPedagioClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [linhas], tamanho: 10 };

    _gridImportacaoPedidoLinha = new GridView("grid-importacao-pedagio-linha", controller + "/Linhas", _informacaoImportacaoSelecionada, menuOpcoes, null, 10, null, true, null, null, 10000, true, null, null, true, null, false);
    _gridImportacaoPedidoLinha.CarregarGrid();
}

function loadGridImportacaoPedagioColunas() {
    _gridImportacaoPedidoColuna = new GridView("grid-importacao-pedagio-coluna", controller + "/Colunas", _informacaoImportacaoSelecionada, null, null, 10, null, true, null, null, 10000, true, null, null, true, null, false);
    _gridImportacaoPedidoColuna.CarregarGrid();
}

function exportarImportacaoPedagioVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedagio.Erro || row.Situacao == EnumSituacaoImportacaoPedagio.Sucesso);
}

function reprocessarImportacaoPedagioVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedagio.Erro || row.Situacao == EnumSituacaoImportacaoPedagio.Sucesso);
}

function cancelarImportacaoPedagioVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedagio.Pendente);
}

function excluirImportacaoPedagioVisible(row) {
    return (row.Situacao != EnumSituacaoImportacaoPedagio.Processando);
}

function pesquisarImportacaoPedagioClick() {
    _gridImportacaoPedagio.CarregarGrid();
}

function visualizarLinhasImportacaoPedagioClick(row) {
    _informacaoImportacaoSelecionada.CodigoImportacao.val(row.Codigo);

    _gridImportacaoPedidoLinha.CarregarGrid();

    $("#divModalImportacaoPedagioLinha")
        .modal("show")
        .on('hidden.bs.modal', function () {
            LimparCampos(_informacaoImportacaoSelecionada);
        });
}

function exportarImportacaoPedagioClick(row) {
    executarDownload(controller + "/Exportar", { codigo: row.Codigo });
}

function visualizarColunasImportacaoPedagioClick(row) {
    _informacaoImportacaoSelecionada.CodigoLinha.val(row.Codigo);

    _gridImportacaoPedidoColuna.CarregarGrid();

    Global.abrirModal('divModalImportacaoPedagioColuna');
}

function reprocessarImportacaoPedagioClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja reprocessar a importação dos pedidos da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Reprocessar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                pesquisarImportacaoPedagioClick();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function cancelarImportacaoPedagioClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar a importação dos pedidos da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Cancelar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                pesquisarImportacaoPedagioClick();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function excluirImportacaoPedagioClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a importação de pedidos da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Excluir", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                pesquisarImportacaoPedagioClick();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}