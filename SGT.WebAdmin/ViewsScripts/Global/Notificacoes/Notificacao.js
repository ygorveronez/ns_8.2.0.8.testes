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


//*******MAPEAMENTO KNOUCKOUT*******

var _notificacaoGlobal;

var NotificacaoGlobal = function () {
    this.CodigoObjeto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, eventChange: notificacoesScroll, visible: ko.observable(false) });
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, type: types.local });
    this.RequisicaoPendente = PropertyEntity({ val: ko.observable(false), def: false, type: types.local });
    this.NaoVistas = PropertyEntity({ val: ko.observable(0), def: 0, eventClick: naoVistasClick });
    this.Notificacoes = ko.observableArray();
    this.DadosPesquisaInicial = undefined;
    this.VerTodasNotificacoes = PropertyEntity({ val: ko.observable(0), type: types.event, def: 0, eventClick: verTodasNotificacoesClick });
}

var DetalheNotificacaoGlobal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoObjetoNotificacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataNotificacao = PropertyEntity();
    this.SituacaoNotificacao = PropertyEntity({ val: ko.observable(EnumSituacaoNotificacao.Nova), def: EnumSituacaoNotificacao.Nova, getType: typesKnockout.int });
    this.TipoNotificacao = PropertyEntity({ val: ko.observable(EnumTipoNotificacao.credito), def: EnumTipoNotificacao.credito, getType: typesKnockout.int });
    this.URLPagina = PropertyEntity({ eventClick: URLPaginaEventClick });
    this.Nota = PropertyEntity();
    this.Icone = PropertyEntity();
    this.IconeCorFundo = PropertyEntity();
    this.ReadClass = PropertyEntity();
}

//*******EVENTOS*******

function loadNotificacaoGlobal(e) {

    _notificacaoGlobal = new NotificacaoGlobal();
    KoBindings(_notificacaoGlobal, "knoutNotificacaoGlobal");
    loadCentralProcessamentoGlobal(function () {
        //initApp.domReadyMisc();
        buscarNotificacoes();
        SignalRNotificarUsuarioEvent = retornoNotificarSignalREvent;
        SignalRInformarPercentualProcessadoEvent = informarPercentualProcessadoREvento;
    });
}

function informarPercentualProcessadoREvento(retorno) {
    if (retorno.TipoNotificacao == EnumTipoNotificacao.comissao) {
        if (typeof AtualizarProgressComissao != "undefined")
            AtualizarProgressComissao(retorno.Codigo, retorno.Percentual);
    }
    else if (retorno.TipoNotificacao == EnumTipoNotificacao.fatura) {
        if (typeof AtualizarProgressFatura != "undefined")
            AtualizarProgressFatura(retorno.Codigo, retorno.Percentual);
    }
    else if (retorno.TipoNotificacao == EnumTipoNotificacao.pedagio) {
        if (typeof AtualizarProgressImportacaoPedagio != "undefined")
            AtualizarProgressImportacaoPedagio(retorno.Percentual);
    }
    else if (retorno.TipoNotificacao == EnumTipoNotificacao.faturamentoMensalDocumentos) {
        if (typeof AtualizarProgressDocumentos != "undefined")
            AtualizarProgressDocumentos(retorno.Percentual, retorno.Codigo);
    }
    else if (retorno.TipoNotificacao == EnumTipoNotificacao.faturamentoMensalEmail) {
        if (typeof AtualizarProgressEmails != "undefined")
            AtualizarProgressEmails(retorno.Percentual, retorno.Codigo);
    }
    else if (retorno.TipoNotificacao == EnumTipoNotificacao.pagamentoAgregado) {
        if (typeof AtualizarProgressDocumentosAgregado != "undefined")
            AtualizarProgressDocumentosAgregado(retorno.Percentual, retorno.Codigo);
    }
    else if (retorno.TipoNotificacao == undefined) {
        var notificacao = JSON.parse(retorno);
        if (notificacao.TipoNotificacao != undefined) {
            if (notificacao.TipoNotificacao == EnumTipoNotificacao.comissao) {
                if (typeof AtualizarProgressComissao != "undefined")
                    AtualizarProgressComissao(notificacao.Codigo, notificacao.Percentual);
            }
            else if (notificacao.TipoNotificacao == EnumTipoNotificacao.fatura) {
                if (typeof AtualizarProgressFatura != "undefined")
                    AtualizarProgressFatura(notificacao.Codigo, notificacao.Percentual);
            }
            else if (notificacao.TipoNotificacao == EnumTipoNotificacao.pedagio) {
                if (typeof AtualizarProgressImportacaoPedagio != "undefined")
                    AtualizarProgressImportacaoPedagio(notificacao.Percentual);
            }
            else if (notificacao.TipoNotificacao == EnumTipoNotificacao.faturamentoMensalDocumentos) {
                if (typeof AtualizarProgressDocumentos != "undefined")
                    AtualizarProgressDocumentos(notificacao.Percentual, notificacao.Codigo);
            }
            else if (notificacao.TipoNotificacao == EnumTipoNotificacao.pagamentoAgregado) {
                if (typeof AtualizarProgressDocumentosAgregado != "undefined")
                    AtualizarProgressDocumentosAgregado(notificacao.Percentual, notificacao.Codigo);
            }
            else if (notificacao.TipoNotificacao == EnumTipoNotificacao.faturamentoMensalEmail) {
                if (typeof AtualizarProgressEmails != "undefined")
                    AtualizarProgressEmails(notificacao.Percentual, notificacao.Codigo);
            }
        }
    }
}

