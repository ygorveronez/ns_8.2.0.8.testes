/// <reference path="IntegracaoFatura.js" />
/// <reference path="Fatura.js" />
/// <reference path="CabecalhoFatura.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="CargaFatura.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="FechamentoFatura.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaFatura;
var _etapaAtual;
var _gerandoFatura = false;

var EtapaFatura = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });

    this.Etapa1 = PropertyEntity({
        text: "Fatura", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaFaturaClick,
        step: ko.observable(1),
        tooltip: ko.observable("É onde se informar os dados iniciais para a geração de uma fatura."),
        tooltipTitle: ko.observable("Fatura")
    });
    this.Etapa2 = PropertyEntity({
        text: "Cargas", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: etapaCargaClick,
        step: ko.observable(2),
        tooltip: ko.observable("É onde são selecionados todas as cargas que serão utilizadas em uma fatura."),
        tooltipTitle: ko.observable("Cargas")
    });
    this.Etapa3 = PropertyEntity({
        text: "Fechamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaFechamentoClick,
        step: ko.observable(3),
        tooltip: ko.observable("Esta etapa é referente a finalização e confirmação de valores para a geração dos títulos."),
        tooltipTitle: ko.observable("Fechamento")
    });
    this.Etapa4 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaIntegracaoClick,
        step: ko.observable(5),
        tooltip: ko.observable("A etapa 4 é responsável pela geração de arquivos para integração com outros sistemas."),
        tooltipTitle: ko.observable("Integração")
    });
    this.Etapa5 = PropertyEntity({
        text: "Documentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaDocumentosClick,
        step: ko.observable(2),
        tooltip: ko.observable("Seleção de documentos para a fatura."),
        tooltipTitle: ko.observable("Documentos")
    });
    this.Etapa6 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: etapaAprovacaoClick,
        step: ko.observable(4),
        tooltip: ko.observable("Esta etapa é referente a Aprovação de Alçadas de Regras da Fatura."),
        tooltipTitle: ko.observable("Aprovação")
    });
}

//*******EVENTOS*******

function loadEtapaFatura() {
    _etapaFatura = new EtapaFatura();
    KoBindings(_etapaFatura, "knockoutEtapaFatura");

    $("#" + _etapaFatura.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFatura.Etapa1.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaFatura.Etapa1.idTab).click();
    $("#" + _etapaFatura.Etapa1.idTab).tab("show");
}

function etapaFaturaClick(e, sender) {
    _etapaAtual = 1;
    VerificarBotoes();
}

function etapaCargaClick(e, sender) {
    _etapaAtual = 2;
    atualizarCargasClick(e, sender);
    BuscarResumoCargaFatura();
    VerificarBotoes();
}

function etapaFechamentoClick(e, sender) {
    _etapaAtual = 4;
    CarregarFechamentoFatura();
    VerificarBotoes();
}

function etapaIntegracaoClick(e, sender) {
    _etapaAtual = 5;
    $("#myTabContentIntegracao a:first").tab("show");
    CarregarIntegracaoFatura();
    VerificarBotoes();
}

function etapaDocumentosClick(e, sender) {
    _etapaAtual = 6;
    CarregarDocumentosFatura();
}

function etapaAprovacaoClick(e, sender) {
    _etapaAtual = 3;
    CarregarAprovacaoAprovacaoFatura();
}

