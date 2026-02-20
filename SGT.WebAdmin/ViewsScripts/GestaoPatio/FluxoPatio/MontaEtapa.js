/// <reference path="Checklist.js" />
/// <reference path="ChegadaSaidaLoja.js" />
/// <reference path="DeslocamentoPatio.js" />
/// <reference path="DocaCarregamento.js" />
/// <reference path="DocumentoFiscal.js" />
/// <reference path="DocumentosTransporte.js" />
/// <reference path="Expedicao.js" />
/// <reference path="FimCarregamento.js" />
/// <reference path="FimDescarregamento.js" />
/// <reference path="FimHigienizacao.js" />
/// <reference path="FimViagem.js" />
/// <reference path="FluxoPatio.js" />
/// <reference path="Guarita.js" />
/// <reference path="InicioCarregamento.js" />
/// <reference path="InicioDescarregamento.js" />
/// <reference path="InicioHigienizacao.js" />
/// <reference path="MontagemCargaPatio.js" />
/// <reference path="Posicao.js" />
/// <reference path="SeparacaoMercadoria.js" />
/// <reference path="SolicitacaoVeiculo.js" />
/// <reference path="TravamentoChave.js" />
/// <reference path="Faturamento.js" />

function MontaEtapaCheckList(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesCheckListFluxoPatio, koutFluxoPatio, objEtapa, infoEtapa); },
        eventClickDesbloqueada: function () { ExibirDetalhesCheckListFluxoPatio(koutFluxoPatio, objEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.ControleDoChecklistDosVeiculos,
        text: fluxoCarregamento.CheckListDescricao || Localization.Resources.GestaoPatio.FluxoPatio.Checklist,
        permanenciaMaximaExcedida: fluxoCarregamento.FimCheckListLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataFimCheckListPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataFimCheckListPrevista
        });
    if (fluxoCarregamento.DataFimCheckList != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataFimCheckList
        });

    if (fluxoCarregamento.DiferencaFimCheckList != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaFimCheckList
        });

    return objEtapa;
}

function MontaEtapaInformarDoca(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesDocaCarregamentoFluxoPatio, koutFluxoPatio, objEtapa, infoEtapa); },
        eventClickDesbloqueada: function () { ExibirDetalhesDocaCarregamentoFluxoPatio(koutFluxoPatio, objEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.IndicaDocaParaCarregamento,
        text: fluxoCarregamento.InformarDocaCarregamentoDescricao || Localization.Resources.GestaoPatio.FluxoPatio.Doca,
        permanenciaMaximaExcedida: fluxoCarregamento.DocaInformadaLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataDocaInformadaPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataDocaInformadaPrevista
        });
    if (fluxoCarregamento.DataDocaInformada != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataDocaInformada
        });

    if (fluxoCarregamento.DiferencaDocaInformada != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaDocaInformada
        });

    return objEtapa;
}

function MontaEtapaGuarita(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesEntradaGuaritaFluxoPatio, koutFluxoPatio, objEtapa, infoEtapa); },
        eventClickDesbloqueada: function () { ExibirDetalhesEntradaGuaritaFluxoPatio(koutFluxoPatio, objEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.ControleDeEntradaDeVeiculos,
        text: fluxoCarregamento.GuaritaEntradaDescricao || Localization.Resources.GestaoPatio.FluxoPatio.ChegadaPortaria,
        permanenciaMaximaExcedida: fluxoCarregamento.EntregaGuaritaLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataEntregaGuaritaPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataEntregaGuaritaPrevista
        });
    if (fluxoCarregamento.DataEntregaGuarita != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataEntregaGuarita
        });

    if (fluxoCarregamento.DiferencaEntregaGuarita != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaEntregaGuarita
        });

    return objEtapa;
}

function MontaEtapaTravamentoChave(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesTravaChaveFluxoPatio, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.ControleDeTravaDeChaveDosVeiculos,
        text: fluxoCarregamento.TravaChaveDescricao || Localization.Resources.GestaoPatio.FluxoPatio.TravamentoDaChave,
        permanenciaMaximaExcedida: fluxoCarregamento.TravaChaveLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataTravaChavePrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataTravaChavePrevista
        });
    if (fluxoCarregamento.DataTravaChave != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataTravaChave
        });

    if (fluxoCarregamento.DiferencaTravaChave != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaTravaChave
        });

    return objEtapa;
}

