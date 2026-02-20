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
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Credito.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Global/SignalR/SignalR.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _ControleSaldo;
var _Creditores;
var _SuperioresHierarquia;

var ControleSaldo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PossuiSuperior = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.Creditos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid(), idTab: guid() });
    this.Saldo = PropertyEntity({ getType: typesKnockout.decimal, text: "Saldo:" });
    this.DetalhesCredito = PropertyEntity({ text: "Ver Detelhes", type: types.local, eventClick: abrirDetalhesCredito });
    this.RetornarEventoCredito = PropertyEntity({ type: types.local });
}

var Credito = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Creditor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:" });
    this.Saldo = PropertyEntity({ getType: typesKnockout.decimal, text: "Saldo:" });
    this.Comprometido = PropertyEntity({ getType: typesKnockout.decimal, text: "Comprometido:" });
    this.CreditoDisponivel = PropertyEntity({ getType: typesKnockout.decimal, text: "Credito Disponivel:" });
}

function CreditoresArray() {
    this.Creditores = ko.observableArray();
    this.ValorLiberar = PropertyEntity({ getType: typesKnockout.decimal, text: "Saldo:" });
    this.ConfirmarUtilizacao = PropertyEntity({ text: "Confirmar", type: types.local, eventClick: confirmarUtilizacaoCreditoClick });
}

function SuperioresHierarquiaArray() {
    this.Creditores = ko.observableArray();
    this.ConfirmarSolicitacao = PropertyEntity({ text: "Confirmar", type: types.local, eventClick: ConfirmarSolicitacaoClick });
}

var _idContent;
function loadControleSaldo() {
    _SuperioresHierarquia = null;
    _Creditores = null;
    _idContent = guid();
    $.get("Content/Static/Credito/ControleSaldo.html?dyn=" + guid(), function (data) {
        var html = data;
        _ControleSaldo = new ControleSaldo();
        html = html.replace(/#knockoutMeuCredito/g, _idContent).replace(/#idModal/g, _ControleSaldo.Creditos.idTab).replace(/#knoutEscolerCredito/g, _ControleSaldo.Creditos.id + "_knoutEscolerCredito").replace(/#knoutSolicitarCredito/g, _ControleSaldo.Creditos.id + "_knoutSolicitarCredito");
        $("#divControleSaldo").html(html);
        KoBindings(_ControleSaldo, _idContent);
        AtualizarDadosControleSaldo();

        SignalRAtualizarSaldoEvent = retornoAtualizacaoSaldoSignalREvent;
    });
}

function retornoAtualizacaoSaldoSignalREvent(retorno) {
    PreecherRetornoViaSignalR(retorno);
}

function PreecherRetornoViaSignalR(retorno) {
    var e = { Data: retorno };
    PreencherObjetoKnout(_ControleSaldo, e);
    PreecherModalEscolhaCredito();
    if (_ControleSaldo.PossuiSuperior.val()) {
        $("#" + _idContent).show();
    } else {
        $("#" + _idContent).hide();
    }
}



function SetarCreditosUtilizadosParaAtualizacao(creditosUtilizados) {
    $.each(_ControleSaldo.Creditos.val(), function (i, credito) {
        $.each(creditosUtilizados, function (j, creditoUtilizado) {
            if (creditoUtilizado.Creditor.Codigo == credito.Creditor.Codigo) {
                var valor = Globalize.parseFloat(_ControleSaldo.Creditos.val()[i].Saldo) + creditoUtilizado.ValorUtilizado;
                _ControleSaldo.Creditos.val()[i].Saldo = Globalize.format(valor, "n2");
            }
        });
    });
    PreecherModalEscolhaCredito();
}

function PreecherModalEscolhaCredito() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        $("#tituloNomeParaSolicitacao").text("Supervisor: ");
        $("#MyTitleCreditoModal").text("Informe para quem deseja solicitar a autorização.");
    }
    if (_Creditores != null) {
        _Creditores.Creditores.removeAll();
        if (_ControleSaldo.Creditos.val().length > 0) {
            $.each(_ControleSaldo.Creditos.val(), function (i, credito) {
                _Creditores.Creditores.push({ Codigo: credito.Codigo, Creditor: credito.Creditor.Descricao, CodigoCreditor: credito.Creditor.Codigo, Saldo: Globalize.format(credito.Saldo, "n2") });
            });
            $.each(_Creditores.Creditores(), function (i, creditor) {
                $("#txtValor_" + creditor.Codigo).maskMoney(ConfigDecimal({ allowZero: true }));
            });
        } else {
            $("#" + _idContent).hide();
        }
    }
}