function CancelarCargaClick(e, sender) {
    if (_fatura == null || _fatura.Codigo == null || _fatura.Codigo.val() == null || _fatura.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor gere uma fatura antes de cancelar.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }
    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    exibirConfirmacao("Confirmação", "Deseja realmente cancelar esta fatura?", function () {
        Salvar(_fatura, "Fatura/CancelarFatura", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _fatura.Situacao.val(EnumSituacoesFatura.Cancelado);
                    PosicionarEtapa(arg.Data);
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fatura cancelada com sucesso.");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    });
}

function GerarFaturaClick(e, sender) {

    if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura já se encontra finalizada.");
        return;
    }

    if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta fatura se encontra cancelada.");
        return;
    }

    if (!_gerandoFatura) {
        _gerandoFatura = true;

        _fatura.Etapa.val(EnumEtapasFatura.Documentos);

        Salvar(_fatura, "Fatura/GerarFatura", function (arg) {
            _gerandoFatura = false;
            if (arg.Success) {
                if (arg.Data) {

                    if (arg.Data.GerarDocumentosAutomaticamente === true) {
                        if (_fatura.NovoModelo.val()) {
                            $("#knockoutFaturaDocumento").hide();
                            $("#knockoutPercentualFaturaDocumento").show();
                            SetarPercentualProcessamentoFaturaNovo(0);
                        } else {
                            SetarPercentualProcessamento(0);
                            $("#fdsCargasDaFatura").hide();
                            $("#fdsPercentualCargasDocumento").show();
                            BuscarProcessamentosPendentes();
                        }
                    }

                    CarregarDadosCabecalho(arg.Data);
                    PosicionarEtapa(arg.Data);

                    _contatoClienteFatura.ShowButton();

                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        }, sender, function () {
            _gerandoFatura = false;
            exibirCamposObrigatorio();
        }, function (x) {
            _gerandoFatura = false;
            exibirMensagem(tipoMensagem.falha, "Falha", "Não foi possível realizar uma requisição para o servidor. Erro: " + x.status + " - " + x.statusText);
        });
    }
}

//*******MÉTODOS*******

function LimparOcultarAbas() {
    $("#" + _etapaFatura.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFatura.Etapa1.idTab + " .step").attr("class", "step");

    $("#" + _etapaFatura.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFatura.Etapa2.idTab).attr("disabled", true);
    $("#" + _etapaFatura.Etapa2.idTab + " .step").attr("class", "step");

    $("#" + _etapaFatura.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFatura.Etapa3.idTab).attr("disabled", true);
    $("#" + _etapaFatura.Etapa3.idTab + " .step").attr("class", "step");

    $("#" + _etapaFatura.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFatura.Etapa4.idTab).attr("disabled", true);
    $("#" + _etapaFatura.Etapa4.idTab + " .step").attr("class", "step");

    $("#" + _etapaFatura.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFatura.Etapa5.idTab).attr("disabled", true);
    $("#" + _etapaFatura.Etapa5.idTab + " .step").attr("class", "step");

    $("#" + _etapaFatura.Etapa6.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFatura.Etapa6.idTab).attr("disabled", true);
    $("#" + _etapaFatura.Etapa6.idTab + " .step").attr("class", "step");
}

function PosicionarEtapa(dado) {
    if (dado.NovoModelo) {
        _etapaFatura.Etapa2.visible(false);
        _etapaFatura.Etapa5.visible(true);
    } else {
        _etapaFatura.Etapa2.visible(true);
        _etapaFatura.Etapa5.visible(false);
    }

    if (dado.Codigo > 0)
        _fatura.ImportarPreFatura.visible(false);

    LimparOcultarAbas();

    if ((dado.Situacao == EnumSituacoesFatura.EmAndamento || dado.Situacao == EnumSituacoesFatura.SemRegraAprovacao || dado.Situacao == EnumSituacoesFatura.AguardandoAprovacao || dado.Situacao == EnumSituacoesFatura.AprovacaoRejeitada) && !_FormularioSomenteLeitura) {
        $("#" + _etapaFatura.Etapa1.idTab + " .step").attr("class", "step lightgreen");

        if (dado.Etapa == EnumEtapasFatura.Fatura || dado.Etapa == EnumEtapasFatura.LancandoCargas) {
            $("#" + _etapaFatura.Etapa1.idTab + " .step").attr("class", "step green");

            if (_fatura.NovoModelo.val()) {
                $("#" + _etapaFatura.Etapa5.idTab).click();
                $("#" + _etapaFatura.Etapa5.idTab).tab("show");
            } else {
                $("#" + _etapaFatura.Etapa2.idTab).click();
                $("#" + _etapaFatura.Etapa2.idTab).tab("show");
            }
        }
        else if (dado.Etapa == EnumEtapasFatura.Cargas) {
            $("#" + _etapaFatura.Etapa1.idTab + " .step").attr("class", "step green");
            $("#" + _etapaFatura.Etapa2.idTab + " .step").attr("class", "step green");
            $("#" + _etapaFatura.Etapa2.idTab).attr("disabled", false);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _aprovacaoFatura.PossuiRegrasBooleano.val()) {
                $("#" + _etapaFatura.Etapa6.idTab + " .step").attr("class", "step lightgreen");
                $("#" + _etapaFatura.Etapa6.idTab).attr("disabled", false);
                $("#" + _etapaFatura.Etapa6.idTab).click();
                $("#" + _etapaFatura.Etapa6.idTab).tab("show");
                _etapaFatura.Etapa6.visible(true);
            } else
                _etapaFatura.Etapa6.visible(false);
        }
        else if (dado.Etapa == EnumEtapasFatura.Aprovacao) {
            $("#" + _etapaFatura.Etapa1.idTab + " .step").attr("class", "step green");

            $("#" + _etapaFatura.Etapa2.idTab + " .step").attr("class", "step green");
            $("#" + _etapaFatura.Etapa2.idTab).attr("disabled", false);

            $("#" + _etapaFatura.Etapa4.idTab + " .step").attr("class", "step");

            $("#" + _etapaFatura.Etapa5.idTab + " .step").attr("class", "step green");
            $("#" + _etapaFatura.Etapa5.idTab).attr("disabled", false);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _aprovacaoFatura.PossuiRegrasBooleano.val()) {
                $("#" + _etapaFatura.Etapa6.idTab + " .step").attr("class", "step green");
                $("#" + _etapaFatura.Etapa6.idTab).attr("disabled", false);

                $("#" + _etapaFatura.Etapa3.idTab + " .step").attr("class", "step green");
                $("#" + _etapaFatura.Etapa3.idTab).attr("disabled", false);
                $("#" + _etapaFatura.Etapa3.idTab).click();
                $("#" + _etapaFatura.Etapa3.idTab).tab("show");

                $("#" + _etapaFatura.Etapa4.idTab + " .step").attr("class", "step yellow");
                $("#" + _etapaFatura.Etapa4.idTab).attr("disabled", false);
                $("#" + _etapaFatura.Etapa4.idTab).click();
                $("#" + _etapaFatura.Etapa4.idTab).tab("show");

                $("#" + _etapaFatura.Etapa6.idTab).click();
                $("#" + _etapaFatura.Etapa6.idTab).tab("show");
                _etapaFatura.Etapa6.visible(true);
            } else
                _etapaFatura.Etapa6.visible(false);
        }
        else if (dado.Etapa == EnumEtapasFatura.Fechamento) {
            $("#" + _etapaFatura.Etapa1.idTab + " .step").attr("class", "step green");

            $("#" + _etapaFatura.Etapa2.idTab + " .step").attr("class", "step green");
            $("#" + _etapaFatura.Etapa2.idTab).attr("disabled", false);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _aprovacaoFatura.PossuiRegrasBooleano.val()) {
                $("#" + _etapaFatura.Etapa6.idTab + " .step").attr("class", "step");
                $("#" + _etapaFatura.Etapa6.idTab).attr("disabled", true);
                _etapaFatura.Etapa6.visible(true);
            } else
                _etapaFatura.Etapa6.visible(false);

            $("#" + _etapaFatura.Etapa3.idTab + " .step").attr("class", "step lightgreen");
            $("#" + _etapaFatura.Etapa3.idTab).attr("disabled", false);

            $("#" + _etapaFatura.Etapa5.idTab + " .step").attr("class", "step green");
            $("#" + _etapaFatura.Etapa5.idTab).attr("disabled", false);

            $("#" + _etapaFatura.Etapa3.idTab).click();
            $("#" + _etapaFatura.Etapa3.idTab).tab("show");

        }
        else if (dado.Etapa == EnumEtapasFatura.Documentos) {
            $("#" + _etapaFatura.Etapa1.idTab + " .step").attr("class", "step green");
            if (_fatura.NovoModelo.val()) {
                $("#" + _etapaFatura.Etapa5.idTab + " .step").attr("class", "step lightgreen");
                $("#" + _etapaFatura.Etapa5.idTab).attr("disabled", false);
                $("#" + _etapaFatura.Etapa5.idTab).click();
                $("#" + _etapaFatura.Etapa5.idTab).tab("show");
            } else {
                $("#" + _etapaFatura.Etapa2.idTab + " .step").attr("class", "step lightgreen");
                $("#" + _etapaFatura.Etapa2.idTab).attr("disabled", false);
                $("#" + _etapaFatura.Etapa2.idTab).click();
                $("#" + _etapaFatura.Etapa2.idTab).tab("show");
            }
        }

        VerificarBotoes();
    } else if (dado.Situacao == EnumSituacoesFatura.Cancelado) {
        $("#" + _etapaFatura.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaFatura.Etapa2.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFatura.Etapa2.idTab).attr("disabled", false);

        $("#" + _etapaFatura.Etapa3.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFatura.Etapa3.idTab).attr("disabled", false);

        $("#" + _etapaFatura.Etapa4.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFatura.Etapa4.idTab).attr("disabled", false);

        $("#" + _etapaFatura.Etapa5.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFatura.Etapa5.idTab).attr("disabled", false);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _aprovacaoFatura.PossuiRegrasBooleano.val()) {
            $("#" + _etapaFatura.Etapa6.idTab + " .step").attr("class", "step green");
            $("#" + _etapaFatura.Etapa6.idTab).attr("disabled", false);
            _etapaFatura.Etapa6.visible(true);
        } else
            _etapaFatura.Etapa6.visible(false);

        $("#" + _etapaFatura.Etapa1.idTab).click();
        $("#" + _etapaFatura.Etapa1.idTab).tab("show");

        VerificarBotoes();
    } else if (dado.Situacao == EnumSituacoesFatura.EmFechamento) {
        $("#" + _etapaFatura.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaFatura.Etapa2.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFatura.Etapa2.idTab).attr("disabled", false);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _aprovacaoFatura.PossuiRegrasBooleano.val()) {
            $("#" + _etapaFatura.Etapa6.idTab + " .step").attr("class", "step green");
            $("#" + _etapaFatura.Etapa6.idTab).attr("disabled", false);
            _etapaFatura.Etapa6.visible(true);
        } else
            _etapaFatura.Etapa6.visible(false);

        $("#" + _etapaFatura.Etapa3.idTab + " .step").attr("class", "step yellow");
        $("#" + _etapaFatura.Etapa3.idTab).attr("disabled", false);

        $("#" + _etapaFatura.Etapa4.idTab + " .step").attr("class", "step");

        $("#" + _etapaFatura.Etapa5.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFatura.Etapa5.idTab).attr("disabled", false);

        _fechamentoFatura.PercentualProcessadoFechamento.visible(true);

        $("#" + _etapaFatura.Etapa3.idTab).click();
        $("#" + _etapaFatura.Etapa3.idTab).tab("show");
    } else if (dado.Situacao == EnumSituacoesFatura.EmCancelamento) {

        $("#" + _etapaFatura.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaFatura.Etapa2.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFatura.Etapa2.idTab).attr("disabled", false);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _aprovacaoFatura.PossuiRegrasBooleano.val()) {
            $("#" + _etapaFatura.Etapa6.idTab + " .step").attr("class", "step green");
            $("#" + _etapaFatura.Etapa6.idTab).attr("disabled", false);
            _etapaFatura.Etapa6.visible(true);
        } else
            _etapaFatura.Etapa6.visible(false);

        $("#" + _etapaFatura.Etapa3.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFatura.Etapa3.idTab).attr("disabled", false);

        $("#" + _etapaFatura.Etapa4.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFatura.Etapa4.idTab).attr("disabled", false);

        $("#" + _etapaFatura.Etapa5.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFatura.Etapa5.idTab).attr("disabled", false);

        _cabecalhoFatura.PercentualProcessadoCancelamento.visible(true);

        $("#" + _etapaFatura.Etapa1.idTab).click();
        $("#" + _etapaFatura.Etapa1.idTab).tab("show");

    } else {
        $("#" + _etapaFatura.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaFatura.Etapa2.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFatura.Etapa2.idTab).attr("disabled", false);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _aprovacaoFatura.PossuiRegrasBooleano.val()) {
            $("#" + _etapaFatura.Etapa6.idTab + " .step").attr("class", "step green");
            $("#" + _etapaFatura.Etapa6.idTab).attr("disabled", false);
            _etapaFatura.Etapa6.visible(true);
        } else
            _etapaFatura.Etapa6.visible(false);

        $("#" + _etapaFatura.Etapa3.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFatura.Etapa3.idTab).attr("disabled", false);

        if (dado.Situacao == EnumSituacoesFatura.ProblemaIntegracao) {
            $("#" + _etapaFatura.Etapa4.idTab + " .step").attr("class", "step red");
        }
        else {
            $("#" + _etapaFatura.Etapa4.idTab + " .step").attr("class", "step green");
        }

        $("#" + _etapaFatura.Etapa4.idTab).attr("disabled", false);

        $("#" + _etapaFatura.Etapa5.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFatura.Etapa5.idTab).attr("disabled", false);

        $("#" + _etapaFatura.Etapa3.idTab).click();
        $("#" + _etapaFatura.Etapa3.idTab).tab("show");

        VerificarBotoes();
    }
}

