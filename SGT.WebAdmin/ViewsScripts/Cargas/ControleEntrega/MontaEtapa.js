/*MontaEtapa.js*/
/// <reference path="InicioViagem.js" />
/// <reference path="FimViagem.js" />

function MontaEtapaInicioViagem(fluxoEntrega, etapa) {
    var descricaoTooltip = Localization.Resources.Cargas.ControleEntrega.ViagemNaoIniciada;

    if (etapa.DataInicioViagem != "")
        descricaoTooltip = Localization.Resources.Cargas.ControleEntrega.ViagemIniciadaEm + " " + etapa.DataInicioViagem;
    else if (etapa.DataPrevisaoInicioViagem)
        descricaoTooltip += ". " + Localization.Resources.Cargas.ControleEntrega.PrevisaoInicioEm + " " + etapa.DataPrevisaoInicioViagem + ".";

    var objEtapa = ObjetoEtapa({
        tipoEtapa: "inicioViagem",
        etapaLiberada: true,
        backEventClick: function () {
            exibirDetalhesInicioViagemControleEntrega(fluxoEntrega, etapa);
        },
        tooltip: descricaoTooltip,
        text: Localization.Resources.Cargas.ControleEntrega.InicioViagem,
        info: [],
        dataChegada: "",
        dataRealizada: etapa.DataInicioViagem,
        dataReprogramada: etapa.DataInicioViagemReprogramada,
        dataPrevista: etapa.DataInicioViagemPrevista,
        tempoAtraso: MinutosEmData(etapa.DiferencaInicioViagem),
        previsaoEntergaNaJanela: "",
        entergaNaJanela: "",
        imagem: etapa.ImagemInicioViagem,
        imagemForaSequencia: "",
        imagemForaRaio: etapa.ImagemInicioDeViagemForaRaio,
        imagemPedidoEmMaisCargas: "",
        imagemParcial: "",
        imagemSemCoordenada: "",
        imagemPedidoReentrega: "",
        imagemEntregaFinalizadaViaFinalizacaoMonitoramento: etapa.ImagemViagemIniciadaViaFinalizacaoMonitoramento,
        indicadorComplementar: "",
        class: ""
    });

    if (fluxoEntrega.DataInicioViagem != "")
        objEtapa.info.push({
            type: 'info',
            data: fluxoEntrega.DataInicioViagem
        });

    objEtapa.cssClass = ko.observable("");
    return objEtapa;
}

function MontaEtapaInicioViagem(fluxoEntrega, etapa) {
    var descricaoTooltip = Localization.Resources.Cargas.ControleEntrega.ViagemNaoIniciada;

    if (etapa.DataInicioViagem != "")
        descricaoTooltip = Localization.Resources.Cargas.ControleEntrega.ViagemIniciadaEm + " " + etapa.DataInicioViagem;
    else if (etapa.DataPrevisaoInicioViagem)
        descricaoTooltip += ". " + Localization.Resources.Cargas.ControleEntrega.PrevisaoInicioEm + " " + etapa.DataPrevisaoInicioViagem + ".";

    var objEtapa = ObjetoEtapa({
        tipoEtapa: "inicioViagem",
        etapaLiberada: true,
        backEventClick: function () {
            exibirDetalhesInicioViagemControleEntrega(fluxoEntrega, etapa);
        },
        tooltip: descricaoTooltip,
        text: Localization.Resources.Cargas.ControleEntrega.InicioViagem,
        info: [],
        dataChegada: "",
        dataRealizada: etapa.DataInicioViagem,
        dataReprogramada: etapa.DataInicioViagemReprogramada,
        dataPrevista: etapa.DataInicioViagemPrevista,
        tempoAtraso: MinutosEmData(etapa.DiferencaInicioViagem),
        previsaoEntergaNaJanela: "",
        entergaNaJanela: "",
        imagem: etapa.ImagemInicioViagem,
        imagemForaSequencia: "",
        imagemForaRaio: etapa.ImagemInicioDeViagemForaRaio,
        imagemPedidoEmMaisCargas: "",
        imagemParcial: "",
        imagemSemCoordenada: "",
        imagemPedidoReentrega: "",
        imagemEntregaFinalizadaViaFinalizacaoMonitoramento: etapa.ImagemViagemIniciadaViaFinalizacaoMonitoramento,
        indicadorComplementar: "",
        class: ""
    });

    if (fluxoEntrega.DataInicioViagem != "")
        objEtapa.info.push({
            type: 'info',
            data: fluxoEntrega.DataInicioViagem
        });

    objEtapa.cssClass = ko.observable("");
    return objEtapa;
}

