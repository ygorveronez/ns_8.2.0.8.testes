/*Fluxo.js*/
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
/// <reference path="MontaEtapa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _KnoutsEntregas = new Array();

var Entrega = function (data) {
    var _this = this;
    this.UtilizaAppTrizy = PropertyEntity({ val: ko.observable(data.UtilizaAppTrizy) });
    this.TipoOperacaoPermiteChat = PropertyEntity({ val: ko.observable(data.TipoOperacaoPermiteChat) });
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    //this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.int, eventClick: obterDetalhesCargaControleEntrega, mouseRightClick: cargaRightMouseClick} );
    this.Carga = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Carga.getFieldDescription(), getType: typesKnockout.int, eventClick: cargaRightMouseClick });
    this.NumeroMotorista = PropertyEntity({ getType: typesKnockout.string });
    this.NomeMotorista = PropertyEntity({ getType: typesKnockout.string });
    this.tamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });
    this.NumeroCarregamento = PropertyEntity({ visible: ko.observable(true) });
    this.CargaCancelada = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Backgroundcolor = PropertyEntity({});
    this.DataInicioViagem = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataInicioViagemPrevista = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataInicioViagemReprogramada = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DiferencaInicioViagem = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataPosicao = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataFimViagem = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataFimViagemPrevista = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataFimViagemReprogramada = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DiferencaFimViagem = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataCarga = PropertyEntity({ visible: ko.observable(false) });
    this.Placas = PropertyEntity({ visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ visible: ko.observable(false) });
    this.Tooltip = PropertyEntity({ val: ko.observable("") });
    this.PermiteAdicionarColeta = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PermiteAdicionarReentrega = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PermiteAdicionarEntrega = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PermiteAdicionarPromotor = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PermiteReordenarEntrega = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PermiteDownloadBoletimViagem = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.InformacoesComplementares = PropertyEntity({ def: [], val: ko.observableArray([]) });
    this.KnoutEtapas = PropertyEntity({ def: [], val: ko.observableArray([]) });
    this.KnoutEtapas.width = ko.computed(setarWidthContainerEtapa(this));
    this.IDEquipamento = PropertyEntity({ visible: ko.observable(true) });
    this.CodigoVeiculo = PropertyEntity({ visible: ko.observable(true) });
    this.ImagemMensagem = PropertyEntity({ visible: ko.observable(true), eventClick: mensagemClick, text: Localization.Resources.Cargas.ControleEntrega.ChatComMotoristaDisponivelAtravesDaIntegracaoComOAppTrizy, icon: ko.observable("../../../../Content/TorreControle/Icones/gerais/mensagem-desabilitada.svg"), enable: this.UtilizaAppTrizy.val() === 'true' && this.TipoOperacaoPermiteChat.val() === true });
    this.GridMonitoramento = PropertyEntity({ html: ko.observable("..."), def: [], val: ko.observable([]), id: guid(), visible: ko.observable(false) });
    this.DataReagendamento = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable("") });
    this.CargaCritica = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.CodigoMonitoramento = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.ImagemImprimirMinuta = PropertyEntity({ visible: ko.observable(true), eventClick: imprimirMinutaClick });
    this.OnlineOffline = PropertyEntity({ visible: ko.observable(true)});

    // Analista da carga
    this.AnalistaResponsavelMonitoramento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(null) });
    this.Selecionado = PropertyEntity({ val: ko.observable(false) })
    this.ModoSelecao = PropertyEntity({ val: ko.observable(false) })
    this.CorFundo = PropertyEntity({ type: types.local, val: ko.observable(true) });
    this.PodeSelecionar = ko.computed(function () {
        let jaTemAnalista = this.AnalistaResponsavelMonitoramento.val() !== null && this.AnalistaResponsavelMonitoramento.val() !== "";
        let temPermissaoParaDelegarMonitoramentos = _containerControleEntrega.PermiteDelegarMonitoramento.val();
        return temPermissaoParaDelegarMonitoramentos || !jaTemAnalista;
    }, this);

    _containerControleEntrega.ModoSelecao.val.subscribe((value) => {
        this.ModoSelecao.val(value)
    });

    _containerControleEntrega.CorFundo.val.subscribe((value) => {
        this.CorFundo.val(value)
    });

    this.ImagemOnlineOffline = ko.computed(() => {
        let hover = EnumStatusAcompanhamento.obterDescricao(this.OnlineOffline.val());
        if (this.OnlineOffline.val() == 0)
            return '<div class="no-signal" title="' + hover + '"></div>';

        let icone = ObterIconeStatusTracking(this.OnlineOffline.val(), 20);
        hover = EnumStatusAcompanhamento.obterDescricao(this.OnlineOffline.val());

        return '<div class="mutable" title="' + hover + '">' + icone + '</div>';
    }, this)


    PreencherObjetoKnout(this, { Data: data });
    preencherEtapasFluxoControleEntrega(this, data);
}

