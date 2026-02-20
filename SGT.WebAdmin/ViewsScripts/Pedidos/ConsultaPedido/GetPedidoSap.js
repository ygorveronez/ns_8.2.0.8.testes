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
/// <reference path="../../Enumeradores/EnumSituacaoCarregamento.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="detalhespedido.js" />

var _dadosLogin;
var _cadastroLogin;
var _numOv;
var _consultaPedido;

var ConsultaPedido = function () {

    this.NumeroPedido = PropertyEntity({ text: "Informe o número do pedido:", getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable(""), visible: ko.observable(true) });

    this.BuscarDadosPedido = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            console.log(e);
            console.log(e.NumeroPedido.val);

            if (e.NumeroPedido.val != "") {
                BuscarAndamentoPedido(e.NumeroPedido.val, _dadosLogin.codEmpresa.val, _dadosLogin.codUserSap.val, _dadosLogin.tipoUsuario.val).then(function () {
                    //Mostrar campos...
                    $("#divConsultarPedido").hide();
                    $("#divDadosPedido").show();
                });
            }
        }, type: types.event, text: "Consultar", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var DadosLogin = function (data) {
    if (data == null)
        data = {};

    this.Data = data;
    this.codEmpresa = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data["Empresa"]), visible: ko.observable(false) });
    this.codUsuario = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data["codUsuario"]), visible: ko.observable(false) });
    this.codUserSap = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data["codUserSap"]), visible: ko.observable(false) });
    this.tipoUsuario = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data["tipoUsuario"]), visible: ko.observable(false) });
    this.eMail = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data["eMail"]), visible: ko.observable(false) });
    this.nomeUsuario = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data["nomeUsuario"]), visible: ko.observable(false) });

}



//*******EVENTOS*******

function loadGetPedidoSap(codUserSap, numOv) {
    _numOv = numOv;

    _consultaPedido = new ConsultaPedido();
    KoBindings(_consultaPedido, "knockoutConsultaPedido");

    if (_numOv != "")
        _consultaPedido.NumeroPedido.val(_numOv);

    carregarHTMLComponenteAcompanhamentoPedido(function () {

        RegistraComponenteConsultarDetalhesPedido();

        if (codUserSap != "") {
            BuscarDadosLogin(codUserSap).then(function () {

            });
        }
    });
}

function BuscarDadosLogin(codUserSap) {

    var p = new promise.Promise();
    var data = new Object();

    data.codUserSap = codUserSap;

    executarReST("GetPedido/ObterDadosDoLoginCliente", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== null) {

                if (arg.Data != undefined && arg.Data.usuarioOk) {
                    _dadosLogin = new DadosLogin(arg.Data);

                    if (_numOv != "") {
                        BuscarAndamentoPedido(_numOv, arg.Data.Empresa, arg.Data.codUserSap, arg.Data.tipoUsuario).then(function () {
                            //Mostrar campos...
                            $("#divConsultarPedido").hide();
                            $("#divDadosPedido").show();
                        });
                    } else
                        MostrarPesquisaNumeroPedido();

                }
                else {
                    $("#divloginInvalido").show();
                    $("#spanMsg").text(arg.Msg);
                }

            } else {
                $("#divloginInvalido").show();
                $("#spanMsg").text(arg.Msg);
            }

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

        p.done();
    });

    return p;
}

function MostrarPesquisaNumeroPedido() {
    $("#divConsultarPedido").show();
}

function RegistraComponenteConsultarDetalhesPedido() {
    if (ko.components.isRegistered('consultar-detalhes-pedido'))
        return;

    ko.components.register('consultar-detalhes-pedido', {
        viewModel: [AndamentoPedido, RemessaPedido],
        template: {
            element: 'consultar-detalhes-pedido-templete'
        }
    });
}

function carregarHTMLComponenteAcompanhamentoPedido(callback) {
    $.get('Content/Static/Pedido/ConsultarAndamentoPedido/ComponenteDetalhesPedido.html?dyn=' + guid(), function (html) {
        $('#divDadosPedido').html(html);
        loadAndamentoPedido();
        callback();
    })
}