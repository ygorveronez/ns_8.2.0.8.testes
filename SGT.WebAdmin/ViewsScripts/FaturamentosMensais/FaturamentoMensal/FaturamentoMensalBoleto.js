/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="FaturamentoMensalEtapa.js" />
/// <reference path="FaturamentoMensalDocumento.js" />
/// <reference path="FaturamentoMensal.js" />
/// <reference path="FaturamentoMensalEnvioEmail.js" />
/// <reference path="../../Enumeradores/EnumEtapaFaturamentoMensal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _geracaoBoletos;
var _gridGeracaoBoletos;

var GeracaoBoletos = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Etapa = PropertyEntity({ val: ko.observable(EnumEtapaFaturamentoMensal.Etapa3), def: EnumEtapaFaturamentoMensal.Etapa3, getType: typesKnockout.int });

    this.FaturamentoParaGeracaoBoleto = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.GerarBoletos = PropertyEntity({ eventClick: GerarBoletosClick, type: types.event, text: "Gerar Boletos", visible: ko.observable(false), enable: ko.observable(true) });
    this.AtualizarDocumentos = PropertyEntity({ eventClick: AtualizarDocumentosBoletoClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.ListaFaturamento = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Proximo = PropertyEntity({ eventClick: ProximoGeracaoBoletoClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparCamposFaturamentoMensal, type: types.event, text: "Limpar/Novo Faturamento", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadGeracaoBoletos() {
    _geracaoBoletos = new GeracaoBoletos();
    KoBindings(_geracaoBoletos, "knockoutGeracaoBoleto");
}

function AtualizarDocumentosBoletoClick(e, sender) {
    buscarFaturamentoMensalBoleto();
}

function ProximoGeracaoBoletoClick(e, sender) {
    if (_geracaoBoletos == undefined) {
        exibirMensagem(tipoMensagem.aviso, "Selecione os Faturamentos", "Retorne a etapa anterior para a geração dos documentos.");
        return;
    }

    var data = { Codigo: _geracaoBoletos.Codigo.val() };
    executarReST("FaturamentoMensal/IniciarEnvioEmail", data, function (arg) {
        if (arg.Success) {
            _selecaoFaturamento.Status.val(EnumStatusFaturamentoMensal.GeradoBoletos);
            $("#knockoutEnvioEmail").show();
            _etapaAtual = 4;
            $("#" + _etapaFaturamentoMensal.Etapa4.idTab).click();
            buscarTitulosParaEnvioEmail();

            exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa geração de documentos concluída, siga a etapa 4.");
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

function GerarBoletosClick(e, sender) {
    if (_geracaoBoletos == undefined) {
        exibirMensagem(tipoMensagem.aviso, "Geração de Documentos", "Retorne a etapa anterior para a geração dos documentos.");
        return;
    }
    var data = { Codigo: _geracaoDocumentos.Codigo.val() };
    executarReST("FaturamentoMensal/GerarBoletoFaturamentoMensal", data, function (arg) {
        if (arg.Success) {
            buscarFaturamentoMensalBoleto();
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo realizado com sucesso.");
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function buscarFaturamentoMensalBoleto() {
    if (_geracaoBoletos.Codigo.val() > 0) {
        var somenteLeitura = false;

        _geracaoBoletos.AtualizarDocumentos.visible(true);
        _geracaoBoletos.GerarBoletos.visible(true);
        _geracaoBoletos.SelecionarTodos.visible(false);
        _geracaoBoletos.SelecionarTodos.val(false);

        var multiplaescolha = {
            basicGrid: null,
            eventos: function () { },
            selecionados: new Array(),
            naoSelecionados: new Array(),
            SelecionarTodosKnout: _geracaoBoletos.SelecionarTodos,
            somenteLeitura: somenteLeitura,
        }

        var download = { descricao: "Download", id: "clasEditar", evento: "onclick", metodo: DownloadBoletoClick, tamanho: "10", icone: "" };
        var menuOpcoes = new Object();
        menuOpcoes.tipo = TypeOptionMenu.link;
        menuOpcoes.opcoes = new Array();
        menuOpcoes.opcoes.push(download);

        _gridGeracaoBoletos = new GridView(_geracaoBoletos.FaturamentoParaGeracaoBoleto.idGrid, "FaturamentoMensal/PesquisaFaturamentoMensal", _geracaoBoletos, menuOpcoes, null, 50, null, null, null, null);
        _gridGeracaoBoletos.CarregarGrid(function (arg) {
            if (arg.data !== false) {
                var data = { Codigo: _geracaoBoletos.Codigo.val() };
                executarReST("FaturamentoMensal/TodosBoletosGeradosFaturamentoMensal", data, function (arg) {
                    if (!arg.Data) {
                        _geracaoBoletos.GerarBoletos.visible(true);
                    } else {
                        _geracaoBoletos.GerarBoletos.visible(false);
                    }
                });
            }
        });
    } else
        $("#knockoutGeracaoBoleto").hide();
}

function DownloadBoletoClick(e, sender) {
    var dados = { Codigo: e.CodigoTitulo }
    executarDownload("TituloFinanceiro/DownloadBoleto", dados);
}

function limparCamposFaturamentoMensalBoleto() {
    LimparCampos(_geracaoBoletos);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}