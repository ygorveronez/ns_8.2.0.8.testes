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
/// <reference path="../../Enumeradores/EnumDiaSemanaMesAno.js" />
/// <reference path="../../Enumeradores/EnumTipoGeracaoTermo.js" />
/// <reference path="../../Enumeradores/EnumTipoTermo.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _termoQuitacao;

var TermoQuitacao = function () {
    this.GerarAvisoPeriodico = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.GerarAvisoPeriodico, visible: ko.observable(true), getType: typesKnockout.bool, def: false });

    this.ACadaAvisoPeriodico = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.ACada.getFieldDescription(), visible: ko.observable(true), maxlength: 2, getType: typesKnockout.int });
    this.PeriodoAvisoPeriodico = PropertyEntity({ val: ko.observable(EnumDiaSemanaMesAno.Dia), options: EnumDiaSemanaMesAno.obterOpcoes(), def: EnumDiaSemanaMesAno.Dia, text: Localization.Resources.Transportadores.Transportador.Periodo.getFieldDescription(), required: false });
    this.TempoAguardarParaGerarTermo = PropertyEntity({ val: ko.observable(0), def: 0, text: Localization.Resources.Transportadores.Transportador.TempoAguardarParaGerarTermo, getType: typesKnockout.int });
    this.DataFimTermoQuitacaoInicial = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Transportadores.Transportador.DataFimTermoQuitacaoInicial.getFieldDescription()});

    this.TipoGeracaoTermo = PropertyEntity({ val: ko.observable(EnumTipoGeracaoTermo.Nenhum), options: EnumTipoGeracaoTermo.obterOpcoesTodos(), def: EnumTipoGeracaoTermo.Nenhum, text: Localization.Resources.Transportadores.Transportador.TipoGeracaoTermo.getFieldDescription(), required: false });
    this.TipoTermo = PropertyEntity({ val: ko.observable(EnumTipoTermo.Nenhum), options: EnumTipoTermo.obterOpcoesTodos(), def: EnumTipoTermo.Nenhum, text: Localization.Resources.Transportadores.Transportador.TipoTermo.getFieldDescription(), required: false });
    this.ACadaTipoTermo = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.ACada.getFieldDescription(), visible: ko.observable(true), maxlength: 2, getType: typesKnockout.int });
    this.PeriodoTipoTermo = PropertyEntity({ val: ko.observable(EnumDiaSemanaMesAno.Dia), options: EnumDiaSemanaMesAno.obterOpcoes(), def: EnumDiaSemanaMesAno.Dia, text: Localization.Resources.Transportadores.Transportador.Periodo.getFieldDescription(), required: false });
};

//*******EVENTOS*******

function loadTermoQuitacao() {
    _termoQuitacao = new TermoQuitacao();
    KoBindings(_termoQuitacao, "knockoutTermoQuitacao");
}

function limparCamposTermoQuitacao() {
    LimparCampos(_termoQuitacao);
}