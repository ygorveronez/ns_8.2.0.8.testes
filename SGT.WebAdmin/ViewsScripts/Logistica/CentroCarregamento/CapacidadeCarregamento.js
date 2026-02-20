/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="../../Enumeradores/EnumTipoCapacidadeCarregamentoPorPeso.js" />
/// <reference path="CapacidadeCarregamentoDados.js" />
/// <reference path="CentroCarregamento.js" />
/// <reference path="DisponibilidadeFrota.js" />
/// <reference path="LimiteCarregamento.js" />
/// <reference path="PeriodoCarregamento.js" />
/// <reference path="PrevisaoCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _capacidadeCarregamento;
var _listaKnockoutCapacidadeCarregamentoDados = new Array();
var _listaKnockoutDisponibilidadeFrota = new Array();
var _listaKnockoutLimiteCarregamento = new Array();
var _listaKnockoutPeriodosCarregamento = new Array();
var _listaKnockoutPrevisoesCarregamento = new Array();

/*
 * Declaração das Classes
 */

var CapacidadeCarregamento = function () {
    this.NumeroDocas = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.NumeroDeDocas.getRequiredFieldDescription(), required: true, visible: ko.observable(true) });
    this.TipoCapacidadeCarregamento = PropertyEntity({ text: ko.observable(Localization.Resources.Logistica.CentroCarregamento.TipoCapacidadeCarregamento), val: ko.observable(EnumTipoCapacidadeCarregamento.Peso), def: EnumTipoCapacidadeCarregamento.Peso, options: EnumTipoCapacidadeCarregamento.obterOpcoes(), enable: ko.observable(true), visible: ko.observable(false) });

    this.NumeroDocas.val.subscribe(function (novoValor) {
        _centroCarregamento.NumeroDocas.val(novoValor);
    });

    this.TipoCapacidadeCarregamento.val.subscribe(controlarVisibilidadeCamposCapacidadeCarregamentoCubagem);
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadCapacidadeCarregamento() {
    _capacidadeCarregamento = new CapacidadeCarregamento();
    KoBindings(_capacidadeCarregamento, "knockoutCapacidadeCarregamento");

    LoadCapacidadeCarregamentoDados();
    loadPeriodosCarregamento();
    loadPrevisoesCarregamento();
    loadDisponibilidadeFrota();
    loadLimiteCarregamento();
}

function LoadCapacidadeCarregamentoDados() {
    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Segunda] = new CapacidadeCarregamentoDados(EnumDiaSemana.Segunda, "knockoutCapacidadeCarregamentoDados_Segunda");
    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Terca] = new CapacidadeCarregamentoDados(EnumDiaSemana.Terca, "knockoutCapacidadeCarregamentoDados_Terca");
    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Quarta] = new CapacidadeCarregamentoDados(EnumDiaSemana.Quarta, "knockoutCapacidadeCarregamentoDados_Quarta");
    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Quinta] = new CapacidadeCarregamentoDados(EnumDiaSemana.Quinta, "knockoutCapacidadeCarregamentoDados_Quinta");
    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Sexta] = new CapacidadeCarregamentoDados(EnumDiaSemana.Sexta, "knockoutCapacidadeCarregamentoDados_Sexta");
    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Sabado] = new CapacidadeCarregamentoDados(EnumDiaSemana.Sabado, "knockoutCapacidadeCarregamentoDados_Sabado");
    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Domingo] = new CapacidadeCarregamentoDados(EnumDiaSemana.Domingo, "knockoutCapacidadeCarregamentoDados_Domingo");

    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Segunda].Load();
    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Terca].Load();
    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Quarta].Load();
    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Quinta].Load();
    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Sexta].Load();
    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Sabado].Load();
    _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Domingo].Load();

    _centroCarregamento.ToleranciaAtrasoSegunda = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Segunda].ToleranciaAtraso;
    _centroCarregamento.ToleranciaAtrasoTerca = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Terca].ToleranciaAtraso;
    _centroCarregamento.ToleranciaAtrasoQuarta = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Quarta].ToleranciaAtraso;
    _centroCarregamento.ToleranciaAtrasoQuinta = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Quinta].ToleranciaAtraso;
    _centroCarregamento.ToleranciaAtrasoSexta = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Sexta].ToleranciaAtraso;
    _centroCarregamento.ToleranciaAtrasoSabado = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Sabado].ToleranciaAtraso;
    _centroCarregamento.ToleranciaAtrasoDomingo = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Domingo].ToleranciaAtraso;

    _centroCarregamento.CapacidadeCarregamentoSegunda = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Segunda].CapacidadeCarregamentoVolume;
    _centroCarregamento.CapacidadeCarregamentoTerca = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Terca].CapacidadeCarregamentoVolume;
    _centroCarregamento.CapacidadeCarregamentoQuarta = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Quarta].CapacidadeCarregamentoVolume;
    _centroCarregamento.CapacidadeCarregamentoQuinta = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Quinta].CapacidadeCarregamentoVolume;
    _centroCarregamento.CapacidadeCarregamentoSexta = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Sexta].CapacidadeCarregamentoVolume;
    _centroCarregamento.CapacidadeCarregamentoSabado = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Sabado].CapacidadeCarregamentoVolume;
    _centroCarregamento.CapacidadeCarregamentoDomingo = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Domingo].CapacidadeCarregamentoVolume;

    _centroCarregamento.CapacidadeCarregamentoCubagemSegunda = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Segunda].CapacidadeCarregamentoCubagem;
    _centroCarregamento.CapacidadeCarregamentoCubagemTerca = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Terca].CapacidadeCarregamentoCubagem;
    _centroCarregamento.CapacidadeCarregamentoCubagemQuarta = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Quarta].CapacidadeCarregamentoCubagem;
    _centroCarregamento.CapacidadeCarregamentoCubagemQuinta = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Quinta].CapacidadeCarregamentoCubagem;
    _centroCarregamento.CapacidadeCarregamentoCubagemSexta = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Sexta].CapacidadeCarregamentoCubagem;
    _centroCarregamento.CapacidadeCarregamentoCubagemSabado = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Sabado].CapacidadeCarregamentoCubagem;
    _centroCarregamento.CapacidadeCarregamentoCubagemDomingo = _listaKnockoutCapacidadeCarregamentoDados[EnumDiaSemana.Domingo].CapacidadeCarregamentoCubagem;
}

