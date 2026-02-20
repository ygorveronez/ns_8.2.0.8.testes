/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoImportacaoPedido.js" />

var _gridRolagemContainer, _gridDetalheRolagemContainer, _pesquisaRolagemContainer, _rolagemContainer, _gridColunaRolagemContainer;

function loadRolagemContainer() {
    _pesquisaRolagemContainer = new PesquisaRolagemContainer();
    KoBindings(_pesquisaRolagemContainer, "knockoutPesquisaRolagemContainer", false, _pesquisaRolagemContainer.Pesquisar.id);

    _rolagemContainer = new ImportarRolagemContainer();
    KoBindings(_rolagemContainer, "knockoutImportarRolagemContainer", false, _rolagemContainer.Importar.id);

    new BuscarFuncionario(_pesquisaRolagemContainer.Funcionario);

    LoadGridRolagemContainer();
}

//*******MAPEAMENTO KNOUCKOUT*******

var PesquisaRolagemContainer = function () {
    this.Planilha = PropertyEntity({ text: "Planilha:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data inicial: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data final: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ text: "Funcionário:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoImportacaoPedido.Todas), options: EnumSituacaoImportacaoPedido.obterOpcoesPesquisa(), def: true, visible: ko.observable(true) });
    this.Mensagem = PropertyEntity({ text: "Mensagem:", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, idGrid: guid(), visible: ko.observable(true), eventClick: pesquisarRolagemContainerClick });
    this.ExibirFiltros = PropertyEntity({ text: "Filtros de Pesquisa", type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); } });
};

var ImportarRolagemContainer = function () {
    this.Exibir = PropertyEntity({ text: "Importar planilha", type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.Exibir.visibleFade(!e.Exibir.visibleFade()); } });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Arquivos:", val: ko.observable("") });
    this.ImportarMesmoSemCTeAbsorvidoAnteriormente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Importar a(s) planilha(s) mesmo sem ter absorvido os CT-e(s) anteriormente?", enable: ko.observable(true), visible: ko.observable(true) });
    this.ImportarMesmoComDocumentacaoDuplicada = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Importar a(s) planilha(s) cujo container apresentou duplicidade na emissão?", enable: ko.observable(true), visible: ko.observable(false) });

    this.Importar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) {
        if (novoValor != "") {
            var file = document.getElementById(_rolagemContainer.Arquivo.id);
            novoValor = "";
            for (var i = 0; i < file.files.length; i++) {
                novoValor = novoValor + file.files[i].name + "; ";
            }
            _rolagemContainer.Arquivo.val(novoValor);
        }
    });
};

function LoadGridRolagemContainer() {
    let reprocessar = { descricao: "Reprocessar", id: guid(), evento: "onclick", metodo: reprocessarRolagemContainerClick, tamanho: "10", icone: "fal fa-sync", visibilidade: reprocessarRolagemContainerVisible };
    let exportar = { descricao: "Exportar", id: guid(), evento: "onclick", metodo: exportarRolagemContainerClick, tamanho: "10", icone: "fal fa-upload", visibilidade: exportarRolagemContainerVisible };
    let excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirRolagemContainerClick, tamanho: "10", icone: "fal fa-trash", visibilidade: excluirRolagemContainerVisible };
    let detalhes = { descricao: "Visualizar Linhas", id: guid(), evento: "onclick", metodo: detalhesRolagemContainerClick, tamanho: "10", icone: "fal fa-search", visibilidade: excluirRolagemContainerVisible };
    let menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [reprocessar, exportar, excluir, detalhes], tamanho: 10 };

    let configExportacao = {
        url: "RolagemContainer/ExportarPesquisa",
        titulo: "RolagemContainer"
    };

    _gridRolagemContainer = new GridViewExportacao("grid-rolagem-container", "RolagemContainer/Pesquisa", _pesquisaRolagemContainer, menuOpcoes, configExportacao, null, 30);
    _gridRolagemContainer.CarregarGrid();
}

function LoadGridDetalheRolagemContainer() {
    let linhas = { descricao: "Colunas", id: guid(), evento: "onclick", metodo: visualizarColunasImportacaoRolagemContainerClick, tamanho: "10", icone: "" };
    let menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [linhas], tamanho: 10 };
    let configExportacao = {
        url: "RolagemContainer/ExportarPesquisaDetalhes",
        titulo: "RolagemContainer"
    };

    Global.abrirModal("divModalDetalheRetorno");

    _gridDetalheRolagemContainer = new GridViewExportacao("grid-detalhe-rolagem-container", "RolagemContainer/PesquisaDetalhes", _pesquisaRolagemContainer, menuOpcoes, configExportacao, null, 30);
    _gridDetalheRolagemContainer.CarregarGrid();
}

function visualizarColunasImportacaoRolagemContainerClick(row) {
    var configuracoesExportacao = {
        url: "RolagemContainer/ExportarColunas?codigo=" + row.Codigo,
        titulo: "ImportacaoRolagemContainerColunas"
    };

    Global.abrirModal('divModalImportacaoRolagemContainerColuna');

    _gridColunaRolagemContainer = new GridView("grid-coluna-rolagem-container", "RolagemContainer/Colunas?codigo=" + row.Codigo, null, null, null, 15, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridColunaRolagemContainer.CarregarGrid();
}

//*******EVENTOS*******

function reprocessarRolagemContainerVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Sucesso || row.Situacao == EnumSituacaoImportacaoPedido.Erro);
}

function exportarRolagemContainerVisible(row) {
    return true;
}

function excluirRolagemContainerVisible(row) {
    return (row.Situacao != EnumSituacaoImportacaoPedido.Processando);
}

function pesquisarRolagemContainerClick(e, sender) {
    LoadGridRolagemContainer();
}

function importarClick(e, sender) {
    var file = document.getElementById(_rolagemContainer.Arquivo.id);

    var formData = new FormData();
    for (var i = 0; i < file.files.length; i++) {
        formData.append("upload", file.files[i]);
    }

    $("#pAlert").remove();
    enviarArquivo("RolagemContainer/Importar?callback=?", null, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo importado com sucesso, por favor aguarde a geração da(s) carga(s)");
                _rolagemContainer.Arquivo.val("");
                LoadGridRolagemContainer();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", "Algumas planilhas possuem problemas na importação.");
                $("#knoutAreaImportacao").before('<p id="pAlert" class="alert alert-info no-margin alert-dismissible"><button class="btn-close" data-bs-dismiss="alert">×</button><i class="fal fa-info me-2"></i><strong>Atenção!</strong> Alguns registros não foram importados:<br/>' + arg.Msg.replace(/\n/g, "<br />") + '</p>');
                _rolagemContainer.Arquivo.val("");
                LoadGridRolagemContainer();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function reprocessarRolagemContainerClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja reprocessar a importação dos feeders da planilha \"" + row.Planilha + "\"?", function () {
        executarReST("RolagemContainer/Reprocessar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                LoadGridRolagemContainer();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function exportarRolagemContainerClick(row) {
    executarDownload("RolagemContainer/Download", { Codigo: row.Codigo });
}

function excluirRolagemContainerClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a importação de feeders da planilha \"" + row.Planilha + "\"?", function () {
        executarReST("RolagemContainer/Excluir", { Codigo: row.Codigo }, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                LoadGridRolagemContainer();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function detalhesRolagemContainerClick(row) {

    _pesquisaRolagemContainer.Codigo.val(row.Codigo);
    LoadGridDetalheRolagemContainer();
}