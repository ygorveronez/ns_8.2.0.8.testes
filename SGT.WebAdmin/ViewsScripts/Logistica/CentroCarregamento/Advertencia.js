/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/MotivoAdvertenciaTransportador.js" />
/// <reference path="../../Enumeradores/EnumTipoManobraAcao.js" />
/// <reference path="CentroCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _centroCarregamentoAdvertencia;

/*
 * Declaração das Classes
 */

var CentroCarregamentoAdvertencia = function () {
    this.MotivoAdvertenciaChegadaEmAtraso = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.AdvertenciaAutomaticaParaChegadaEmAtraso.getFieldDescription(), idBtnSearch: guid() });
    this.TempoToleranciaChegadaAtraso = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 3, text: Localization.Resources.Logistica.CentroCarregamento.TempoDeToleranciaMinutos.getFieldDescription() });
    this.TempoToleranciaCargaFechada = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 3, text: Localization.Resources.Logistica.CentroCarregamento.TempoDeToleranciaCargaFechadaMinutos.getFieldDescription() });
    this.HabilitarTermoChegadaHorario = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.HabilitarTermoChegadaHorario.getFieldDescription() });
    this.TermoChegadaHorario = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 5000, text: Localization.Resources.Logistica.CentroCarregamento.TermoChegadaHorario.getFieldDescription() });
}

function loadCentroCarregamentoAdvertencia() {
    _centroCarregamentoAdvertencia = new CentroCarregamentoAdvertencia();
    KoBindings(_centroCarregamentoAdvertencia, "knockoutAdvertencia");

    new BuscarMotivoAdvertenciaTransportador(_centroCarregamentoAdvertencia.MotivoAdvertenciaChegadaEmAtraso);

    _centroCarregamento.MotivoAdvertenciaChegadaEmAtraso = _centroCarregamentoAdvertencia.MotivoAdvertenciaChegadaEmAtraso;
    _centroCarregamento.TempoToleranciaChegadaAtraso = _centroCarregamentoAdvertencia.TempoToleranciaChegadaAtraso;
    _centroCarregamento.TempoToleranciaCargaFechada = _centroCarregamentoAdvertencia.TempoToleranciaCargaFechada;
    _centroCarregamento.HabilitarTermoChegadaHorario = _centroCarregamentoAdvertencia.HabilitarTermoChegadaHorario;
    _centroCarregamento.TermoChegadaHorario = _centroCarregamentoAdvertencia.TermoChegadaHorario;
}

function limparCamposCentroCarregamentoManobraAcao() {
    LimparCampos(_centroCarregamentoAdvertencia);
}
