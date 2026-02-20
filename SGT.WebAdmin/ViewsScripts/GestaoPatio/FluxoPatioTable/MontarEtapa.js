/// <reference path="../FluxoPatio/Checklist.js" />
/// <reference path="../FluxoPatio/ChegadaSaidaLoja.js" />
/// <reference path="../FluxoPatio/DeslocamentoPatio.js" />
/// <reference path="../FluxoPatio/DocaCarregamento.js" />
/// <reference path="../FluxoPatio/DocumentoFiscal.js" />
/// <reference path="../FluxoPatio/DocumentosTransporte.js" />
/// <reference path="../FluxoPatio/Expedicao.js" />
/// <reference path="../FluxoPatio/FimCarregamento.js" />
/// <reference path="../FluxoPatio/FimDescarregamento.js" />
/// <reference path="../FluxoPatio/FimHigienizacao.js" />
/// <reference path="../FluxoPatio/FimViagem.js" />
/// <reference path="../FluxoPatio/FluxoPatio.js" />
/// <reference path="../FluxoPatio/Guarita.js" />
/// <reference path="../FluxoPatio/InicioCarregamento.js" />
/// <reference path="../FluxoPatio/InicioDescarregamento.js" />
/// <reference path="../FluxoPatio/InicioHigienizacao.js" />
/// <reference path="../FluxoPatio/MontagemCargaPatio.js" />
/// <reference path="../FluxoPatio/Posicao.js" />
/// <reference path="../FluxoPatio/SeparacaoMercadoria.js" />
/// <reference path="../FluxoPatio/SolicitacaoVeiculo.js" />
/// <reference path="../FluxoPatio/TravamentoChave.js" />

function MontaEtapaCheckList(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesCheckListFluxoPatio, koutFluxoPatio, objEtapa); },
        Tooltip: "Controle do checklist dos veículos.",
        Descricao: koutFluxoPatio.CheckListDescricao.val() || "Checklist",
        DataRealizada: koutFluxoPatio.DataFimCheckList.val(),
        Atrasada: koutFluxoPatio.DiferencaFimCheckList.val() < 0,
        DataPrevista: koutFluxoPatio.DataFimCheckListPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataFimCheckListReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaFimCheckList.val()),
        Temperatura : "",
    };

    return objEtapa;
}

function MontaEtapaInformarDoca(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesDocaCarregamentoFluxoPatio, koutFluxoPatio, objEtapa); },
        Tooltip: "Indica a doca para carregamento.",
        Descricao: koutFluxoPatio.InformarDocaCarregamentoDescricao.val() || "Doca",
        DataRealizada: koutFluxoPatio.DataDocaInformada.val(),
        Atrasada: koutFluxoPatio.DiferencaDocaInformada.val() < 0,
        DataPrevista: koutFluxoPatio.DataDocaInformadaPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataDocaInformadaReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaDocaInformada.val()),
        Temperatura: "",
    };

    return objEtapa;
}

function MontaEtapaGuarita(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesEntradaGuaritaFluxoPatio, koutFluxoPatio, objEtapa); },
        Tooltip: "Controle de entrada de veículos.",
        Descricao: koutFluxoPatio.GuaritaEntradaDescricao.val() || "Chegada Portaria",
        DataRealizada: koutFluxoPatio.DataChegadaVeiculo.val(),
        Atrasada: koutFluxoPatio.DiferencaChegadaVeiculo.val() < 0,
        DataPrevista: koutFluxoPatio.DataChegadaVeiculoPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataChegadaVeiculoReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaChegadaVeiculo.val()),
        Temperatura : "",
    };

    return objEtapa;
}

function MontaEtapaTravamentoChave(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesTravaChaveFluxoPatio, koutFluxoPatio, objEtapa); },
        Tooltip: "Controle de trava de chave dos veículos.",
        Descricao: koutFluxoPatio.TravaChaveDescricao.val() || "Travamento da Chave",
        DataRealizada: koutFluxoPatio.DataTravaChave.val(),
        Atrasada: koutFluxoPatio.DiferencaTravaChave.val() < 0,
        DataPrevista: koutFluxoPatio.DataTravaChavePrevista.val(),
        DataReprogramada: koutFluxoPatio.DataTravaChaveReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaTravaChave.val()),
        Temperatura : "",
    };

    return objEtapa;
}

