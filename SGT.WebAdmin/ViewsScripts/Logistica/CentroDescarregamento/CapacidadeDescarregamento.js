/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="../../Enumeradores/EnumTipoCapacidadeDescarregamentoPorPeso.js" />
/// <reference path="CapacidadeDescarregamentoDados.js" />
/// <reference path="CentroDescarregamento.js" />
/// <reference path="LimiteDescarregamento.js" />
/// <reference path="PeriodoDescarregamento.js" />
/// <reference path="PrevisaoDescarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _capacidadeDescarregamento;
var _listaKnockoutCapacidadeDescarregamentoDados = new Array();
var _listaKnockoutLimiteDescarregamento = new Array();
var _listaKnockoutPeriodosDescarregamento = new Array();
var _listaKnockoutPrevisoesDescarregamento = new Array();

/*
 * Declaração das Classes
 */

var CapacidadeDescarregamento = function () {
    this.NumeroDocas = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.NumeroDocas.getRequiredFieldDescription(), required: true, visible: ko.observable(false) });
    this.NumeroDocas.val.subscribe(function (novoValor) {
        _centroDescarregamento.NumeroDocas.val(novoValor);
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadCapacidadeDescarregamento() {
    _capacidadeDescarregamento = new CapacidadeDescarregamento();
    KoBindings(_capacidadeDescarregamento, "knockoutCapacidadeDescarregamento");

    LoadCapacidadeDescarregamentoDados();
    loadPeriodoDescarregamento();
    loadPrevisoesDescarregamento();
    loadLimiteDescarregamento();
}

function LoadCapacidadeDescarregamentoDados() {
    _listaKnockoutCapacidadeDescarregamentoDados[0] = new CapacidadeDescarregamentoDados(0, "knockoutCapacidadeDescarregamentoDados");
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Segunda] = new CapacidadeDescarregamentoDados(EnumDiaSemana.Segunda, "knockoutCapacidadeDescarregamentoDados_Segunda");
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Terca] = new CapacidadeDescarregamentoDados(EnumDiaSemana.Terca, "knockoutCapacidadeDescarregamentoDados_Terca");
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Quarta] = new CapacidadeDescarregamentoDados(EnumDiaSemana.Quarta, "knockoutCapacidadeDescarregamentoDados_Quarta");
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Quinta] = new CapacidadeDescarregamentoDados(EnumDiaSemana.Quinta, "knockoutCapacidadeDescarregamentoDados_Quinta");
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Sexta] = new CapacidadeDescarregamentoDados(EnumDiaSemana.Sexta, "knockoutCapacidadeDescarregamentoDados_Sexta");
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Sabado] = new CapacidadeDescarregamentoDados(EnumDiaSemana.Sabado, "knockoutCapacidadeDescarregamentoDados_Sabado");
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Domingo] = new CapacidadeDescarregamentoDados(EnumDiaSemana.Domingo, "knockoutCapacidadeDescarregamentoDados_Domingo");

    _listaKnockoutCapacidadeDescarregamentoDados[0].Load();
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Segunda].Load();
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Terca].Load();
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Quarta].Load();
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Quinta].Load();
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Sexta].Load();
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Sabado].Load();
    _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Domingo].Load();

    _centroDescarregamento.CapacidadeDescarregamento = _listaKnockoutCapacidadeDescarregamentoDados[0].CapacidadeDescarregamento;
    _centroDescarregamento.CapacidadeDescarregamentoSegunda = _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Segunda].CapacidadeDescarregamento;
    _centroDescarregamento.CapacidadeDescarregamentoTerca = _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Terca].CapacidadeDescarregamento;
    _centroDescarregamento.CapacidadeDescarregamentoQuarta = _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Quarta].CapacidadeDescarregamento;
    _centroDescarregamento.CapacidadeDescarregamentoQuinta = _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Quinta].CapacidadeDescarregamento;
    _centroDescarregamento.CapacidadeDescarregamentoSexta = _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Sexta].CapacidadeDescarregamento;
    _centroDescarregamento.CapacidadeDescarregamentoSabado = _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Sabado].CapacidadeDescarregamento;
    _centroDescarregamento.CapacidadeDescarregamentoDomingo = _listaKnockoutCapacidadeDescarregamentoDados[EnumDiaSemana.Domingo].CapacidadeDescarregamento;
}

