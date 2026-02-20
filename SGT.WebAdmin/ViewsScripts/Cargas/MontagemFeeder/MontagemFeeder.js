/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/PedidoViagemNavio.js" />
/// <reference path="../../Consultas/Porto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoImportacaoPedido.js" />
/// <reference path="SignalRMontagemFeeder.js" />

var controller = "MontagemFeeder";
var _gridMontagemFeeder = null, _pesquisaMontagemFeeder = null, _montagemFeeder = null;

function loadMontagemFeeder() {
    _pesquisaMontagemFeeder = new PesquisaMontagemFeeder();
    KoBindings(_pesquisaMontagemFeeder, "knockoutPesquisaMontagemFeeder", false, _pesquisaMontagemFeeder.Pesquisar.id);
    new BuscarFuncionario(_pesquisaMontagemFeeder.Funcionario);

    new BuscarPedidoViagemNavio(_pesquisaMontagemFeeder.Viagem);
    new BuscarPorto(_pesquisaMontagemFeeder.PortoOrigem);
    new BuscarPorto(_pesquisaMontagemFeeder.PortoDestino);

    _montagemFeeder = new ImportarPedido();
    KoBindings(_montagemFeeder, "knockoutImportarMontagemFeeder", false, _montagemFeeder.Importar.id);

    if (_CONFIGURACAO_TMS.MontagemCarga.AtivarTratativaDuplicidadeEmissaoCargasFeeder)
        _montagemFeeder.ImportarMesmoComDocumentacaoDuplicada.visible(true);

    loadGridMontagemFeeder();
    SignalRMontagemFeeder();
}

//*******MAPEAMENTO KNOUCKOUT*******

var PesquisaMontagemFeeder = function () {
    this.Planilha = PropertyEntity({ text: "Planilha:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data inicial: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data final: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ text: "Funcionário:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoImportacaoPedido.Todas), options: EnumSituacaoImportacaoPedido.obterOpcoesPesquisa(), def: true, visible: ko.observable(true) });
    this.Mensagem = PropertyEntity({ text: "Mensagem:", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Viagem = PropertyEntity({ text: "Viagem:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.PortoOrigem = PropertyEntity({ text: "Porto Origem:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.PortoDestino = PropertyEntity({ text: "Porto Destino:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroBooking = PropertyEntity({ text: "Nº Booking:", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, idGrid: guid(), visible: ko.observable(true), eventClick: pesquisarMontagemFeederClick });
    this.ExibirFiltros = PropertyEntity({ text: "Filtros de Pesquisa", type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); } });
};

var ImportarPedido = function () {
    this.Exibir = PropertyEntity({ text: "Importar planilha", type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), eventClick: function (e) { e.Exibir.visibleFade(!e.Exibir.visibleFade()); } });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Arquivos:", val: ko.observable("") });
    this.ImportarMesmoSemCTeAbsorvidoAnteriormente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Importar a(s) planilha(s) mesmo sem ter absorvido os CT-e(s) anteriormente?", enable: ko.observable(true), visible: ko.observable(true) });
    this.ImportarMesmoComDocumentacaoDuplicada = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Importar a(s) planilha(s) cujo container apresentou duplicidade na emissão?", enable: ko.observable(true), visible: ko.observable(false) });

    this.Importar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) {
        if (novoValor != "") {
            var file = document.getElementById(_montagemFeeder.Arquivo.id);
            novoValor = "";
            for (var i = 0; i < file.files.length; i++) {
                novoValor = novoValor + file.files[i].name + "; ";
            }
            _montagemFeeder.Arquivo.val(novoValor);
        }
    });
};

function loadGridMontagemFeeder() {
    var reprocessar = { descricao: "Reprocessar", id: guid(), evento: "onclick", metodo: reprocessarMontagemFeederClick, tamanho: "10", icone: "", visibilidade: reprocessarMontagemFeederVisible };
    var cancelar = { descricao: "Cancelar", id: guid(), evento: "onclick", metodo: cancelarMontagemFeederClick, tamanho: "10", icone: "", visibilidade: cancelarMontagemFeederVisible };
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirMontagemFeederClick, tamanho: "10", icone: "", visibilidade: excluirMontagemFeederVisible };
    var detalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: detalhesMontagemFeederClick, tamanho: "10", icone: "", visibilidade: excluirMontagemFeederVisible };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [reprocessar, cancelar, excluir, detalhes], tamanho: 10 };

    var configExportacao = {
        url: controller + "/ExportarPesquisa",
        titulo: "Feeder"
    };

    _gridMontagemFeeder = new GridViewExportacao("grid-importacao-feeder", controller + "/Pesquisa", _pesquisaMontagemFeeder, menuOpcoes, configExportacao, null, 30);
    
    _gridMontagemFeeder.CarregarGrid();

    
}

//*******EVENTOS*******

function reprocessarMontagemFeederVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Sucesso);
}

function cancelarMontagemFeederVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoPedido.Pendente);
}

function excluirMontagemFeederVisible(row) {
    return (row.Situacao != EnumSituacaoImportacaoPedido.Processando);
}

function pesquisarMontagemFeederClick(e, sender) {
    loadGridMontagemFeeder();
}

function importarClick(e, sender) {

    var file = document.getElementById(_montagemFeeder.Arquivo.id);

    var formData = new FormData();
    for (var i = 0; i < file.files.length; i++) {
        formData.append("upload", file.files[i]);
    }

    enviarArquivo(controller + "/Importar?callback=?", { ImportarMesmoSemCTeAbsorvidoAnteriormente: _montagemFeeder.ImportarMesmoSemCTeAbsorvidoAnteriormente.val(), ImportarMesmoComDocumentacaoDuplicada: _montagemFeeder.ImportarMesmoComDocumentacaoDuplicada.val() }, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo importado com sucesso, por favor aguarde a geração da(s) carga(s)");
                _montagemFeeder.Arquivo.val("");
                loadGridMontagemFeeder();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", "Algumas planilhas possuem problemas na importação.");
                $("#knoutAreaImportacao").before('<p class="alert alert-info no-margin alert-dismissible"><button class="btn-close" data-bs-dismiss="alert">×</button><i class="fal fa-info me-2"></i><strong>Atenção!</strong> Alguns registros não foram importados:<br/>' + arg.Msg.replace(/\n/g, "<br />") + '</p>');
                _montagemFeeder.Arquivo.val("");
                loadGridMontagemFeeder();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function reprocessarMontagemFeederClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja reprocessar a importação dos feeders da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Reprocessar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridMontagemFeeder();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function cancelarMontagemFeederClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar a importação dos feeders da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Cancelar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridMontagemFeeder();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function excluirMontagemFeederClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a importação de feeders da planilha \"" + row.Planilha + "\"?", function () {
        executarReST(controller + "/Excluir", { Codigo: row.Codigo }, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridMontagemFeeder();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function detalhesMontagemFeederClick(row) {
    Global.abrirModal("divModalDetalheMensagemRetorno");
    $("#pMensagemAlerta").remove();
    $("#contentDetalheMensagemRetorno").before('<p id="pMensagemAlerta" class="alert alert-info no-margin alert-dismissible"><button class="btn-close" data-bs-dismiss="alert">×</button><i class="fal fa-info me-2"></i><strong>Atenção!</strong> Retorno da importação:<br/>' + row.Mensagem.replace(/\n/g, "<br />") + '</p>');
}