function retornoNotificarSignalREvent(retorno) {
    var notificacao = JSON.parse(retorno);
    _notificacaoGlobal.NaoVistas.val(_notificacaoGlobal.NaoVistas.val() + 1);

    if (!verificarURLssaoIgual(location.hash, notificacao.URLPagina)) {
        ExibirInformacoesNotificacaoForaPagina(notificacao);
    } else {
        ValidarRetornoNotificacoesNaPagina(notificacao);
    }
}

function verificarURLssaoIgual(url1, url2) {
    if (url1.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "").replace("#", "").toLowerCase() == url2.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "").replace("#", "").toLowerCase()) {
        return true;
    } else {
        return false;
    }
}

function naoVistasClick() {
    _RequisicaoIniciada = true;
    if (_notificacaoGlobal.NaoVistas.val() > 0) {
        executarReST("Notificacao/SetarParaVisualizadas", null, function (arg) {
            if (arg.Success) {
                _notificacaoGlobal.NaoVistas.val(_notificacaoGlobal.NaoVistas.def);
            } else {
                _notificacaoGlobal.NaoVistas.val(_notificacaoGlobal.NaoVistas.val());
            }
            _RequisicaoIniciada = false;
        });
    }

}

function notificacoesScroll(e, sender) {
    var elem = sender.target;
    if (_notificacaoGlobal.Inicio.val() < _notificacaoGlobal.Total.val()) {
        if (elem.scrollTop > (elem.scrollHeight - elem.offsetHeight - 200)) {
            buscarNotificacoes();
        }
    }
}

function URLPaginaEventClick(e) {
    _RequisicaoIniciada = true;
    MarcarNotificacaoComoLida(e);

    if (e.TipoNotificacao.val() == EnumTipoNotificacao.relatorio)
        baixarRelatorio(e);
    else if (e.TipoNotificacao.val() == EnumTipoNotificacao.arquivo)
        baixarArquivo(e);
    else if (e.CodigoObjetoNotificacao.val() && e.URLPagina.val() != "") {
        _notificacaoGlobal.CodigoObjeto.val(e.CodigoObjetoNotificacao.val());

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe)
            VerificarENavegarParaURLNotificacao(e.URLPagina.val());

        fecharNotificacao();
    }
}

function VerificarENavegarParaURLNotificacao(URLPagina) {
    if (verificarURLssaoIgual(location.hash, URLPagina))
        checkURL();
    else
        location.href = "#" + URLPagina;
}

function MarcarNotificacaoComoLida(e) {
    if (e.SituacaoNotificacao.val() == EnumSituacaoNotificacao.Nova) {
        var data = { Codigo: e.Codigo.val() };
        executarReST("Notificacao/MarcarNotificacaoComoLida", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    e.SituacaoNotificacao.val(EnumSituacaoNotificacao.Lida);
                    e.ReadClass.val("");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            }
            _RequisicaoIniciada = false;
        });
    }
}