function MontaEtapaPreTripNaoIniciado(fluxoEntrega, etapa) {
    var descricaoTooltip = "Pré Trip não iniciado ";

    var objEtapa = ObjetoEtapa({
        tipoEtapa: "preTripNaoIniciado",
        etapaLiberada: true,
        backEventClick: function () {
            informarInicioPreTrip(fluxoEntrega, etapa);
        },
        tooltip: descricaoTooltip,
        text: Localization.Resources.Cargas.ControleEntrega.InicioViagem,
        info: [],
        dataChegada: "",
        dataRealizada: etapa.DataInicioViagem,
        dataReprogramada: etapa.DataInicioViagemReprogramada,
        dataPrevista: etapa.DataInicioViagemPrevista,
        tempoAtraso: "",
        previsaoEntergaNaJanela: "",
        entergaNaJanela: "",
        imagem: etapa.ImagemPreTripNaoIniciado,
        imagemForaSequencia: "",
        imagemForaRaio: "",
        imagemPedidoEmMaisCargas: "",
        imagemParcial: "",
        imagemSemCoordenada: "",
        imagemPedidoReentrega: "",
        imagemEntregaFinalizadaViaFinalizacaoMonitoramento: "",
        indicadorComplementar: "",
        class: ""
    });


    objEtapa.cssClass = ko.observable("");
    return objEtapa;
}
function MontaEtapaPreTripIniciado(fluxoEntrega, etapa) {
    var descricaoTooltip = "Pré Trip iniciado " + etapa.DataPreViagemInicio;

    var objEtapa = ObjetoEtapa({
        tipoEtapa: "preTripIniciado",
        etapaLiberada: true,
        backEventClick: function () {
            informarFimPreTrip(fluxoEntrega, etapa);
        },
        tooltip: descricaoTooltip,
        text: Localization.Resources.Cargas.ControleEntrega.InicioViagem,
        info: [],
        dataChegada: "",
        dataRealizada: etapa.DataInicioViagem,
        dataReprogramada: etapa.DataInicioViagemReprogramada,
        dataPrevista: etapa.DataInicioViagemPrevista,
        tempoAtraso: MinutosEmData(etapa.DiferencaInicioViagem),
        previsaoEntergaNaJanela: "",
        entergaNaJanela: "",
        imagem: etapa.ImagemPreTripIniciado,
        imagemForaSequencia: "",
        imagemForaRaio: "",
        imagemPedidoEmMaisCargas: "",
        imagemParcial: "",
        imagemSemCoordenada: "",
        imagemPedidoReentrega: "",
        imagemEntregaFinalizadaViaFinalizacaoMonitoramento: "",
        indicadorComplementar: "",
        class: ""
    });


    objEtapa.cssClass = ko.observable("");
    return objEtapa;
}

function MontaEtapaPreTripFinalizado(fluxoEntrega, etapa) {
    var descricaoTooltip = "Pré Trip finalizado " + etapa.DataPreViagemFim;

    var objEtapa = ObjetoEtapa({
        tipoEtapa: "preTripFinalizado",
        etapaLiberada: true,
        tooltip: descricaoTooltip,
        text: Localization.Resources.Cargas.ControleEntrega.InicioViagem,
        info: [],
        dataChegada: "",
        dataRealizada: etapa.DataInicioViagem,
        dataReprogramada: etapa.DataInicioViagemReprogramada,
        dataPrevista: etapa.DataInicioViagemPrevista,
        tempoAtraso: MinutosEmData(etapa.DiferencaInicioViagem),
        previsaoEntergaNaJanela: "",
        entergaNaJanela: "",
        imagem: etapa.ImagemPreTripFinalizado,
        imagemForaSequencia: "",
        imagemForaRaio: "",
        imagemPedidoEmMaisCargas: "",
        imagemParcial: "",
        imagemSemCoordenada: "",
        imagemPedidoReentrega: "",
        imagemEntregaFinalizadaViaFinalizacaoMonitoramento: "",
        indicadorComplementar: "",
        class: ""
    });


    objEtapa.cssClass = ko.observable("");
    return objEtapa;
}

