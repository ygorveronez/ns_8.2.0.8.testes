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


//*******MAPEAMENTO KNOUCKOUT*******

var _janelaColetaPeriodoSegunda;
var _janelaColetaPeriodoTerca;
var _janelaColetaPeriodoQuarta;
var _janelaColetaPeriodoQuinta;
var _janelaColetaPeriodoSexta;
var _janelaColetaPeriodoSabado;
var _janelaColetaPeriodoDomingo;

var JanelaColetaPeriodoModelo = function (diaSemana) {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiaSemana = PropertyEntity({ val: ko.observable(diaSemana), def: diaSemana, getType: typesKnockout.int });
    this.HoraInicio = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: "*Início:", required: true });
    this.HoraTermino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.time, text: "*Término:", required: true });
    this.FazColeta = PropertyEntity({ text: "Faz coleta?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
}


//*******EVENTOS*******

function LoadJanelaColetaPeriodo() {
    _janelaColetaPeriodoDomingo = new JanelaColetaPeriodoModelo();
    KoBindings(_janelaColetaPeriodoDomingo, "knockoutJanelaColetaPeriodo_Domingo");

    _janelaColetaPeriodoSegunda = new JanelaColetaPeriodoModelo(2);
    KoBindings(_janelaColetaPeriodoSegunda, "knockoutJanelaColetaPeriodo_Segunda");

    _janelaColetaPeriodoTerca = new JanelaColetaPeriodoModelo(3);
    KoBindings(_janelaColetaPeriodoTerca, "knockoutJanelaColetaPeriodo_Terca");

    _janelaColetaPeriodoQuarta = new JanelaColetaPeriodoModelo(4);
    KoBindings(_janelaColetaPeriodoQuarta, "knockoutJanelaColetaPeriodo_Quarta");

    _janelaColetaPeriodoQuinta = new JanelaColetaPeriodoModelo(5);
    KoBindings(_janelaColetaPeriodoQuinta, "knockoutJanelaColetaPeriodo_Quinta");

    _janelaColetaPeriodoSexta = new JanelaColetaPeriodoModelo(6);
    KoBindings(_janelaColetaPeriodoSexta, "knockoutJanelaColetaPeriodo_Sexta");

    _janelaColetaPeriodoSabado = new JanelaColetaPeriodoModelo(7);
    KoBindings(_janelaColetaPeriodoSabado, "knockoutJanelaColetaPeriodo_Sabado");
}


function obterJanelasColetaPeriodo() {
    var listaPeriodos = [];

    listaPeriodos.push(RetornarObjetoPesquisa(_janelaColetaPeriodoSegunda));
    listaPeriodos.push(RetornarObjetoPesquisa(_janelaColetaPeriodoTerca));
    listaPeriodos.push(RetornarObjetoPesquisa(_janelaColetaPeriodoQuarta));
    listaPeriodos.push(RetornarObjetoPesquisa(_janelaColetaPeriodoQuinta));
    listaPeriodos.push(RetornarObjetoPesquisa(_janelaColetaPeriodoSexta));
    listaPeriodos.push(RetornarObjetoPesquisa(_janelaColetaPeriodoSabado));
    listaPeriodos.push(RetornarObjetoPesquisa(_janelaColetaPeriodoDomingo));

    return listaPeriodos;
}

function limparCamposJanelaColetaPeriodo() {
    LimparCampos(_janelaColetaPeriodoSegunda);
    LimparCampos(_janelaColetaPeriodoTerca);
    LimparCampos(_janelaColetaPeriodoQuarta);
    LimparCampos(_janelaColetaPeriodoQuinta);
    LimparCampos(_janelaColetaPeriodoSexta);
    LimparCampos(_janelaColetaPeriodoSabado);
    LimparCampos(_janelaColetaPeriodoDomingo);
}


function obterObjPorDiaSemana(dia) {

    switch (dia) {
        case 1: return _janelaColetaPeriodoDomingo;
        case 2: return _janelaColetaPeriodoSegunda; 
        case 3: return _janelaColetaPeriodoTerca;
        case 4: return _janelaColetaPeriodoQuarta;
        case 5: return _janelaColetaPeriodoQuinta;
        case 6: return _janelaColetaPeriodoSexta;
        case 7: return _janelaColetaPeriodoSabado;
        default: return null
    }
}

function PreencherObjetoPeridodo(periodo) {
    var obj = obterObjPorDiaSemana(periodo.DiaSemana)
    obj.Codigo.val(periodo.Codigo);
    obj.DiaSemana.val(periodo.DiaSemana);
    obj.HoraInicio.val(periodo.HoraInicio)
    obj.HoraTermino.val(periodo.HoraTermino)
    obj.FazColeta.val(periodo.Ativo);
}

function RecarregarJanelaColetaPeriodo() {
    var listaPeriodos = _janelaColeta.PeriodosColeta.val();

    for (var i = 0; i < listaPeriodos.length; i++) {
        PreencherObjetoPeridodo(listaPeriodos[i]);

    }
}
