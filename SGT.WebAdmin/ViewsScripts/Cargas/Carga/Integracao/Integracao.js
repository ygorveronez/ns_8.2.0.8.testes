/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../DadosEmissao/Configuracao.js" />
/// <reference path="../DadosEmissao/DadosEmissao.js" />
/// <reference path="../DadosEmissao/Geral.js" />
/// <reference path="../DadosEmissao/Lacre.js" />
/// <reference path="../DadosEmissao/LocaisPrestacao.js" />
/// <reference path="../DadosEmissao/Observacao.js" />
/// <reference path="../DadosEmissao/Passagem.js" />
/// <reference path="../DadosEmissao/Percurso.js" />
/// <reference path="../DadosEmissao/Rota.js" />
/// <reference path="../DadosEmissao/Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="IntegracaoCarga.js" />
/// <reference path="IntegracaoCTe.js" />
/// <reference path="IntegracaoEDI.js" />
/// <reference path="Avon/IntegracaoMinutaAvon.js" />
/// <reference path="Avon/IntegracaoMinutaAvonSignalR.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoAutorizacaoIntegracaoCTe.js" />

var _integracaoGeral;

//*******EVENTOS*******

var IntegracaoGeral = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FilialEmissora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaIntegracao();
        }, type: types.event, text: Localization.Resources.Cargas.Carga.FinalizarEtapa, idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });

    this.VisualizarAprovadores = PropertyEntity({
        eventClick: visualizarAprovadoresClick, type: types.event, text: Localization.Resources.Cargas.Carga.ExibirAprovadores, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
}

function BuscarDadosIntegracoesCarga(e, sender, filialEmissora) {
    ocultarTodasAbas(e);

    if (filialEmissora == null)
        filialEmissora = false;

    executarReST("CargaIntegracao/ObterDadosIntegracoes", { Carga: e.Codigo.val(), FilialEmissora: filialEmissora }, function (r) {
        if (r.Success) {
            _cargaAtual = e;

            if (r.Data.TiposIntegracoesCTe.length > 0 || r.Data.TiposIntegracoesEDI.length > 0 || r.Data.TiposIntegracoesCarga.length > 0) {
                $("#" + e.EtapaIntegracao.idGrid + " .DivIntegracaoCarga").html(_HTMLIntegracaoCarga.replace(/\b#divIntegracao\b/g, e.EtapaIntegracao.idGrid));

                if (_cargaAtual.SituacaoAutorizacaoIntegracaoCTe.val() != EnumSituacaoAutorizacaoIntegracaoCTe.NaoInformado)
                    loadAprovadoresAutorizacaoIntegracaoCTe();

                _integracaoGeral = new IntegracaoGeral();
                _integracaoGeral.Carga.val(e.Codigo.val());

                _integracaoGeral.FilialEmissora.val(filialEmissora);

                KoBindings(_integracaoGeral, "divIntegracao_" + e.EtapaIntegracao.idGrid);

                LocalizeCurrentPage();

                if (r.Data.TiposIntegracoesEDI.length > 0) {
                    LoadIntegracaoEDI(e, "divIntegracaoEDI_" + e.EtapaIntegracao.idGrid);
                } else {
                    $("#" + "divIntegracaoEDI_" + e.EtapaIntegracao.idGrid).hide();
                    $("#" + "liIntegracaoEDI_" + e.EtapaIntegracao.idGrid).hide();
                    $("#" + "liIntegracaoCTe_" + e.EtapaIntegracao.idGrid + " a").tab('show');
                }

                if (r.Data.TiposIntegracoesCTe.length > 0) {
                    LoadIntegracaoCTe(e, "divIntegracaoCTe_" + e.EtapaIntegracao.idGrid);

                    if (r.Data.TiposIntegracoesCTe.length > 1) {
                        $("#" + "divIntegracaoCTe_" + e.EtapaIntegracao.idGrid + " .divBotoesIntegracaoCTe").removeClass("col-md-9 col-lg-10").addClass("col-md-6 col-lg-8");
                        $("#" + _integracaoCTe.Pesquisar.id).removeClass("input-margin-top-24-md");
                        $("#" + _integracaoCTe.ReenviarTodos.id).removeClass("input-margin-top-24-md");
                        _integracaoCTe.Tipo.visible(true);
                    } else {
                        $("#" + "divIntegracaoCTe_" + e.EtapaIntegracao.idGrid + " .divBotoesIntegracaoCTe").removeClass("col-md-6 col-lg-8").addClass("col-md-9 col-lg-10");
                    }
                } else {
                    $("#" + "divIntegracaoCTe_" + e.EtapaIntegracao.idGrid).hide();
                    $("#" + "liIntegracaoCTe_" + e.EtapaIntegracao.idGrid).hide();

                    if (r.Data.TiposIntegracoesEDI.length <= 0)
                        $("#" + "liIntegracaoCarga_" + e.EtapaIntegracao.idGrid + " a").tab('show');
                }

                if (r.Data.TiposIntegracoesCarga.length > 0) {
                    LoadIntegracaoCarga(e, "divIntegracaoCarga_" + e.EtapaIntegracao.idGrid);

                    if (r.Data.TiposIntegracoesCarga.length > 1) {
                        $("#" + "divIntegracaoCarga_" + e.EtapaIntegracao.idGrid + " .divBotoesIntegracaoCarga").removeClass("col-md-9 col-lg-10").addClass("col-md-6 col-lg-8");
                        $("#" + _integracaoCarga.Pesquisar.id).removeClass("input-margin-top-24-md");
                        $("#" + _integracaoCarga.ReenviarTodos.id).removeClass("input-margin-top-24-md");
                        $("#" + _integracaoCarga.DownloadLoteDocumentos.id).removeClass("input-margin-top-24-md");
                        _integracaoCarga.Tipo.visible(true);
                    } else {
                        $("#" + "divIntegracaoCarga_" + e.EtapaIntegracao.idGrid + " .divBotoesIntegracaoCarga").removeClass("col-md-6 col-lg-8").addClass("col-md-9 col-lg-10");
                    }
                } else {
                    $("#" + "divIntegracaoCarga_" + e.EtapaIntegracao.idGrid).hide();
                    $("#" + "liIntegracaoCarga_" + e.EtapaIntegracao.idGrid).hide();
                }

                if (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgIntegracao)
                    _integracaoGeral.FinalizarEtapa.visible(true);

                if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ConfirmarIntegracao, _PermissoesPersonalizadasCarga))
                    _integracaoGeral.FinalizarEtapa.visible(false);

                if (_cargaAtual.SituacaoAutorizacaoIntegracaoCTe.val() == EnumSituacaoAutorizacaoIntegracaoCTe.NaoInformado)
                    _integracaoGeral.VisualizarAprovadores.visible(false);

            }
            else if (_cargaAtual.SituacaoAutorizacaoIntegracaoCTe.val() == EnumSituacaoAutorizacaoIntegracaoCTe.AguardandoAprovacao) {
                _integracaoGeral = new IntegracaoGeral();
                _integracaoGeral.Carga.val(e.Codigo.val());

                loadAprovadoresAutorizacaoIntegracaoCTe();

                htmlAguardandoAprovacao = '<fieldset id="fieldsetAguardandoAprovacao" class="padding-top-0">';
                htmlAguardandoAprovacao += '<div class="row">';
                htmlAguardandoAprovacao += '<section class="col col-xs-12 col-sm-12 col-md-12 col-lg-12">';
                htmlAguardandoAprovacao += '<li data-bind="visible: VisualizarAprovadores.visible, click: VisualizarAprovadores.eventClick, attr: { id: VisualizarAprovadores.id}, enable: VisualizarAprovadores.enable" type="button" class="btn btn-light pull-right" style="padding: 6px 12px; margin: 3px; border-color: #cccccc;">';
                htmlAguardandoAprovacao += '<span data-bind="text: VisualizarAprovadores.text"></span>';
                htmlAguardandoAprovacao += '</li>';
                htmlAguardandoAprovacao += '</section>';
                htmlAguardandoAprovacao += '<section class="col col-xs-12 col-sm-12 col-md-12 col-lg-12">';
                htmlAguardandoAprovacao += '<p class="alert alert-info margin-top-0">' + Localization.Resources.Cargas.Carga.CargaEstaAguardandoAprovacaoDaIntegracao + '</p>';
                htmlAguardandoAprovacao += '</section>';
                htmlAguardandoAprovacao += '</div>';
                htmlAguardandoAprovacao += '</fieldset>';

                $("#" + e.EtapaIntegracao.idGrid + " .DivIntegracaoCarga").html(htmlAguardandoAprovacao);

                KoBindings(_integracaoGeral, "fieldsetAguardandoAprovacao");
            }
            else {
                $("#" + e.EtapaIntegracao.idGrid + " .DivIntegracaoCarga").html('<p class="alert alert-success">' + Localization.Resources.Cargas.Carga.NaoExistemIntegracoesDisponiveisParaEstaCarga + '</p>');
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function FinalizarEtapaIntegracao() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteFinalizarEtapaDeIntegracaoSemConcluirAsIntegracoes, function () {
        executarReST("CargaIntegracao/Finalizar", { Carga: _cargaAtual.Codigo.val() }, function (r) {
            if (r.Data != null) {
                if (r.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.EtapaFinalizadaComSucesso);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function EtapaIntegracaoDesabilitada(e) {
    $("#" + e.EtapaIntegracao.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaIntegracao.idTab + " .step").attr("class", "step");
    e.EtapaIntegracao.eventClick = function (e) { };
}

function EtapaIntegracaoAguardando(e) {
    $("#" + e.EtapaIntegracao.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaIntegracao.idTab + " .step").attr("class", "step yellow");
    e.EtapaIntegracao.eventClick = BuscarDadosIntegracoesCarga;
    AprovarEtapaAnteriorIntegracao(e);
}

function EtapaIntegracaoLiberada(e) {
    $("#" + e.EtapaIntegracao.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaIntegracao.idTab + " .step").attr("class", "step yellow");
    e.EtapaIntegracao.eventClick = BuscarDadosIntegracoesCarga;
    AprovarEtapaAnteriorIntegracao(e);
}

function EtapaIntegracaoAprovada(e) {
    $("#" + e.EtapaIntegracao.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaIntegracao.idTab + " .step").attr("class", "step green");

    e.EtapaIntegracao.eventClick = BuscarDadosIntegracoesCarga;

    AprovarEtapaAnteriorIntegracao(e);

    if (_integracaoGeral != null && _integracaoGeral.Carga.val() == e.Codigo.val())
        _integracaoGeral.FinalizarEtapa.visible(false);
}

function EtapaIntegracaoProblema(e) {
    $("#" + e.EtapaIntegracao.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaIntegracao.idTab + " .step").attr("class", "step red");
    e.EtapaIntegracao.eventClick = BuscarDadosIntegracoesCarga;
    AprovarEtapaAnteriorIntegracao(e);
}

function EtapaIntegracaoEdicaoDesabilitada(e) {
    e.EtapaIntegracao.enable(false);
    EtapaMDFeEdicaoDesabilitada(e);
    EtapaTransbordoEdicaoDesabilitada(e);
}

function AprovarEtapaAnteriorIntegracao(e) {
    if (e.PossuiCTeSubcontratacaoFilialEmissora.val() && e.EmiteMDFeFilialEmissora.val())
        EtapaCTeNFsAprovada(e);
    else
        EtapaMDFeAprovada(e);
}


function EtapaIntegracaoFilialEmissoraDesabilitada(e) {
    $("#" + e.EtapaIntegracaoFilialEmissora.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaIntegracaoFilialEmissora.idTab + " .step").attr("class", "step");
    e.EtapaIntegracaoFilialEmissora.eventClick = function (e) { };
}

function EtapaIntegracaoFilialEmissoraAguardando(e) {
    $("#" + e.EtapaIntegracaoFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaIntegracaoFilialEmissora.idTab + " .step").attr("class", "step yellow");
    e.EtapaIntegracaoFilialEmissora.eventClick = function (e, sender) { BuscarDadosIntegracoesCarga(e, sender, true) };
    AprovarEtapaAnteriorIntegracaoFilialEmissora(e);
}

function EtapaIntegracaoFilialEmissoraLiberada(e) {
    $("#" + e.EtapaIntegracaoFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaIntegracaoFilialEmissora.idTab + " .step").attr("class", "step yellow");
    e.EtapaIntegracaoFilialEmissora.eventClick = function (e, sender) { BuscarDadosIntegracoesCarga(e, sender, true) };
    AprovarEtapaAnteriorIntegracaoFilialEmissora(e);
}

function EtapaIntegracaoFilialEmissoraAprovada(e) {
    $("#" + e.EtapaIntegracaoFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaIntegracaoFilialEmissora.idTab + " .step").attr("class", "step green");

    e.EtapaIntegracaoFilialEmissora.eventClick = function (e, sender) { BuscarDadosIntegracoesCarga(e, sender, true) };

    AprovarEtapaAnteriorIntegracaoFilialEmissora(e);

    //if (_IntegracaoFilialEmissoraGeral != null && _IntegracaoFilialEmissoraGeral.Carga.val() == e.Codigo.val())
    //    _IntegracaoFilialEmissoraGeral.FinalizarEtapa.visible(false);
}

function EtapaIntegracaoFilialEmissoraProblema(e) {
    $("#" + e.EtapaIntegracaoFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaIntegracaoFilialEmissora.idTab + " .step").attr("class", "step red");
    e.EtapaIntegracaoFilialEmissora.eventClick = function (e, sender) { BuscarDadosIntegracoesCarga(e, sender, true) };
    AprovarEtapaAnteriorIntegracaoFilialEmissora(e);
}

function EtapaIntegracaoFilialEmissoraEdicaoDesabilitada(e) {
    e.EtapaIntegracaoFilialEmissora.enable(false);
    //EtapaMDFeEdicaoDesabilitada(e);
    //EtapaTransbordoEdicaoDesabilitada(e);
    EtapaNotaFiscalEdicaoDesabilitada(e);
    EtapaFreteTMSEdicaoDesabilitada(e);

    if (e.EmiteMDFeFilialEmissora.val()) {
        EtapaCTeFilialEmissoraAprovada(e);
    }
    else
        EtapaCTeFilialEmissoraEdicaoDesabilitada(e);
}

function AprovarEtapaAnteriorIntegracaoFilialEmissora(e) {
    if (e.EmiteMDFeFilialEmissora.val())
        EtapaMDFeAprovada(e);
    else
        EtapaCTeFilialEmissoraAprovada(e);
}

function visualizarAprovadoresClick() {
    Global.abrirModal("divModalAprovadoresAutorizacaoIntegracaoCTe");
}