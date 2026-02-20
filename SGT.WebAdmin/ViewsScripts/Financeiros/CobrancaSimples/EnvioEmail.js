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
/// <reference path="GeracaoBoleto.js" />
/// <reference path="SelecaoDados.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _envioEmail;
var _gridEnvioEmail;

var EnvioEmail = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TitulosParaEnvioEmail = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.EnviarEmail = PropertyEntity({ eventClick: EnviarEmailClick, type: types.event, text: "Enviar E-mail", visible: ko.observable(true), enable: ko.observable(true) });
    this.ListaTitulos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.MensagemEmail = PropertyEntity({ text: "Mensagem para o Email:", maxlength: 5000, enable: ko.observable(true) });

    this.Finalizar = PropertyEntity({ eventClick: FinalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadEnvioEmail() {

    _envioEmail = new EnvioEmail();
    KoBindings(_envioEmail, "knockoutEnvioEmail");

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

    _gridEnvioEmail = new BasicDataTable(_envioEmail.TitulosParaEnvioEmail.idGrid, header, menuOpcoes);

    $("#knockoutEnvioEmail").hide();
}

function FinalizarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar o processo de geração de boletos?", function () {
        limparCamposCobrancaSimples();
    });
}

function EnviarEmailClick(e, sender) {
    if (_selecaoDado.Codigo.val() > 0) {
        var data = { Codigo: _selecaoDado.Codigo.val(), MensagemEmail: _envioEmail.MensagemEmail.val() };
        executarReST("CobrancaSimples/EnviarEmailBoletos", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de envio de e-mail concluido.");
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.aviso, "Envio de E-mail", "Favor gerar o boleto antes de enviar por e-mail.");
        $("#knockoutEnvioEmail").hide();
    }
}

//*******MÉTODOS*******