function MontaEtapaExpedicao(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesExpedicaoFluxoPatio, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.OndeControladaExpedicaoDosVeiculos,
        text: fluxoCarregamento.ExpedicaoDescricao || Localization.Resources.GestaoPatio.FluxoPatio.Expedicao,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataFimCarregamento != "")
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataFimCarregamento
        });
    else if (fluxoCarregamento.DataInicioCarregamento != "") {
        if (fluxoCarregamento.DataInicioCarregamentoPrevista != "")
            objEtapa.info.push({
                type: 'previsao',
                data: fluxoCarregamento.DataInicioCarregamentoPrevista
            });

        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataInicioCarregamento
        });
    }

    if (fluxoCarregamento.DiferencaFimCarregamento != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaFimCarregamento
        });
    else if (fluxoCarregamento.DiferencaInicioCarregamento != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaInicioCarregamento
        });

    return objEtapa;
}

function MontaEtapaLiberacaoChave(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesLiberaChaveFluxoPatio, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.ControleDeLiberacaoDaTravaDeChaveDosVeiculos,
        text: fluxoCarregamento.LiberaChaveDescricao || Localization.Resources.GestaoPatio.FluxoPatio.LiberacaoDaChave,
        permanenciaMaximaExcedida: fluxoCarregamento.LiberacaoChaveLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataLiberacaoChavePrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataLiberacaoChavePrevista
        });
    if (fluxoCarregamento.DataLiberacaoChave != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataLiberacaoChave
        });

    if (fluxoCarregamento.DiferencaLiberacaoChave != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaLiberacaoChave
        });

    return objEtapa;
}

function MontaEtapaFaturamento(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesFaturamento, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.IndicaFaturamentoDaCarga,
        text: fluxoCarregamento.FaturamentoDescricao || Localization.Resources.GestaoPatio.FluxoPatio.Faturamento,
        permanenciaMaximaExcedida: fluxoCarregamento.FaturamentoLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataFaturamentoPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataFaturamentoPrevista
        });
    if (fluxoCarregamento.DataFaturamento != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataFaturamento
        });

    if (fluxoCarregamento.DiferencaFaturamento != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaFaturamento
        });

    return objEtapa;
}

function MontaEtapaChegadaVeiculo(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesChegadaVeiculoFluxoPatio, koutFluxoPatio, objEtapa, infoEtapa); },
        eventClickDesbloqueada: function () { ExibirDetalhesChegadaVeiculoFluxoPatio(koutFluxoPatio, objEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.InformaChegadaDoVeiculo,
        text: fluxoCarregamento.ChegadaVeiculoDescricao || Localization.Resources.GestaoPatio.FluxoPatio.ChegadaVeiculo,
        permanenciaMaximaExcedida: fluxoCarregamento.ChegadaVeiculoLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataChegadaVeiculoPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataChegadaVeiculoPrevista
        });
    if (fluxoCarregamento.DataChegadaVeiculo != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataChegadaVeiculo
        });

    if (fluxoCarregamento.DiferencaChegadaVeiculo != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaChegadaVeiculo
        });

    return objEtapa;
}

function MontaEtapaInicioCarregamento(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesInicioCarregamento, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.InicioDoCarregamento,
        text: fluxoCarregamento.InicioCarregamentoDescricao || Localization.Resources.GestaoPatio.FluxoPatio.InicioDoCarregamento,
        permanenciaMaximaExcedida: fluxoCarregamento.InicioCarregamentoLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataInicioCarregamentoPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataInicioCarregamentoPrevista
        });

    if (fluxoCarregamento.DataInicioCarregamento != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataInicioCarregamento
        });

    if (fluxoCarregamento.DiferencaInicioCarregamento != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaInicioCarregamento
        });

    return objEtapa;
}