function loadLimiteDescarregamento() {
    _listaKnockoutLimiteDescarregamento[0] = new LimiteDescarregamento(0, "knockoutLimiteDescarregamento", _centroDescarregamento.LimitesDescarregamento);
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Segunda] = new LimiteDescarregamento(EnumDiaSemana.Segunda, "knockoutLimiteDescarregamento_Segunda", _centroDescarregamento.LimitesDescarregamento);
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Terca] = new LimiteDescarregamento(EnumDiaSemana.Terca, "knockoutLimiteDescarregamento_Terca", _centroDescarregamento.LimitesDescarregamento);
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Quarta] = new LimiteDescarregamento(EnumDiaSemana.Quarta, "knockoutLimiteDescarregamento_Quarta", _centroDescarregamento.LimitesDescarregamento);
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Quinta] = new LimiteDescarregamento(EnumDiaSemana.Quinta, "knockoutLimiteDescarregamento_Quinta", _centroDescarregamento.LimitesDescarregamento);
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Sexta] = new LimiteDescarregamento(EnumDiaSemana.Sexta, "knockoutLimiteDescarregamento_Sexta", _centroDescarregamento.LimitesDescarregamento);
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Sabado] = new LimiteDescarregamento(EnumDiaSemana.Sabado, "knockoutLimiteDescarregamento_Sabado", _centroDescarregamento.LimitesDescarregamento);
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Domingo] = new LimiteDescarregamento(EnumDiaSemana.Domingo, "knockoutLimiteDescarregamento_Domingo", _centroDescarregamento.LimitesDescarregamento);

    _listaKnockoutLimiteDescarregamento[0].Load();
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Segunda].Load();
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Terca].Load();
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Quarta].Load();
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Quinta].Load();
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Sexta].Load();
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Sabado].Load();
    _listaKnockoutLimiteDescarregamento[EnumDiaSemana.Domingo].Load();

    if (_CONFIGURACAO_TMS.ExibirLimiteCarregamento) {
        $("#liLimiteDescarregamento").removeClass("hidden");
        $("#liLimiteDescarregamento_Segunda").removeClass("hidden");
        $("#liLimiteDescarregamento_Terca").removeClass("hidden");
        $("#liLimiteDescarregamento_Quarta").removeClass("hidden");
        $("#liLimiteDescarregamento_Quinta").removeClass("hidden");
        $("#liLimiteDescarregamento_Sexta").removeClass("hidden");
        $("#liLimiteDescarregamento_Sabado").removeClass("hidden");
        $("#liLimiteDescarregamento_Domingo").removeClass("hidden");
    }
}

