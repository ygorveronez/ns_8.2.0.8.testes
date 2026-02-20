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
/// <reference path="../../Enumeradores/EnumSituacaoChamado.js" />
/// <reference path="Chamado.js" />
/// <reference path="Analise.js" />
/// <reference path="Ocorrencias.js" />
/// <reference path="Integracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapa;

var Etapas = function () {
    this.Etapa1 = PropertyEntity({
        text: Localization.Resources.Cargas.ControleEntrega.Abertura, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable(Localization.Resources.Cargas.ControleEntrega.EstaEtapaDestinadaAberturaChamado),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.ControleEntrega.Abertura)
    });
    this.Etapa2 = PropertyEntity({
        text: Localization.Resources.Cargas.ControleEntrega.Analise, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable(Localization.Resources.Cargas.ControleEntrega.OndeOcorreAnaliseOperadores),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.ControleEntrega.Analise)
    });
    this.Etapa3 = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Ocorrencia), type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable(Localization.Resources.Cargas.ControleEntrega.QuandoNecessarioCriadoOcorrencia),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.ControleEntrega.Ocorrencia)
    });
    this.Etapa4 = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Integracao), type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable(Localization.Resources.Cargas.ControleEntrega.QuandoConfiguradoSeraGeradoIntegracoes),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.ControleEntrega.Integracao)
    });

    var etapas = 0;
    for (var i in this) if (/(Etapa)[0-9]+/.test(i)) etapas++;
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable((100 / etapas).toFixed(2) + "%") });
};

//*******EVENTOS*******

function loadEtapasChamado() {
    _etapa = new Etapas();
    KoBindings(_etapa, "knockoutEtapaChamado");
    Etapa1LiberadaChamado();
}

function SetarEtapaInicioChamado() {
    DesabilitarTodasEtapasChamado();
    Etapa1LiberadaChamado();
}

function DefinirEtapa3() {
    _analise.ImprimirDevolucao.visible(false);
    $("#knockoutOcorrencia").hide();
    $("#knockoutCargaDevolucao").hide();
    $("#knockoutValePallet").hide();

    if (_motivoChamadoConfiguracao.GerarValePallet)
        Etapa3ComoValePallet();
    else if (_chamado.GerarCargaDevolucao.val())
        Etapa3ComoDevolucao();
    else
        Etapa3ComoOcorrencia();
}

function DefinirEtapa2() {
    $("#knockoutAnalise").hide();
    $("#knockoutAnaliseDevolucao").hide();
    Etapa2Padrao();

    //if (_motivoChamadoConfiguracao.MotivoDevolucao)
    //    //Etapa2ComoDevolucao();
    //    Etapa2Padrao();
    //else 
    //    Etapa2Padrao();
}

function Etapa2Padrao() {
    $("#knockoutAnalise").show();
}

function ObterEventoEtapa3() {
    if (_chamado.GerarCargaDevolucao.val())
        return EtapaCargaChamadoClick;
    else if (_motivoChamadoConfiguracao.GerarValePallet)
        return EtapaValePalletClick;
    else
        return EtapaOcorrenciaChamadoClick;
}

function SetarEtapaChamado() {
    var situacao = _chamado.Situacao.val();
    DefinirEtapa2();
    DefinirEtapa3();
    if (situacao === EnumSituacaoChamado.Todas)
        Etapa1LiberadaChamado();
    else if (situacao === EnumSituacaoChamado.Aberto || situacao === EnumSituacaoChamado.EmTratativa || situacao === EnumSituacaoChamado.AgGeracaoLote)
        Etapa2LiberadaChamado();
    else if (situacao === EnumSituacaoChamado.SemRegra)
        Etapa2AguardandoChamado();
    else if (situacao === EnumSituacaoChamado.Finalizado || situacao === EnumSituacaoChamado.RecusadoPeloCliente) {
        Etapa4AprovadaChamado();
        OcultarBotoesAnalisePorSituacao();
        _pesquisaOcorrenciaNoChamado.Cancelar.visible(false);
        RenderizarEtapaIntegracaoChamado();
    }
    else if (situacao === EnumSituacaoChamado.LiberadaOcorrencia) {
        _pesquisaOcorrenciaNoChamado.AdicionarOcorrenciaAtendimento.visible(true);
        _pesquisaOcorrenciaNoChamado.Fechar.visible(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ChamadoOcorrencia_PermiteFinalizarEmLiberadoParaOcorrencia, _PermissoesPersonalizadasChamado));
        Etapa3AguardandoChamado();
        OcultarBotoesAnalisePorSituacao();
    }
    else if (situacao === EnumSituacaoChamado.LiberadaValePallet) {
        Etapa3AguardandoChamado();
        OcultarBotoesAnalisePorSituacao();
    }
    else if (situacao === EnumSituacaoChamado.Cancelada) {
        Etapa3ReprovadaChamado();
        OcultarBotoesAnalisePorSituacao();
        _pesquisaOcorrenciaNoChamado.AdicionarOcorrenciaAtendimento.visible(false);
        _pesquisaOcorrenciaNoChamado.Fechar.visible(false);
        _pesquisaOcorrenciaNoChamado.Cancelar.visible(false);
    }
    else if (situacao === EnumSituacaoChamado.AgIntegracao) {
        Etapa4AguardandoChamado();
        OcultarBotoesAnalisePorSituacao();
        RenderizarEtapaIntegracaoChamado();
    }
    else if (situacao === EnumSituacaoChamado.FalhaIntegracao) {
        Etapa4ReprovadaChamado();
        OcultarBotoesAnalisePorSituacao();
        RenderizarEtapaIntegracaoChamado();
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaOcorrenciaNoChamado.AdicionarOcorrenciaAtendimento.visible(true);
        _pesquisaOcorrenciaNoChamado.Fechar.visible(false);
        _pesquisaOcorrenciaNoChamado.Cancelar.visible(false);
        OcultarBotoesAnalisePorSituacao();
    }
}

