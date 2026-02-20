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
/// <reference path="BoletoAlteracao.js" />
/// <reference path="EtapaBoletoAlteracao.js" />
/// <reference path="AlteracaoBoletoAlteracao.js" />
/// <reference path="ImpressaoBoletoAlteracao.js" />
/// <reference path="RemessaBoletoAlteracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _emailBoletoAlteracao;
var _gridEmailBoleto;

var EmailBoletoAlteracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Mensagem = PropertyEntity({ text: "Mensagem: ", required: true, maxlength: 5000, visible: ko.observable(true) });

    this.TagNumeroDocumento = PropertyEntity({ eventClick: function (e) { InserirTag(_emailBoletoAlteracao.Mensagem.id, "#NumeroDocumento"); }, type: types.event, text: "Nº Documento" });
    this.TagValorOriginal = PropertyEntity({ eventClick: function (e) { InserirTag(_emailBoletoAlteracao.Mensagem.id, "#ValorOriginal"); }, type: types.event, text: "Valor do Título" });
    this.TagObservacaoBoleto = PropertyEntity({ eventClick: function (e) { InserirTag(_emailBoletoAlteracao.Mensagem.id, "#ObservacaoBoleto"); }, type: types.event, text: "Observação" });
    this.TagDataVencimentoOriginal = PropertyEntity({ eventClick: function (e) { InserirTag(_emailBoletoAlteracao.Mensagem.id, "#DataVencimentoOriginal"); }, type: types.event, text: "Data de Vencimento Original" });
    this.TagDataVencimentoAlterada = PropertyEntity({ eventClick: function (e) { InserirTag(_emailBoletoAlteracao.Mensagem.id, "#DataVencimentoAlterada"); }, type: types.event, text: "Data de Vencimento Alterada" });

    this.Enviar = PropertyEntity({ eventClick: EnviarEmailClick, type: types.event, text: "Enviar E-mail", visible: ko.observable(true), enable: ko.observable(true) });
    this.Boletos = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });

    this.Anterior = PropertyEntity({ eventClick: AnteriorEmailBoletoAlteracaoClick, type: types.event, text: "Anterior", visible: ko.observable(true), enable: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: FinalizarEmailBoletoAlteracaoClick, type: types.event, text: "Finalizar", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadEmailBoletoAlteracao() {
    _emailBoletoAlteracao = new EmailBoletoAlteracao();
    KoBindings(_emailBoletoAlteracao, "knockoutEmailBoletoAlteracao");

    var header = [{ data: "Codigo", visible: false },
    { data: "CodigoRemessa", visible: false },
    { data: "BoletoStatusTitulo", visible: false },
    { data: "Pessoa", title: "Pessoa", width: "20%", className: "text-align-left" },
    { data: "CodigoTitulo", title: "Nº Título", width: "20%", className: "text-align-left" },
    { data: "DataEmissao", title: "Data Emissão", width: "8%", className: "text-align-right" },
    { data: "DataVencimentoOriginal", title: "Vencimento Original", width: "10%", className: "text-align-center" },
    { data: "DataVencimentoAlterado", title: "Vencimento Alterado", width: "10%", className: "text-align-center" },
    { data: "Valor", title: "Valor", width: "8%", className: "text-align-right" },
    { data: "NossoNumero", title: "Nosso Número", width: "8%", className: "text-align-center" },
    { data: "NumeroDocumento", title: "Nº Documento", width: "8%", className: "text-align-center" },
    { data: "Observacao", title: "Observação", width: "10%", className: "text-align-left" },
    { data: "NumeroRemessa", title: "Número Remessa", width: "8%", className: "text-align-center" }
    ];

    _gridEmailBoleto = new BasicDataTable(_emailBoletoAlteracao.Boletos.idGrid, header);
}

function EnviarEmailClick(e, sender) {
    var data = { Codigo: _emailBoletoAlteracao.Codigo.val(), Mensagem: _emailBoletoAlteracao.Mensagem.val() };
    executarReST("BoletoAlteracao/EnviarEmailBoletos", data, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de envio de e-mail finalizado.");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function FinalizarEmailBoletoAlteracaoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar o processo de alteração?", function () {

        var data = { Codigo: _emailBoletoAlteracao.Codigo.val() };
        executarReST("BoletoAlteracao/Finalizar", data, function (arg) {
            if (arg.Success) {
                

                limparCamposBoletoAlteracao();
                limparCamposAlteracao();
                limparCamposImpressaoBoleto();
                limparCamposRemessaBoleto();
                limparCamposEmailBoleto();

                _boletoAlteracao.Codigo.val(0);
                _alteracaoBoletoAlteracao.Codigo.val(0);
                _emailBoletoAlteracao.Codigo.val(0);
                _remessaBoletoAlteracao.Codigo.val(0);
                _impressaoBoletoAlteracao.Codigo.val(0);
                _boletoAlteracao.Etapa.val(EnumBoletoAlteracaoEtapa.Selecao);

                LimparOcultarAbas();
                $("#" + _etapaBoletoAlteracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
                $("#" + _etapaBoletoAlteracao.Etapa1.idTab + " .step").attr("class", "step lightgreen");
                $("#" + _etapaBoletoAlteracao.Etapa1.idTab).click();

                buscarAlteracoesBoleto();
                buscarBoletosAlteracao();
                BuscarBoletosImpressao();
                BuscarBoletosRemessa();
                BuscarBoletosEmail();

                VerificarBotoes();

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alteração de boletos finalizado.");
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });

    });

}

function AnteriorEmailBoletoAlteracaoClick(e, sender) {
    var data = { Codigo: _emailBoletoAlteracao.Codigo.val(), Etapa: EnumBoletoAlteracaoEtapa.Remessa };
    executarReST("BoletoAlteracao/AtualizarEtapa", data, function (arg) {
        if (arg.Success) {
            _boletoAlteracao.Etapa.val(arg.Data.Etapa);
            PosicionarEtapa(arg.Data);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function BuscarBoletosEmail() {
    if (_emailBoletoAlteracao.Codigo.val() > 0) {
        var data = { Codigo: _emailBoletoAlteracao.Codigo.val() };
        executarReST("BoletoAlteracao/AtualizarBoletos", data, function (arg) {
            if (arg.Success) {
                recarregarGridEmailBoleto(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        $("#knockoutEmailBoletoAlteracao").hide();
    }
}

function recarregarGridEmailBoleto(data) {
    var dataGrid = new Array();

    $.each(data, function (i, titulo) {
        var obj = new Object();

        obj.Codigo = titulo.Codigo;
        obj.CodigoRemessa = titulo.CodigoRemessa;
        obj.BoletoStatusTitulo = titulo.BoletoStatusTitulo;
        obj.Pessoa = titulo.Pessoa;
        obj.CodigoTitulo = titulo.CodigoTitulo;
        obj.DataEmissao = titulo.DataEmissao;
        obj.DataVencimentoOriginal = titulo.DataVencimentoOriginal;
        obj.DataVencimentoAlterado = titulo.DataVencimentoAlterado;
        obj.Valor = titulo.Valor;
        obj.NossoNumero = titulo.NossoNumero;
        obj.NumeroDocumento = titulo.NumeroDocumento;
        obj.Observacao = titulo.Observacao;
        obj.NumeroRemessa = titulo.NumeroRemessa;

        dataGrid.push(obj);
    });

    _gridEmailBoleto.CarregarGrid(dataGrid);
}


function limparCamposEmailBoleto() {
    LimparCampos(_emailBoletoAlteracao);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}