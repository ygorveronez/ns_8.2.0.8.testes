/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="../../Enumeradores/EnumTipoCapacidadeCarregamentoPorPeso.js" />
/// <reference path="PeriodoCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _listaKnockoutPeriodosCarregamento = new Array();

/*
 * Declaração das Funções de Inicialização
 */

function LoadCapacidadeCarregamento() {
    loadPeriodosCarregamento();
}

function loadPeriodosCarregamento() {
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Segunda] = new PeriodoCarregamento(EnumDiaSemana.Segunda, "knockoutPeriodoCarregamento_Segunda", _regrasMultaAtrasoRetirada.PeriodosCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Terca] = new PeriodoCarregamento(EnumDiaSemana.Terca, "knockoutPeriodoCarregamento_Terca", _regrasMultaAtrasoRetirada.PeriodosCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quarta] = new PeriodoCarregamento(EnumDiaSemana.Quarta, "knockoutPeriodoCarregamento_Quarta", _regrasMultaAtrasoRetirada.PeriodosCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quinta] = new PeriodoCarregamento(EnumDiaSemana.Quinta, "knockoutPeriodoCarregamento_Quinta", _regrasMultaAtrasoRetirada.PeriodosCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sexta] = new PeriodoCarregamento(EnumDiaSemana.Sexta, "knockoutPeriodoCarregamento_Sexta", _regrasMultaAtrasoRetirada.PeriodosCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sabado] = new PeriodoCarregamento(EnumDiaSemana.Sabado, "knockoutPeriodoCarregamento_Sabado", _regrasMultaAtrasoRetirada.PeriodosCarregamento);
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Domingo] = new PeriodoCarregamento(EnumDiaSemana.Domingo, "knockoutPeriodoCarregamento_Domingo", _regrasMultaAtrasoRetirada.PeriodosCarregamento);

    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Segunda].Load();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Terca].Load();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quarta].Load();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quinta].Load();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sexta].Load();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sabado].Load();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Domingo].Load();
}

/*
 * Declaração das Funções Públicas
 */

function LimparCamposCapacidadeCarregamento() {
    recarregarGridsCapacidadeCarregamento();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Segunda].LimparCampos();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Terca].LimparCampos();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quarta].LimparCampos();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quinta].LimparCampos();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sexta].LimparCampos();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sabado].LimparCampos();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Domingo].LimparCampos();
}

function recarregarGridsCapacidadeCarregamento() {
    recarregarGridPeriodosCarregamento();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Segunda].RecarregarGrid();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Terca].RecarregarGrid();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quarta].RecarregarGrid();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Quinta].RecarregarGrid();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sexta].RecarregarGrid();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Sabado].RecarregarGrid();
    _listaKnockoutPeriodosCarregamento[EnumDiaSemana.Domingo].RecarregarGrid();
}

/*
 * Declaração das Funções Privadas
 */

function recarregarGridPeriodosCarregamento() {
    for (var i = 0; i < _listaKnockoutPeriodosCarregamento.length; i++) {
        if (_listaKnockoutPeriodosCarregamento[i] != null) {
            _listaKnockoutPeriodosCarregamento[i].RecarregarGrid();
            _listaKnockoutPeriodosCarregamento[i].LimparCampos();
        }
    }
}