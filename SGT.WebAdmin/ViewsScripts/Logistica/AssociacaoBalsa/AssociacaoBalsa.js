/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoImportacaoPedido.js" />

var _gridAssociacaoBalsa, _gridDetalheAssociacaoBalsa, _pesquisaAssociacaoBalsa, _importarAssociacaoBalsa, _gridColunaAssociacaoBalsa;

function loadAssociacaoBalsa() {
    _pesquisaAssociacaoBalsa = new PesquisaAssociacaoBalsa();
    KoBindings(_pesquisaAssociacaoBalsa, "knockoutPesquisaAssociacaoBalsa", false, _pesquisaAssociacaoBalsa.Pesquisar.id);

    _importarAssociacaoBalsa = new ImportarAssociacaoBalsa();
    KoBindings(_importarAssociacaoBalsa, "knockoutImportarAssociacaoBalsa", false, _importarAssociacaoBalsa.Importar.id);

    new BuscarFuncionario(_pesquisaAssociacaoBalsa.Funcionario);

    LoadGridAssociacaoBalsa();
}

//*******MAPEAMENTO KNOUCKOUT*******

var PesquisaAssociacaoBalsa = function () {
    this.Planilha = PropertyEntity({ text: "Planilha:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data inicial: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data final: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ text: "Funcionário:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoImportacaoPedido.Todas), options: EnumSituacaoImportacaoPedido.obterOpcoesPesquisa(), def: true, visible: ko.observable(true) });
    this.Mensagem = PropertyEntity({ text: "Mensagem:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Booking = PropertyEntity({ text: "Booking:", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, idGrid: guid(), visible: ko.observable(true), eventClick: pesquisarAssociacaoBalsaClick });
    this.ExibirFiltros = PropertyEntity({ text: "Filtros de Pesquisa", type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); } });
};

var ImportarAssociacaoBalsa = function () {
    this.Exibir = PropertyEntity({ text: "Importar planilha", type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.Exibir.visibleFade(!e.Exibir.visibleFade()); } });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Arquivos:", val: ko.observable("") });

    this.Importar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) {
        if (novoValor != "") {
            var file = document.getElementById(_importarAssociacaoBalsa.Arquivo.id);
            novoValor = "";
            for (var i = 0; i < file.files.length; i++) {
                novoValor = novoValor + file.files[i].name + "; ";
            }
            _importarAssociacaoBalsa.Arquivo.val(novoValor);
        }
    });
};

function LoadGridAssociacaoBalsa() {
    let reprocessar = { descricao: "Reprocessar", id: guid(), evento: "onclick", metodo: reprocessarAssociacaoBalsaClick, tamanho: "10", icone: "fal fa-sync", visibilidade: reprocessarAssociacaoBalsaVisible };
    let exportar = { descricao: "Exportar", id: guid(), evento: "onclick", metodo: exportarAssociacaoBalsaClick, tamanho: "10", icone: "fal fa-upload", visibilidade: exportarAssociacaoBalsaVisible };
    let excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirAssociacaoBalsaClick, tamanho: "10", icone: "fal fa-trash", visibilidade: excluirAssociacaoBalsaVisible };
    let detalhes = { descricao: "Visualizar Linhas", id: guid(), evento: "onclick", metodo: detalhesAssociacaoBalsaClick, tamanho: "10", icone: "fal fa-search", visibilidade: excluirAssociacaoBalsaVisible };
    let menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [reprocessar, exportar, excluir, detalhes], tamanho: 10 };

    let configExportacao = {
        url: "AssociacaoBalsa/ExportarPesquisa",
        titulo: "Exportar Associação Balsa"
    };

    _gridAssociacaoBalsa = new GridViewExportacao("grid-associacao-balsa", "AssociacaoBalsa/Pesquisa", _pesquisaAssociacaoBalsa, menuOpcoes, configExportacao, null, 30);
    _gridAssociacaoBalsa.CarregarGrid();
}