function PreecherModalEscolhaCreditor(hierarquias) {
    _SuperioresHierarquia.Creditores.removeAll();
    $.each(hierarquias, function (i, hierarquia) {
        _SuperioresHierarquia.Creditores.push({ Creditor: hierarquia.Nome, CodigoCreditor: hierarquia.Codigo });
    });
}

function AtualizarDadosControleSaldo(callback) {
    BuscarPorCodigo(_ControleSaldo, "ControleSaldo/BuscarInformacoesSaldoCredito", function (arg) {
        if (_ControleSaldo.PossuiSuperior.val()) {
            executarReST("ControleSaldo/BuscarSuperiores", null, function (retHierarquia) {
                if (retHierarquia.Success) {
                    $("#" + _idContent).show();
                    if (_Creditores == null) {
                        _Creditores = new CreditoresArray();
                        KoBindings(_Creditores, _ControleSaldo.Creditos.id + "_knoutEscolerCredito");
                    }
                    if (_SuperioresHierarquia == null) {
                        _SuperioresHierarquia = new SuperioresHierarquiaArray();
                        KoBindings(_SuperioresHierarquia, _ControleSaldo.Creditos.id + "_knoutSolicitarCredito");
                    }
                    PreecherModalEscolhaCreditor(retHierarquia.Data);
                    PreecherModalEscolhaCredito();
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", retHierarquia.Msg);
                }
            });
        } else {
            $("#" + _idContent).hide();
        }
        if (callback != null)
            callback();

    });
}

function selecionarCreditor(e) {

    var valorJaUtilizado = 0;
    $.each(_Creditores.Creditores(), function (i, creditor) {
        if ($("#chkCreditor_" + creditor.Codigo).prop("checked")) {
            if ($("#txtValor_" + creditor.Codigo).val() != "")
                valorJaUtilizado += Globalize.parseFloat($("#txtValor_" + creditor.Codigo).val());
        }
    });

    var valorRestante = _Creditores.ValorLiberar.val - valorJaUtilizado;

    if ($("#chkCreditor_" + e.Codigo).prop("checked")) {
        $("#txtValor_" + e.Codigo).removeAttr("disabled");
        var valor = valorRestante;
        if (valorRestante > Globalize.parseFloat(e.Saldo))
            valor = e.Saldo;
        $("#txtValor_" + e.Codigo).val(Globalize.format(valor, "n2"));
    } else {
        $("#txtValor_" + e.Codigo).attr("disabled", "disabled");
        $("#txtValor_" + e.Codigo).val("");
    }
    return true;
}

function validarValorInformado(e) {
    var valorJaUtilizado = 0;
    $.each(_Creditores.Creditores(), function (i, creditor) {
        if ($("#chkCreditor_" + creditor.Codigo).prop("checked")) {
            if ($("#txtValor_" + creditor.Codigo).val() != "")
                valorJaUtilizado += Globalize.parseFloat($("#txtValor_" + creditor.Codigo).val());
        }
    });

    if (_Creditores.ValorLiberar.val < valorJaUtilizado) {
        var diferenca = valorJaUtilizado - _Creditores.ValorLiberar.val;
        var valorInformado = Globalize.parseFloat($("#txtValor_" + e.Codigo).val()) - diferenca;
        if (valorInformado > Globalize.parseFloat(e.Saldo))
            valorInformado = Globalize.parseFloat(e.Saldo);
        $("#txtValor_" + e.Codigo).val(Globalize.format(valorInformado, "n2"));
    } else {
        var valorInformado = Globalize.parseFloat($("#txtValor_" + e.Codigo).val())
        if (valorInformado > Globalize.parseFloat(e.Saldo)) {
            valorInformado = Globalize.parseFloat(e.Saldo);
            $("#txtValor_" + e.Codigo).val(Globalize.format(valorInformado, "n2"));
        }
    }
}


