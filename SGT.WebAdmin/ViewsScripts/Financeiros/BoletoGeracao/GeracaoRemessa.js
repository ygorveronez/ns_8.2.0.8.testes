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
/// <reference path="EnvioEmail.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="BoletoGeracaoEtapa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _geracaoRemessa;
var _gridRemessas;

var GeracaoRemessa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.GerarRemessa = PropertyEntity({ eventClick: GerarRemessaClick, type: types.event, text: "Gerar Remessa", visible: ko.observable(false), enable: ko.observable(true) });
    this.DownloadRemessa = PropertyEntity({ eventClick: DownloadRemessaClick, type: types.event, text: "Download Remessa(s)", visible: ko.observable(false), enable: ko.observable(true) });
    this.AtualizarRemessas = PropertyEntity({ eventClick: AtualizarRemessasClick, type: types.event, text: "Atualizar Remessas", visible: ko.observable(false), enable: ko.observable(true) });
    this.GerarRemessasEtapa3 = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.TotalizadorValorSelecionado = PropertyEntity({ text: "Valor total títulos: ", getType: typesKnockout.string, def: "0,00", val: ko.observable("0,00"), enable: ko.observable(true), visible: ko.observable(true) });

    this.Proximo = PropertyEntity({ eventClick: ProximoGeracaoRemessaClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadGeracaoRemessa() {
    _geracaoRemessa = new GeracaoRemessa();
    KoBindings(_geracaoRemessa, "knockoutGeracaoRemessa");

    var download = { descricao: "Download", id: "clasEditar", evento: "onclick", metodo: DownloadRemessaTituloClick, tamanho: "10", icone: "" };
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

    _gridRemessas = new BasicDataTable(_geracaoRemessa.GerarRemessasEtapa3.idGrid, header, menuOpcoes);
}

function DownloadRemessaTituloClick(e) {
    var dados = { Codigo: e.Codigo };
    executarDownload("TituloFinanceiro/DownloadRemessa", dados);
}

function DownloadRemessaClick(e, sender) {
    var dados = { ListaTitulos: PreencherListaCodigosRemessa() };
    executarDownload("BoletoGeracao/DownloadRemessa", dados);
}

function GerarRemessaClick(e, sender) {
    var data = { TipoAtualizacao: 3, ListaTitulos: PreencherListaRemessaEtapa3() };

    executarReST("BoletoGeracao/AtualizarBoletos", data, function (arg) {
        if (arg.Success) {
            recarregarGridRemessaEtapa3(arg.Data);
            exibirMensagem(tipoMensagem.ok, "Geração de Remessa", "Remessas enviadas ao integrador, favor aguarde e atualize a lista dos títulos.");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AtualizarRemessasClick(e, sender) {
    var data = { TipoAtualizacao: 1, ListaTitulos: PreencherListaRemessaEtapa3() };
    executarReST("BoletoGeracao/AtualizarBoletos", data, function (arg) {
        if (arg.Success) {
            recarregarGridRemessaEtapa3(arg.Data);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ProximoGeracaoRemessaClick(e, sender) {
    var boletos = _gridRemessas.BuscarRegistros();
    if (boletos.length > 0) {
        var dataGrid = new Array();
        var contemBoletosSemRemessaSemBoleto = false;

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

            if (titulo.BoletoStatusTitulo !== 6) {
                $("#knockoutEnvioEmail").hide();
                exibirMensagem(tipoMensagem.aviso, "Geração de Remessa", "Existem alguns títulos sem remessa para seguir a próxima etapa.");
                return;
            }

            if (!contemBoletosSemRemessaSemBoleto)
                contemBoletosSemRemessaSemBoleto = titulo.BoletoStatusTitulo === 6;

            dataGrid.push(obj);
        });

        if (!contemBoletosSemRemessaSemBoleto) {
            _envioEmail.EnviarEmail.visible(false);
        } else {
            _envioEmail.EnviarEmail.visible(true);
        }

        _envioEmail.ListaTitulos.val(JSON.stringify(dataGrid));
        _etapaAtual = 4;

        $("#" + _etapaBoletoGeracao.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa2.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa3.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa4.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoGeracao.Etapa4.idTab + " .step").attr("class", "step lightgreen");
        $("#" + _etapaBoletoGeracao.Etapa4.idTab).tab("show");
        buscarTitulosParaEnvioEmail();

        $("#knockoutEnvioEmail").show();
    } else {
        exibirMensagem(tipoMensagem.aviso, "Geração de Remessa", "Por favor gere os boletos na etapa 2 antes de gerar a remessa.");
        $("#knockoutGeracaoRemessa").hide();
    }
}

//*******MÉTODOS*******

function recarregarGridRemessaEtapa3(data) {
    var dataGrid = new Array();
    var todosBoletosComRemessa = true;

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

        if (todosBoletosComRemessa)
            todosBoletosComRemessa = titulo.BoletoStatusTitulo === 6;

        dataGrid.push(obj);
    });

    if (todosBoletosComRemessa)
        _geracaoRemessa.DownloadRemessa.visible(true);
    else
        _geracaoRemessa.DownloadRemessa.visible(false);

    _gridRemessas.CarregarGrid(dataGrid);
}

function PreencherListaRemessaEtapa3() {
    var listaTitulo = new Array();

    $.each(_gridRemessas.BuscarRegistros(), function (i, titulo) {
        listaTitulo.push({ Titulo: titulo });
    });

    return JSON.stringify(listaTitulo);
}

function PreencherListaCodigosRemessa() {
    var codigosRemessas = new Array();
    var contemRemessa = false;
    $.each(_gridRemessas.BuscarRegistros(), function (i, titulo) {

        if (codigosRemessas.length > 0) {
            $.each(codigosRemessas, function (j, remessa) {
                if (remessa.Codigo == titulo.CodigoRemessa && !contemRemessa)
                    contemRemessa = true;
            });

            if (!contemRemessa)
                codigosRemessas.push({ Codigo: titulo.CodigoRemessa });
        }
        else
            codigosRemessas.push({ Codigo: titulo.CodigoRemessa });

        contemRemessa = false;
    });
    return JSON.stringify(codigosRemessas);
}

function limparCamposFatura() {
    LimparCampos(_geracaoRemessa);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}