function VerificarBotoes() {
    if ((_fatura.Situacao.val() == EnumSituacoesFatura.SemRegraAprovacao || _fatura.Situacao.val() == EnumSituacoesFatura.AguardandoAprovacao || _fatura.Situacao.val() == EnumSituacoesFatura.AprovacaoRejeitada) && !_FormularioSomenteLeitura && _fatura.Codigo.val() > 0) {
        HabilitarTodosBotoes(false);
        if (_fechamentoFatura != null) {
            _fechamentoFatura.ReAbrirFatura.visible(false);
            _fechamentoFatura.FecharFatura.visible(false);
            _fechamentoFatura.FecharFatura.enable(false);
            _fechamentoFatura.LiquidarFatura.visible(false);
            _fechamentoFatura.SalvarObservacao.visible(false);

            if (_fatura.NovoModelo.val()) {
                _fechamentoFatura.VisualizarFatura.visible(true);
                _fechamentoFatura.SalvarValores.visible(true);
            } else {
                _fechamentoFatura.VisualizarFatura.visible(false);
            }

        }
    } else if (_fatura.Situacao.val() == EnumSituacoesFatura.EmAndamento && !_FormularioSomenteLeitura && _fatura.Codigo.val() > 0) {
        HabilitarTodosBotoes(true);
        if (_fechamentoFatura != null) {
            _fechamentoFatura.ReAbrirFatura.visible(false);
            _fechamentoFatura.FecharFatura.visible(true);
            _fechamentoFatura.FecharFatura.enable(true);
            _fechamentoFatura.LiquidarFatura.visible(false);
            _fechamentoFatura.SalvarObservacao.visible(false);

            if (_fatura.NovoModelo.val()) {
                _fechamentoFatura.VisualizarFatura.visible(true);
                _fechamentoFatura.SalvarValores.visible(true);
            } else {
                _fechamentoFatura.VisualizarFatura.visible(false);
            }

        }
    } else if (_fatura.Situacao.val() == EnumSituacoesFatura.Cancelado && !_FormularioSomenteLeitura && _fatura.Codigo.val() > 0) {
        HabilitarTodosBotoes(false);
        if (_fechamentoFatura != null) {
            _fechamentoFatura.ReAbrirFatura.visible(false);
            _fechamentoFatura.ReAbrirFatura.enable(false);
            _fechamentoFatura.FecharFatura.enable(false);
            _fechamentoFatura.LiquidarFatura.visible(false);
            _fechamentoFatura.VisualizarFatura.visible(false);
            _fechamentoFatura.SalvarObservacao.visible(false);
        }
    } else if (_fatura.Situacao.val() == EnumSituacoesFatura.Fechado && !_FormularioSomenteLeitura && _fatura.Codigo.val() > 0) {
        HabilitarTodosBotoes(false);
        if (_fechamentoFatura != null) {
            _fechamentoFatura.ReAbrirFatura.visible(true);
            _fechamentoFatura.ReAbrirFatura.enable(true);
            _fechamentoFatura.FecharFatura.visible(false);
            _fechamentoFatura.LiquidarFatura.visible(true);
            _fechamentoFatura.VisualizarFatura.visible(true);
            _fechamentoFatura.SalvarObservacao.visible(true);
            _fechamentoFatura.ObservacaoFatura.enable(true);
            _fechamentoFatura.ImprimirObservacaoFatura.enable(true);
        }
    } else if (_fatura.Situacao.val() == EnumSituacoesFatura.Liquidado && !_FormularioSomenteLeitura && _fatura.Codigo.val() > 0) {
        HabilitarTodosBotoes(false);
        if (_fechamentoFatura != null) {
            _fechamentoFatura.ReAbrirFatura.visible(true);
            _fechamentoFatura.ReAbrirFatura.enable(true);
            _fechamentoFatura.FecharFatura.visible(false);
            _fechamentoFatura.LiquidarFatura.visible(false);
            _fechamentoFatura.VisualizarFatura.visible(true);
            _fechamentoFatura.SalvarObservacao.visible(true);
            _fechamentoFatura.ObservacaoFatura.enable(true);
            _fechamentoFatura.ImprimirObservacaoFatura.enable(true);
        }
    } else if (_fatura.Situacao.val() == EnumSituacoesFatura.ProblemaIntegracao && !_FormularioSomenteLeitura && _fatura.Codigo.val() > 0) {
        HabilitarTodosBotoes(false);
        if (_fechamentoFatura != null) {
            _fechamentoFatura.ReAbrirFatura.visible(true);
            _fechamentoFatura.ReAbrirFatura.enable(true);
            _fechamentoFatura.FecharFatura.visible(true);
            _fechamentoFatura.LiquidarFatura.visible(false);
            _fechamentoFatura.VisualizarFatura.visible(true);
            _fechamentoFatura.SalvarObservacao.visible(false);
            _fechamentoFatura.ObservacaoFatura.enable(false);
            _fechamentoFatura.ImprimirObservacaoFatura.enable(false);
        }
    } else {
        HabilitarTodosBotoes(false);
        if (_fechamentoFatura != null) {
            _fechamentoFatura.ReAbrirFatura.visible(false);
            _fechamentoFatura.ReAbrirFatura.enable(false);
            _fechamentoFatura.FecharFatura.enable(false);
            _fechamentoFatura.LiquidarFatura.visible(false);
            _fechamentoFatura.VisualizarFatura.visible(false);
            _fechamentoFatura.SalvarObservacao.visible(false);
        }
    }
}