function setarWidthContainerEtapa(self) {
    return function () {
        var lengthEtapas = self.KnoutEtapas.val().length;
        return lengthEtapas > 10 ? (lengthEtapas * 220 + 'px') : ''
    };
}

//*******MÉTODOS*******
function preencherEtapasFluxoControleEntrega(fluxoEntrega, dadosEtapa) {
    _KnoutsEntregas.push(fluxoEntrega);
    var knoutEtapas = [];

    if (dadosEtapa.ImagemPreTripFinalizado != "" && dadosEtapa.ImagemPreTripFinalizado != null) {
        objetoEtapa = MontaEtapaPreTripFinalizado(fluxoEntrega, dadosEtapa);
        knoutEtapas.push(objetoEtapa);
    }
    else if (dadosEtapa.ImagemPreTripIniciado != "" && dadosEtapa.ImagemPreTripIniciado != null) {
        objetoEtapa = MontaEtapaPreTripIniciado(fluxoEntrega, dadosEtapa);
        knoutEtapas.push(objetoEtapa);
    }
    else if (dadosEtapa.ImagemPreTripNaoIniciado != "" && dadosEtapa.ImagemPreTripNaoIniciado != null) {
        objetoEtapa = MontaEtapaPreTripNaoIniciado(fluxoEntrega, dadosEtapa);
        knoutEtapas.push(objetoEtapa);
    }

    if (_controleEntregaVisaoPrevisao) {
        objetoEtapa = MontaEtapaPosicao(fluxoEntrega, dadosEtapa);
        knoutEtapas.push(objetoEtapa);
    }

    objetoEtapa = MontaEtapaInicioViagem(fluxoEntrega, dadosEtapa);
    knoutEtapas.push(objetoEtapa);

    objetoEtapa = MontaEtapaAlertasSemEntregaVinculada(dadosEtapa);
    knoutEtapas.push(objetoEtapa);

    dadosEtapa.Entregas.forEach(function (entrega, i) {
        objetoEtapa = MontaEtapaEntregas(fluxoEntrega, entrega)

        knoutEtapas.push(objetoEtapa);
    });

    objetoEtapa = MontaEtapaFimViagem(fluxoEntrega, dadosEtapa);
    knoutEtapas.push(objetoEtapa);

    for (var i = 0; i < knoutEtapas.length; i++)
        validarEtapaFluxoControleEntrega(knoutEtapas[i]);

    if (_controleEntregaVisaoPrevisao)
        tamanhoEtapa(fluxoEntrega, knoutEtapas.length);

    fluxoEntrega.KnoutEtapas.val(knoutEtapas);

    if (_pesquisaControleEntrega) fluxoEntrega.GridMonitoramento.visible(_pesquisaControleEntrega.RetornarInformacoesMonitoramento.val());
    else fluxoEntrega.GridMonitoramento.visible(false);
}

function preencherFluxoControleEntrega(fluxoEntrega, data) {
    PreencherObjetoKnout(fluxoEntrega, { Data: data });
    preencherEtapasFluxoControleEntrega(fluxoEntrega, data);
}

function situacaoEtapaEntrega(etapa) {

    if (etapa.etapaLiberada) {
        if (etapa.situacao == EnumSituacaoEntrega.Entregue) {

            if (etapa.entergaNaJanela == EnumStatusPrazoEntrega.Antecipado)
                setarEtapaFluxoControleEntregaAntecipado(etapa);
            else if (etapa.entergaNaJanela == EnumStatusPrazoEntrega.Atrasado)
                setarEtapaFluxoControleEntregaAtrasado(etapa);
            else
                setarEtapaFluxoControleEntregaAprovada(etapa);
        }
        else if (etapa.situacao == EnumSituacaoEntrega.Rejeitado)
            setarEtapaFluxoControleEntregaRejeitada(etapa);
        else
            setarEtapaFluxoControleEntregaAguardando(etapa);
    }
}