function abrirDetalhesCredito(e) {

    var header = [
       { data: "Creditor", title: "Creditor", width: "30%" },
       { data: "DataFimCredito", title: "Data Expira", width: "10%", className: "text-align-right" },
       { data: "Credito", title: "Crédito", width: "15%", className: "text-align-right" },
       { data: "Utilizado", title: "Utilizado", width: "15%", className: "text-align-right" },
       { data: "Obtido", title: "Obtido", width: "15%", className: "text-align-right" },
       { data: "Comprometido", title: "Comprometido", width: "15%", className: "text-align-right" },
       { data: "Saldo", title: "Saldo", width: "15%", className: "text-align-right" },
    ];

    var lista = new Array();

    $.each(_ControleSaldo.Creditos.val(), function (i, credito) {
        var cred = new Object();
        cred.Creditor = credito.Creditor.Descricao;
        cred.DataFimCredito = credito.DataFimCredito;
        cred.Credito = Globalize.format(credito.Credito, "n2");
        cred.Comprometido = Globalize.format(credito.Comprometido, "n2");
        cred.Obtido = Globalize.format(credito.Obtido, "n2");
        cred.Utilizado = Globalize.format(credito.Utilizado, "n2");
        cred.Saldo = Globalize.format(credito.Saldo, "n2");
        lista.push(cred);
    });


    var gridDetalhesCredito = new BasicDataTable(_ControleSaldo.Creditos.idGrid, header, null);
    gridDetalhesCredito.CarregarGrid(lista);    
    Global.abrirModal(_ControleSaldo.Creditos.idTab);

}

function confirmarUtilizacaoCreditoClick(e) {
    var valorInformado = 0;
    $.each(_Creditores.Creditores(), function (i, creditor) {
        if ($("#chkCreditor_" + creditor.Codigo).prop("checked")) {
            if ($("#txtValor_" + creditor.Codigo).val() != "")
                valorInformado += Globalize.parseFloat($("#txtValor_" + creditor.Codigo).val());
        }
    });

    if (Globalize.format(_Creditores.ValorLiberar.val, 'n2') == Globalize.format(valorInformado, 'n2')) {
        ChamarCallbackRetorno();
        Global.fecharModal(_ControleSaldo.Creditos.id + "_knoutEscolerCredito");
    } else {
        exibirMensagem(tipoMensagem.atencao, "Valor Diferente", "O valor solicitado (" + Globalize.format(_Creditores.ValorLiberar.val, 'n2') + ") é diferente do informado (" + Globalize.format(valorInformado, 'n2') + ")")
    }
}

