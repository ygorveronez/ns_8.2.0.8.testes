/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />

/*
 * Declaração das Classes
 */

var CapacidadeDescarregamentoDadosModel = function (instancia) {
    this.DiaSemana = PropertyEntity({ val: ko.observable(instancia.DiaSemana), def: instancia.DiaSemana, getType: typesKnockout.int });
    this.Dia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Mes = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CapacidadeDescarregamento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.CapacidadeDescarregamentoKg.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 15 });
}

var CapacidadeDescarregamentoDados = function (diaSemana, idKnockout) {
    var $this = this;

    $this.DiaSemana = diaSemana;
    $this.IdKnockout = idKnockout;

    this.Load = function () {
        $this.CapacidadeDescarregamentoDados = new CapacidadeDescarregamentoDadosModel($this);
        KoBindings($this.CapacidadeDescarregamentoDados, $this.IdKnockout);

        $this.CapacidadeDescarregamento = $this.CapacidadeDescarregamentoDados.CapacidadeDescarregamento;
    }
}