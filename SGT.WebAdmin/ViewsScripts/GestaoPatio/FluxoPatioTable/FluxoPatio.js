/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _fluxoAtual = null;
var FluxoPatio = function (data) {
    var _this = this;

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.int, eventClick: ObterDetalhesCargaFluxoClick, OcultarFluxoCarga: _configuracaoGestaoPatio.OcultarFluxoCarga });
    this.PreCarga = PropertyEntity({ text: "Pré Carga: ", getType: typesKnockout.int });
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });
    this.NumeroCarregamento = PropertyEntity({});
    this.PossuiCarga = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CargaCancelada = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SituacaoEtapaFluxoGestaoPatio = PropertyEntity({});
    this.EtapaFluxoGestaoPatioAtual = PropertyEntity({});
    this.EtapaAtual = PropertyEntity({});

    this.Auditar = PropertyEntity({ eventClick: AuditarEtapaFluxoGestaoPatio, visible: ko.observable(PermiteAuditar()) });

    this.InformarDocaCarregamentoDescricao = PropertyEntity({});
    this.MontagemCargaDescricao = PropertyEntity({});
    this.ChegadaVeiculoDescricao = PropertyEntity({});
    this.GuaritaEntradaDescricao = PropertyEntity({});
    this.CheckListDescricao = PropertyEntity({});
    this.TravaChaveDescricao = PropertyEntity({});
    this.ExpedicaoDescricao = PropertyEntity({});
    this.LiberaChaveDescricao = PropertyEntity({});
    this.FaturamentoDescricao = PropertyEntity({});
    this.GuaritaSaidaDescricao = PropertyEntity({});
    this.PosicaoDescricao = PropertyEntity({});
    this.Temperatura = PropertyEntity({});
    this.ChegadaLojaDescricao = PropertyEntity({});
    this.DeslocamentoPatioDescricao = PropertyEntity({});
    this.SaidaLojaDescricao = PropertyEntity({});
    this.FimViagemDescricao = PropertyEntity({});
    this.InicioHigienizacaoDescricao = PropertyEntity({});
    this.FimHigienizacaoDescricao = PropertyEntity({});
    this.InicioCarregamentoDescricao = PropertyEntity({});
    this.FimCarregamentoDescricao = PropertyEntity({});
    this.SeparacaoMercadoriaDescricao = PropertyEntity({});
    this.SolicitacaoVeiculoDescricao = PropertyEntity({});
    this.DocumentoFiscalDescricao = PropertyEntity({});
    this.DocumentosTransporteDescricao = PropertyEntity({});

    var camposData = [
        "DocaInformada",
        "ChegadaVeiculo",
        "EntregaGuarita",
        "Faturamento",
        "FimCheckList",
        "InicioViagem",
        "LiberacaoChave",
        "TravaChave",
        "Posicao",
        "ChegadaLoja",
        "DeslocamentoPatio",
        "SaidaLoja",
        "FimViagem",
        "InicioHigienizacao",
        "FimHigienizacao",
        "InicioCarregamento",
        "FimCarregamento",
        "SeparacaoMercadoria",
        "SolicitacaoVeiculo",
        "DocumentoFiscal",
        "DocumentosTransporte"
    ];

    for (var i in camposData) {
        this["Data" + camposData[i]] = PropertyEntity({});
        this["Data" + camposData[i] + "Prevista"] = PropertyEntity({});
        this["Data" + camposData[i] + "Reprogramada"] = PropertyEntity({});
        this["Diferenca" + camposData[i]] = PropertyEntity({});
    }
    
    this.Placas = PropertyEntity({ visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ visible: ko.observable(false) });
    this.DataCarga = PropertyEntity({ visible: ko.observable(false) });
    this.Etapas = PropertyEntity({ def: [], val: ko.observableArray([]), ToTime: DecimalToTime });

    this.Remetente.valsubstring = ko.computed(CortarDescricaoRemetente, this);
    this.AlturaFluxo = PropertyEntity({ type: types.local, val: ko.computed(AlturaFluxo, this) });

    BindingComponente(this, data);
}


//*******EVENTOS*******
function BindingComponente(_ko, data) {
    PreencherObjetoKnout(_ko, { Data: data });
}

function AuditarEtapaFluxoGestaoPatio(e) {
    var _fn = OpcaoAuditoria("FluxoGestaoPatio", "Codigo", e);

    _fn({ Codigo: e.Codigo.val() });
}

function SetarEtapaFluxoDesabilitada(etapa) {
    etapa.cssClass("step");
    etapa.eventClick = function () { };
}

function SetarEtapaFluxoAprovada(etapa) {
    etapa.cssClass("step green");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoAprovadaComPendencia(etapa) {
    etapa.cssClass("step cyan");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoAguardando(etapa) {
    etapa.cssClass("step yellow");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoProblema(etapa) {
    etapa.cssClass("step red");
    etapa.eventClick = etapa.backEventClick;

}

function SetarEtapaFluxoLiberada(etapa) {
    etapa.cssClass("step yellow");
    etapa.eventClick = etapa.backEventClick;
}

function _RetornaDataExibicao(realizado, previsto) {
    return function () {
        var data = realizado.val();

        if (data == "") return previsto.val();

        return data;
    }
}

function AlturaFluxo() {
    // Altura minima para que fluxo seja exibido sem quebras
    var alturaMinima = 75;

    // Altura necessária com outros elementos que não os passíveis de serem ocultos
    var alturaComplementar = 27;

    // Cada span, requer xx de altura
    var alturaPorLinha = 17;

    // Quantidade de elementos visíveis
    var qtdLinhasVisiveis = 0;

    if (this.Remetente.visible())
        qtdLinhasVisiveis++;

    if (this.TipoOperacao.visible())
        qtdLinhasVisiveis++;

    if (this.Transportador.visible())
        qtdLinhasVisiveis += 2;

    if (this.Placas.visible())
        qtdLinhasVisiveis++;

    if (this.DataCarga.visible())
        qtdLinhasVisiveis++;

    var alturaFluxo = alturaComplementar + (alturaPorLinha * qtdLinhasVisiveis);

    return (alturaFluxo < alturaMinima ? alturaMinima : alturaFluxo) + "px";
}

function CortarDescricaoRemetente() {
    var remetente = this.Remetente.val();
    var destinatario = this.Destinatario.val();
    var origemdestino = remetente + (destinatario != "" ? " x " : "") + destinatario;
    var cortado = origemdestino.substring(0, 15);

    if (origemdestino.length >= 18)
        cortado += "...";

    return cortado;
}

function DecimalToTime(val) {
    var num = Math.abs(val);
    var horas = Math.floor(num / 60);
    var minutos = (num % 60);

    return (horas > 9 ? horas : '0' + horas) + ':' + (minutos > 9 ? minutos : '0' + minutos);
}
