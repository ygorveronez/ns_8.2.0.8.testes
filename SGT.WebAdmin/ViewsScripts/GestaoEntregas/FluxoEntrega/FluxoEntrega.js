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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEtapaFluxoGestaoEntrega.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var Entrega = function (data) {
    var _this = this;

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.int, eventClick: ObterDetalhesCargaFluxoClick, OcultarFluxoCarga: _configuracaoGestaoEntrega.OcultarFluxoCarga });
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });
    this.NumeroCarregamento = PropertyEntity({});
    this.CargaCancelada = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Situacao = PropertyEntity({});
    this.IndexEtapa = PropertyEntity({});
    this.EtapaAtual = PropertyEntity({});

    this.Auditar = PropertyEntity({ eventClick: AuditarEtapaFluxoGestaoEntrega, visible: ko.observable(PermiteAuditar()) });

    this.DataInicioViagem = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataInicioViagemPrevista = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataInicioViagemReprogramada = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DiferencaInicioViagem = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataPosicao = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataFimViagem = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataFimViagemPrevista = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataFimViagemReprogramada = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DiferencaFimViagem = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });

    

    this.DataEntrega = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });

    this.Placas = PropertyEntity({ visible: ko.observable(true) });
    this.DataCarga = PropertyEntity({ visible: ko.observable(false) });

    this.KnoutEtapas = PropertyEntity({ def: [], val: ko.observableArray([]) });
    this.KnoutEtapas.width = ko.computed(WidthContainerEtapa(this));

    this.Placas = PropertyEntity({ visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ visible: ko.observable(false) });
   
    PreencherObjetoKnout(this, { Data: data });
    PreencherEtapasFluxo(this, data);
}


//*******EVENTOS*******
function AuditarEtapaFluxoGestaoEntrega(e) {
    var _fn = OpcaoAuditoria("FluxoGestaoEntrega", "Codigo", e);

    _fn({ Codigo: e.Codigo.val() });
}

function ObterDetalhesCargaFluxoClick() {

}

function WidthContainerEtapa(self) {
    return function () {
        var lengthEtapas = self.KnoutEtapas.val().length;
        return lengthEtapas > 10 ? (lengthEtapas * 220 + 'px') : ''
    };
}

//*******MÉTODOS*******
function PreencherEtapasFluxo(fluxoEntrega, data) {
    var knoutEtapas = [];
    // Quando fluxo passou ja da etapa de pedido, cada etapa é 
    // validada pela stiuação do pedido, e não pela situação do fluxo
    var validarEtapasPeloPedido = fluxoEntrega.EtapaAtual.val() != EnumEtapaFluxoGestaoPatio.InicioViagem;

    data.Etapas.forEach(function (etapa, i) {
        var objetoEtapa = MontaEtapa(data, etapa, fluxoEntrega);

        if (objetoEtapa == null)
            return;

        objetoEtapa.cssClass = ko.observable("");

        ValidarEtapaFluxo(objetoEtapa, data.IndexEtapa, i, data, validarEtapasPeloPedido);

        knoutEtapas.push(objetoEtapa);
    });

    TamanhoEtapa(fluxoEntrega, knoutEtapas.length);
    fluxoEntrega.KnoutEtapas.val(knoutEtapas);
}

function PreencherFluxoEntrega(fluxoEntrega, data) {
    PreencherObjetoKnout(fluxoEntrega, { Data: data });
    PreencherEtapasFluxo(fluxoEntrega, data);
}

function TamanhoEtapa(ko_, quantidade) {
    //var quantidadeCorrigida = quantidade;
    //if (quantidadeCorrigida > 10 && !isMobile) 
    //    quantidadeCorrigida = 10;
    var width = ((100 / quantidade) + "%").replace(",", ".");
    ko_.TamanhoEtapa.val(width);
}

function ValidarEtapaFluxo(etapa, indexEtapaAtual, indexEtapaFluxo, fluxo, validarEtapasPeloPedido) {
    var situacaoFluxo = fluxo.Situacao;

    if (validarEtapasPeloPedido && etapa.pedido != null)
        SituacaoEtapaPedido(etapa);
    else if (indexEtapaAtual > indexEtapaFluxo) {
        SetarEtapaFluxoAprovada(etapa);
    } else if (indexEtapaAtual == indexEtapaFluxo) {

        if (situacaoFluxo == EnumSituacaoEtapaFluxoGestaoEntrega.Aguardando)
            SetarEtapaFluxoAguardando(etapa);
        else
            SetarEtapaFluxoAprovada(etapa);

    } else if (etapa.etapaLiberada) {
        SetarEtapaFluxoAguardando(etapa);
    } else
        SetarEtapaFluxoDesabilitada(etapa);
}

function SituacaoEtapaPedido(etapa) {
    if (etapa.pedido.Situacao == EnumSituacaoEntregaPedido.Entregue)
        SetarEtapaFluxoAprovada(etapa);
    else if (etapa.pedido.Situacao == EnumSituacaoEntregaPedido.Rejeitado)
        SetarEtapaFluxoRejeitada(etapa);
    else if (!etapa.pedido.PrivisaoEntergaNaJanela)
        SetarEtapaFluxoForaJanela(etapa);
    else
        SetarEtapaFluxoAguardando(etapa);
}

function SetarEtapaFluxoDesabilitada(etapa) {
    etapa.cssClass("step");
    etapa.eventClick = function () { };
}

function SetarEtapaFluxoAprovada(etapa) {
    etapa.cssClass("step green");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoRejeitada(etapa) {
    etapa.cssClass("step red");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoAguardando(etapa) {
    etapa.cssClass("step yellow");
    etapa.eventClick = etapa.backEventClick;
}

function SetarEtapaFluxoForaJanela(etapa) {
    etapa.cssClass("step orange");
    etapa.eventClick = etapa.backEventClick;
}

//function SetarEtapaFluxoLiberada(etapa) {
//    etapa.cssClass("step yellow");
//    etapa.eventClick = etapa.backEventClick;
//}
