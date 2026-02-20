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
/// <reference path="VendaDireta.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _vendaDiretaBoleto;
var _gridVendaDiretaBoleto;

var VendaDiretaBoleto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AtualizarBoletos = PropertyEntity({ eventClick: AtualizarBoletosClick, type: types.event, text: "Atualizar Boletos", visible: ko.observable(true), enable: ko.observable(true) });

    this.Grid = PropertyEntity({ idGrid: guid() });

    this.Mensagem = PropertyEntity({ text: "Mensagem para E-mail:", maxlength: 5000, val: ko.observable(""), enable: ko.observable(true) });
    this.EnviarEmail = PropertyEntity({ eventClick: enviarEmailVendaDiretaClick, type: types.event, text: "Enviar Email", enable: ko.observable(true), visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadVendaDiretaBoleto() {
    _vendaDiretaBoleto = new VendaDiretaBoleto();
    KoBindings(_vendaDiretaBoleto, "knockoutVendaDiretaBoleto");

    var download = { descricao: "Download", id: "clasEditar", evento: "onclick", metodo: DownloadBoletoClick, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(download);
    var header = [
        { data: "Codigo", visible: false },
        { data: "BoletoStatusTitulo", visible: false },
        { data: "Pessoa", title: "Pessoa", width: "28%", className: "text-align-left" },
        { data: "DescricaoStatusBoleto", title: "Status Boleto", width: "10%", className: "text-align-left" },
        { data: "DataEmissao", title: "Data Emissão", width: "10%", className: "text-align-center" },
        { data: "DataVencimento", title: "Data Vencimento", width: "10%", className: "text-align-center" },
        { data: "Valor", title: "Valor", width: "8%", className: "text-align-right" },
        { data: "NossoNumero", title: "Nosso Número", width: "8%", className: "text-align-center" },
        { data: "CaminhoBoleto", visible: false }
    ];

    _gridVendaDiretaBoleto = new BasicDataTable(_vendaDiretaBoleto.Grid.id, header, menuOpcoes);
    recarregarGridVendaDiretaBoleto({ ListaTitulos: [] });
}

function DownloadBoletoClick(e, sender) {
    var dados = { Codigo: e.Codigo }
    executarDownload("TituloFinanceiro/DownloadBoleto", dados);
}

function AtualizarBoletosClick(e, sender) {
    var data = { Codigo: _vendaDireta.Codigo.val() };
    executarReST("VendaDireta/AtualizarBoletos", data, function (arg) {
        if (arg.Success) {
            recarregarGridVendaDiretaBoleto(arg.Data);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function enviarEmailVendaDiretaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja enviar o(s) boleto(s) ao cliente?", function () {
        var data = { Codigo: _vendaDireta.Codigo.val(), MensagemEmail: _vendaDiretaBoleto.Mensagem.val() };
        executarReST("VendaDireta/EnviarEmailBoletos", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de envio de e-mails concluído.");
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function recarregarGridVendaDiretaBoleto(data) {
    var dataGrid = new Array();

    $.each(data.ListaTitulos, function (i, titulo) {
        var obj = new Object();
        obj.Codigo = titulo.Codigo;
        obj.BoletoStatusTitulo = titulo.BoletoStatusTitulo;
        obj.Pessoa = titulo.Pessoa;
        obj.DescricaoStatusBoleto = titulo.DescricaoStatusBoleto;
        obj.DataEmissao = titulo.DataEmissao;
        obj.DataVencimento = titulo.DataVencimento;
        obj.Valor = titulo.Valor;
        obj.NossoNumero = titulo.NossoNumero;
        obj.CaminhoBoleto = titulo.CaminhoBoleto;

        dataGrid.push(obj);
    });

    _gridVendaDiretaBoleto.CarregarGrid(dataGrid);
}