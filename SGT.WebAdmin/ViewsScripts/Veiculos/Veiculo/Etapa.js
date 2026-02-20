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
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Consultas/SegmentoVeiculo.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Equipamento.js" />
/// <reference path="../../Consultas/MarcaVeiculo.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ModeloVeiculo.js" />
/// <reference path="../../Consultas/SegmentoVeiculo.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoPlotagem.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/GrupoServico.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Enumeradores/EnumStatusLicenca.js" />
/// <reference path="../../Enumeradores/EnumTipoFrota.js" />
/// <reference path="../../Enumeradores/EnumPaises.js" />
/// <reference path="../../Enumeradores/EnumTipoCombustivel.js" />
/// <reference path="../../Enumeradores/EnumTipoSistemaElevacao.js" />
/// <reference path="../../Enumeradores/EnumTipoCarreta.js" />
/// <reference path="../../Enumeradores/EnumTipoMaterial.js" />
/// <reference path="Integracoes.js" />
/// <reference path="VeiculosVinculados.js" />
/// <reference path="../VeiculoLicenca/VeiculoLicenca.js" />
/// <reference path="Anexo.js" />
/// <reference path="Currais.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCadastroVeiculo;

var EtapaAprovacaoCadastroVeiculo = function () {
    this.AprovacaoHabilitada = PropertyEntity({ val: ko.observable(false) });
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: Localization.Resources.Veiculos.Veiculo.DescricaoVeiculo, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable(Localization.Resources.Veiculos.Veiculo.OndeSeInformarOsDadosParaOCadastroDeVeiclo),
        tooltipTitle: ko.observable(Localization.Resources.Veiculos.Veiculo.DescricaoVeiculo)
    });
    this.Etapa2 = PropertyEntity({
        text: Localization.Resources.Gerais.Geral.Aprovacao, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable(Localization.Resources.Veiculos.Veiculo.EtapaQueGerenciaAprovacaoDoCadastroDoVeiculo),
        tooltipTitle: ko.observable(Localization.Resources.Gerais.Geral.Aprovacao)
    });
}

//*******EVENTOS*******

function loadAprovacaoCadastroVeiculo() {
    _etapaCadastroVeiculo = new EtapaAprovacaoCadastroVeiculo();
    KoBindings(_etapaCadastroVeiculo, "knockoutEtapaCadastroVeiculo");

    setarEtapasCadastroVeiculo();

    _etapaCadastroVeiculo.AprovacaoHabilitada.val(_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoVeiculo);
}

function setarEtapaInicioCadastroVeiculo() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaCadastroVeiculo.Etapa1.idTab).click();
}

function setarEtapasCadastroVeiculo() {
    if (!_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoVeiculo)
        return;

    var situacaoCadastroVeiculo = _veiculo.SituacaoCadastro.val();

    if (situacaoCadastroVeiculo == EnumSituacaoCadastroVeiculo.Pendente) {
        Etapa2Aguardando();
        BloquearCamposCadastroVeiculo();
    }
    else if (situacaoCadastroVeiculo == EnumSituacaoCadastroVeiculo.Rejeitada) {
        Etapa2Reprovada();
    }
    else if (situacaoCadastroVeiculo == EnumSituacaoCadastroVeiculo.SemRegraAprovacao) {
        Etapa2Reprovada();
    }
    else if (situacaoCadastroVeiculo == EnumSituacaoCadastroVeiculo.Aprovado) {
        Etapa2Aprovada();
    }
    else
        Etapa1Liberada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaCadastroVeiculo.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCadastroVeiculo.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaCadastroVeiculo.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCadastroVeiculo.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Aguardando() {
    $("#" + _etapaCadastroVeiculo.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCadastroVeiculo.Etapa1.idTab + " .step").attr("class", "step yellow");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaCadastroVeiculo.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCadastroVeiculo.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    $("#" + _etapaCadastroVeiculo.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCadastroVeiculo.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaCadastroVeiculo.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCadastroVeiculo.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaCadastroVeiculo.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCadastroVeiculo.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaCadastroVeiculo.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCadastroVeiculo.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}