function loadDisponibilidadeFrota() {
    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Segunda] = new DisponibilidadeFrota(EnumDiaSemana.Segunda, "knockoutDisponibilidadeFrota_Segunda", _centroCarregamento.DisponibilidadesFrota);
    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Terca] = new DisponibilidadeFrota(EnumDiaSemana.Terca, "knockoutDisponibilidadeFrota_Terca", _centroCarregamento.DisponibilidadesFrota);
    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Quarta] = new DisponibilidadeFrota(EnumDiaSemana.Quarta, "knockoutDisponibilidadeFrota_Quarta", _centroCarregamento.DisponibilidadesFrota);
    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Quinta] = new DisponibilidadeFrota(EnumDiaSemana.Quinta, "knockoutDisponibilidadeFrota_Quinta", _centroCarregamento.DisponibilidadesFrota);
    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Sexta] = new DisponibilidadeFrota(EnumDiaSemana.Sexta, "knockoutDisponibilidadeFrota_Sexta", _centroCarregamento.DisponibilidadesFrota);
    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Sabado] = new DisponibilidadeFrota(EnumDiaSemana.Sabado, "knockoutDisponibilidadeFrota_Sabado", _centroCarregamento.DisponibilidadesFrota);
    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Domingo] = new DisponibilidadeFrota(EnumDiaSemana.Domingo, "knockoutDisponibilidadeFrota_Domingo", _centroCarregamento.DisponibilidadesFrota);

    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Segunda].Load();
    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Terca].Load();
    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Quarta].Load();
    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Quinta].Load();
    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Sexta].Load();
    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Sabado].Load();
    _listaKnockoutDisponibilidadeFrota[EnumDiaSemana.Domingo].Load();

    if (_CONFIGURACAO_TMS.ExibirDisponibilidadeFrotaCarregamento) {
        $("#liDisponibilidadeFrota_Segunda").removeClass("d-none");
        $("#liDisponibilidadeFrota_Terca").removeClass("d-none");
        $("#liDisponibilidadeFrota_Quarta").removeClass("d-none");
        $("#liDisponibilidadeFrota_Quinta").removeClass("d-none");
        $("#liDisponibilidadeFrota_Sexta").removeClass("d-none");
        $("#liDisponibilidadeFrota_Sabado").removeClass("d-none");
        $("#liDisponibilidadeFrota_Domingo").removeClass("d-none");
    }
}

