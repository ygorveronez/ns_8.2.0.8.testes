

function MontaEtapaInicioViagem(fluxoEntrega, koutFluxoPatio, infoEtapa) {
    var objEtapa = ObjetoEtapa({
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () {
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
                ExibirDetalhesInicioViagemFluxoEntrega(koutFluxoPatio, objEtapa);
            else 
                ExibirDetalhesSaidaGuaritaFluxoEntrega(koutFluxoPatio, objEtapa);
        },
        tooltip: "Caso tenha integração, nessa etapa é possível acompanhar.",
        text: fluxoEntrega.GuaritaSaidaDescricao || "Início Viagem",
        info: [],
        dataRealizada: fluxoEntrega.DataInicioViagem,
        dataReprogramada: fluxoEntrega.DataInicioViagemReprogramada,
        dataPrevista: fluxoEntrega.DataInicioViagemPrevista,
        tempoAtraso: MinutosEmData(fluxoEntrega.DiferencaInicioViagem),
        privisaoEntergaNaJanela: ""

    });
    
    if (fluxoEntrega.DataInicioViagem != "")
        objEtapa.info.push({
            type: 'info',
            data: fluxoEntrega.DataInicioViagem
        });
    
    return objEtapa;
}

function MontaEtapaPosicao(fluxoEntrega, koutFluxoPatio, infoEtapa) {
    var objEtapa = ObjetoEtapa({
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { ExibirRastreioCarga(koutFluxoPatio); },
        tooltip: "Nessa etapa é possível acompanhar a posição no mapa que a carga se encontra.",
        text: fluxoEntrega.PosicaoDescricao || "Posição",
        info: [],
        dataRealizada: fluxoEntrega.DataPosicao,
        dataReprogramada: "",
        dataPrevista: "",
        tempoAtraso: "",
        privisaoEntergaNaJanela: ""
    });
    
    return objEtapa;
}

function MontaEtapaEntregas(fluxoEntrega, koutFluxoPatio, infoEtapa) {
    var objEtapa = ObjetoEtapa({
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { ExibirDetalhesPedido(koutFluxoPatio, infoEtapa.Pedido); },
        tooltip: "Nessa etapa é possível acompanhar as entrega do pedido.",
        text: infoEtapa.Pedido.Numero || infoEtapa.Pedido.Codigo,
        info: [],
        pedido: infoEtapa.Pedido,
        dataRealizada: infoEtapa.Pedido.DataEntrega,
        dataReprogramada: infoEtapa.Pedido.DataPrevisaoEntregaReprogramada,
        dataPrevista: infoEtapa.Pedido.DataPrevisaoEntrega,
        tempoAtraso: MinutosEmData(infoEtapa.Pedido.DiferencaEntrega),
        privisaoEntergaNaJanela: infoEtapa.Pedido.PrivisaoEntergaNaJanela

    });

    if (infoEtapa.Pedido.Situacao == EnumSituacaoEntregaPedido.Entregue)
        objEtapa.info.push({
            type: 'entregue',
            data: infoEtapa.Pedido.DataEntrega
        });
    else if (infoEtapa.Pedido.Situacao == EnumSituacaoEntregaPedido.Rejeitado)
        objEtapa.info.push({
            type: 'rejeitado',
            data: infoEtapa.Pedido.DataRejeitado
        });
    else 
        objEtapa.info.push({
            type: 'info',
            data: infoEtapa.Pedido.DataPrevisaoEntrega
        });

    return objEtapa;
}

function MontaEtapaFimViagem(fluxoEntrega, koutFluxoPatio, infoEtapa) {
    var objEtapa = ObjetoEtapa({
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { ExibirFimViagem(koutFluxoPatio); },
        tooltip: "Nessa etapa é possível informar quando a viagem chegou ao fim.",
        text: fluxoEntrega.FimViagemDescricao || "Fim da Viagem",
        info: [],
        dataRealizada: fluxoEntrega.DataFimViagem,
        dataReprogramada: fluxoEntrega.DataFimViagemReprogramada,
        dataPrevista: fluxoEntrega.DataFimViagemPrevista,
        tempoAtraso: MinutosEmData(fluxoEntrega.DiferencaFimViagem),
        privisaoEntergaNaJanela: ""
    });
    
    if (fluxoEntrega.DataFimViagem != "")
        objEtapa.info.push({
            type: 'info',
            data: fluxoEntrega.DataFimViagem
        });

    return objEtapa;
}

function MontaEtapa(fluxoEntrega, etapa, koutFluxoPatio) {
    switch (etapa.Etapa) {
        case EnumEtapaFluxoGestaoPatio.InicioViagem:
            return MontaEtapaInicioViagem(fluxoEntrega, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.Posicao:
            return MontaEtapaPosicao(fluxoEntrega, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.Entregas:
            return MontaEtapaEntregas(fluxoEntrega, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.FimViagem:
            return MontaEtapaFimViagem(fluxoEntrega, koutFluxoPatio, etapa);

        default:
            return null;
    }
}

function ObjetoEtapa(obj) {
    return $.extend(true, {
        etapaLiberada: false,
        backEventClick: function () { },
        tooltip: "",
        text: "",
        info: [],
        pedido: null,
        dataRealizada: "",
        dataReprogramada: "",
        dataPrevista: "",
        tempoAtraso: "",
        privisaoEntergaNaJanela: ""
    }, obj);
}

function MinutosEmData(val) {
    if (val == null || val > 0)
        return "";

    var num = Math.abs(val);
    var horas = Math.floor(num / 60);
    var minutos = (num % 60);

    return (horas > 9 ? horas : '0' + horas) + ':' + (minutos > 9 ? minutos : '0' + minutos);
}