function MontaEtapaPosicao(fluxoEntrega, etapa) {
    var objEtapa = ObjetoEtapa({
        tipoEtapa: "posicao",
        etapaLiberada: true,
        backEventClick: function () { exibirPosicaoControleEntrega(etapa); },
        tooltip: Localization.Resources.Cargas.ControleEntrega.NessaEtapaPossivelAcompanharPosicaoNoMapaEmQueCargaSeEncontra,
        text: etapa.PosicaoDescricao || Localization.Resources.Cargas.ControleEntrega.Posicao,
        info: [],
        dataChegada: "",
        dataRealizada: etapa.DataPosicao,
        dataReprogramada: "",
        dataPrevista: "",
        tempoAtraso: "",
        previsaoEntergaNaJanela: "",
        entergaNaJanela: "",
        imagemForaSequencia: "",
        imagemForaRaio: "",
        imagemPedidoEmMaisCargas: "",
        imagemParcial: "",
        imagemSemCoordenada: "",
        imagemEntregaFinalizadaViaFinalizacaoMonitoramento: "",
        class: ""
    });

    objEtapa.cssClass = ko.observable("");
    return objEtapa;
}

function obterTooltipEntrega(entrega) {
    descricaoTooltip = Localization.Resources.Cargas.ControleEntrega.Cliente.getFieldDescription() + " " + entrega.Pessoa;

    if (entrega.DataEntrega != "") {
        descricaoTooltip = descricaoTooltip + "<br>" + (entrega.Coleta ? Localization.Resources.Cargas.ControleEntrega.DataColeta.getFieldDescription() : Localization.Resources.Cargas.ControleEntrega.DataRealizada.getFieldDescription()) + " " + (entrega.Coleta ? entrega.DataChegada : entrega.DataEntrega);
        descricaoTooltip = descricaoTooltip + "<br>" + Localization.Resources.Cargas.ControleEntrega.NoPrazo.getFieldDescription() + " " + entrega.DescricaoEntregaNoPrazo;
    }
    else {
        if (entrega.DataPrevista != "")
            descricaoTooltip = descricaoTooltip + "<br>" + (entrega.Coleta ? Localization.Resources.Cargas.ControleEntrega.DataPrevisaoDaColeta.getFieldDescription() : Localization.Resources.Cargas.ControleEntrega.DataPrevisaoDaEntrega.getFieldDescription()) + " " + entrega.DataPrevista;
        if (entrega.DataReprogramada != "" && entrega.DataReprogramada != entrega.DataPrevista)
            descricaoTooltip = descricaoTooltip + "<br>" + (entrega.Coleta ? Localization.Resources.Cargas.ControleEntrega.DataDaColetaReprogramada.getFieldDescription() : Localization.Resources.Cargas.ControleEntrega.DataDaEntregaReprogramada.getFieldDescription()) + " " + entrega.DataReprogramada;
        if (entrega.DataAgendamento != "")
            descricaoTooltip = descricaoTooltip + "<br>" + (entrega.Coleta ? Localization.Resources.Cargas.ControleEntrega.DataAgendamentoDeColeta.getFieldDescription() : Localization.Resources.Cargas.ControleEntrega.DataAgendamentoDeEntrega.getFieldDescription()) + " " + entrega.DataAgendamento;

        descricaoTooltip = descricaoTooltip + "<br>" + (entrega.Coleta ? Localization.Resources.Cargas.ControleEntrega.TendenciaColeta.getFieldDescription() : Localization.Resources.Cargas.ControleEntrega.TendenciaEntrega.getFieldDescription()) + " " + entrega.Tendencia;
    }

    if (entrega.NumeroCTe != undefined && entrega.NumeroCTe != null && entrega.NumeroCTe != "")
        descricaoTooltip += "<br>" + Localization.Resources.Cargas.ControleEntrega.Cte.getFieldDescription() + " " + entrega.NumeroCTe + "<br>";

    if (entrega.ResponsavelChamado)
        descricaoTooltip += "<br>" + Localization.Resources.Cargas.ControleEntrega.Responsavel.getFieldDescription() + " " + entrega.ResponsavelChamado + "<br>";

    if (entrega.ExigirInformarNumeroPacotesNaColetaTrizy)
        descricaoTooltip += "<br>" + Localization.Resources.Cargas.ControleEntrega.PacotesColetados.getFieldDescription() + " " + entrega.QuantidadePacotesColetados + "<br>";

    if (entrega.ExibirDataPrevisaoEntregaTransportador)
        descricaoTooltip += "<br>" + Localization.Resources.Cargas.ControleEntrega.DataPrevisaoEntregaTransportador.getFieldDescription() + " " + entrega.DataPrevisaoEntregaTransportador + "<br>";

    return descricaoTooltip;
}