function loadLimiteCarregamento() {
    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Segunda] = new LimiteCarregamento(EnumDiaSemana.Segunda, "knockoutLimiteCarregamento_Segunda", _centroCarregamento.LimitesCarregamento);
    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Terca] = new LimiteCarregamento(EnumDiaSemana.Terca, "knockoutLimiteCarregamento_Terca", _centroCarregamento.LimitesCarregamento);
    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Quarta] = new LimiteCarregamento(EnumDiaSemana.Quarta, "knockoutLimiteCarregamento_Quarta", _centroCarregamento.LimitesCarregamento);
    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Quinta] = new LimiteCarregamento(EnumDiaSemana.Quinta, "knockoutLimiteCarregamento_Quinta", _centroCarregamento.LimitesCarregamento);
    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Sexta] = new LimiteCarregamento(EnumDiaSemana.Sexta, "knockoutLimiteCarregamento_Sexta", _centroCarregamento.LimitesCarregamento);
    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Sabado] = new LimiteCarregamento(EnumDiaSemana.Sabado, "knockoutLimiteCarregamento_Sabado", _centroCarregamento.LimitesCarregamento);
    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Domingo] = new LimiteCarregamento(EnumDiaSemana.Domingo, "knockoutLimiteCarregamento_Domingo", _centroCarregamento.LimitesCarregamento);

    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Segunda].Load();
    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Terca].Load();
    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Quarta].Load();
    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Quinta].Load();
    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Sexta].Load();
    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Sabado].Load();
    _listaKnockoutLimiteCarregamento[EnumDiaSemana.Domingo].Load();

    if (_CONFIGURACAO_TMS.ExibirLimiteCarregamento) {
        $("#liLimiteCarregamento_Segunda").removeClass("d-none");
        $("#liLimiteCarregamento_Terca").removeClass("d-none");
        $("#liLimiteCarregamento_Quarta").removeClass("d-none");
        $("#liLimiteCarregamento_Quinta").removeClass("d-none");
        $("#liLimiteCarregamento_Sexta").removeClass("d-none");
        $("#liLimiteCarregamento_Sabado").removeClass("d-none");
        $("#liLimiteCarregamento_Domingo").removeClass("d-none");
    }
}

function loadPeriodosCarregamento() {
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Segunda] = new PeriodoCarregamento(EnumDiaSemana.Segunda, "knockoutPeriodoCarregamento_Segunda", _centroCarregamento.PeriodosCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Terca] = new PeriodoCarregamento(EnumDiaSemana.Terca, "knockoutPeriodoCarregamento_Terca", _centroCarregamento.PeriodosCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quarta] = new PeriodoCarregamento(EnumDiaSemana.Quarta, "knockoutPeriodoCarregamento_Quarta", _centroCarregamento.PeriodosCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quinta] = new PeriodoCarregamento(EnumDiaSemana.Quinta, "knockoutPeriodoCarregamento_Quinta", _centroCarregamento.PeriodosCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sexta] = new PeriodoCarregamento(EnumDiaSemana.Sexta, "knockoutPeriodoCarregamento_Sexta", _centroCarregamento.PeriodosCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sabado] = new PeriodoCarregamento(EnumDiaSemana.Sabado, "knockoutPeriodoCarregamento_Sabado", _centroCarregamento.PeriodosCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Domingo] = new PeriodoCarregamento(EnumDiaSemana.Domingo, "knockoutPeriodoCarregamento_Domingo", _centroCarregamento.PeriodosCarregamento);

    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Segunda].Load();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Terca].Load();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quarta].Load();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quinta].Load();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sexta].Load();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sabado].Load();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Domingo].Load();
}

function loadPrevisoesCarregamento() {
    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Segunda] = new PrevisaoCarregamento(EnumDiaSemana.Segunda, "knockoutPrevisaoCarregamento_Segunda", _centroCarregamento.PrevisoesCarregamento);
    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Terca] = new PrevisaoCarregamento(EnumDiaSemana.Terca, "knockoutPrevisaoCarregamento_Terca", _centroCarregamento.PrevisoesCarregamento);
    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Quarta] = new PrevisaoCarregamento(EnumDiaSemana.Quarta, "knockoutPrevisaoCarregamento_Quarta", _centroCarregamento.PrevisoesCarregamento);
    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Quinta] = new PrevisaoCarregamento(EnumDiaSemana.Quinta, "knockoutPrevisaoCarregamento_Quinta", _centroCarregamento.PrevisoesCarregamento);
    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Sexta] = new PrevisaoCarregamento(EnumDiaSemana.Sexta, "knockoutPrevisaoCarregamento_Sexta", _centroCarregamento.PrevisoesCarregamento);
    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Sabado] = new PrevisaoCarregamento(EnumDiaSemana.Sabado, "knockoutPrevisaoCarregamento_Sabado", _centroCarregamento.PrevisoesCarregamento);
    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Domingo] = new PrevisaoCarregamento(EnumDiaSemana.Domingo, "knockoutPrevisaoCarregamento_Domingo", _centroCarregamento.PrevisoesCarregamento);

    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Segunda].Load();
    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Terca].Load();
    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Quarta].Load();
    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Quinta].Load();
    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Sexta].Load();
    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Sabado].Load();
    _listaKnockoutPrevisoesCarregamento[EnumDiaSemana.Domingo].Load();

    if (_CONFIGURACAO_TMS.ExibirPrevisaoCarregamento) {
        $("#liPrevisaoCarregamento_Segunda").removeClass("d-none");
        $("#liPrevisaoCarregamento_Terca").removeClass("d-none");
        $("#liPrevisaoCarregamento_Quarta").removeClass("d-none");
        $("#liPrevisaoCarregamento_Quinta").removeClass("d-none");
        $("#liPrevisaoCarregamento_Sexta").removeClass("d-none");
        $("#liPrevisaoCarregamento_Sabado").removeClass("d-none");
        $("#liPrevisaoCarregamento_Domingo").removeClass("d-none");
    }
}

