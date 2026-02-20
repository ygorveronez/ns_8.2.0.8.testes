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
/// <reference path="../../../js/libs/jquery.signalR-2.2.0.js" />
/// <reference path="../../Enumeradores/EnumSituacaoNotificacao.js" />
/// <reference path="../SignalR/SignalR.js" />
/// <reference path="../../Enumeradores/EnumTipoNotificacao.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />

var _ListaAjudas = new Array();

function openWhatsApp() {

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFeAdmin) {
        var whatsappUrl = "https://wa.me/554999902849";
        window.open(whatsappUrl, '_blank');
    }
}

function iniciarAjuda(issue) {
    Global.ResetarAba("body-ajuda");
    //$("#tab-r1-ajuda").prop("class", "tab-pane active");
    //$("#tab-r2-ajuda").prop("class", "tab-pane");
    //$("#tabDicas").prop("class", "tab-pane");
    _ListaAjudas = new Array();
    buscarAjuda(issue);
    carregarDicas(issue);
}

function voltarAjudaClick() {
    if (_ListaAjudas.length > 1) {
        var issue = _ListaAjudas[_ListaAjudas.length - 2];
        _ListaAjudas.splice(_ListaAjudas.length - 1, 1);
        _ListaAjudas.splice(_ListaAjudas.length - 1, 1);
        buscarAjuda(issue);
    }
}

function fecharAjudaClick() {
    $("#iFrameVideoAjuda").prop("src", "");
    $("#divModalUserHelp").modal('hide');
}