function validarEtapaFluxoControleEntregaPrevisao(etapa) {

    if (etapa.tipoEtapa == "posicao") {
        setarEtapaFluxoControleEntregaAprovada(etapa);
        return;
    }

    if (etapa.tipoEtapa == "entrega") {
        situacaoEtapaEntrega(etapa);
        return;
    }

    if (etapa.dataRealizada != null && etapa.dataRealizada != "") {
        setarEtapaFluxoControleEntregaAprovada(etapa);
        return;
    }

    if (!etapa.etapaLiberada) {
        setarEtapaFluxoControleEntregaDesabilitada(etapa);
        return;
    }

    if (etapa.dataChegada != "") {
        setarEtapaFluxoControleEntregaChegada(etapa);
        return;
    }

    if (etapa.tempoAtraso != "") {
        setarEtapaFluxoControleEntregaAtrasado(etapa);
        return;
    }

    setarEtapaFluxoControleEntregaAguardando(etapa);
}

function validarEtapaFluxoControleEntrega(etapa) {

    if (_controleEntregaVisaoPrevisao)
        validarEtapaFluxoControleEntregaPrevisao(etapa)
    else
        setarFluxoEtapa(etapa);
}

function setarFluxoEtapa(etapa) {
    etapa.eventClick = etapa.backEventClick;
    etapa.eventAlertaClick = etapa.backEventAlertaClick;
}

function setarFluxoTabelaPrevisao(etapa) {
    if (etapa.tipoEtapa == "posicao") {
        setarEtapaFluxoControleEntregaAprovada(etapa);
        return;
    }

    if (etapa.tipoEtapa == "entrega") {
        situacaoEtapaEntrega(etapa);
        return;
    }

    if (etapa.dataRealizada != null && etapa.dataRealizada != "") {
        setarEtapaFluxoControleEntregaAprovada(etapa);
        return;
    }

    if (!etapa.etapaLiberada) {
        setarEtapaFluxoControleEntregaDesabilitada(etapa);
        return;
    }

    if (etapa.dataChegada != "") {
        setarEtapaFluxoControleEntregaChegada(etapa);
        return;
    }

    if (etapa.tempoAtraso != "") {
        setarEtapaFluxoControleEntregaAtrasado(etapa);
        return;
    }

    setarEtapaFluxoControleEntregaAguardando(etapa);

}


function tamanhoEtapa(ko_, quantidade) {

    var width = ((100 / quantidade) + "%").replace(",", ".");
    ko_.tamanhoEtapa.val(width);
}

function setarEtapaFluxoControleEntregaDesabilitada(etapa) {
    etapa.cssClass("step");
    etapa.eventClick = function () { };
}

function setarEtapaFluxoControleEntregaAprovada(etapa) {
    etapa.cssClass("step green");
    etapa.eventClick = etapa.backEventClick;
}

function setarEtapaFluxoControleEntregaChegada(etapa) {
    etapa.cssClass("step blue");
    etapa.eventClick = etapa.backEventClick;
}

function setarEtapaFluxoControleEntregaRejeitada(etapa) {
    etapa.cssClass("step red");
    etapa.eventClick = etapa.backEventClick;
}

function setarEtapaFluxoControleEntregaAguardando(etapa) {
    etapa.cssClass("step yellow");
    etapa.eventClick = etapa.backEventClick;
}

function setarEtapaFluxoControleEntregaAtrasado(etapa) {
    etapa.cssClass("step orange");
    etapa.eventClick = etapa.backEventClick;
}

function setarEtapaFluxoControleEntregaAntecipado(etapa) {
    etapa.cssClass("step cyan");
    etapa.eventClick = etapa.backEventClick;
}

function mostrarImprimirMinuta(carga) {
    for (var i = 0; i < _KnoutsEntregas.length; i++) {
        if (_KnoutsEntregas[i].Carga == carga) {
            _KnoutsEntregas[i].ImagemImprimirMinuta.val("../../../../Content/TorreControle/Icones/alertas/imprimir-minuta.svg");
        }
    }
}