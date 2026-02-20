/// <reference path="CancelamentoFatura.js" />
/// <reference path="CancelamentoTitulo.js" />

var _HTMLDadosFaturamento = "";
var _etapaFaturamento;
var _gridDadosFaturamento;

var EtapaFaturamento = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDadosFaturamento.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
};

function buscarDadosFaturamentoClick(e) {
    _cargaAtual = e;

    let strknoutEtapaFaturamento = "knoutEtapaFaturamento" + e.EtapaFaturamento.idGrid;
    let html = _HTMLDadosFaturamento.replace("#knoutEtapaFaturamento", strknoutEtapaFaturamento);

    $("#" + e.EtapaFaturamento.idGrid).html(html);

    _etapaFaturamento = new EtapaFaturamento();
    KoBindings(_etapaFaturamento, strknoutEtapaFaturamento);

    _etapaFaturamento.Carga.val(_cargaAtual.Codigo.val());

    montarGridDadosFaturamento();
}

function visibilidadeCancelarFaturaEtapaFaturamento(data) {
    return data.CodigoFatura > 0;
}

function visibilidadeCancelarTituloEtapaFaturamento(data) {
    return data.CodigoTitulo > 0;
}

function visibilidadeDownloadBoletoEtapaFaturamento(data) {
    return data.CodigoFatura > 0;
}

function montarGridDadosFaturamento() {
    var cancelarFatura = { descricao: Localization.Resources.Cargas.Carga.CancelarFatura, id: guid(), metodo: cancelarFaturaEtapaFaturamento, icone: "", visibilidade: visibilidadeCancelarFaturaEtapaFaturamento };
    var cancelarTitulo = { descricao: Localization.Resources.Cargas.Carga.CancelarTitulo, id: guid(), metodo: cancelarTituloEtapaFaturamento, icone: "", visibilidade: visibilidadeCancelarTituloEtapaFaturamento };
    var download = { descricao: Localization.Resources.Cargas.Carga.Download, id: guid(), metodo: downloadTituloFaturaEtapaFaturamento, icone: "" };
    var downloadBoleto = { descricao: Localization.Resources.Cargas.Carga.DownloadBoleto, id: guid(), metodo: downloadBoletoFaturaEtapaFaturamento, icone: "", visibilidade: visibilidadeDownloadBoletoEtapaFaturamento };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [cancelarFatura, cancelarTitulo, download, downloadBoleto] };

    _gridDadosFaturamento = new GridView(_etapaFaturamento.Pesquisar.idGrid, "CargaFaturamento/PesquisaFaturamento", _etapaFaturamento, menuOpcoes);
    _gridDadosFaturamento.CarregarGrid();
}

function cancelarFaturaEtapaFaturamento(faturamento) {
    abrirModalCancelamentoFaturaCarga(faturamento);
}

function cancelarTituloEtapaFaturamento(faturamento) {
    return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Funcional apenas no Pós MVP");//Apagar essa linha quando solicitarem
    abrirModalCancelamentoTituloCarga(faturamento)
}

function downloadTituloFaturaEtapaFaturamento(faturamento) {
    var dados = { Codigo: faturamento.CodigoFatura };
    executarDownload("Fatura/DownloadPDFFatura", dados, null, function () {
        executarDownloadArquivo("Relatorios/FaturaRelatorio/GerarRelatorio", { Codigo: faturamento.CodigoFatura });
    }) ;
}

function downloadBoletoFaturaEtapaFaturamento(faturamento, sender) {
    if (faturamento == null || faturamento.CodigoFatura == null || faturamento.CodigoFatura == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor selecione uma fatura antes de visualizar o relatório.");
        return;
    }

    if (faturamento.CodigoFatura > 0 && faturamento.CodigoFatura != "") {
        var dados = { CodigoFatura: faturamento.CodigoFatura }
        executarDownload("TituloFinanceiro/DownloadBoleto", dados);
    } else {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor selecione uma fatura.");
    }
}

//*******ETAPA*******

function EtapaFaturamentoDesabilitada(e) {
    $("#" + e.EtapaFaturamento.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaFaturamento.idTab + " .step").attr("class", "step");
    e.EtapaFaturamento.eventClick = function (e) { buscarDadosFaturamentoClick(e) };
}

function EtapaFaturamentoLiberada(e) {
    $("#" + e.EtapaFaturamento.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaFaturamento.idTab + " .step").attr("class", "step yellow");
    e.EtapaFaturamento.eventClick = function (e) { buscarDadosFaturamentoClick(e) };
}

function EtapaFaturamentoProblema(e) {
    $("#" + e.EtapaFaturamento.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaFaturamento.idTab + " .step").attr("class", "step red");
    e.EtapaFaturamento.eventClick = function (e) { buscarDadosFaturamentoClick(e) };
}

function EtapaFaturamentoAguardando(e) {
    $("#" + e.EtapaFaturamento.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaFaturamento.idTab + " .step").attr("class", "step yellow");
    e.EtapaFaturamento.eventClick = function (e) { buscarDadosFaturamentoClick(e) };
}

function EtapaFaturamentoAprovada(e) {
    $("#" + e.EtapaFaturamento.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaFaturamento.idTab + " .step").attr("class", "step green");
    e.EtapaFaturamento.eventClick = function (e) { buscarDadosFaturamentoClick(e) };
}