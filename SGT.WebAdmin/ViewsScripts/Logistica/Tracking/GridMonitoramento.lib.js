/*GridMonitoramento.lib.js*/

// #region Funções Privadas

function obterHtmlColunaCritico(dadosLinha) {
    if (dadosLinha.Critico)
        return '<i class="fa fa-exclamation-circle" style="color: red; font-size: 15px;" title="Crítico"></i>';

    return "<span></span>";
}

function obterHtmlColunaIgnicao(dadosLinha) {
    var icone = TrackingIconIgnicao(dadosLinha.StatusIgnicao != "Desligado");

    return '<div class="tracking-indicador" title="' + dadosLinha.StatusIgnicao + '">' + icone + '</div>';
}

function obterHtmlColunaPercentualViagem(dadosLinha) {
    var percentual = parseInt(dadosLinha.PercentualViagem);

    if (percentual < 0)
        percentual = 0;
    else if (percentual > 100)
        percentual = 100;

    var descricaoIn = "&nbsp;";
    var descricaoOut = "";

    if (percentual > 50)
        descricaoIn = percentual + '%';
    else
        descricaoOut = '<span class="progress-bar-desc">' + percentual + '%</span>';

    return '<div class="progress-bar" style="width: ' + percentual + '%">' + descricaoIn + '</div>' + descricaoOut;
}

function obterHtmlColunaRastreador(dadosLinha) {
    var icone = ObterIconeStatusTracking(parseInt(dadosLinha.RastreadorOnlineOffline), 20);

    return '<div class="tracking-indicador" title="' + dadosLinha.DataPosicaoAtual + '">' + icone + '</div>';
}

function obterHtmlColunaAlertasAbertos(dadosLinha) {
    let icone = dadosLinha.IconeUltimoAlertaExibirTela ? '<img class="blink" src="' + dadosLinha.IconeUltimoAlertaExibirTela + '" width="20" height="20" alt="Icon" onclick="loadTratativaAlerta({ CodigoAlerta: ' + dadosLinha.CodigoUltimoAlerta + ' }, []);" style="cursor: pointer;"/>' : "";
    return '<div class="tracking-indicador" title="' + dadosLinha.DataPosicaoAtual + '">' + icone + '</div>';
}


function obterHtmlColunaSemaforo(dadosLinha) {
    return '<div class="tracking-indicador" style="background-color: #' + dadosLinha.CorSemaforo + '"></div>';
}

function obterHtmlColunaTemperatura(dadosLinha) {
    var className = dadosLinha.ControleDeTemperatura.toLowerCase().split(' ').join('-');
    var title = dadosLinha.ControleDeTemperatura + ((className != 'sem-controle') ? ' (' + dadosLinha.TemperaturaFaixaInicial + ' a ' + dadosLinha.TemperaturaFaixaFinal + ')' : '');

    return '<span class="temperatura-gm ' + className + '" title="' + title + '">' + ((dadosLinha.Temperatura) ? dadosLinha.Temperatura : '...') + '</span>';
}

function obterHtmlColunaTendenciaProximaParadaDescricao(dadosLinha) {
    let title = dadosLinha.TendenciaProximaParadaDescricao;
    if (title == '-')
        title = "Não calculado por ausência de dados";
    return '<div title="' + title + '"><span>' + dadosLinha.TendenciaProximaParadaDescricao + '</span></div>';
}

function obterHtmlColunaTendenciaColetaDescricao(dadosLinha) {
    let title = dadosLinha.TendenciaColetaDescricao;
    if (title == '-')
        title = "Não calculado por ausência de dados";
    return '<div title="' + title + '"><span>' + dadosLinha.TendenciaColetaDescricao + '</span></div>';
}

function obterHtmlColunaPrazoColetaDescricao(dadosLinha) {
    let title = dadosLinha.PrazoColetaDescricao;
    if (title == '-')
        title = "Não calculado por ausência de dados";
    return '<div title="' + title + '"><span>' + dadosLinha.PrazoColetaDescricao + '</span></div>';
}

function obterHtmlColunaTendenciaEntregaDescricao(dadosLinha) {
    let title = dadosLinha.TendenciaEntregaDescricao;
    if (title == '-')
        title = "Não calculado por ausência de dados";
    return '<div title="' + title + '"><span>' + dadosLinha.TendenciaEntregaDescricao + '</span></div>';
}

function obterHtmlColunaPrazoEntregaDescricao(dadosLinha) {
    let title = dadosLinha.PrazoEntregaDescricao;
    if (title == '-')
        title = "Não calculado por ausência de dados";
    return '<div title="' + title + '"><span>' + dadosLinha.PrazoEntregaDescricao + '</span></div>';
}

// #endregion Funções Privadas

// #region Funções Públicas

function gridMonitoramentoCallbackColumnDefault(cabecalho, valorColuna, dadosLinha) {

    if (cabecalho.name == "StatusIgnicao")
        return obterHtmlColunaIgnicao(dadosLinha);
    else if (cabecalho.name == "Temperatura")
        return obterHtmlColunaTemperatura(dadosLinha);
    else if (cabecalho.name == "PercentualViagem")
        return obterHtmlColunaPercentualViagem(dadosLinha);
    else if (cabecalho.name == "RastreadorOnlineOffline")
        return obterHtmlColunaRastreador(dadosLinha);
    else if (cabecalho.name == "CorSemaforo")
        return obterHtmlColunaSemaforo(dadosLinha);
    else if (cabecalho.name == "Critico")
        return obterHtmlColunaCritico(dadosLinha);
    else if (cabecalho.name == "AlertasAbertos")
        return obterHtmlColunaAlertasAbertos(dadosLinha);
    else if (cabecalho.name == "TendenciaProximaParadaDescricao")
        return obterHtmlColunaTendenciaProximaParadaDescricao(dadosLinha);
    else if (cabecalho.name == "TendenciaColetaDescricao")
        return obterHtmlColunaTendenciaColetaDescricao(dadosLinha);
    else if (cabecalho.name == "PrazoColetaDescricao")
        return obterHtmlColunaPrazoColetaDescricao(dadosLinha);
    else if (cabecalho.name == "TendenciaEntregaDescricao")
        return obterHtmlColunaTendenciaEntregaDescricao(dadosLinha);
    else if (cabecalho.name == "PrazoEntregaDescricao")
        return obterHtmlColunaPrazoEntregaDescricao(dadosLinha);
}

function gridMonitoramentoCallbackRow(row, data) {
    var color = data.Critico ? '#FFF0F0' : 'none';

    $(row).css("background-color", color);
}

// #endregion Funções Públicas