function loadPeriodoDescarregamento() {
    _listaKnockoutPeriodosDescarregamento[0] = new PeriodoDescarregamento(0, "knockoutPeriodoDescarregamento", _centroDescarregamento.PeriodosDescarregamento);
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Segunda] = new PeriodoDescarregamento(EnumDiaSemana.Segunda, "knockoutPeriodoDescarregamento_Segunda", _centroDescarregamento.PeriodosDescarregamento);
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Terca] = new PeriodoDescarregamento(EnumDiaSemana.Terca, "knockoutPeriodoDescarregamento_Terca", _centroDescarregamento.PeriodosDescarregamento);
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Quarta] = new PeriodoDescarregamento(EnumDiaSemana.Quarta, "knockoutPeriodoDescarregamento_Quarta", _centroDescarregamento.PeriodosDescarregamento);
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Quinta] = new PeriodoDescarregamento(EnumDiaSemana.Quinta, "knockoutPeriodoDescarregamento_Quinta", _centroDescarregamento.PeriodosDescarregamento);
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Sexta] = new PeriodoDescarregamento(EnumDiaSemana.Sexta, "knockoutPeriodoDescarregamento_Sexta", _centroDescarregamento.PeriodosDescarregamento);
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Sabado] = new PeriodoDescarregamento(EnumDiaSemana.Sabado, "knockoutPeriodoDescarregamento_Sabado", _centroDescarregamento.PeriodosDescarregamento);
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Domingo] = new PeriodoDescarregamento(EnumDiaSemana.Domingo, "knockoutPeriodoDescarregamento_Domingo", _centroDescarregamento.PeriodosDescarregamento);

    _listaKnockoutPeriodosDescarregamento[0].Load();
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Segunda].Load();
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Terca].Load();
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Quarta].Load();
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Quinta].Load();
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Sexta].Load();
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Sabado].Load();
    _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Domingo].Load();
}

function loadPrevisoesDescarregamento() {
    _listaKnockoutPrevisoesDescarregamento[0] = new PrevisaoDescarregamento(0, "knockoutPrevisaoDescarregamento", _centroDescarregamento.PrevisoesDescarregamento);
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Segunda] = new PrevisaoDescarregamento(EnumDiaSemana.Segunda, "knockoutPrevisaoDescarregamento_Segunda", _centroDescarregamento.PrevisoesDescarregamento);
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Terca] = new PrevisaoDescarregamento(EnumDiaSemana.Terca, "knockoutPrevisaoDescarregamento_Terca", _centroDescarregamento.PrevisoesDescarregamento);
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Quarta] = new PrevisaoDescarregamento(EnumDiaSemana.Quarta, "knockoutPrevisaoDescarregamento_Quarta", _centroDescarregamento.PrevisoesDescarregamento);
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Quinta] = new PrevisaoDescarregamento(EnumDiaSemana.Quinta, "knockoutPrevisaoDescarregamento_Quinta", _centroDescarregamento.PrevisoesDescarregamento);
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Sexta] = new PrevisaoDescarregamento(EnumDiaSemana.Sexta, "knockoutPrevisaoDescarregamento_Sexta", _centroDescarregamento.PrevisoesDescarregamento);
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Sabado] = new PrevisaoDescarregamento(EnumDiaSemana.Sabado, "knockoutPrevisaoDescarregamento_Sabado", _centroDescarregamento.PrevisoesDescarregamento);
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Domingo] = new PrevisaoDescarregamento(EnumDiaSemana.Domingo, "knockoutPrevisaoDescarregamento_Domingo", _centroDescarregamento.PrevisoesDescarregamento);

    _listaKnockoutPrevisoesDescarregamento[0].Load();
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Segunda].Load();
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Terca].Load();
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Quarta].Load();
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Quinta].Load();
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Sexta].Load();
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Sabado].Load();
    _listaKnockoutPrevisoesDescarregamento[EnumDiaSemana.Domingo].Load();
}

/*
 * Declaração das Funções Públicas
 */

