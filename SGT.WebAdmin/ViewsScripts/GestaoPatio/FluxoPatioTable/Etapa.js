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
var Etapa = function (data) {
    var _this = this;

    this.EtapaLiberada = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Descricao = PropertyEntity({ val: ko.observable("") });
    this.Tooltip = PropertyEntity({ val: ko.observable("") });
    this.Etapa = PropertyEntity({ eventClick: function () { }, cssClass: ko.observable("") });
    this.EtapaFluxoGestaoPatio = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Atrasada = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.int, eventClick: ObterDetalhesCargaFluxoClick, OcultarFluxoCarga: _configuracaoGestaoPatio.OcultarFluxoCarga });
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });
    this.NumeroCarregamento = PropertyEntity({});
    this.CargaCancelada = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SituacaoEtapaFluxoGestaoPatio = PropertyEntity({});
    this.EtapaFluxoGestaoPatioAtual = PropertyEntity({});
    this.EtapaAtual = PropertyEntity({});
    
    this.DataRealizada = PropertyEntity({ val: ko.observable("") });
    this.DataReprogramada = PropertyEntity({ val: ko.observable("") });
    this.DataPrevista = PropertyEntity({ val: ko.observable("") });
    this.TempoAtraso = PropertyEntity({ val: ko.observable("") });
    this.Temperatura = PropertyEntity({ val: ko.observable("") });

    this.Placas = PropertyEntity({ visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ visible: ko.observable(false) });
    this.DataCarga = PropertyEntity({ visible: ko.observable(false) });

    this.Etapas = PropertyEntity({ def: [], val: ko.observableArray([]), ToTime: DecimalToTime });
    this.Remetente.valsubstring = ko.computed(CortarDescricaoRemetente, this);
    this.AlturaFluxo = PropertyEntity({ type: types.local, val: ko.computed(AlturaFluxo, this) });

    BindingComponenteEtapa(this, data);
}

//*******EVENTOS*******
function BindingComponenteEtapa(_ko, data) {
    PreencherObjetoKnout(_ko, { Data: data.Etapa });
    
    var objEtapa = MontaEtapa(_ko, data.Fluxo);

    if (objEtapa != null) {
        PreencherObjetoKnout(_ko, { Data: objEtapa });
        _ko.Etapa.backEventClick = objEtapa.backEventClick;
        
        setarSituacaoEtapa(data.Fluxo, data.Etapa, _ko, data.Index);
    }
}

function setarSituacaoEtapa(fluxoCarregamento, etapa, objetoEtapa, etapaFluxo) {
    if (fluxoCarregamento.EtapaAtual.val() > etapaFluxo) {
        if ((etapa.EtapaFluxoGestaoPatio == EnumEtapaFluxoGestaoPatio.Faturamento) && !fluxoCarregamento.FaturamentoFinalizado)
            SetarEtapaFluxoAprovadaComPendencia(objetoEtapa.Etapa);
        else if (objetoEtapa.Atrasada.val())
            SetarEtapaFluxoAprovadaComAtraso(objetoEtapa.Etapa);
        else
            SetarEtapaFluxoAprovada(objetoEtapa.Etapa);
    }
    else if (fluxoCarregamento.EtapaAtual.val() == etapaFluxo) {
        if (!fluxoCarregamento.PossuiCarga.val() && !EnumEtapaFluxoGestaoPatio.isEtapaHabilitadaPreCarga(etapa.EtapaFluxoGestaoPatio))
            SetarEtapaFluxoDesabilitada(objetoEtapa.Etapa);
        else if (fluxoCarregamento.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando)
            SetarEtapaFluxoAguardando(objetoEtapa.Etapa);
        else if (fluxoCarregamento.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado)
            SetarEtapaFluxoProblema(objetoEtapa.Etapa);
        else {
            if (objetoEtapa.Atrasada.val())
                SetarEtapaFluxoAprovadaComAtraso(objetoEtapa.Etapa);
            else
                SetarEtapaFluxoAprovada(objetoEtapa.Etapa);
        }
    }
    else if (objetoEtapa.EtapaLiberada.val())
        SetarEtapaFluxoAguardando(objetoEtapa.Etapa);
    else
        SetarEtapaFluxoDesabilitada(objetoEtapa.Etapa);
}

function MinutosEmData(val) {
    if (val == null || val > 0)
        return "";

    var num = Math.abs(val);
    var horas = Math.floor(num / 60);
    var minutos = (num % 60);

    return (horas > 9 ? horas : '0' + horas) + ':' + (minutos > 9 ? minutos : '0' + minutos);
}
