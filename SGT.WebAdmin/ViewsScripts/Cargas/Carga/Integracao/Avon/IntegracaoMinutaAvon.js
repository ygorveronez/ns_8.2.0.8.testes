//*******MAPEAMENTO KNOUCKOUT*******

var _HTMLIntegracaoMinutaAvon = "";
var _integracaoMinutaAvon;
var _pesquisaHistoricoConsultaMinutaAvon;
var _gridHistoricoConsultaMinutaAvon;

var PesquisaHistoricoConsultaMinutaAvon = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var IntegracaoMinutaAvon = function () {

    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroMinuta = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 15, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.NumeroDaMinuta.getFieldDescription(), visible: ko.observable(true), enable: ko.observable(true), configInt: { thousands: "", allowZero: false, precision: 0 }, required: true });

    this.MensagemErro = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.RetornoAvon.getFieldDescription() });
    this.MensagemSucesso = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.RetornoAvon.getFieldDescription() });
    this.DataConsulta = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.DataDaConsulta.getFieldDescription() });
    this.PesoTotalDocumentos = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.PesoTotalNFes.getFieldDescription() });
    this.QuantidadeDocumentos = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.QuantidadeDeNFes.getFieldDescription() });
    this.QuantidadeDocumentosGerados = PropertyEntity({ val: ko.observable("0"), def: "0", text: Localization.Resources.Cargas.Carga.QuantidadeDeNFes.getFieldDescription() });
    this.ValorTotalDocumentos = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.ValorTotalDeNFes.getFieldDescription() });
    this.Usuario = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.Operador.getFieldDescription() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoMinutaAvon.Problemas), def: EnumSituacaoMinutaAvon.Problemas, getType: typesKnockout.int });
    this.Manual = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.SalvarManual = PropertyEntity({
        eventClick: function (e) {
            SalvarMinutaManualAvon();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.SalvarMinutaManual, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });

    this.Consultar = PropertyEntity({
        eventClick: function (e) {
            ConsultarMinutaAvon();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.Consultar, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });

    this.ExcluirMinuta = PropertyEntity({
        eventClick: function (e) {
            ExcluirMinutaAvon();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.ExcluirMinuta, idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });

    this.HistoricoConsultas = PropertyEntity({
        eventClick: function (e) {
            ExibirHistoricoConsultaMinutaAvon();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.HistoricoDeConsultas, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
}

function LoadIntegracaoMinutaAvon(carga, dados, integracoes) {
    _integracaoMinutaAvon = null;
    _pesquisaHistoricoConsultaMinutaAvon = null;

    var idDivIntegracaoAvon = "divIntegracaoAvon_" + carga.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao";
    var divIntegracaoAvon = $("#" + idDivIntegracaoAvon);

    if (integracoes != null && integracoes.length > 0) {
        for (var i = 0; i < integracoes.length; i++) {
            if (integracoes[i] == EnumTipoIntegracao.Avon) {

                CarregarHTMLIntegracaoMinutaAvon().then(function () {

                    divIntegracaoAvon.html(_HTMLIntegracaoMinutaAvon);

                    _integracaoMinutaAvon = new IntegracaoMinutaAvon();
                    _integracaoMinutaAvon.Carga.val(carga.Codigo.val());

                    _pesquisaHistoricoConsultaMinutaAvon = new PesquisaHistoricoConsultaMinutaAvon();
                    _pesquisaHistoricoConsultaMinutaAvon.Carga.val(carga.Codigo.val());

                    KoBindings(_integracaoMinutaAvon, idDivIntegracaoAvon);

                    AlterarEstadoIntegracaoMinutaAvon(dados);

                    divIntegracaoAvon.removeClass("d-none");

                    if (_cargaAtual.SituacaoCarga.val() != EnumSituacoesCarga.AgNFe && dados != null && dados.Situacao == EnumSituacaoMinutaAvon.Sucesso) {
                        _integracaoMinutaAvon.ExcluirMinuta.visible(false);
                        _documentoEmissao.Pedido.enable(false);
                        $("#" + idDivIntegracaoAvon + " footer").addClass("d-none");
                    }
                    else if (_cargaAtual.SituacaoCarga.val() != EnumSituacoesCarga.AgNFe)
                        divIntegracaoAvon.addClass("d-none")

                    if (dados != null && (dados.Situacao == EnumSituacaoMinutaAvon.Sucesso || dados.Situacao == EnumSituacaoMinutaAvon.Excluido))
                        carregarGridDocumentosParaEmissao();

                    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_InformarDocumentosFiscais, _PermissoesPersonalizadasCarga)) {
                        _integracaoMinutaAvon.ExcluirMinuta.enable(false);
                        _integracaoMinutaAvon.Consultar.enable(false);
                        _integracaoMinutaAvon.SalvarManual.enable(false);
                    }
                });

                break;
            }
        }
    } else {
        divIntegracaoAvon.addClass("d-none");
    }
}

function CarregarHTMLIntegracaoMinutaAvon() {
    var p = new promise.Promise();

    if (_HTMLIntegracaoMinutaAvon.length == 0) {
        $.get("Content/Static/Carga/IntegracaoAvon.html?dyn=" + guid(), function (data) {
            _HTMLIntegracaoMinutaAvon = data;
            p.done();
        });
    } else {
        p.done();
    }

    return p;
}

function ConsultarMinutaAvon() {
    var valido = ValidarCamposObrigatorios(_integracaoMinutaAvon);

    if (valido) {
        executarReST("CargaIntegracaoMinutaAvon/ConsultarMinuta", { Carga: _integracaoMinutaAvon.Carga.val(), NumeroMinuta: _integracaoMinutaAvon.NumeroMinuta.val() }, function (r) {
            if (r.Success) {
                if (r.Data.Situacao == EnumSituacaoMinutaAvon.Sucesso) {
                    _gridNotasParaEmissao.Destroy();
                    _gridNotasParaEmissao = null;
                    carregarGridDocumentosParaEmissao();
                }

                AlterarEstadoIntegracaoMinutaAvon(r.Data);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function SalvarMinutaManualAvon() {
    var valido = ValidarCamposObrigatorios(_integracaoMinutaAvon);

    if (valido) {
        executarReST("CargaIntegracaoMinutaAvon/SalvarMinutaManual", { Carga: _integracaoMinutaAvon.Carga.val(), NumeroMinuta: _integracaoMinutaAvon.NumeroMinuta.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    AlterarEstadoIntegracaoMinutaAvon(r.Data);
                } else {

                }

            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function ExcluirMinutaAvon() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteExcluirOsDadosDaMinutaNumeroTodosOsDadosDeDocumentosSeraoApagados.format(_integracaoMinutaAvon.NumeroMinuta.val()), function () {
        executarReST("CargaIntegracaoMinutaAvon/ExcluirMinuta", { Carga: _integracaoMinutaAvon.Carga.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.MinutaExcluidoComSucesso);

                    if (_gridNotasParaEmissao != null) {
                        _gridNotasParaEmissao.Destroy();
                        _gridNotasParaEmissao = null;
                    }

                    carregarGridDocumentosParaEmissao();

                    _integracaoMinutaAvon.NumeroMinuta.visible(true);
                    _integracaoMinutaAvon.MensagemErro.visible(false);
                    _integracaoMinutaAvon.MensagemSucesso.visible(false);
                    _integracaoMinutaAvon.ExcluirMinuta.visible(false);
                    _integracaoMinutaAvon.Consultar.visible(true);
                    _integracaoMinutaAvon.SalvarManual.visible(true);

                    var divConteudoDireita = $("#divConteudoDireita_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao");
                    var divConteudoEsquerda = $("#divConteudoEsquerda_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao");
                    var divDocumentoManual = $("#divDocumentoManual_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao");

                    divConteudoEsquerda.removeClass("col-lg-12");
                    divConteudoEsquerda.addClass("col-lg-6");
                    divConteudoDireita.removeClass("d-none");
                    divDocumentoManual.removeClass("d-none");
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function AlterarEstadoIntegracaoMinutaAvon(dados) {
    if (dados == null)
        return;

    _integracaoMinutaAvon.MensagemErro.visible(false);
    _integracaoMinutaAvon.MensagemSucesso.visible(false);
    _integracaoMinutaAvon.ExcluirMinuta.visible(false);

    _integracaoMinutaAvon.NumeroMinuta.val(dados.NumeroMinuta);
    _integracaoMinutaAvon.DataConsulta.val(dados.DataConsulta);
    _integracaoMinutaAvon.Situacao.val(dados.Situacao);
    _integracaoMinutaAvon.Manual.val(dados.Manual);

    if (dados.Situacao == EnumSituacaoMinutaAvon.Sucesso || dados.Situacao == EnumSituacaoMinutaAvon.SalvandoNotasFiscais) {
        _integracaoMinutaAvon.NumeroMinuta.visible(false);
        _integracaoMinutaAvon.MensagemSucesso.val(dados.Mensagem);
        _integracaoMinutaAvon.PesoTotalDocumentos.val(dados.PesoTotalDocumentos);
        _integracaoMinutaAvon.QuantidadeDocumentos.val(dados.QuantidadeDocumentos);
        _integracaoMinutaAvon.ValorTotalDocumentos.val(dados.ValorTotalDocumentos);
        _integracaoMinutaAvon.Usuario.val(dados.Usuario);
        _integracaoMinutaAvon.MensagemSucesso.visible(true);
        _integracaoMinutaAvon.Consultar.visible(false);
        _integracaoMinutaAvon.SalvarManual.visible(false);
        _integracaoMinutaAvon.ExcluirMinuta.visible(true);

        var divConteudoDireita = $("#divConteudoDireita_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao");
        var divConteudoEsquerda = $("#divConteudoEsquerda_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao");
        var divDocumentoManual = $("#divDocumentoManual_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao");

        if (_integracaoMinutaAvon.Manual.val() !== true || _cargaAtual.SituacaoCarga.val() != EnumSituacoesCarga.AgNFe) {
            divConteudoDireita.addClass("d-none");
            divDocumentoManual.addClass("d-none");

            divConteudoEsquerda.removeClass("col-lg-6");
            divConteudoEsquerda.addClass("col-lg-12");
        }

        if (dados.Situacao == EnumSituacaoMinutaAvon.SalvandoNotasFiscais) {
            var divAtualizacaoDocumentosGerados = $("#divIntegracaoAvon_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao .notasFiscaisGeradas");
            divAtualizacaoDocumentosGerados.removeClass("d-none");
            _documentoEmissao.Pedido.enable(false);
            _integracaoMinutaAvon.ExcluirMinuta.visible(true);
        } else {
            _documentoEmissao.Pedido.enable(true);
            _integracaoMinutaAvon.ExcluirMinuta.visible(true);
        }

    } else if (dados.Situacao == EnumSituacaoMinutaAvon.Problemas) {

        _integracaoMinutaAvon.MensagemErro.val(dados.Mensagem);
        _integracaoMinutaAvon.MensagemErro.visible(true);
    }
}

function HideIntegracaoMinutaAvon() {
    var divIntegracaoAvon = $("#divIntegracaoAvon_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao");

    divIntegracaoAvon.addClass("d-none");
}

function ShowIntegracaoMinutaAvon() {
    var divIntegracaoAvon = $("#divIntegracaoAvon_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao");

    divIntegracaoAvon.removeClass("d-none");
}

function HandleAtualizacaoMinutaAvonSignalR(retorno, knoutCarga) {
    if (_cargaAtual != null && _integracaoMinutaAvon != null && retorno.CodigoCarga == _cargaAtual.Codigo.val()) {
        if (retorno.Situacao != EnumSituacaoMinutaAvon.SalvandoNotasFiscais) {
            executarReST("CargaNotasFiscais/ObterInformacoesGerais", { Carga: _cargaAtual.Codigo.val() }, function (r) {
                if (r.Success) {
                    loadInfoDocumentosParaEmissao(_cargaAtual, (r.Data.IntegracaoAvon == null ? true : (r.Data.IntegracaoAvon.Situacao == EnumSituacaoMinutaAvon.Problemas ? true : false)));
                    LoadIntegracaoMinutaAvon(_cargaAtual, r.Data.IntegracaoAvon, r.Data.Integracoes);
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                }
            });
        }

        _integracaoMinutaAvon.QuantidadeDocumentosGerados.val(retorno.QuantidadeGerada);
    }
}

function ExibirHistoricoConsultaMinutaAvon() {
    BuscarHistoricoConsultaMinutaAvon();
    Global.abrirModal("divModalHistoricoConsultaMinutaAvon");
}

function BuscarHistoricoConsultaMinutaAvon() {
    var download = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoConsultaMinutaAvon, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoConsultaMinutaAvon = new GridView("tblHistoricoConsultaMinutaAvon", "CargaIntegracaoMinutaAvon/ConsultarHistoricoIntegracao", _pesquisaHistoricoConsultaMinutaAvon, menuOpcoes, { column: 2, dir: orderDir.desc });
    _gridHistoricoConsultaMinutaAvon.CarregarGrid();
}

function DownloadArquivosHistoricoConsultaMinutaAvon(historicoConsulta) {
    executarDownload("CargaIntegracaoMinutaAvon/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}