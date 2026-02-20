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
/// <reference path="EnvioEmail.js" />
/// <reference path="GeracaoRemessa.js" />
/// <reference path="SelecaoTitulo.js" />
/// <reference path="../../Consultas/BoletoConfiguracao.js" />
/// <reference path="GeracaoFrancesinha.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _geracaoBoleto;
var _gridBoletosEtapa2;

var GeracaoBoleto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ConfiguracaoBoleto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Config. Boleto (Banco):", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.GerarBoletosEtapa2 = PropertyEntity({ eventClick: GerarBoletosClick, type: types.event, text: "Gerar Boletos", visible: ko.observable(false), enable: ko.observable(true) });

    this.AtualizarBoletos = PropertyEntity({ eventClick: AtualizarBoletosClick, type: types.event, text: "Atualizar Boletos", visible: ko.observable(false), enable: ko.observable(true) });

    this.TitulosEtapa2 = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.TotalizadorValorSelecionado = PropertyEntity({ text: "Valor total títulos: ", getType: typesKnockout.string, def: "0,00", val: ko.observable("0,00"), enable: ko.observable(true), visible: ko.observable(true) });

    this.Proximo = PropertyEntity({ eventClick: ProximoGeracaoBoletoClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadGeracaoBoleto() {
    _geracaoBoleto = new GeracaoBoleto();
    KoBindings(_geracaoBoleto, "knockoutGeracaoBoletos");

    new BuscarBoletoConfiguracao(_geracaoBoleto.ConfiguracaoBoleto, retornoConfiguracaoBoleto);

    var download = { descricao: "Download", id: "clasEditar", evento: "onclick", metodo: DownloadBoletoClick, tamanho: "10", icone: "" };
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

    _gridBoletosEtapa2 = new BasicDataTable(_geracaoBoleto.TitulosEtapa2.idGrid, header, menuOpcoes);

    loadEtapaBoletoGeracao();
    loadSelecaoTitulo();
    loadGeracaoRemessa();
    loadEnvioEmail();
    loadGeracaoFrancesinha();

    $("#knockoutGeracaoBoletos").hide();
    $("#knockoutGeracaoRemessa").hide();
    $("#knockoutEnvioEmail").hide();
    $("#knockoutFrancesinha").hide();
}

function retornoConfiguracaoBoleto(data) {
    _geracaoBoleto.ConfiguracaoBoleto.val(data.DescricaoBanco);
    _geracaoBoleto.ConfiguracaoBoleto.codEntity(data.Codigo);
}

function DownloadBoletoClick(e, sender) {
    var dados = { Codigo: e.Codigo }
    executarDownload("TituloFinanceiro/DownloadBoleto", dados);
}

function GerarBoletosClick(e, sender) {
    var data = { TipoAtualizacao: 2, ListaTitulos: PreencherListaTitulosEtapa2(), CodigoConfiguracaoBoleto: _geracaoBoleto.ConfiguracaoBoleto.codEntity() };

    if (_geracaoBoleto.ConfiguracaoBoleto.codEntity() == 0 || _geracaoBoleto.ConfiguracaoBoleto.codEntity() == "") {
        exibirMensagem(tipoMensagem.aviso, "Geração de Boletos", "Por favor selecione o banco antes da geração.");
        return;
    }

    executarReST("BoletoGeracao/AtualizarBoletos", data, function (arg) {
        if (arg.Success) {
            recarregarGridTitulosEtapa2(arg.Data);
            exibirMensagem(tipoMensagem.ok, "Geração de Boletos", "Títulos enviados ao integrador, favor aguarde e atualize a lista dos títulos.");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AtualizarBoletosClick(e, sender) {
    var data = { TipoAtualizacao: 1, ListaTitulos: PreencherListaTitulosEtapa2() };
    executarReST("BoletoGeracao/AtualizarBoletos", data, function (arg) {
        if (arg.Success) {
            recarregarGridTitulosEtapa2(arg.Data);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ProximoGeracaoBoletoClick(e, sender) {
    
    var boletos = _gridBoletosEtapa2.BuscarRegistros();
    if (boletos.length > 0) {
        var dataGrid = new Array();
        var contemBoletosSemRemessa = false;

        $.each(boletos, function (i, titulo) {

            var obj = new Object();
            obj.Codigo = titulo.Codigo;
            obj.CodigoRemessa = titulo.CodigoRemessa;
            obj.BoletoStatusTitulo = titulo.BoletoStatusTitulo;
            obj.Pessoa = titulo.Pessoa;
            obj.DescricaoStatusBoleto = titulo.DescricaoStatusBoleto;
            obj.DataEmissao = titulo.DataEmissao;
            obj.DataVencimento = titulo.DataVencimento;
            obj.Valor = titulo.Valor;
            obj.NossoNumero = titulo.NossoNumero;
            obj.NumeroRemessa = titulo.NumeroRemessa;
            obj.CaminhoBoleto = titulo.CaminhoBoleto;

            if (titulo.BoletoStatusTitulo == 0 || titulo.BoletoStatusTitulo == 1 || titulo.BoletoStatusTitulo == 3 || titulo.BoletoStatusTitulo == 4) {
                $("#knockoutGeracaoRemessa").hide();
                exibirMensagem(tipoMensagem.aviso, "Geração de Boletos", "Existem alguns títulos com boletos não gerados.");
                return;
            }

            if (!contemBoletosSemRemessa)
                contemBoletosSemRemessa = titulo.BoletoStatusTitulo == 2;

            dataGrid.push(obj);
        });

        if (!contemBoletosSemRemessa) {
            _geracaoRemessa.GerarRemessa.visible(false);
            _geracaoRemessa.DownloadRemessa.visible(false);
        } else {
            _geracaoRemessa.GerarRemessa.visible(true);
            _geracaoRemessa.DownloadRemessa.visible(true);
        }

        _gridRemessas.CarregarGrid(dataGrid);
        _etapaAtual = 3;

        $("#" + _etapaBoletoGeracao.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa2.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoGeracao.Etapa3.idTab + " .step").attr("class", "step lightgreen");
        $("#" + _etapaBoletoGeracao.Etapa3.idTab).tab("show");

        _geracaoRemessa.AtualizarRemessas.visible(true);

        $("#knockoutGeracaoRemessa").show();
    } else {
        exibirMensagem(tipoMensagem.aviso, "Geração de Boletos", "Por favor selecione os títulos na etapa 1 antes de gerar os boletos.");
        $("#knockoutGeracaoBoletos").hide();
    }
}

//*******MÉTODOS*******

function recarregarGridTitulosEtapa2(data) {
    var dataGrid = new Array();

    $.each(data.ListaTitulos, function (i, titulo) {
        var obj = new Object();
        obj.Codigo = titulo.Codigo;
        obj.CodigoRemessa = titulo.CodigoRemessa;
        obj.BoletoStatusTitulo = titulo.BoletoStatusTitulo;
        obj.Pessoa = titulo.Pessoa;
        obj.DescricaoStatusBoleto = titulo.DescricaoStatusBoleto;
        obj.DataEmissao = titulo.DataEmissao;
        obj.DataVencimento = titulo.DataVencimento;
        obj.Valor = titulo.Valor;
        obj.NossoNumero = titulo.NossoNumero;
        obj.NumeroRemessa = titulo.NumeroRemessa;
        obj.CaminhoBoleto = titulo.CaminhoBoleto;

        dataGrid.push(obj);
    });

    _gridBoletosEtapa2.CarregarGrid(dataGrid);
}

function PreencherListaTitulosEtapa2() {
    var listaTitulo = new Array();

    $.each(_gridBoletosEtapa2.BuscarRegistros(), function (i, titulo) {
        listaTitulo.push({ Titulo: titulo });
    });

    return JSON.stringify(listaTitulo);
}

function limparCamposFatura() {
    LimparCampos(_geracaoBoleto);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}