function baixarArquivo(e) {
    BuscarProcessamentosPendentes(function () {
        var data = { Codigo: e.CodigoObjetoNotificacao.val() };
        executarDownload("ControleGeracaoArquivo/DownloadArquivo", data);
    });
}

function baixarRelatorio(e) {
    BuscarProcessamentosPendentes(function () {
        var data = { Codigo: e.CodigoObjetoNotificacao.val() };
        executarDownload("Relatorios/Relatorio/DownloadRelatorio", data);
    });
}

function verTodasNotificacoesClick(e) {
    location.href = "#Notificacoes/Notificacao";
    fecharNotificacao();
}

//*******MÉTODOS*******

function ExibirInformacoesNotificacaoForaPagina(notificacao) {
    if (
        (notificacao.TipoNotificacao == EnumTipoNotificacao.relatorio) ||
        (notificacao.TipoNotificacao == EnumTipoNotificacao.comissao) ||
        (notificacao.TipoNotificacao == EnumTipoNotificacao.fatura) ||
        (notificacao.TipoNotificacao == EnumTipoNotificacao.pedagio) ||
        (notificacao.TipoNotificacao == EnumTipoNotificacao.arquivo)
    )
        BuscarProcessamentosPendentes();

    _notificacaoGlobal.Total.val(_notificacaoGlobal.Total.val() + 1);
    _notificacaoGlobal.Inicio.val(_notificacaoGlobal.Inicio.val() + 1);
    preecherNotificacaoGlobal(notificacao, true);
    validarNaoVistas();
    exibirAlertaNotificacao(notificacao.Nota, notificacao.Icone, notificacao.DataNotificacao);
}

function ValidarRetornoNotificacoesNaPagina(notificacao) {
    if (notificacao.TipoNotificacao == EnumTipoNotificacao.relatorio) {
        var knouNotificacao = preecherNotificacaoGlobal(notificacao, true);
        naoVistasClick();
        MarcarNotificacaoComoLida(knouNotificacao);
        baixarRelatorio(knouNotificacao);
        BuscarProcessamentosPendentes();
    } else if (notificacao.TipoNotificacao == EnumTipoNotificacao.arquivo) {
        var knouNotificacao = preecherNotificacaoGlobal(notificacao, true);
        naoVistasClick();
        MarcarNotificacaoComoLida(knouNotificacao);
        baixarArquivo(knouNotificacao);
        BuscarProcessamentosPendentes();
    } else if (notificacao.TipoNotificacao == EnumTipoNotificacao.comissao) {
        if (typeof VerificarSeComissaoNotificadaEstaSelecionada != "undefined") {//método do js de comissões
            var knouNotificacao = preecherNotificacaoGlobal(notificacao, true);
            naoVistasClick();
            MarcarNotificacaoComoLida(knouNotificacao);
            VerificarSeComissaoNotificadaEstaSelecionada(notificacao.CodigoObjetoNotificacao);
            BuscarProcessamentosPendentes();
        } else {
            ExibirInformacoesNotificacaoForaPagina(notificacao);
        }
    } else if (notificacao.TipoNotificacao == EnumTipoNotificacao.fatura) {
        if (typeof VerificarSeFaturaNotificadaEstaSelecionada != "undefined") {//método do js de comissões
            var knouNotificacao = preecherNotificacaoGlobal(notificacao, true);
            naoVistasClick();
            MarcarNotificacaoComoLida(knouNotificacao);
            VerificarSeFaturaNotificadaEstaSelecionada(notificacao.CodigoObjetoNotificacao);
            BuscarProcessamentosPendentes();
        } else {
            ExibirInformacoesNotificacaoForaPagina(notificacao);
        }
    } else if (notificacao.TipoNotificacao == EnumTipoNotificacao.pedagios) {
        if (typeof VerificarImportacaoPedagioEstaSelecionada != "undefined") {//método do js de comissões
            var knouNotificacao = preecherNotificacaoGlobal(notificacao, true);
            naoVistasClick();
            MarcarNotificacaoComoLida(knouNotificacao);
            VerificarImportacaoPedagioEstaSelecionada();
            BuscarProcessamentosPendentes();
        } else {
            ExibirInformacoesNotificacaoForaPagina(notificacao);
        }
    } else if (notificacao.TipoNotificacao == EnumTipoNotificacao.faturamentoMensalDocumentos) {
        if (typeof VerificarSeDocumentosNotificadaEstaSelecionada != "undefined") {//método do js de comissões
            ExibirInformacoesNotificacaoForaPagina(notificacao);
            var knouNotificacao = preecherNotificacaoGlobal(notificacao, true);
            naoVistasClick();
            MarcarNotificacaoComoLida(knouNotificacao);
            VerificarSeDocumentosNotificadaEstaSelecionada(notificacao.CodigoObjetoNotificacao);
            BuscarProcessamentosPendentes();
        } else {
            ExibirInformacoesNotificacaoForaPagina(notificacao);
        }
    } else if (notificacao.TipoNotificacao == EnumTipoNotificacao.pagamentoAgregado) {
        if (typeof VerificarSeDocumentosagregadoNotificadaEstaSelecionada != "undefined") {//método do js de comissões
            ExibirInformacoesNotificacaoForaPagina(notificacao);
            var knouNotificacao = preecherNotificacaoGlobal(notificacao, true);
            naoVistasClick();
            MarcarNotificacaoComoLida(knouNotificacao);
            VerificarSeDocumentosagregadoNotificadaEstaSelecionada(notificacao.CodigoObjetoNotificacao);
            BuscarProcessamentosPendentes();
        } else {
            ExibirInformacoesNotificacaoForaPagina(notificacao);
        }
    } else if (notificacao.TipoNotificacao == EnumTipoNotificacao.faturamentoMensalEmail) {
        if (typeof VerificarSeEmailsNotificadaEstaSelecionada != "undefined") {//método do js de comissões
            ExibirInformacoesNotificacaoForaPagina(notificacao);
            var knouNotificacao = preecherNotificacaoGlobal(notificacao, true);
            naoVistasClick();
            MarcarNotificacaoComoLida(knouNotificacao);
            VerificarSeEmailsNotificadaEstaSelecionada(notificacao.CodigoObjetoNotificacao);
            BuscarProcessamentosPendentes();
        } else {
            ExibirInformacoesNotificacaoForaPagina(notificacao);
        }
    } else {
        ExibirInformacoesNotificacaoForaPagina(notificacao);
    }
}