function MontaEtapaExpedicao(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesExpedicaoFluxoPatio, koutFluxoPatio, objEtapa); },
        Tooltip: "É onde é controlada a expedição dos veículos.",
        Descricao: koutFluxoPatio.ExpedicaoDescricao.val() || "Expedição",
        DataRealizada: koutFluxoPatio.DataFimCarregamento.val(),
        Atrasada: koutFluxoPatio.DiferencaFimCarregamento.val() < 0,
        DataPrevista: koutFluxoPatio.DataFimCarregamentoPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataFimCarregamentoReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaFimCarregamento.val()),
        Temperatura : "",
    };

    return objEtapa;
}

function MontaEtapaLiberacaoChave(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesLiberaChaveFluxoPatio, koutFluxoPatio, objEtapa); },
        Tooltip: "Controle da liberação da trava de chave dos veículos.",
        Descricao: koutFluxoPatio.LiberaChaveDescricao.val() || "Liberação da Chave",
        DataRealizada: koutFluxoPatio.DataLiberacaoChave.val(),
        Atrasada: koutFluxoPatio.DiferencaLiberacaoChave.val() < 0,
        DataPrevista: koutFluxoPatio.DataLiberacaoChavePrevista.val(),
        DataReprogramada: koutFluxoPatio.DataLiberacaoChaveReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaLiberacaoChave.val()),
        Temperatura : "",
    };

    return objEtapa;
}

function MontaEtapaFaturamento(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesFaturamento, koutFluxoPatio, objEtapa); },
        Tooltip: "Indica o faturamento da carga.",
        Descricao: koutFluxoPatio.FaturamentoDescricao.val() || "Faturamento",
        DataRealizada: koutFluxoPatio.DataFaturamento.val(),
        Atrasada: koutFluxoPatio.DiferencaFaturamento.val() < 0,
        DataPrevista: koutFluxoPatio.DataFaturamentoPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataFaturamentoReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaFaturamento.val()),
        Temperatura : "",
    };

    return objEtapa;
}

function MontaEtapaChegadaVeiculo(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesChegadaVeiculoFluxoPatio, koutFluxoPatio, objEtapa); },
        Tooltip: "Informa Chegada do Veículo.",
        Descricao: koutFluxoPatio.ChegadaVeiculoDescricao.val() || "Chegada Veículo",
        DataRealizada: koutFluxoPatio.DataChegadaVeiculo.val(),
        Atrasada: koutFluxoPatio.DiferencaChegadaVeiculo.val() < 0,
        DataPrevista: koutFluxoPatio.DataChegadaVeiculoPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataChegadaVeiculoReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaChegadaVeiculo.val()),
        Temperatura : "",
    };

    return objEtapa;
}

function MontaEtapaInicioCarregamento(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesInicioCarregamento, koutFluxoPatio, objEtapa); },
        Tooltip: "Início do carregamento",
        Descricao: koutFluxoPatio.InicioCarregamentoDescricao.val() || "Início do Carregamento",
        DataRealizada: koutFluxoPatio.DataInicioCarregamento.val(),
        Atrasada: koutFluxoPatio.DiferencaInicioCarregamento.val() < 0,
        DataPrevista: koutFluxoPatio.DataInicioCarregamentoPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataInicioCarregamentoReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaInicioCarregamento.val()),
        Temperatura: "",
    };

    return objEtapa;
}

function MontaEtapaInicioDescarregamento(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesInicioDescarregamento, koutFluxoPatio, objEtapa); },
        Tooltip: "Início do descarregamento",
        Descricao: koutFluxoPatio.InicioDescarregamentoDescricao.val() || "Início do Descarregamento",
        DataRealizada: koutFluxoPatio.DataInicioDescarregamento.val(),
        Atrasada: koutFluxoPatio.DiferencaInicioDescarregamento.val() < 0,
        DataPrevista: koutFluxoPatio.DataInicioDescarregamentoPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataInicioDescarregamentoReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaInicioDescarregamento.val()),
        Temperatura: "",
    };

    return objEtapa;
}

