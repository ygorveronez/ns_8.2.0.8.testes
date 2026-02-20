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
/// <reference path="BoletoGeracaoEtapa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _geracaoFrancesinha;
var _gridGeracaoFrancesinha;

var GeracaoFrancesinha = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TitulosParaFrancesinha = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.GerarFrancesinha = PropertyEntity({ eventClick: DownloadFrancesinhaClick, type: types.event, text: "Download Francesinha(s)", visible: ko.observable(true), enable: ko.observable(true) });
    this.TotalizadorValorSelecionado = PropertyEntity({ text: "Valor total títulos: ", getType: typesKnockout.string, def: "0,00", val: ko.observable("0,00"), enable: ko.observable(true), visible: ko.observable(true) });

    this.Finalizar = PropertyEntity({ eventClick: FinalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadGeracaoFrancesinha() {
    _geracaoFrancesinha = new GeracaoFrancesinha();
    KoBindings(_geracaoFrancesinha, "knockoutFrancesinha");

    var download = { descricao: "Download", id: "clasEditar", evento: "onclick", metodo: DownloadFrancesinhaTituloClick, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(download);

    var header = [
        { data: "Codigo", title: "Título", width: "8%", className: "text-align-right" },
        { data: "CodigoRemessa", visible: false },
        { data: "BoletoStatusTitulo", visible: false },
        { data: "Pessoa", title: "Pessoa", width: "20%", className: "text-align-left" },
        { data: "DescricaoStatusBoleto", title: "Status Boleto", width: "10%", className: "text-align-left" },
        { data: "DataEmissao", title: "Data Emissão", width: "8%", className: "text-align-center" },
        { data: "DataVencimento", title: "Data Vencimento", width: "8%", className: "text-align-center" },
        { data: "Valor", title: "Valor", width: "8%", className: "text-align-right" },
        { data: "NossoNumero", title: "Nosso Número", width: "8%", className: "text-align-center" },
        { data: "NumeroRemessa", title: "Número Remessa", width: "8%", className: "text-align-center" },
        { data: "CaminhoBoleto", visible: false }
    ];

    _gridGeracaoFrancesinha = new BasicDataTable(_geracaoFrancesinha.TitulosParaFrancesinha.idGrid, header, menuOpcoes);
}

function DownloadFrancesinhaTituloClick(e, sender) {
    var listaTitulo = new Array();
    listaTitulo.push({ Titulo: e });

    var data = { ListaTitulos: JSON.stringify(listaTitulo) };

    executarReST("BoletoGeracao/DownloadFrancesinha", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function DownloadFrancesinhaClick(e, sender) {
    var data = { ListaTitulos: PreencherListaCodigosTitulos() };
    executarReST("BoletoGeracao/DownloadFrancesinha", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function FinalizarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar o processo de geração de boletos?", function () {
        location.reload();
    });
}

//*******MÉTODOS*******

function PreencherListaCodigosTitulos() {
    var listaTitulo = new Array();

    $.each(_gridGeracaoFrancesinha.BuscarRegistros(), function (i, titulo) {
        listaTitulo.push({ Titulo: titulo });
    });

    return JSON.stringify(listaTitulo);
}

function limparCamposFatura() {
    LimparCampos(_geracaoFrancesinha);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}