function fecharNotificacao() {
    var activityEle = $("#activity");
    activityEle.next(".ajax-dropdown").fadeOut(150);
    activityEle.removeClass("active");
}

function buscarNotificacoes() {
    if (!_notificacaoGlobal.RequisicaoPendente.val()) {
        _notificacaoGlobal.RequisicaoPendente.val(true);
        var quantidadePorVez = 6;
        _RequisicaoIniciada = true;
        var data = { inicio: _notificacaoGlobal.Inicio.val(), limite: quantidadePorVez };
        executarReST("Notificacao/BuscarNovasNotificacoes", data, function (arg) {
            if (arg.Success) {
                _notificacaoGlobal.Total.val(arg.Data.Total);
                _notificacaoGlobal.Inicio.val(_notificacaoGlobal.Inicio.val() + quantidadePorVez);
                var retorno = arg.Data;
                _notificacaoGlobal.NaoVistas.val(retorno.NaoVistas);
                $.each(retorno.Notificacoes, function (i, notificacao) {
                    preecherNotificacaoGlobal(notificacao, false);
                });
                validarNaoVistas();
            }
            _notificacaoGlobal.RequisicaoPendente.val(false);
            _RequisicaoIniciada = false;
        });
    }

}

function preecherNotificacaoGlobal(notificacao, topo) {
    var knouNotificacao = new DetalheNotificacaoGlobal();
    var data = { Data: notificacao }
    PreencherObjetoKnout(knouNotificacao, data);
    if (topo)
        unshiftNotificacao(knouNotificacao);
    else
        pushNotificacao(knouNotificacao);

    return knouNotificacao;
}

function unshiftNotificacao(knouNotificacao) {
    _notificacaoGlobal.Notificacoes.unshift(knouNotificacao);
}

function pushNotificacao(knouNotificacao) {
    _notificacaoGlobal.Notificacoes.push(knouNotificacao);
}

function validarNaoVistas() {
    if (_notificacaoGlobal.NaoVistas.val() > 0) {
        var badge = $("#activity > .badge");
        badge.addClass("bg-color-red bounceIn animated");
    }
    if (_notificacaoGlobal.Total.val() > 0) {
        _notificacaoGlobal.Total.visible(true);
    }
}