function MontaEtapaInicioDescarregamento(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesInicioDescarregamento, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.InicioDoDescarregamento,
        text: fluxoCarregamento.InicioDescarregamentoDescricao || Localization.Resources.GestaoPatio.FluxoPatio.InicioDoDescarregamento,
        permanenciaMaximaExcedida: fluxoCarregamento.InicioDescarregamentoLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataInicioDescarregamentoPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataInicioDescarregamentoPrevista
        });

    if (fluxoCarregamento.DataInicioDescarregamento != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataInicioDescarregamento
        });

    if (fluxoCarregamento.DiferencaInicioDescarregamento != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaInicioDescarregamento
        });

    return objEtapa;
}

function MontaEtapaInicioHigienizacao(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesInicioHigienizacao, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.InicioDaHigienizacao,
        text: fluxoCarregamento.InicioHigienizacaoDescricao || Localization.Resources.GestaoPatio.FluxoPatio.InicioDaHigienizacao,
        permanenciaMaximaExcedida: fluxoCarregamento.InicioHigienizacaoLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataInicioHigienizacaoPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataInicioHigienizacaoPrevista
        });

    if (fluxoCarregamento.DataInicioHigienizacao != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataInicioHigienizacao
        });

    if (fluxoCarregamento.DiferencaInicioHigienizacao != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaInicioHigienizacao
        });

    return objEtapa;
}

function MontaEtapaInicioViagem(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesSaidaGuaritaFluxoPatio, koutFluxoPatio, objEtapa, infoEtapa); },
        eventClickDesbloqueada: function () { ExibirDetalhesPesagemFinalFluxoPatio(koutFluxoPatio, objEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.CasoTenhaIntegracaoNessaEtapaPossivelAcompanhar,
        text: fluxoCarregamento.GuaritaSaidaDescricao || Localization.Resources.GestaoPatio.FluxoPatio.InicioViagem,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataInicioViagemPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataInicioViagemPrevista
        });
    if (fluxoCarregamento.DataInicioViagem != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataInicioViagem
        });

    if (fluxoCarregamento.DiferencaInicioViagem != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaInicioViagem
        });

    return objEtapa;
}

function MontaEtapaPosicao(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirRastreioCarga, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.NessaEtapaPossivelAcompanharPosicaoNoMapaQueCargaSeEncontra,
        text: fluxoCarregamento.PosicaoDescricao || Localization.Resources.GestaoPatio.FluxoPatio.Posicao,
        permanenciaMaximaExcedida: fluxoCarregamento.PosicaoLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    //if (fluxoCarregamento.DataPosicao != "")
    //    objEtapa.info.push({
    //        type: 'info',
    //        data: fluxoCarregamento.DataPosicao
    //    });
    //if (fluxoCarregamento.DataPosicaoPrevista != "")
    //    objEtapa.info.push({
    //        type: 'previsao',
    //        data: fluxoCarregamento.DataPosicaoPrevista
    //    });

    //if (fluxoCarregamento.DiferencaPosicao != null)
    //    objEtapa.info.push({
    //        type: 'time-diff',
    //        data: fluxoCarregamento.DiferencaPosicao
    //    });

    return objEtapa;
}

function MontaEtapaChegadaLoja(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirChegadaVeiculoLoja, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.NessaEtapaPossivelInformarQuandoVeiculoChegouAoDestinatario,
        text: fluxoCarregamento.ChegadaLojaDescricao || Localization.Resources.GestaoPatio.FluxoPatio.ChegadaNoDestinatario,
        permanenciaMaximaExcedida: fluxoCarregamento.ChegadaLojaLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataChegadaLojaPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataChegadaLojaPrevista
        });
    if (fluxoCarregamento.DataChegadaLoja != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataChegadaLoja
        });

    if (fluxoCarregamento.DiferencaChegadaLoja != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaChegadaLoja
        });

    return objEtapa;
}