function ValidarUtilizacaoSaldo(valorLiberar, callback, permiteSolictarMaisCredito) {
    if (_CONFIGURACAO_TMS.DesabilitarUtilizacaoCreditoOperadores) {
        callback(null);
        return;
    }

    if (permiteSolictarMaisCredito == null)
        permiteSolictarMaisCredito = false;

    if (_ControleSaldo.PossuiSuperior.val()) {
        _Creditores.ValorLiberar.val = Globalize.parseFloat(valorLiberar);
        _ControleSaldo.RetornarEventoCredito.eventClick = callback;

        var saldoTotalDisponivel = BuscarSaldoTotal();
        if (saldoTotalDisponivel >= _Creditores.ValorLiberar.val) {

            if (_ControleSaldo.Creditos.val().length > 1) {
                $.each(_Creditores.Creditores(), function (i, creditor) {
                    $("#chkCreditor_" + creditor.Codigo).removeAttr("checked");
                    $("#txtValor_" + creditor.Codigo).attr("disabled", "disabled");
                    $("#txtValor_" + creditor.Codigo).val("");
                });                
                Global.abrirModal(_ControleSaldo.Creditos.id + "_knoutEscolerCredito");
            } else {
                ChamarCallbackRetorno();
            }
        } else {
            if (permiteSolictarMaisCredito) {

                var superior = "";
                if (_SuperioresHierarquia.Creditores().length == 1)
                    superior = " para " + _SuperioresHierarquia.Creditores()[0].Creditor;

                var mensagem = "O valor de credito que você possui (" + Globalize.format(saldoTotalDisponivel, 'n2') + ") não é suficiente para o valor informado (" + Globalize.format(_Creditores.ValorLiberar.val, 'n2') + ") , deseja solicitar" + superior + " a liberação de crédito da diferença (" + Globalize.format(_Creditores.ValorLiberar.val - saldoTotalDisponivel, 'n2') + ") ?";
                if (_ControleSaldo.Creditos.val().length == 0)
                    mensagem = "Deseja solicitar" + superior + " a liberação de crédito (" + Globalize.format(_Creditores.ValorLiberar.val, 'n2') + ") para este complemento de frete?"



                exibirConfirmacao("Solicitar Crédito", mensagem, function () {
                    if (_SuperioresHierarquia.Creditores().length > 1) {
                        Global.abrirModal(_ControleSaldo.Creditos.id + "_knoutSolicitarCredito");                        
                    } else {
                        ChamarCallbackRetornoSolicitarMaisCredito(_SuperioresHierarquia.Creditores()[0].CodigoCreditor);
                    }
                });
            } else {
                exibirMensagem(tipoMensagem.aviso, "Crédito insuficiente", "O valor de credito que você possui (" + Globalize.format(saldoTotalDisponivel, 'n2') + ") não é suficiente para essa operação.")
            }
        }
    } else {
        callback(null);
    }
}

function ConfirmarSolicitacaoClick(e) {
    var codigoCreditorSolicitar = $("#selectCreditorSolicitado").val().split("_")[1];
    ChamarCallbackRetornoSolicitarMaisCredito(codigoCreditorSolicitar);
    Global.fecharModal(_ControleSaldo.Creditos.id + "_knoutSolicitarCredito");
}

function ChamarCallbackRetornoSolicitarMaisCredito(codigoCreditorSolicitar) {
    var creditosUtilizados = new Array();
    $.each(_ControleSaldo.Creditos.val(), function (i, credito) {
        if (Globalize.parseFloat(credito.Saldo) > 0) {
            creditosUtilizados.push(criarObjetoCreditoCallback(credito, Globalize.parseFloat(credito.Saldo)));
        }
    });
    _ControleSaldo.RetornarEventoCredito.eventClick(creditosUtilizados, codigoCreditorSolicitar);
}

function ChamarCallbackRetorno() {
    var creditosUtilizados = new Array();
    if (_ControleSaldo.Creditos.val().length > 1) {
        $.each(_Creditores.Creditores(), function (j, creditor) {
            if ($("#chkCreditor_" + creditor.Codigo).prop("checked")) {
                if ($("#txtValor_" + creditor.Codigo).val() != "" && Globalize.parseFloat($("#txtValor_" + creditor.Codigo).val()) > 0) {
                    $.each(_ControleSaldo.Creditos.val(), function (i, credito) {
                        if (credito.Codigo == creditor.Codigo) {
                            creditosUtilizados.push(criarObjetoCreditoCallback(credito, Globalize.parseFloat($("#txtValor_" + creditor.Codigo).val())));
                            return false;
                        }
                    });
                }
            }
        });
    } else {
        creditosUtilizados.push(criarObjetoCreditoCallback(_ControleSaldo.Creditos.val()[0], _Creditores.ValorLiberar.val));
    }
    _ControleSaldo.RetornarEventoCredito.eventClick(creditosUtilizados);
}

function criarObjetoCreditoCallback(credito, valor) {
    var creditoUtilizado = {
        Codigo: credito.Codigo,
        Operador: credito.Creditor.Codigo,
        ValorUtilizado: valor
    };
    return creditoUtilizado;
}

function BuscarSaldoTotal() {
    var saldoTotal = 0;
    $.each(_ControleSaldo.Creditos.val(), function (i, credito) {
        saldoTotal += Globalize.parseFloat(credito.Saldo);
    });
    return saldoTotal;
}
