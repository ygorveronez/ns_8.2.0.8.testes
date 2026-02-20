//*******MAPEAMENTO KNOUCKOUT*******

var _HTMLIntegracaoDocumentoTransporteNatura = "";
var _pesquisaDocumentoTransporteNatura;
var _gridDocumentoTransporteNatura;

var PesquisaDocumentoTransporteNatura = function () {

    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CargaPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 20, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.NumeroDoDT.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(true), configInt: { thousands: "", allowZero: false, precision: 0 } });
    this.NumeroNF = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 15, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.NumeroDaNFe.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(true), configInt: { thousands: "", allowZero: false, precision: 0 } });
    this.Consultar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentoTransporteNatura.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.Consultar, icon: "fal fa-search", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });

    this.Vincular = PropertyEntity({
        eventClick: function (e) {
            VincularDocumentoTransporteNatura();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.Vincular, icon: "fal fa-chevron-down", visible: ko.observable(false), enable: ko.observable(true)
    });

    this.Excluir = PropertyEntity({
        eventClick: function (e) {
            DesvincularDocumentoTransporteNatura();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.Desvincular, icon: "fal fa-times", visible: ko.observable(false), enable: ko.observable(true)
    });
}

function LoadIntegracaoDocumentoTransporteNatura(carga, dados, integracoes) {
    _pesquisaDocumentoTransporteNatura = null;
    _gridDocumentoTransporteNatura = null;

    var idDivIntegracaoNatura = "divIntegracaoNatura_" + carga.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao";
    var divIntegracaoNatura = $("#" + idDivIntegracaoNatura);

    if (carga.CargaTransbordo.val() === true) {
        divIntegracaoNatura.addClass("d-none");
        return;
    }

    if (integracoes != null && integracoes.length > 0) {
        for (var i = 0; i < integracoes.length; i++) {
            if (integracoes[i] == EnumTipoIntegracao.Natura) {

                CarregarHTMLIntegracaoDocumentoTransporteNatura().then(function () {

                    divIntegracaoNatura.html(_HTMLIntegracaoDocumentoTransporteNatura);

                    _pesquisaDocumentoTransporteNatura = new PesquisaDocumentoTransporteNatura();
                    _pesquisaDocumentoTransporteNatura.Carga.val(carga.Codigo.val());
                    _pesquisaDocumentoTransporteNatura.CargaPedido.val(_documentoEmissao.CargaPedido.val());

                    KoBindings(_pesquisaDocumentoTransporteNatura, idDivIntegracaoNatura);
                    
                    divIntegracaoNatura.removeClass("d-none");

                    if (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe) {
                        if (dados.length > 0) {
                            _pesquisaDocumentoTransporteNatura.Excluir.visible(true);
                        } else {
                            _pesquisaDocumentoTransporteNatura.Vincular.visible(true);
                            _pesquisaDocumentoTransporteNatura.Numero.visible(true);
                        }
                    } else {
                        var divConteudoEsquerda = $("#divConteudoEsquerda_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao");

                        divConteudoEsquerda.removeClass("col-lg-6");
                        divConteudoEsquerda.addClass("col-lg-12");

                        _pesquisaDocumentoTransporteNatura.Excluir.visible(false);
                        _pesquisaDocumentoTransporteNatura.Vincular.visible(false);
                        _pesquisaDocumentoTransporteNatura.Numero.visible(false);
                    }

                    BuscarDocumentosTransporteNatura(dados.length > 0);

                    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_InformarDocumentosFiscais, _PermissoesPersonalizadasCarga)) {
                        _pesquisaDocumentoTransporteNatura.Excluir.enable(false);
                        _pesquisaDocumentoTransporteNatura.Vincular.enable(false);
                    }
                });

                break;
            }
        }
    } else {
        divIntegracaoNatura.addClass("d-none");
    }
}

function CarregarHTMLIntegracaoDocumentoTransporteNatura() {
    var p = new promise.Promise();

    if (_HTMLIntegracaoDocumentoTransporteNatura.length == 0) {
        $.get("Content/Static/Carga/IntegracaoNatura.html?dyn=" + guid(), function (data) {
            _HTMLIntegracaoDocumentoTransporteNatura = data;
            p.done();
        });
    } else {
        p.done();
    }

    return p;
}

function BuscarDocumentosTransporteNatura(somenteLeitura) {
    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        somenteLeitura: somenteLeitura
    }

    _gridDocumentoTransporteNatura = new GridView(_pesquisaDocumentoTransporteNatura.Consultar.idGrid, "CargaIntegracaoDocumentoTransporteNatura/Pesquisa", _pesquisaDocumentoTransporteNatura, null, { column: 2, dir: orderDir.desc }, 5, null, null, null, multiplaescolha);
    _gridDocumentoTransporteNatura.CarregarGrid();

}

function DesvincularDocumentoTransporteNatura() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteDesvincularTodosOsDocumentosDeTransporteDeNaturaDestaCargaTodasAsNotasFiscaisTambemSeraoApagadas, function () {
        executarReST("CargaIntegracaoDocumentoTransporteNatura/Desvincular", { Carga: _pesquisaDocumentoTransporteNatura.Carga.val(), CargaPedido: _pesquisaDocumentoTransporteNatura.CargaPedido.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DocumentosDeTransporteDesvinculadosComSucesso);

                    _gridNotasParaEmissao.CarregarGrid();
                    _gridDocumentoTransporteNatura.SetarRegistrosSomenteLeitura(false);
                    _gridDocumentoTransporteNatura.CarregarGrid();

                    _pesquisaDocumentoTransporteNatura.Excluir.visible(false);
                    _pesquisaDocumentoTransporteNatura.Vincular.visible(true);
                    _pesquisaDocumentoTransporteNatura.Numero.visible(true);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function VincularDocumentoTransporteNatura() {
    var documentosSelecionados = _gridDocumentoTransporteNatura.ObterMultiplosSelecionados();

    if (documentosSelecionados.length <= 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.SelecioneAoMenosUmDocumentoParaRealizarVinculoCarga);
        return;
    }

    var codigosDocumentosSelecionados = new Array();

    for (var i = 0; i < documentosSelecionados.length; i++)
        codigosDocumentosSelecionados.push(documentosSelecionados[i].Codigo);

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteVincularOsDocumentosDeTransporteSelecionadosEstaCarga.format(codigosDocumentosSelecionados.length), function () {
        executarReST("CargaIntegracaoDocumentoTransporteNatura/Vincular", { Carga: _pesquisaDocumentoTransporteNatura.Carga.val(), CargaPedido: _pesquisaDocumentoTransporteNatura.CargaPedido.val(), Documentos: JSON.stringify(codigosDocumentosSelecionados) }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DocumentosDeTransporteVinculadosComSucesso);

                    _gridNotasParaEmissao.CarregarGrid();

                    _gridDocumentoTransporteNatura.SetarRegistrosSomenteLeitura(true);
                    _gridDocumentoTransporteNatura.CarregarGrid();

                    _pesquisaDocumentoTransporteNatura.Excluir.visible(true);
                    _pesquisaDocumentoTransporteNatura.Vincular.visible(false);
                    _pesquisaDocumentoTransporteNatura.Numero.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}