function MontaEtapaDeslocamentoPatio(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDeslocamentoPatio, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.NessaEtapaPossivelAveriguarQuandoVeiculoEstaSeDeslocandoDentroDoPatio,
        text: fluxoCarregamento.DeslocamentoPatioDescricao || Localization.Resources.GestaoPatio.FluxoPatio.DeslocamentoPatio,
        permanenciaMaximaExcedida: fluxoCarregamento.DeslocamentoPatioLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataDeslocamentoPatioPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataDeslocamentoPatioPrevista
        });
    if (fluxoCarregamento.DataDeslocamentoPatio != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataDeslocamentoPatio
        });

    if (fluxoCarregamento.DiferencaDeslocamentoPatio != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaDeslocamentoPatio
        });

    return objEtapa;
}

function MontaEtapaDocumentoFiscal(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesDocumentoFiscal, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.DocumentoFiscal,
        text: fluxoCarregamento.DocumentoFiscalDescricao || Localization.Resources.GestaoPatio.FluxoPatio.DocumentoFiscal,
        permanenciaMaximaExcedida: fluxoCarregamento.DocumentoFiscalLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataDocumentoFiscalPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataDocumentoFiscalPrevista
        });

    if (fluxoCarregamento.DataDocumentoFiscal != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataDocumentoFiscal
        });

    if (fluxoCarregamento.DiferencaDocumentoFiscal != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaDocumentoFiscal
        });

    return objEtapa;
}

function MontaEtapaDocumentosTransporte(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesDocumentosTransporte, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.DocumentosDeTransporte,
        text: fluxoCarregamento.DocumentosTransporteDescricao || Localization.Resources.GestaoPatio.FluxoPatio.DocumentosDeTransporte,
        permanenciaMaximaExcedida: fluxoCarregamento.DocumentosTransporteLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataDocumentosTransportePrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataDocumentosTransportePrevista
        });

    if (fluxoCarregamento.DataDocumentosTransporte != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataDocumentosTransporte
        });

    if (fluxoCarregamento.DiferencaDocumentosTransporte != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaDocumentosTransporte
        });

    return objEtapa;
}

function MontaEtapaSaidaLoja(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirSaidaVeiculoLoja, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.NessaEtapaPossivelInformarQuandoVeiculoSaiuDoDestinatario,
        text: fluxoCarregamento.SaidaLojaDescricao || Localization.Resources.GestaoPatio.FluxoPatio.SaidaDoDestinatario,
        permanenciaMaximaExcedida: fluxoCarregamento.SaidaLojaLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataSaidaLojaPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataSaidaLojaPrevista
        });
    if (fluxoCarregamento.DataSaidaLoja != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataSaidaLoja
        });

    if (fluxoCarregamento.DiferencaSaidaLoja != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaSaidaLoja
        });

    return objEtapa;
}

function MontaEtapaFimCarregamento(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesFimCarregamento, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.FimDoCarregamento,
        text: fluxoCarregamento.FimCarregamentoDescricao || Localization.Resources.GestaoPatio.FluxoPatio.FimDoCarregamento,
        permanenciaMaximaExcedida: fluxoCarregamento.FimCarregamentoLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataFimCarregamentoPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataFimCarregamentoPrevista
        });

    if (fluxoCarregamento.DataFimCarregamento != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataFimCarregamento
        });

    if (fluxoCarregamento.DiferencaFimCarregamento != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaFimCarregamento
        });

    return objEtapa;
}

function MontaEtapaFimDescarregamento(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesFimDescarregamento, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.FimDoDescarregamento,
        text: fluxoCarregamento.FimDescarregamentoDescricao || Localization.Resources.GestaoPatio.FluxoPatio.FimDoDescarregamento,
        permanenciaMaximaExcedida: fluxoCarregamento.FimDescarregamentoLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataFimDescarregamentoPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataFimDescarregamentoPrevista
        });

    if (fluxoCarregamento.DataFimDescarregamento != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataFimDescarregamento
        });

    if (fluxoCarregamento.DiferencaFimDescarregamento != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaFimDescarregamento
        });

    return objEtapa;
}

