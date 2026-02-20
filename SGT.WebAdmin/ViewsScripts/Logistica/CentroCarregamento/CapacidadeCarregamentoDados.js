/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />

/*
 * Declaração das Classes
 */

var CapacidadeCarregamentoDadosModel = function (instancia) {
    this.DiaSemana = PropertyEntity({ val: ko.observable(instancia.DiaSemana), def: instancia.DiaSemana, getType: typesKnockout.int });
    this.ToleranciaAtraso = PropertyEntity({ val: ko.observable(""), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.ToleranciaEmDiasParaConsiderarAtraso.getFieldDescription(), issue: 324 });
    this.CapacidadeCarregamentoVolume = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: ko.observable(Localization.Resources.Logistica.CentroCarregamento.CapacidadeDeCarregamento.getFieldDescription()), required: false, visible: ko.observable(false), maxlength: 15 });
    this.CapacidadeCarregamentoCubagem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: ko.observable(Localization.Resources.Logistica.CentroCarregamento.CapacidadeCarregamentoCubagem.getFieldDescription()), required: false, visible: ko.observable(false), maxlength: 15 });
}

var CapacidadeCarregamentoDados = function (diaSemana, idKnockout) {
    var $this = this;

    $this.DiaSemana = diaSemana;
    $this.IdKnockout = idKnockout;

    this.Load = function () {
        $this.CapacidadeCarregamentoDados = new CapacidadeCarregamentoDadosModel($this);
        KoBindings($this.CapacidadeCarregamentoDados, $this.IdKnockout);

        $this.CapacidadeCarregamentoVolume = $this.CapacidadeCarregamentoDados.CapacidadeCarregamentoVolume;
        $this.CapacidadeCarregamentoCubagem = $this.CapacidadeCarregamentoDados.CapacidadeCarregamentoCubagem;
        $this.ToleranciaAtraso = $this.CapacidadeCarregamentoDados.ToleranciaAtraso;
    }
}