/*
 * Declaração das Funções Públicas
 */

function controlarCampoTipoCapacidadeCarregamento() {
    if (_centroCarregamento.UtilizarCapacidadeCarregamentoPorPeso.val()) {
        if (_capacidadeCarregamento.TipoCapacidadeCarregamento.val() == EnumTipoCapacidadeCarregamento.Todos) {
            _capacidadeCarregamento.TipoCapacidadeCarregamento.val(EnumTipoCapacidadeCarregamento.Peso);
            _capacidadeCarregamento.TipoCapacidadeCarregamento.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeDeCarregamentoKG.getFieldDescription());
        }

    }
}

function controlarVisibilidadeCamposCapacidadeCarregamentoCubagem() {
    var exibirCapacidadeCarregamentoCubagem = (_capacidadeCarregamento.TipoCapacidadeCarregamento.val() == EnumTipoCapacidadeCarregamento.CubagemVolume && _centroCarregamento.TipoCapacidadeCarregamentoPorPeso.val() == EnumTipoCapacidadeCarregamentoPorPeso.DiaSemana);

    _centroCarregamento.CapacidadeCarregamentoCubagemSegunda.visible(exibirCapacidadeCarregamentoCubagem);
    _centroCarregamento.CapacidadeCarregamentoCubagemTerca.visible(exibirCapacidadeCarregamentoCubagem);
    _centroCarregamento.CapacidadeCarregamentoCubagemQuarta.visible(exibirCapacidadeCarregamentoCubagem);
    _centroCarregamento.CapacidadeCarregamentoCubagemQuinta.visible(exibirCapacidadeCarregamentoCubagem);
    _centroCarregamento.CapacidadeCarregamentoCubagemSexta.visible(exibirCapacidadeCarregamentoCubagem);
    _centroCarregamento.CapacidadeCarregamentoCubagemSabado.visible(exibirCapacidadeCarregamentoCubagem);
    _centroCarregamento.CapacidadeCarregamentoCubagemDomingo.visible(exibirCapacidadeCarregamentoCubagem);

    if (exibirCapacidadeCarregamentoCubagem || _capacidadeCarregamento.TipoCapacidadeCarregamento.val() == EnumTipoCapacidadeCarregamento.Volume) {
        _centroCarregamento.CapacidadeCarregamentoSegunda.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeCarregamentoVolume);
        _centroCarregamento.CapacidadeCarregamentoTerca.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeCarregamentoVolume);
        _centroCarregamento.CapacidadeCarregamentoQuarta.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeCarregamentoVolume);
        _centroCarregamento.CapacidadeCarregamentoQuinta.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeCarregamentoVolume);
        _centroCarregamento.CapacidadeCarregamentoSexta.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeCarregamentoVolume);
        _centroCarregamento.CapacidadeCarregamentoSabado.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeCarregamentoVolume);
        _centroCarregamento.CapacidadeCarregamentoDomingo.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeCarregamentoVolume);
    } else {
        _centroCarregamento.CapacidadeCarregamentoSegunda.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeDeCarregamentoKG);
        _centroCarregamento.CapacidadeCarregamentoTerca.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeDeCarregamentoKG);
        _centroCarregamento.CapacidadeCarregamentoQuarta.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeDeCarregamentoKG);
        _centroCarregamento.CapacidadeCarregamentoQuinta.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeDeCarregamentoKG);
        _centroCarregamento.CapacidadeCarregamentoSexta.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeDeCarregamentoKG);
        _centroCarregamento.CapacidadeCarregamentoSabado.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeDeCarregamentoKG);
        _centroCarregamento.CapacidadeCarregamentoDomingo.text(Localization.Resources.Logistica.CentroCarregamento.CapacidadeDeCarregamentoKG);
    }

    controlarVisibilidadeCamposCapacidadeCarregamentoPorPeso();
}