function MontaEtapaFimHigienizacao(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesFimHigienizacao, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.FimDaHigienizacao,
        text: fluxoCarregamento.FimHigienizacaoDescricao || Localization.Resources.GestaoPatio.FluxoPatio.FimDaHigienizacao,
        permanenciaMaximaExcedida: fluxoCarregamento.FimHigienizacaoLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataFimHigienizacaoPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataFimHigienizacaoPrevista
        });

    if (fluxoCarregamento.DataFimHigienizacao != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataFimHigienizacao
        });

    if (fluxoCarregamento.DiferencaFimHigienizacao != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaFimHigienizacao
        });

    return objEtapa;
}

function MontaEtapaFimViagem(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirFimViagem, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.NessaEtapaPossivelInformarQuandoViagemChegouAoFim,
        text: fluxoCarregamento.FimViagemDescricao || Localization.Resources.GestaoPatio.FluxoPatio.FimDaViagem,
        permanenciaMaximaExcedida: fluxoCarregamento.FimViagemLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataFimViagemPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataFimViagemPrevista
        });
    if (fluxoCarregamento.DataFimViagem != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataFimViagem
        });

    if (fluxoCarregamento.DiferencaFimViagem != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaFimViagem
        });

    return objEtapa;
}

function MontaEtapaSeparacaoMercadoria(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesSeparacaoMercadoria, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.SeparacaoDaMercadoria,
        text: fluxoCarregamento.SeparacaoMercadoriaDescricao || Localization.Resources.GestaoPatio.FluxoPatio.SeparacaoDaMercadoria,
        permanenciaMaximaExcedida: fluxoCarregamento.SeparacaoMercadoriaLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataSeparacaoMercadoriaPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataSeparacaoMercadoriaPrevista
        });

    if (fluxoCarregamento.DataSeparacaoMercadoria != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataSeparacaoMercadoria
        });

    if (fluxoCarregamento.DiferencaSeparacaoMercadoria != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaSeparacaoMercadoria
        });

    return objEtapa;
}

function MontaEtapaSolicitacaoVeiculo(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesSolicitacaoVeiculo, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.SeparacaoDaMercadoria,
        text: fluxoCarregamento.SolicitacaoVeiculoDescricao || Localization.Resources.GestaoPatio.FluxoPatio.SolicitacaoDoVeiculo,
        permanenciaMaximaExcedida: fluxoCarregamento.SolicitacaoVeiculoLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataSolicitacaoVeiculoPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataSolicitacaoVeiculoPrevista
        });

    if (fluxoCarregamento.DataSolicitacaoVeiculo != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataSolicitacaoVeiculo
        });

    if (fluxoCarregamento.DiferencaSolicitacaoVeiculo != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaSolicitacaoVeiculo
        });

    return objEtapa;
}

function MontaEtapaMontagemCarga(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesMontagemCarga, koutFluxoPatio, objEtapa, infoEtapa); },
        eventClickDesbloqueada: function () { exibirDetalhesMontagemCarga(koutFluxoPatio, objEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.MontagemDeCarga,
        text: fluxoCarregamento.MontagemCargaDescricao || Localization.Resources.GestaoPatio.FluxoPatio.MontagemDeCarga,
        permanenciaMaximaExcedida: fluxoCarregamento.MontagemCargaLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataMontagemCargaPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataMontagemCargaPrevista
        });

    if (fluxoCarregamento.DataMontagemCarga != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataMontagemCarga
        });

    if (fluxoCarregamento.DiferencaMontagemCarga != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaMontagemCarga
        });

    return objEtapa;
}