function MontaEtapaInicioHigienizacao(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesInicioHigienizacao, koutFluxoPatio, objEtapa); },
        Tooltip: "Início da higienização",
        Descricao: koutFluxoPatio.InicioHigienizacaoDescricao.val() || "Início da Higienização",
        DataRealizada: koutFluxoPatio.DataInicioHigienizacao.val(),
        Atrasada: koutFluxoPatio.DiferencaInicioHigienizacao.val() < 0,
        DataPrevista: koutFluxoPatio.DataInicioHigienizacaoPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataInicioHigienizacaoReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaInicioHigienizacao.val()),
        Temperatura: "",
    };

    return objEtapa;
}

function MontaEtapaInicioViagem(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDetalhesSaidaGuaritaFluxoPatio, koutFluxoPatio, objEtapa); },
        Tooltip: "Caso tenha integração, nessa etapa é possível acompanhar.",
        Descricao: koutFluxoPatio.GuaritaSaidaDescricao.val() || "Início Viagem",
        DataRealizada: koutFluxoPatio.DataInicioViagem.val(),
        Atrasada: koutFluxoPatio.DiferencaInicioViagem.val() < 0,
        DataPrevista: koutFluxoPatio.DataInicioViagemPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataInicioViagemReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaInicioViagem.val()),
        Temperatura : "",
    };

    return objEtapa;
}

function MontaEtapaPosicao(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(ExibirRastreioCarga, koutFluxoPatio, objEtapa); },
        Tooltip: "Nessa etapa é possível acompanhar a posição no mapa que a carga se encontra.",
        Descricao: koutFluxoPatio.PosicaoDescricao.val() || "Posição",
        DataRealizada: "",
        Atrasada: false,
        DataPrevista: "",
        TempoAtraso: "",
        Temperatura: koutFluxoPatio.Temperatura.val(),
    };

    return objEtapa;
}

function MontaEtapaChegadaLoja(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(ExibirChegadaVeiculoLoja, koutFluxoPatio, objEtapa); },
        Tooltip: "Nessa etapa é possível informar quando o veículo chegou ao destinatário.",
        Descricao: koutFluxoPatio.ChegadaLojaDescricao.val() || "Chegada no Destinatário",
        DataRealizada: koutFluxoPatio.DataChegadaLoja.val(),
        Atrasada: koutFluxoPatio.DiferencaChegadaLoja.val() < 0,
        DataPrevista: koutFluxoPatio.DataChegadaLojaPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataChegadaLojaReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaChegadaLoja.val()),
        Temperatura : "",
    };

    return objEtapa;
}

function MontaEtapaDeslocamentoPatio(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(ExibirDeslocamentoPatio, koutFluxoPatio, objEtapa); },
        Tooltip: "Nessa etapa é possível averiguar quando o veículo está se deslocando dentro do pátio.",
        Descricao: koutFluxoPatio.DeslocamentoPatioDescricao.val() || "Deslocamento Pátio",
        DataRealizada: koutFluxoPatio.DataDeslocamentoPatio.val(),
        Atrasada: koutFluxoPatio.DiferencaDeslocamentoPatio.val() < 0,
        DataPrevista: koutFluxoPatio.DataDeslocamentoPatioPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataDeslocamentoPatioReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaDeslocamentoPatio.val()),
        Temperatura : "",
    };

    return objEtapa;
}

function MontaEtapaDocumentoFiscal(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesDocumentoFiscal, koutFluxoPatio, objEtapa); },
        Tooltip: "Documento fiscal",
        Descricao: koutFluxoPatio.DocumentoFiscalDescricao.val() || "Documento Fiscal",
        DataRealizada: koutFluxoPatio.DataDocumentoFiscal.val(),
        Atrasada: koutFluxoPatio.DiferencaDocumentoFiscal.val() < 0,
        DataPrevista: koutFluxoPatio.DataDocumentoFiscalPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataDocumentoFiscalReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaDocumentoFiscal.val()),
        Temperatura: "",
    };

    return objEtapa;
}

