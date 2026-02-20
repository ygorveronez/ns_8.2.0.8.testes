/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
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
/// <reference path="CobrancaSimplesEtapa.js" />
/// <reference path="EnvioEmail.js" />
/// <reference path="SelecaoDados.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _geracaoBoleto;
var _gridBoletosEtapa2;

var GeracaoBoleto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.GerarBoletosEtapa2 = PropertyEntity({ eventClick: GerarBoletosClick, type: types.event, text: "Gerar Boletos", visible: ko.observable(true), enable: ko.observable(true) });
    this.AtualizarBoletos = PropertyEntity({ eventClick: AtualizarBoletosClick, type: types.event, text: "Atualizar Boletos", visible: ko.observable(true), enable: ko.observable(true) });

    this.TitulosEtapa2 = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });

    this.Proximo = PropertyEntity({ eventClick: ProximoGeracaoBoletoClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadGeracaoBoleto() {
    _geracaoBoleto = new GeracaoBoleto();
    KoBindings(_geracaoBoleto, "knockoutGeracaoBoleto");

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

    _gridBoletosEtapa2 = new BasicDataTable(_geracaoBoleto.TitulosEtapa2.idGrid, header, menuOpcoes);

    $("#knockoutGeracaoBoleto").hide();
}

function DownloadBoletoClick(e, sender) {
    var dados = { Codigo: e.Codigo }
    executarDownload("TituloFinanceiro/DownloadBoleto", dados);
}

function GerarBoletosClick(e, sender) {
    var data = { Codigo: _selecaoDado.Codigo.val() };
    executarReST("CobrancaSimples/AtualizarBoletos", data, function (arg) {
        if (arg.Success) {
            recarregarGridTitulosEtapa2(arg.Data);
            exibirMensagem(tipoMensagem.ok, "Geração de Boletos", "Título enviado ao integrador, favor aguarde e atualize o mesmo.");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AtualizarBoletosClick(e, sender) {
    var data = { Codigo: _selecaoDado.Codigo.val() };
    executarReST("CobrancaSimples/AtualizarBoletos", data, function (arg) {
        if (arg.Success) {
            recarregarGridTitulosEtapa2(arg.Data);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ProximoGeracaoBoletoClick(e, sender) {
    var boletos = _gridBoletosEtapa2.BuscarRegistros();
    if (boletos.length > 0 && boletos[0].BoletoStatusTitulo == 2) {
        var dataGrid = new Array();

        $.each(boletos, function (i, titulo) {

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

        _gridEnvioEmail.CarregarGrid(dataGrid);
        _etapaAtual = 3;
        $("#" + _etapaCobrancaSimples.Etapa3.idTab).click();

        $("#knockoutEnvioEmail").show();
    } else {
        exibirMensagem(tipoMensagem.aviso, "Geração de Boletos", "Ainda não foi gerado o boleto do título.");
        $("#knockoutEnvioEmail").hide();
    }
}

//*******MÉTODOS*******

function recarregarGridTitulosEtapa2(data) {
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

    _gridBoletosEtapa2.CarregarGrid(dataGrid);
}