function MontaEtapaEntregas(fluxoEntrega, entrega) {

    let shouldBlink = entrega.ChamadoEmAberto || entrega.Imagem.includes("entrou-e-saiu-sem-entregar") || entrega.MenosDaMetadeDoTempoParaResolverChamado || entrega.MotoristaChegou || entrega.ChamadoEntregaMesmaCarga;

    var objEtapa = ObjetoEtapa({
        tipoEtapa: "entrega",
        etapaLiberada: fluxoEntrega.DataInicioViagem.val() != "",
        backEventClick: function () { exibirDetalhesEntrega(fluxoEntrega, entrega); },
        backEventAlertaClick: function (alerta) { exibirDetalhesAlerta(alerta, entrega.Alertas); },
        tooltip: obterTooltipEntrega(entrega),
        text: entrega.Descricao,
        responsavelChamado: ObterIniciaisNome(entrega.ResponsavelChamado),
        exibirInicialResponsavel: entrega.ResponsavelChamado != "" && entrega.ChamadoEmAberto,
        info: [],
        dataChegada: entrega.DataChegada,
        dataRealizada: entrega.DataRealizada,
        dataReprogramada: entrega.DataReprogramada,
        dataPrevista: entrega.DataPrevista,
        tempoAtraso: MinutosEmData(entrega.DiferencaEntrega),
        situacao: entrega.Situacao,
        previsaoEntergaNaJanela: entrega.PrevisaoEntergaNaJanela,
        entergaNaJanela: entrega.EntergaNaJanela,
        imagem: entrega.Imagem,
        imagemForaSequencia: entrega.ImagemForaSequencia,
        imagemForaRaio: entrega.ImagemForaRaio,
        imagemParcial: entrega.ImagemParcial,
        imagemSemCoordenada: entrega.ImagemSemCoordenada,
        indicadorComplementar: entrega.IndicadorComplementar,
        alertas: ObterAlertasAgrupados(entrega.Alertas),
        destacarFiltrosConsultados: entrega.DestacarFiltrosConsultados,
        class: shouldBlink ? "blink" : "",
    });

    objEtapa.cssClass = ko.observable("");

    return objEtapa;
}

function MontaEtapaAlertasSemEntregaVinculada(etapa) {

    var objEtapa = ObjetoEtapa({
        tipoEtapa: "alertas",
        backEventAlertaClick: function (alerta) { exibirDetalhesAlerta(alerta, etapa.AlertasSemEntregaVinculada); },
        alertas: ObterAlertasAgrupados(etapa.AlertasSemEntregaVinculada),
        mostrarEtapa: false
    });

    objEtapa.cssClass = ko.observable("");

    return objEtapa;
}