function MontaEtapaDocumentosTransporte(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesDocumentosTransporte, koutFluxoPatio, objEtapa); },
        Tooltip: "Documentos de transporte",
        Descricao: koutFluxoPatio.DocumentosTransporteDescricao.val() || "Documentos de Transporte",
        DataRealizada: koutFluxoPatio.DataDocumentosTransporte.val(),
        Atrasada: koutFluxoPatio.DiferencaDocumentosTransporte.val() < 0,
        DataPrevista: koutFluxoPatio.DataDocumentosTransportePrevista.val(),
        DataReprogramada: koutFluxoPatio.DataDocumentosTransporteReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaDocumentosTransporte.val()),
        Temperatura: "",
    };

    return objEtapa;
}

function MontaEtapaSaidaLoja(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(ExibirSaidaVeiculoLoja, koutFluxoPatio, objEtapa); },
        Tooltip: "Nessa etapa é possível informar quando o veículo saiu do destinatário.",
        Descricao: koutFluxoPatio.SaidaLojaDescricao.val() || "Saída do Destinatário",
        DataRealizada: koutFluxoPatio.DataSaidaLoja.val(),
        Atrasada: koutFluxoPatio.DiferencaSaidaLoja.val() < 0,
        DataPrevista: koutFluxoPatio.DataSaidaLojaPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataSaidaLojaReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaSaidaLoja.val()),
        Temperatura : "",
    };

    return objEtapa;
}

function MontaEtapaFimCarregamento(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesFimCarregamento, koutFluxoPatio, objEtapa); },
        Tooltip: "Fim do carregamento",
        Descricao: koutFluxoPatio.FimCarregamentoDescricao.val() || "Fim do Carregamento",
        DataRealizada: koutFluxoPatio.DataFimCarregamento.val(),
        Atrasada: koutFluxoPatio.DiferencaFimCarregamento.val() < 0,
        DataPrevista: koutFluxoPatio.DataFimCarregamentoPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataFimCarregamentoReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaFimCarregamento.val()),
        Temperatura: "",
    };

    return objEtapa;
}

function MontaEtapaFimDescarregamento(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesFimDescarregamento, koutFluxoPatio, objEtapa); },
        Tooltip: "Fim do descarregamento",
        Descricao: koutFluxoPatio.FimDescarregamentoDescricao.val() || "Fim do Descarregamento",
        DataRealizada: koutFluxoPatio.DataFimDescarregamento.val(),
        Atrasada: koutFluxoPatio.DiferencaFimDescarregamento.val() < 0,
        DataPrevista: koutFluxoPatio.DataFimDescarregamentoPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataFimDescarregamentoReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaFimDescarregamento.val()),
        Temperatura: "",
    };

    return objEtapa;
}

function MontaEtapaFimHigienizacao(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesFimHigienizacao, koutFluxoPatio, objEtapa); },
        Tooltip: "Fim da higienização",
        Descricao: koutFluxoPatio.FimHigienizacaoDescricao.val() || "Fim da Higienização",
        DataRealizada: koutFluxoPatio.DataFimHigienizacao.val(),
        Atrasada: koutFluxoPatio.DiferencaFimHigienizacao.val() < 0,
        DataPrevista: koutFluxoPatio.DataFimHigienizacaoPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataFimHigienizacaoReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaFimHigienizacao.val()),
        Temperatura: "",
    };

    return objEtapa;
}

function MontaEtapaFimViagem(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(ExibirFimViagem, koutFluxoPatio, objEtapa); },
        Tooltip: "Nessa etapa é possível informar quando a viagem chegou ao fim.",
        Descricao: koutFluxoPatio.FimViagemDescricao.val() || "Fim da Viagem",
        DataRealizada: koutFluxoPatio.DataFimViagem.val(),
        Atrasada: koutFluxoPatio.DiferencaFimViagem.val() < 0,
        DataPrevista: koutFluxoPatio.DataFimViagemPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataFimViagemReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaFimViagem.val()),
        Temperatura : "",
    };

    return objEtapa;
}

function MontaEtapaSeparacaoMercadoria(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesSeparacaoMercadoria, koutFluxoPatio, objEtapa); },
        Tooltip: "Separação de Mercadoria",
        Descricao: koutFluxoPatio.SeparacaoMercadoriaDescricao.val() || "Separação de Mercadoria",
        DataRealizada: koutFluxoPatio.DataSeparacaoMercadoria.val(),
        Atrasada: koutFluxoPatio.DiferencaSeparacaoMercadoria.val() < 0,
        DataPrevista: koutFluxoPatio.DataSeparacaoMercadoriaPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataSeparacaoMercadoriaReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaSeparacaoMercadoria.val()),
        Temperatura: "",
    };

    return objEtapa;
}