function buscarAjuda(issue) {
    _ListaAjudas.push(issue);
    data = { issue: issue };
    executarReST("Ajuda/BuscarAjuda", data, function (ret) {
        if (ret.Success) {
            var arg = JSON.parse(ret.Data);
            var video = "";

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
                if (arg.issue.custom_fields != undefined && arg.issue.custom_fields[0]?.value != null) {
                    video = arg.issue.custom_fields[0].value;
                }
            }

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
                if (arg.issue.custom_fields != undefined && arg.issue.custom_fields[2]?.value != null)
                    video = arg.issue.custom_fields[2].value;
            }

            if (video != "") {
                $("#iFrameVideoAjuda").prop("src", video);
                $("#tabVideoAjuda").show();
            } else {
                $("#tabVideoAjuda").hide();
                $("#iFrameVideoAjuda").prop("src", "");
            }

            var texto = arg.issue.description == "" ? arg.issue.custom_fields[2].value : arg.issue.description;

            $("#TituloAjuda").text(arg.issue.subject);

            var encontrouAsterisco = false;
            var encontrouImagem = false;
            var tempHTML = "";
            var novoTexto = "";
            var tempTexto = "";
            var abreLI = false;
            var numeroTarefa = "";
            var encontrouTag = false;
            var removerpontovirgula = false;

            for (var i = 0; i < texto.length; i++) {
                if (texto[i] == "!") {
                    if (texto[i] == "!" && texto[i + 1] == " ") {
                        abreLI = true;
                        if (texto[i - 1] != "!")
                            tempTexto += "!";
                    }
                    if (encontrouImagem && texto[i - 1] != "!") {
                        novoTexto += tempTexto.replace("!", '<img src="') + '" alt="">';
                        encontrouImagem = false;
                        tempTexto = "";
                    } else {
                        encontrouImagem = true;
                        tempTexto += texto[i];
                    }
                } else if (texto[i] == "*") {
                    if (texto[i] == "*" && texto[i + 1] == " ") {
                        abreLI = true;
                        if (texto[i - 1] != "*")
                            tempTexto += "*";
                    }
                    if (encontrouAsterisco && texto[i - 1] != "*") {
                        novoTexto += tempTexto.replace("*", "<strong>") + "</strong>";
                        encontrouAsterisco = false;
                        tempTexto = "";
                    } else {
                        encontrouAsterisco = true;
                        tempTexto += texto[i];
                    }
                } else {
                    if (encontrouImagem) {
                        if (texto[i] != ";" || !removerpontovirgula) {
                            tempTexto += texto[i];
                        } else {
                            removerpontovirgula = false;
                        }
                        if (texto[i] == "\r") {
                            if (abreLI) {
                                novoTexto += tempTexto.replace("!! ", "<ul><li>") + "</li></ul>";
                                abreLI = false;
                            } else {
                                novoTexto += tempTexto;
                            }
                            tempTexto = "";
                            encontrouAsterisco = false;
                        }
                    } else if (encontrouAsterisco) {
                        if (texto[i] != ";" || !removerpontovirgula) {
                            tempTexto += texto[i];
                        } else {
                            removerpontovirgula = false;
                        }
                        if (texto[i] == "\r") {
                            if (abreLI) {
                                novoTexto += tempTexto.replace("** ", "<ul><li>") + "</li></ul>";
                                abreLI = false;
                            } else {
                                novoTexto += tempTexto;
                            }
                            tempTexto = "";
                            encontrouAsterisco = false;
                        }
                    } else {
                        if (texto[i] != ";" || !removerpontovirgula) {
                            novoTexto += texto[i];
                        } else {
                            removerpontovirgula = false;
                        }
                    }

                    if (encontrouTag) {
                        if (isNaN(parseInt(texto[i]))) {
                            var linkAcesso = "<a href='javascript:void(0);' onclick='buscarAjuda(" + numeroTarefa + ")'><i class='fal fa-question-circle'></i>&nbsp;</a>";
                            tempTexto = tempTexto.replace("(#" + numeroTarefa + ")", linkAcesso);
                            novoTexto = novoTexto.replace("(#" + numeroTarefa + ")", linkAcesso);

                            tempTexto = tempTexto.replace("#" + numeroTarefa, linkAcesso);
                            novoTexto = novoTexto.replace("#" + numeroTarefa, linkAcesso);

                            numeroTarefa = "";
                            encontrouTag = false;
                            removerpontovirgula = true;
                        } else {
                            numeroTarefa += texto[i];
                        }
                    }
                    if (texto[i] == "#") {
                        encontrouTag = true;
                    }

                }
            }
            texto = novoTexto.replace(/\r\n/g, "<br/>");
            $("#contentInformacaoAjuda").html(texto);

            removerTodasTag();
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
                $("#contentInformacaoAjuda").find("ntms").hide();
                $("#contentInformacaoAjuda").find("ntms").next("br").hide();
                $("#contentInformacaoAjuda").find("tms").show();
                $("#contentInformacaoAjuda").find("tms").next("br").show();
            } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
                $("#contentInformacaoAjuda").find("nembarcador").hide();
                $("#contentInformacaoAjuda").find("nembarcador").next("br").hide();
                $("#contentInformacaoAjuda").find("embarcador").show();
                $("#contentInformacaoAjuda").find("embarcador").next("br").show();
            } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
                $("#contentInformacaoAjuda").find("nmulticte").hide();
                $("#contentInformacaoAjuda").find("nmulticte").next("br").hide();
                $("#contentInformacaoAjuda").find("multicte").show();
                $("#contentInformacaoAjuda").find("multicte").next("br").show();
            } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.CallCenter) {
                $("#contentInformacaoAjuda").find("ncallcenter").hide();
                $("#contentInformacaoAjuda").find("ncallcenter").next("br").hide();
                $("#contentInformacaoAjuda").find("callcenter").show();
                $("#contentInformacaoAjuda").find("callcenter").next("br").show();
            } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Terceiros) {
                $("#contentInformacaoAjuda").find("nterceiros").hide();
                $("#contentInformacaoAjuda").find("nterceiros").next("br").hide();
                $("#contentInformacaoAjuda").find("terceiros").show();
                $("#contentInformacaoAjuda").find("terceiros").next("br").show();
            } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe) {
                $("#contentInformacaoAjuda").find("nfe").hide();
                $("#contentInformacaoAjuda").find("nfe").next("br").hide();
                $("#contentInformacaoAjuda").find("nfe").show();
                $("#contentInformacaoAjuda").find("nfe").next("br").show();
            }
            $("#divModalUserHelp").modal({ keyboard: true, backdrop: 'static' });
            $("#divModalUserHelp").modal('show');
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", ret.Msg);
        }
    });
}

function removerTodasTag() {
    $("#contentInformacaoAjuda").find("tms").hide();
    $("#contentInformacaoAjuda").find("tms").next("br").hide();

    $("#contentInformacaoAjuda").find("embarcador").hide();
    $("#contentInformacaoAjuda").find("embarcador").next("br").hide();

    $("#contentInformacaoAjuda").find("multicte").hide();
    $("#contentInformacaoAjuda").find("multicte").next("br").hide();

    $("#contentInformacaoAjuda").find("callcenter").hide();
    $("#contentInformacaoAjuda").find("callcenter").next("br").hide();

    $("#contentInformacaoAjuda").find("terceiros").hide();
    $("#contentInformacaoAjuda").find("terceiros").next("br").hide();

    $("#contentInformacaoAjuda").find("nfe").hide();
    $("#contentInformacaoAjuda").find("nfe").next("br").hide();

    $("#contentInformacaoAjuda").find("homolog").hide();
    $("#contentInformacaoAjuda").find("homolog").next("br").hide();

}