function controlarVisibilidadeCamposCapacidadeDescarregamentoPorPeso() {
    var exibirCapacidadeDescarregamentoPorDiaSemana = (_centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso.val() == EnumTipoCapacidadeDescarregamentoPorPeso.DiaSemana);
    var exibirCapacidadeDescarregamentoPorPeriodoCarregamento = (_centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso.val() == EnumTipoCapacidadeDescarregamentoPorPeso.PeriodoDescarregamento);

    _centroDescarregamento.CapacidadeDescarregamento.visible(exibirCapacidadeDescarregamentoPorDiaSemana);
    _centroDescarregamento.CapacidadeDescarregamentoSegunda.visible(exibirCapacidadeDescarregamentoPorDiaSemana);
    _centroDescarregamento.CapacidadeDescarregamentoTerca.visible(exibirCapacidadeDescarregamentoPorDiaSemana);
    _centroDescarregamento.CapacidadeDescarregamentoQuarta.visible(exibirCapacidadeDescarregamentoPorDiaSemana);
    _centroDescarregamento.CapacidadeDescarregamentoQuinta.visible(exibirCapacidadeDescarregamentoPorDiaSemana);
    _centroDescarregamento.CapacidadeDescarregamentoSexta.visible(exibirCapacidadeDescarregamentoPorDiaSemana);
    _centroDescarregamento.CapacidadeDescarregamentoSabado.visible(exibirCapacidadeDescarregamentoPorDiaSemana);
    _centroDescarregamento.CapacidadeDescarregamentoDomingo.visible(exibirCapacidadeDescarregamentoPorDiaSemana);

    if (_listaKnockoutPeriodosDescarregamento != undefined && _listaKnockoutPeriodosDescarregamento.length > 0) {
        _listaKnockoutPeriodosDescarregamento[0].ControlarExibicaoCapacidadeDescarregamento(exibirCapacidadeDescarregamentoPorPeriodoCarregamento);
        _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Segunda].ControlarExibicaoCapacidadeDescarregamento(exibirCapacidadeDescarregamentoPorPeriodoCarregamento);
        _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Terca].ControlarExibicaoCapacidadeDescarregamento(exibirCapacidadeDescarregamentoPorPeriodoCarregamento);
        _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Quarta].ControlarExibicaoCapacidadeDescarregamento(exibirCapacidadeDescarregamentoPorPeriodoCarregamento);
        _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Quinta].ControlarExibicaoCapacidadeDescarregamento(exibirCapacidadeDescarregamentoPorPeriodoCarregamento);
        _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Sexta].ControlarExibicaoCapacidadeDescarregamento(exibirCapacidadeDescarregamentoPorPeriodoCarregamento);
        _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Sabado].ControlarExibicaoCapacidadeDescarregamento(exibirCapacidadeDescarregamentoPorPeriodoCarregamento);
        _listaKnockoutPeriodosDescarregamento[EnumDiaSemana.Domingo].ControlarExibicaoCapacidadeDescarregamento(exibirCapacidadeDescarregamentoPorPeriodoCarregamento);
    }
}

function LimparCamposCapacidadeDescarregamento() {
    LimparCampos(_capacidadeDescarregamento);

    recarregarGridsCapacidadeDescarregamento();
}

function recarregarGridsCapacidadeDescarregamento() {
    recarregarGridPeriodosDescarregamento();
    recarregarGridPrevisoesDescarregamento();
    recarregarGridLimiteDescarregamento();
}

/*
 * Declaração das Funções Privadas
 */

function recarregarGridLimiteDescarregamento() {
    for (var i = 0; i < _listaKnockoutLimiteDescarregamento.length; i++) {
        if (_listaKnockoutLimiteDescarregamento[i] != null) {
            _listaKnockoutLimiteDescarregamento[i].RecarregarGrid();
            _listaKnockoutLimiteDescarregamento[i].LimparCampos();
        }
    }
}

function recarregarGridPeriodosDescarregamento() {
    for (var i = 0; i < _listaKnockoutPeriodosDescarregamento.length; i++) {
        if (_listaKnockoutPeriodosDescarregamento[i] != null) {
            _listaKnockoutPeriodosDescarregamento[i].RecarregarGrid();
            _listaKnockoutPeriodosDescarregamento[i].LimparCampos();
        }
    }
}

function recarregarGridPrevisoesDescarregamento() {
    for (var i = 0; i < _listaKnockoutPrevisoesDescarregamento.length; i++) {
        if (_listaKnockoutPrevisoesDescarregamento[i] != null) {
            _listaKnockoutPrevisoesDescarregamento[i].RecarregarGrid();
            _listaKnockoutPrevisoesDescarregamento[i].LimparCampos();
        }
    }
}