function MontaEtapaAvaliacaoDescarga(fluxoCarregamento, koutFluxoPatio, infoEtapa) {
    var objEtapa = {
        etapaLiberada: infoEtapa.EtapaLiberada,
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesAvaliacaoDescargaFluxoPatio, koutFluxoPatio, objEtapa, infoEtapa); },
        tooltip: Localization.Resources.GestaoPatio.FluxoPatio.ControleDoAvaliacaoDescargaDosVeiculos,
        text: fluxoCarregamento.AvaliacaoDescargaDescricao || "Avaliação de Descarga",
        permanenciaMaximaExcedida: fluxoCarregamento.FimAvaliacaoDescargaLimitePermanencia,
        info: [],
        cor: infoEtapa.Cor,
        exibirAlerta: infoEtapa.ExibirAlerta
    };

    if (fluxoCarregamento.DataFimAvaliacaoDescargaPrevista != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'previsao',
            data: fluxoCarregamento.DataFimAvaliacaoDescargaPrevista
        });
    if (fluxoCarregamento.DataFimAvaliacaoDescarga != "" || _configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca)
        objEtapa.info.push({
            type: 'info',
            data: fluxoCarregamento.DataFimAvaliacaoDescarga
        });

    if (fluxoCarregamento.DiferencaFimAvaliacaoDescarga != null)
        objEtapa.info.push({
            type: 'time-diff',
            data: fluxoCarregamento.DiferencaFimAvaliacaoDescarga
        });

    return objEtapa;
}

function MontaEtapa(fluxoCarregamento, etapa, koutFluxoPatio) {
    switch (etapa.EtapaFluxoGestaoPatio) {
        case EnumEtapaFluxoGestaoPatio.CheckList:
            return MontaEtapaCheckList(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.Expedicao:
            return MontaEtapaExpedicao(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.Faturamento:
            return MontaEtapaFaturamento(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.ChegadaVeiculo:
            return MontaEtapaChegadaVeiculo(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.Guarita:
            return MontaEtapaGuarita(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.InformarDoca:
            return MontaEtapaInformarDoca(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.InicioCarregamento:
            return MontaEtapaInicioCarregamento(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.InicioHigienizacao:
            return MontaEtapaInicioHigienizacao(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.InicioViagem:
            return MontaEtapaInicioViagem(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.LiberacaoChave:
            return MontaEtapaLiberacaoChave(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.TravamentoChave:
            return MontaEtapaTravamentoChave(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.Posicao:
            return MontaEtapaPosicao(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.ChegadaLoja:
            return MontaEtapaChegadaLoja(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.DeslocamentoPatio:
            return MontaEtapaDeslocamentoPatio(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.SaidaLoja:
            return MontaEtapaSaidaLoja(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.FimCarregamento:
            return MontaEtapaFimCarregamento(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.FimHigienizacao:
            return MontaEtapaFimHigienizacao(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.FimViagem:
            return MontaEtapaFimViagem(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.SeparacaoMercadoria:
            return MontaEtapaSeparacaoMercadoria(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.SolicitacaoVeiculo:
            return MontaEtapaSolicitacaoVeiculo(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.InicioDescarregamento:
            return MontaEtapaInicioDescarregamento(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.FimDescarregamento:
            return MontaEtapaFimDescarregamento(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.DocumentoFiscal:
            return MontaEtapaDocumentoFiscal(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.DocumentosTransporte:
            return MontaEtapaDocumentosTransporte(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.MontagemCarga:
            return MontaEtapaMontagemCarga(fluxoCarregamento, koutFluxoPatio, etapa);

        case EnumEtapaFluxoGestaoPatio.AvaliacaoDescarga:
            return MontaEtapaAvaliacaoDescarga(fluxoCarregamento, koutFluxoPatio, etapa);

        default:
            return null;
    }
}

function fluxoPatioEtapaClick(callback, knoutFluxo, objetoEtapa, infoEtapa) {
    _fluxoAtual = knoutFluxo;
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador) {
        callback(knoutFluxo, objetoEtapa);
        return;
    }

    var dados = {
        Codigo: knoutFluxo.Codigo.val(),
        Etapa: infoEtapa.EtapaFluxoGestaoPatio
    };

    executarReST("FluxoPatio/DefinirEtapaComoVisualizada", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var data = retorno.Data;
                if (data.AlertaSonoro) {
                    _quantidadeFluxosEmitemAlertaSom = _quantidadeFluxosEmitemAlertaSom - 1;
                    if (_quantidadeFluxosEmitemAlertaSom <= 0)
                        limparTimerSonoro();
                }

                if (data.AlertaVisual)
                    SetarEtapaFluxoAguardando(objetoEtapa);

                callback(knoutFluxo, objetoEtapa);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}