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
                BuscarAndamentoPedido(e.NumeroPedido.val).then(function () {
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
    this.Email = PropertyEntity({ text: "Confirme seu E-mail:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true), required: true });

    this.codEmpresa = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data["Empresa"]), visible: ko.observable(false) });
    this.codUsuario = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data["codUsuario"]), visible: ko.observable(false) });

    this.codUserSap = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data["codUserSap"]), visible: ko.observable(false) });
    this.tipoUsuario = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data["tipoUsuario"]), visible: ko.observable(false) });
    this.eMail = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data["eMail"]), visible: ko.observable(false) });
    this.nomeUsuario = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data["nomeUsuario"]), visible: ko.observable(false) });
    this.userCadastrado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(data["userCadastrado"]), visible: ko.observable(false) });

    this.Senha = PropertyEntity({ text: "Senha:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false), required: false });

    this.ConfirmarEmail = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            validarDados(e);

        }, type: types.event, text: "Confirmar E-mail", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ConfirmarSenha = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            validarDadosSenha(e);

        }, type: types.event, text: "Confirmar Senha", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false)
    });
}

var NovoCadastro = function (data) {

    this.codUserSap = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data.codUsuario.val()), visible: ko.observable(false) });
    this.codEmpresa = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data.codEmpresa.val()), visible: ko.observable(false) });
    this.email = PropertyEntity({ text: "E-mail:", getType: typesKnockout.string, val: ko.observable(data.eMail.val()), visible: ko.observable(true) });
    this.nomeUsuario = PropertyEntity({ text: "Nome:", getType: typesKnockout.string, val: ko.observable(data.nomeUsuario.val()), visible: ko.observable(true) });
    this.tipoUsuario = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(data.tipoUsuario.val()), visible: ko.observable(false) });

    this.CPF = PropertyEntity({ text: "CPF:", getType: typesKnockout.cpfCnpj, val: ko.observable(""), visible: ko.observable(true), required: true });
    this.Senha = PropertyEntity({ text: "Senha:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true), required: true });
    this.ConfirmacaoSenha = PropertyEntity({ text: "Confirmação Senha:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true), required: true });

    this.ConfirmarNovaSenha = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            CadastrarCliente();

        }, type: types.event, text: "Cadastrar", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******

function loadConsultaAndamentoPedido(token, codUserSap, numOv, User) {
    _numOv = numOv;

    _consultaPedido = new ConsultaPedido();
    KoBindings(_consultaPedido, "knockoutConsultaPedido");

    if (_numOv != "")
        _consultaPedido.NumeroPedido.val(_numOv);

    carregarHTMLComponenteAcompanhamentoPedido(function () {

        RegistraComponenteConsultarDetalhesPedido();
        // if (codUserSap != "" && token != "") {
        if (codUserSap != "") {
            BuscarDadosLogin(token, codUserSap).then(function () {

            });
        } else if (numOv != "" && User != "") {
            BuscarAndamentoPedido(numOv).then(function () {
                //Mostrar campos...
                $("#divDadosPedido").show();
            });
        } else if (numOv == "" && User != "") {
            // mostrar campo para pesquisar numero pedido..
            MostrarPesquisaNumeroPedido();
        }
    });
}

function BuscarDadosLogin(token, codUserSap) {

    var p = new promise.Promise();
    var data = new Object();

    data.token = token;
    data.codUserSap = codUserSap;

    executarReST("ConsultarAndamentoPedido/ObterDadosDoLoginCliente", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== null) {

                if (arg.Data != undefined) {
                    _dadosLogin = new DadosLogin(arg.Data);
                    KoBindings(_dadosLogin, "knockoutLoginUser");
                }

                if (arg.Data.userCadastrado)
                    mostrarCamposSenha();
                else
                    mostrarCamposLogin();

                Global.abrirModal("divConfirmLogin");

            } else {
                $("#divloginInvalido").show();
                $("#spanMsg").text(arg.Msg);
                //exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

        p.done();
    });

    return p;
}

function validarDados(dados) {
    var valido = ValidarCamposObrigatorios(_dadosLogin);
    if (!valido)
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.FiltrosObrigatorios, "Campo E-mail é obrigatório");

    if (_dadosLogin.eMail.val().toLowerCase() == _dadosLogin.Email.val().toLowerCase()) {
        _cadastroLogin = new NovoCadastro(dados);
        KoBindings(_cadastroLogin, "knockoutNovoCadastro");
        Global.fecharModal("divConfirmLogin");
        Global.abrirModal("divCadastroSenha");
    } else {
        exibirMensagem(tipoMensagem.atencao, "E-mail Incorreto", "E-mail não encontrado, Por favor verifique seu email cadastrado");
    }
}


function validarDadosSenha() {
    var data = new Object();
    data.senha = _dadosLogin.Senha.val;
    data.codigo = _dadosLogin.codUsuario.val;

    executarReST("ConsultarAndamentoPedido/ValidarSenhaUsuario", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                Global.fecharModal("divConfirmLogin");
                if (_numOv != "") {
                    BuscarAndamentoPedido(_numOv, _dadosLogin.codEmpresa.val(), _dadosLogin.codUserSap.val()).then(function () {
                        //Mostrar campos...
                        $("#divDadosPedido").show();
                    });
                } else {
                    // mostrar campo para pesquisar numero pedido..
                    MostrarPesquisaNumeroPedido();
                }

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function CadastrarCliente() {

    var valido = ValidarCamposObrigatorios(_cadastroLogin);
    if (!valido)
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.FiltrosObrigatorios, "Verifique os campos obrigatórios");

    if (_cadastroLogin.Senha.val() != _cadastroLogin.ConfirmacaoSenha.val()) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "As senha e confirmação de senha não coincidem");
        return;
    }

    var data = RetornarObjetoPesquisa(_cadastroLogin);

    executarReST("ConsultarAndamentoPedido/CadastrarNovoUsuario", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                Global.fecharModal("divCadastroSenha");

                if (_numOv != "") {
                    BuscarAndamentoPedido(_numOv, _dadosLogin.codEmpresa.val(), _dadosLogin.codUserSap.val()).then(function () {
                        //Mostrar campos...
                        $("#divDadosPedido").show();
                    });
                } else {
                    // mostrar campo para pesquisar numero pedido..
                    MostrarPesquisaNumeroPedido();
                }

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

}

function mostrarCamposSenha() {
    $("#divSenha").show();
    $("#divLogin").hide();
    _dadosLogin.Senha.visible(true);
    _dadosLogin.Email.visible(false);
    _dadosLogin.ConfirmarSenha.visible(true);
    _dadosLogin.ConfirmarEmail.visible(false);
}

function mostrarCamposLogin() {
    $("#divSenha").hide();
    $("#divLogin").show();
    _dadosLogin.Senha.visible(false);
    _dadosLogin.Email.visible(true);
    _dadosLogin.ConfirmarSenha.visible(false);
    _dadosLogin.ConfirmarEmail.visible(true);
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