function OcultarBotoesAnalisePorSituacao() {
    _analise.Finalizar.visible(false);
    _analise.Fechar.visible(false);
    _analise.Cancelar.visible(false);
    _analise.Recusar.visible(false);
}

function DesabilitarTodasEtapasChamado() {
    Etapa2DesabilitadaChamado();
    Etapa3DesabilitadaChamado();
    Etapa4DesabilitadaChamado();
}

//*******Etapa 1*******

function Etapa1LiberadaChamado() {
    $("#" + _etapa.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapa.Etapa1.idTab + " .step").attr("class", "step yellow");

    Global.ExibirStep(_etapa.Etapa1.idTab);

    Etapa2DesabilitadaChamado();
}

function Etapa1AprovadaChamado() {
    $("#" + _etapa.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapa.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2DesabilitadaChamado() {
    $("#" + _etapa.Etapa2.idTab).prop("disabled", true);
    $("#" + _etapa.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3DesabilitadaChamado()
}

function Etapa2AprovadaChamado() {
    $("#" + _etapa.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapa.Etapa2.idTab + " .step").attr("class", "step green");

    Etapa1AprovadaChamado();
}

function Etapa2LiberadaChamado() {
    $("#" + _etapa.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapa.Etapa2.idTab + " .step").attr("class", "step yellow");

    Global.ExibirStep(_etapa.Etapa2.idTab);

    Etapa1AprovadaChamado();

    if (_motivoChamadoConfiguracao.PermitirAcessarEtapaOcorrenciaSemFinalizarEtapaAnalise) {
        Etapa3AguardandoChamado();
    }
}

function Etapa2AguardandoChamado() {
    $("#" + _etapa.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapa.Etapa2.idTab + " .step").attr("class", "step yellow");

    Global.ExibirStep(_etapa.Etapa2.idTab);

    Etapa1AprovadaChamado();

    if (_motivoChamadoConfiguracao.PermitirAcessarEtapaOcorrenciaSemFinalizarEtapaAnalise) {
        Etapa3AguardandoChamado();
    }
}

//*******Etapa 3*******

function Etapa3DesabilitadaChamado() {
    $("#" + _etapa.Etapa3.idTab).prop("disabled", true);
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step");

    Etapa4DesabilitadaChamado();
}

function Etapa3AguardandoChamado() {
    $("#" + _etapa.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step yellow");

    _etapa.Etapa3.eventClick = ObterEventoEtapa3();

    ObterEventoEtapa3();

    if (!_motivoChamadoConfiguracao.PermitirAcessarEtapaOcorrenciaSemFinalizarEtapaAnalise) {
        Global.ExibirStep(_etapa.Etapa3.idTab);
        Etapa2AprovadaChamado();
    }
}

function Etapa3AprovadaChamado() {
    $("#" + _etapa.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step green");

    _etapa.Etapa3.eventClick = ObterEventoEtapa3();

    Etapa2AprovadaChamado();
}

function Etapa3ReprovadaChamado() {
    $("#" + _etapa.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step red");

    _etapa.Etapa3.eventClick = ObterEventoEtapa3();

    ObterEventoEtapa3()();

    Global.ExibirStep(_etapa.Etapa3.idTab);

    Etapa2AprovadaChamado();
}

function Etapa3LiberadaChamado() {
    $("#" + _etapa.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapa.Etapa3.idTab + " .step").attr("class", "step blue");

    _etapa.Etapa3.eventClick = ObterEventoEtapa3();

    ObterEventoEtapa3()();

    Global.ExibirStep(_etapa.Etapa3.idTab);

    Etapa2AprovadaChamado();
}

//*******Etapa 4*******

function Etapa4DesabilitadaChamado() {
    _etapa.Etapa4.eventClick = function () { };

    $("#" + _etapa.Etapa4.idTab).prop("disabled", true);
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step");
}

function Etapa4LiberadaChamado() {
    _etapa.Etapa4.eventClick = recarregarIntegracoesChamado;

    $("#" + _etapa.Etapa4.idTab).prop("disabled", false);
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step yellow");

    Etapa3AprovadaChamado();
}

function Etapa4AguardandoChamado() {
    _etapa.Etapa4.eventClick = recarregarIntegracoesChamado;

    $("#" + _etapa.Etapa4.idTab).prop("disabled", false);
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step yellow");

    recarregarIntegracoesChamado();

    Global.ExibirStep(_etapa.Etapa4.idTab);

    Etapa3AprovadaChamado();
}

function Etapa4AprovadaChamado() {
    _etapa.Etapa4.eventClick = recarregarIntegracoesChamado;

    $("#" + _etapa.Etapa4.idTab).prop("disabled", false);
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step green");

    recarregarIntegracoesChamado();

    Global.ExibirStep(_etapa.Etapa4.idTab);

    Etapa3AprovadaChamado();
}

function Etapa4ReprovadaChamado() {
    _etapa.Etapa4.eventClick = recarregarIntegracoesChamado;

    $("#" + _etapa.Etapa4.idTab).prop("disabled", false);
    $("#" + _etapa.Etapa4.idTab + " .step").attr("class", "step red");

    recarregarIntegracoesChamado();

    Global.ExibirStep(_etapa.Etapa4.idTab);

    Etapa3AprovadaChamado();
}