function MontaEtapaFimViagem(fluxoEntrega, etapa) {

    var descricaoTooltip = Localization.Resources.Cargas.ControleEntrega.ViagemNaoFinalizada;
    if (etapa.DataFimViagem != "")
        descricaoTooltip = Localization.Resources.Cargas.ControleEntrega.ViagemFinalizadaEm + " " + etapa.DataFimViagem;

    var tempoatraso = MinutosEmData(etapa.DiferencaFimViagem);

    descricaoTempoAtraso = "";

    if (etapa.DiferencaFimViagem) {

        descricaoTempoAtraso = Localization.Resources.Cargas.ControleEntrega.NoPrazo;

        if (tempoatraso != "")
            descricaoTempoAtraso = Localization.Resources.Cargas.ControleEntrega.Atraso.getFieldDescription() + " " + tempoatraso
    }

    var objEtapa = ObjetoEtapa({
        tipoEtapa: "fimViagem",
        etapaLiberada: fluxoEntrega.DataInicioViagem.val() != "",
        backEventClick: function () { exibirFimViagem(fluxoEntrega, etapa); },
        tooltip: descricaoTooltip,
        text: etapa.FimViagemDescricao || Localization.Resources.Cargas.ControleEntrega.FimDaViagem,
        info: [],
        dataChegada: "",
        dataRealizada: etapa.DataFimViagem,
        dataReprogramada: etapa.DataFimViagemReprogramada,
        dataPrevista: etapa.DataFimViagemPrevista,
        tempoAtraso: descricaoTempoAtraso,
        previsaoEntergaNaJanela: "",
        imagem: etapa.ImagemFimDeViagem,
        imagemForaSequencia: "",
        imagemForaRaio: etapa.ImagemFimDeViagemForaRaio,
        imagemPedidoEmMaisCargas: "",
        imagemParcial: "",
        imagemSemCoordenada: "",
        imagemPedidoReentrega: "",
        imagemEntregaFinalizadaViaFinalizacaoMonitoramento: etapa.ImagemViagemFinalizadaViaFinalizacaoMonitoramento,
        indicadorComplementar: "",
        class: ""
    });

    if (fluxoEntrega.DataFimViagem != "")
        objEtapa.info.push({
            type: 'info',
            data: fluxoEntrega.DataFimViagem
        });

    objEtapa.cssClass = ko.observable("");
    return objEtapa;
}

function ObjetoEtapa(obj) {
    return $.extend(true, {
        etapaLiberada: false,
        backEventClick: function () { },
        tooltip: "",
        text: "",
        exibirInicialResponsavel: false,
        info: [],
        situacao: null,
        dataRealizada: "",
        dataReprogramada: "",
        imagemNotaCobertura: "",
        dataPrevista: "",
        tempoAtraso: "",
        previsaoEntergaNaJanela: "",
        mostrarEtapa: true,
    }, obj);
}

function MinutosEmData(val) {
    if (val == null || val >= 0)
        return "";

    var num = Math.abs(val);
    var horas = Math.floor(num / 60);
    var minutos = (num % 60);

    return (horas > 9 ? horas : '0' + horas) + ':' + (minutos > 9 ? minutos : '0' + minutos);
}

function ObterIniciaisNome(nome) {
    var cache = window.iniciaisNome || {};

    if (nome in cache) {
        return cache[nome];
    }

    var nomeSeparado = (nome || "").split(" ");
    var primeiroNome = nomeSeparado.shift();
    var sobrenome = nomeSeparado.reverse().shift();
    var letrasIniciais = [];

    if (primeiroNome) letrasIniciais.push(primeiroNome[0]);
    if (sobrenome) letrasIniciais.push(sobrenome[0]);

    var iniciais = letrasIniciais.join("").toUpperCase();
    cache[nome] = iniciais;
    window.iniciaisNome = cache;

    return iniciais;
}

function ObterAlertasAgrupados(alertas) {
    var listaAlertasAgrupados = [];
    var primeiroAlertaTiposAlerta = [];
    var tiposAlertaComPrimeiroAlerta = [];

    listaAlertasAgrupados = Object.groupBy(alertas, (x) => x.TipoAlerta);

    var keys = Object.keys(listaAlertasAgrupados);

    for (var i = 0; i < keys.length; i++) {
        var key = keys[i];
        var alertasPorTipo = listaAlertasAgrupados[key];

        // Encontrar o primeiro alerta com a situação em aberto
        var alertaEmAberto = alertasPorTipo.find(alerta => alerta.Status === 0);

        // Se não encontrar, pegar o primeiro alerta
        var primeiroAlerta = alertaEmAberto || alertasPorTipo[0];
        var numeroDeAlertasPorGrupo = alertasPorTipo.length;

        primeiroAlerta.numeroDeAlertasPorGrupo = numeroDeAlertasPorGrupo > 99 ? "99+" : String(numeroDeAlertasPorGrupo);

        tiposAlertaComPrimeiroAlerta.push(primeiroAlerta);
    }

    return tiposAlertaComPrimeiroAlerta;
}