function MontaEtapaSolicitacaoVeiculo(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesSolicitacaoVeiculo, koutFluxoPatio, objEtapa); },
        Tooltip: "Solicitação de Veículo",
        Descricao: koutFluxoPatio.SolicitacaoVeiculoDescricao.val() || "Solicitação de Veículo",
        DataRealizada: koutFluxoPatio.DataSolicitacaoVeiculo.val(),
        Atrasada: koutFluxoPatio.DiferencaSolicitacaoVeiculo.val() < 0,
        DataPrevista: koutFluxoPatio.DataSolicitacaoVeiculoPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataSolicitacaoVeiculoReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaSolicitacaoVeiculo.val()),
        Temperatura: "",
    };

    return objEtapa;
}

function MontaEtapaMontagemCarga(koutFluxoPatio) {
    var objEtapa = {
        backEventClick: function () { fluxoPatioEtapaClick(exibirDetalhesMontagemCarga, koutFluxoPatio, objEtapa); },
        Tooltip: "Montagem de Carga",
        Descricao: koutFluxoPatio.MontagemCargaDescricao.val() || "Montagem de Carga",
        DataRealizada: koutFluxoPatio.DataMontagemCarga.val(),
        Atrasada: koutFluxoPatio.DiferencaMontagemCarga.val() < 0,
        DataPrevista: koutFluxoPatio.DataMontagemCargaPrevista.val(),
        DataReprogramada: koutFluxoPatio.DataMontagemCargaReprogramada.val(),
        TempoAtraso: MinutosEmData(koutFluxoPatio.DiferencaMontagemCarga.val()),
        Temperatura: "",
    };

    return objEtapa;
}

function MontaEtapa(knoutEtapa, koutFluxoPatio) {
    switch (knoutEtapa.EtapaFluxoGestaoPatio.val()) {
        case EnumEtapaFluxoGestaoPatio.CheckList:
            return MontaEtapaCheckList(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.Expedicao:
            return MontaEtapaExpedicao(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.Faturamento:
            return MontaEtapaFaturamento(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.ChegadaVeiculo:
            return MontaEtapaChegadaVeiculo(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.Guarita:
            return MontaEtapaGuarita(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.InformarDoca:
            return MontaEtapaInformarDoca(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.InicioCarregamento:
            return MontaEtapaInicioCarregamento(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.InicioHigienizacao:
            return MontaEtapaInicioHigienizacao(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.InicioViagem:
            return MontaEtapaInicioViagem(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.LiberacaoChave:
            return MontaEtapaLiberacaoChave(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.TravamentoChave:
            return MontaEtapaTravamentoChave(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.Posicao:
            return MontaEtapaPosicao(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.ChegadaLoja:
            return MontaEtapaChegadaLoja(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.DeslocamentoPatio:
            return MontaEtapaDeslocamentoPatio(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.SaidaLoja:
            return MontaEtapaSaidaLoja(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.FimCarregamento:
            return MontaEtapaFimCarregamento(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.FimHigienizacao:
            return MontaEtapaFimHigienizacao(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.FimViagem:
            return MontaEtapaFimViagem(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.SeparacaoMercadoria:
            return MontaEtapaSeparacaoMercadoria(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.SolicitacaoVeiculo:
            return MontaEtapaSolicitacaoVeiculo(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.InicioDescarregamento:
            return MontaEtapaInicioDescarregamento(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.FimDescarregamento:
            return MontaEtapaFimDescarregamento(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.DocumentoFiscal:
            return MontaEtapaDocumentoFiscal(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.DocumentosTransporte:
            return MontaEtapaDocumentosTransporte(koutFluxoPatio);

        case EnumEtapaFluxoGestaoPatio.MontagemCarga:
            return MontaEtapaMontagemCarga(koutFluxoPatio);

        default:
            return null;
    }
}

function fluxoPatioEtapaClick(callback, knoutFluxo, objetoEtapa) {
    _fluxoAtual = knoutFluxo;

    callback(knoutFluxo, objetoEtapa);
}