function LoadGridDetalheAssociacaoBalsa() {
    let linhas = { descricao: "Colunas", id: guid(), evento: "onclick", metodo: visualizarColunasImportacaoAssociacaoBalsaClick, tamanho: "10", icone: "" };
    let menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [linhas], tamanho: 10 };
    let configExportacao = {
        url: "AssociacaoBalsa/ExportarPesquisaDetalhes",
        titulo: "AssociacaoBalsa"
    };

    Global.abrirModal("divModalDetalheRetorno");

    _gridDetalheAssociacaoBalsa = new GridViewExportacao("grid-detalhe-associacao-balsa", "AssociacaoBalsa/PesquisaDetalhes", _pesquisaAssociacaoBalsa, menuOpcoes, configExportacao, null, 30);
    _gridDetalheAssociacaoBalsa.CarregarGrid();
}

function visualizarColunasImportacaoAssociacaoBalsaClick(row) {
    var configuracoesExportacao = {
        url: "AssociacaoBalsa/ExportarColunas?codigo=" + row.Codigo,
        titulo: "ImportacaoAssociacaoBalsaColunas"
    };

    Global.abrirModal('divModalImportacaoAssociacaoBalsaColuna');

    _gridColunaRolagemContainer = new GridView("grid-coluna-associacao-balsa", "AssociacaoBalsa/Colunas?codigo=" + row.Codigo, null, null, null, 15, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridColunaRolagemContainer.CarregarGrid();
}

//*******EVENTOS*******

function reprocessarAssociacaoBalsaVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Sucesso || row.Situacao == EnumSituacaoImportacaoPedido.Erro);
}

function exportarAssociacaoBalsaVisible(row) {
    return true;
}

function excluirAssociacaoBalsaVisible(row) {
    return (row.Situacao != EnumSituacaoImportacaoPedido.Processando);
}

function pesquisarAssociacaoBalsaClick(e, sender) {
    LoadGridAssociacaoBalsa();
}

function importarClick(e, sender) {
    var file = document.getElementById(_importarAssociacaoBalsa.Arquivo.id);

    var formData = new FormData();
    for (var i = 0; i < file.files.length; i++) {
        formData.append("upload", file.files[i]);
    }

    $("#pAlert").remove();
    enviarArquivo("AssociacaoBalsa/Importar?callback=?", null, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo importado com sucesso, por favor aguarde a geração da(s) carga(s)");
                _importarAssociacaoBalsa.Arquivo.val("");
                LoadGridAssociacaoBalsa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", "Algumas planilhas possuem problemas na importação.");
                $("#knoutAreaImportacao").before('<p id="pAlert" class="alert alert-info no-margin alert-dismissible"><button class="btn-close" data-bs-dismiss="alert">×</button><i class="fal fa-info me-2"></i><strong>Atenção!</strong> Alguns registros não foram importados:<br/>' + arg.Msg.replace(/\n/g, "<br />") + '</p>');
                _importarAssociacaoBalsa.Arquivo.val("");
                LoadGridAssociacaoBalsa();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function reprocessarAssociacaoBalsaClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja reprocessar a importação de balsas da planilha \"" + row.Planilha + "\"?", function () {
        executarReST("AssociacaoBalsa/Reprocessar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                LoadGridAssociacaoBalsa();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function exportarAssociacaoBalsaClick(row) {
    executarDownload("AssociacaoBalsa/Download", { Codigo: row.Codigo });
}

function excluirAssociacaoBalsaClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a importação de balsas da planilha \"" + row.Planilha + "\"?", function () {
        executarReST("AssociacaoBalsa/Excluir", { Codigo: row.Codigo }, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                LoadGridAssociacaoBalsa();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function detalhesAssociacaoBalsaClick(row) {
    _pesquisaAssociacaoBalsa.Codigo.val(row.Codigo);
    LoadGridDetalheAssociacaoBalsa();
}