function controlarVisibilidadeCamposCapacidadeCarregamentoPorPeso() {
    var exibirCapacidadeCarregamentoPorDiaSemana = (_centroCarregamento.TipoCapacidadeCarregamentoPorPeso.val() == EnumTipoCapacidadeCarregamentoPorPeso.DiaSemana);
    var exibirCapacidadeCarregamentoPorPeriodoCarregamento = (_centroCarregamento.TipoCapacidadeCarregamentoPorPeso.val() == EnumTipoCapacidadeCarregamentoPorPeso.PeriodoCarregamento);

    _centroCarregamento.CapacidadeCarregamentoSegunda.visible(exibirCapacidadeCarregamentoPorDiaSemana);
    _centroCarregamento.CapacidadeCarregamentoTerca.visible(exibirCapacidadeCarregamentoPorDiaSemana);
    _centroCarregamento.CapacidadeCarregamentoQuarta.visible(exibirCapacidadeCarregamentoPorDiaSemana);
    _centroCarregamento.CapacidadeCarregamentoQuinta.visible(exibirCapacidadeCarregamentoPorDiaSemana);
    _centroCarregamento.CapacidadeCarregamentoSexta.visible(exibirCapacidadeCarregamentoPorDiaSemana);
    _centroCarregamento.CapacidadeCarregamentoSabado.visible(exibirCapacidadeCarregamentoPorDiaSemana);
    _centroCarregamento.CapacidadeCarregamentoDomingo.visible(exibirCapacidadeCarregamentoPorDiaSemana);

    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Segunda].ControlarExibicaoCapacidadeCarregamento(exibirCapacidadeCarregamentoPorPeriodoCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Terca].ControlarExibicaoCapacidadeCarregamento(exibirCapacidadeCarregamentoPorPeriodoCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quarta].ControlarExibicaoCapacidadeCarregamento(exibirCapacidadeCarregamentoPorPeriodoCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quinta].ControlarExibicaoCapacidadeCarregamento(exibirCapacidadeCarregamentoPorPeriodoCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sexta].ControlarExibicaoCapacidadeCarregamento(exibirCapacidadeCarregamentoPorPeriodoCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sabado].ControlarExibicaoCapacidadeCarregamento(exibirCapacidadeCarregamentoPorPeriodoCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Domingo].ControlarExibicaoCapacidadeCarregamento(exibirCapacidadeCarregamentoPorPeriodoCarregamento);


}

function LimparCamposCapacidadeCarregamento() {
    LimparCampos(_capacidadeCarregamento);

    recarregarGridsCapacidadeCarregamento();
}

function recarregarGridsCapacidadeCarregamento() {
    recarregarGridPeriodosCarregamento();
    recarregarGridPrevisoesCarregamento();
    recarregarGridDisponibilidadeFrota();
    recarregarGridLimiteCarregamento();
}

/*
 * Declaração das Funções Privadas
 */

function recarregarGridDisponibilidadeFrota() {
    for (var i = 0; i < _listaKnockoutDisponibilidadeFrota.length; i++) {
        if (_listaKnockoutDisponibilidadeFrota[i] != null) {
            _listaKnockoutDisponibilidadeFrota[i].RecarregarGrid();
            _listaKnockoutDisponibilidadeFrota[i].LimparCampos();
        }
    }
}

function recarregarGridLimiteCarregamento() {
    for (var i = 0; i < _listaKnockoutLimiteCarregamento.length; i++) {
        if (_listaKnockoutLimiteCarregamento[i] != null) {
            _listaKnockoutLimiteCarregamento[i].RecarregarGrid();
            _listaKnockoutLimiteCarregamento[i].LimparCampos();
        }
    }
}

function recarregarGridPeriodosCarregamento() {
    for (var i = 0; i < _listaKnockoutPeriodosCarregamento.length; i++) {
        if (_listaKnockoutPeriodosCarregamento[i] != null) {
            _listaKnockoutPeriodosCarregamento[i].RecarregarGrid();
            _listaKnockoutPeriodosCarregamento[i].LimparCampos();
        }
    }
}

function recarregarGridPrevisoesCarregamento() {
    for (var i = 0; i < _listaKnockoutPrevisoesCarregamento.length; i++) {
        if (_listaKnockoutPrevisoesCarregamento[i] != null) {
            _listaKnockoutPrevisoesCarregamento[i].RecarregarGrid();
            _listaKnockoutPrevisoesCarregamento[i].LimparCampos();
        }
    }
}