function HabilitarTodosBotoes(v) {
    if (_FormularioSomenteLeitura)
        v = false;

    if (_fatura.Codigo.val() <= 0 && v == false && !_FormularioSomenteLeitura) {
        _fatura.GerarDocumentosAutomaticamente.enable(true);
        _fatura.NaoUtilizarMoedaEstrangeira.enable(true);
        _fatura.TipoPessoa.enable(true);
        _fatura.TipoOperacao.enable(true);
        _fatura.Pessoa.enable(true);
        _fatura.GrupoPessoa.enable(true);
        _fatura.Transportador.enable(true);
        _fatura.TipoCarga.enable(true);
        _fatura.AliquotaICMS.enable(true);
        _fatura.Veiculo.enable(true);
        _fatura.DataInicial.enable(true);
        _fatura.DataFatura.enable(true);
        _fatura.NumeroPreFatura.enable(true);
        _fatura.NumeroFaturaOriginal.enable(true);
        _fatura.DataFinal.enable(true);
        _fatura.Observacao.enable(true);
        _fatura.GerarGatura.enable(true);
        _fatura.CancelarFatura.enable(true);

        _fatura.PedidoViagemNavio.enable(true);
        _fatura.TerminalOrigem.enable(true);
        _fatura.TerminalDestino.enable(true);
        _fatura.Origem.enable(true);
        _fatura.Destino.enable(true);
        _fatura.NumeroBooking.enable(true);
        _fatura.TipoPropostaMultimodal.enable(true);
        _fatura.PaisOrigem.enable(true);
        _fatura.Filial.enable(true);
        _fatura.Tomador.enable(true);

        v = false;
        if (_resumoCarga != null) {
            _resumoCarga.BuscarCargas.enable(v);
            _resumoCarga.ImportarPreFatura.enable(v);
            _resumoCarga.BaixarXMLDocumentosFatura.enable(v);
            _resumoCarga.Arquivo.enable(v);
            _resumoCarga.NumeroDocumento.enable(v);
            _resumoCarga.AdicionarDocumento.enable(v);
        }
        if (_carga != null) {
            _carga.NaoFaturarCarga.enable(v);
        }
        if (_pesquisaDocumento != null) {
            _pesquisaDocumento.NaoFaturarCarga.enable(v);
        }
        if (_fechamentoFatura != null) {
            _fechamentoFatura.AdicionarAcrescimoDesconto.enable(v);
            _fechamentoFatura.AcrescimosDescontos.enable(v);
            _fechamentoFatura.ObservacaoFatura.enable(v);
            _fechamentoFatura.ImprimirObservacaoFatura.enable(v);
            _fechamentoFatura.SalvarValores.enable(v);
            _fechamentoFatura.QuantidadeParcelas.enable(v);
            _fechamentoFatura.IntervaloDeDias.enable(v);
            _fechamentoFatura.DataPrimeiroVencimento.enable(v);
            _fechamentoFatura.TipoArredondamento.enable(v);
            _fechamentoFatura.FormaTitulo.enable(v);
            _fechamentoFatura.GerarParcelas.enable(v);

            _fechamentoFatura.TomadorFatura.enable(v);
            _fechamentoFatura.EmpresaFatura.enable(v);
            _fechamentoFatura.Banco.enable(v);
            _fechamentoFatura.Agencia.enable(v);
            _fechamentoFatura.Digito.enable(v);
            _fechamentoFatura.NumeroConta.enable(v);
            _fechamentoFatura.TipoConta.enable(v);
        }
        if (_detalheParcela != null) {
            _detalheParcela.Valor.enable(v);
            _detalheParcela.ValorDesconto.enable(v);
            _detalheParcela.DataVencimento.enable(v);
            _detalheParcela.FormaTitulo.enable(v);
            _detalheParcela.SalvarParcela.enable(v);
        }

        if (_knotSalvarCarga != null) {
            _knotSalvarCarga.SalvarCargas.enable(v);
        }
    } else {
        _fatura.GerarDocumentosAutomaticamente.enable(v);
        _fatura.NaoUtilizarMoedaEstrangeira.enable(false); //não deixar atualizar essa informação
        _fatura.TipoPessoa.enable(v);
        _fatura.Pessoa.enable(v);
        _fatura.TipoOperacao.enable(v);
        _fatura.GrupoPessoa.enable(v);
        _fatura.Transportador.enable(v);
        _fatura.TipoCarga.enable(v);
        _fatura.AliquotaICMS.enable(v);
        _fatura.Veiculo.enable(v);
        _fatura.DataInicial.enable(v);
        _fatura.DataFatura.enable(v);
        _fatura.NumeroPreFatura.enable(v);
        _fatura.NumeroFaturaOriginal.enable(v);
        _fatura.DataFinal.enable(v);
        _fatura.Observacao.enable(v);
        _fatura.GerarGatura.enable(v);
        _fatura.CancelarFatura.enable(v);
        _fatura.PedidoViagemNavio.enable(v);
        _fatura.TerminalOrigem.enable(v);
        _fatura.TerminalDestino.enable(v);
        _fatura.Origem.enable(v);
        _fatura.PaisOrigem.enable(v);
        _fatura.Filial.enable(v);
        _fatura.Tomador.enable(v);
        _fatura.Destino.enable(v);
        _fatura.NumeroBooking.enable(v);
        _fatura.TipoPropostaMultimodal.enable(v);

        if (_resumoCarga != null) {
            _resumoCarga.BuscarCargas.enable(v);
            _resumoCarga.ImportarPreFatura.enable(v);
            _resumoCarga.BaixarXMLDocumentosFatura.enable(v);
            _resumoCarga.Arquivo.enable(v);
            _resumoCarga.NumeroDocumento.enable(v);
            _resumoCarga.AdicionarDocumento.enable(v);
        }
        if (_carga != null) {
            _carga.NaoFaturarCarga.enable(v);
        }
        if (_pesquisaDocumento != null) {
            _pesquisaDocumento.NaoFaturarCarga.enable(v);
        }
        if (_fechamentoFatura != null) {
            _fechamentoFatura.AdicionarAcrescimoDesconto.enable(v);
            _fechamentoFatura.AcrescimosDescontos.enable(v);
            _fechamentoFatura.ObservacaoFatura.enable(v);
            _fechamentoFatura.ImprimirObservacaoFatura.enable(v);

            _fechamentoFatura.SalvarValores.enable(v);
            _fechamentoFatura.QuantidadeParcelas.enable(v);
            _fechamentoFatura.IntervaloDeDias.enable(v);
            _fechamentoFatura.DataPrimeiroVencimento.enable(v);
            _fechamentoFatura.TipoArredondamento.enable(v);
            _fechamentoFatura.FormaTitulo.enable(v);
            _fechamentoFatura.GerarParcelas.enable(v);

            _fechamentoFatura.TomadorFatura.enable(v);
            _fechamentoFatura.EmpresaFatura.enable(v);
            _fechamentoFatura.Banco.enable(v);
            _fechamentoFatura.Agencia.enable(v);
            _fechamentoFatura.Digito.enable(v);
            _fechamentoFatura.NumeroConta.enable(v);
            _fechamentoFatura.TipoConta.enable(v);
        }
        if (_detalheParcela != null) {
            _detalheParcela.Valor.enable(v);
            _detalheParcela.ValorDesconto.enable(v);
            _detalheParcela.DataVencimento.enable(v);
            _detalheParcela.FormaTitulo.enable(v);
            _detalheParcela.SalvarParcela.enable(v);
        }

        if (_knotSalvarCarga != null) {
            _knotSalvarCarga.SalvarCargas.enable(v);
        }
    }
}

function LimparEtapaFatura() {
    LimparOcultarAbas();
    VerificarBotoes();
    _fechamentoFatura.ReAbrirFatura.visible(false);
    _fechamentoFatura.ReAbrirFatura.enable(true);
    _fechamentoFatura.FecharFatura.enable(false);
    _fechamentoFatura.LiquidarFatura.visible(false);
    _fechamentoFatura.VisualizarFatura.visible(false);
    _fechamentoFatura.SalvarObservacao.visible(false);
    $("#" + _etapaFatura.Etapa1.idTab).click();
    $("#" + _etapaFatura.Etapa1.idTab).tab("show");
}