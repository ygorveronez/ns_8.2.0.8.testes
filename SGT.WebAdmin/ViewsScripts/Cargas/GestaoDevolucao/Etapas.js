//#region Objetos Globais do Arquivo
var _etapasGestaoDevolucao;
// #endregion Objetos Globais do Arquivo

//#region Classes
var EtapasGestaoDevolucao = function () {
    this.EtapaGestaoDevolucao = $.extend(newEtapaBase(), {
        text: "Gestão de Devolução",
        eventClick: limparEtapasDevolucao,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/document.svg",
        etapa: 0,
    });

    this.EtapaDefinicaoTipoDevolucao = $.extend(newEtapaBase(), {
        text: "Definição Tipo de Devolução",
        eventClick: loadDefinicaoTipoDevolucao,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/check.svg",
        name: "EtapaDefinicaoTipoDevolucao",
        etapa: 1,
        acaoTransportador: true
    });

    this.AprovacaoTipoDevolucao = $.extend(newEtapaBase(), {
        text: "Aprovação",
        eventClick: loadAprovacaoTipoDevolucao,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/check.svg",
        name: "AprovacaoTipoDevolucao",
        etapa: 2
    });

    this.OrdemeRemessa = $.extend(newEtapaBase(), {
        text: "Ordem e Remessa",
        eventClick: loadGestaoDevolucaoEtapaOrdemERemessa,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/deliver.svg",
        name: "OrdemeRemessa",
        etapa: 3
    });

    this.GeracaoOcorrenciaDebito = $.extend(newEtapaBase(), {
        text: "Geração Ocorrência Débito",
        eventClick: loadGestaoDevolucaoEtapaGeracaoOcorrenciaDebito,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/money.svg",
        name: "GeracaoOcorrenciaDebito",
        etapa: 4
    });

    this.DefinicaoLocalColeta = $.extend(newEtapaBase(), {
        text: "Definição Local de Coleta",
        eventClick: loadGestaoDevolucaoEtapaDefinicaoLocalColeta,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/home.svg",
        name: "DefinicaoLocalColeta",
        etapa: 5,
        acaoTransportador: true
    });

    this.GeracaoCargaDevolucao = $.extend(newEtapaBase(), {
        text: "Geração Carga de Devolução",
        eventClick: loadGestaoDevolucaoGeracaoCargaDevolucao,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/arrow-left.svg",
        name: "GeracaoCargaDevolucao",
        etapa: 6
    });

    this.AgendamentoParaDescarga = $.extend(newEtapaBase(), {
        text: "Agendamento para Descarga",
        eventClick: loadGestaoDevolucaoEtapaAgendamentoParaDescarga,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/calendar.svg",
        name: "AgendamentoParaDescarga",
        etapa: 7
    });

    this.GestaoCustoContabil = $.extend(newEtapaBase(), {
        text: "Gestão de Custo e Contábil",
        eventClick: loadGestaoDevolucaoEtapaGestaoCustoContabil,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/money.svg",
        name: "GestaoCustoContabil",
        etapa: 8
    });

    this.Agendamento = $.extend(newEtapaBase(), {
        text: "Agendamento",
        eventClick: loadGestaoDevolucaoEtapaAgendamento,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/calendar.svg",
        name: "Agendamento",
        etapa: 9,
        acaoTransportador: true
    });

    this.AprovacaoDataDescarga = $.extend(newEtapaBase(), {
        text: "Aprovação Data Descarga",
        eventClick: loadAprovacaoDataDescarga,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/check.svg",
        name: "AprovacaoDataDescarga",
        etapa: 10
    });

    this.Monitoramento = $.extend(newEtapaBase(), {
        text: "Monitoramento",
        eventClick: loadGestaoDevolucaoEtapaMonitoramento,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/wireless.svg",
        name: "Monitoramento",
        etapa: 11
    });

    this.GeracaoLaudo = $.extend(newEtapaBase(), {
        text: "Geração de Laudo",
        eventClick: loadGeracaoLaudo,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/news.svg",
        name: "GeracaoLaudo",
        etapa: 12
    });

    this.AprovacaoLaudo = $.extend(newEtapaBase(), {
        text: "Aprovação Laudo",
        eventClick: loadGestaoDevolucaoEtapaAprovacaoLaudo,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/check.svg",
        name: "AprovacaoLaudo",
        etapa: 13
    });

    this.IntegracaoLaudo = $.extend(newEtapaBase(), {
        text: "Integração Laudo",
        eventClick: loadGestaoDevolucaoEtapaIntegracaoLaudo,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/link.svg",
        name: "IntegracaoLaudo",
        etapa: 14
    });

    this.CenarioPosEntrega = $.extend(newEtapaBase(), {
        text: "Cenário Pós Entrega",
        eventClick: loadGestaoDevolucaoEtapaPosEntrega,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/deliver.svg",
        name: "CenarioPosEntrega",
        etapa: 15
    });

    this.RegistroDocumentosPallet = $.extend(newEtapaBase(), {
        text: "Registro de Documentos",
        eventClick: loadGestaoRegistroDocumentosPallet,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/document.svg",
        name: "RegistroDocumentosPallet",
        etapa: 16
    });

    this.DocumentacaoEntradaFiscal = $.extend(newEtapaBase(), {
        text: "Documentação de entrada fiscal",
        eventClick: loadDocumentacaoEntradaFiscal,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/arrow-left.svg",
        name: "DocumentacaoEntradaFiscal",
        etapa: 17
    });

    this.AprovacaoCenarioPosEntrega = $.extend(newEtapaBase(), {
        text: "Avaliação e Liberação",
        eventClick: loadAprovacaoCenarioPosEntrega,
        icon: "Content/TorreControle/Icones/gestaoDevolucao/check.svg",
        name: "AprovacaoCenarioPosEntrega",
        etapa: 18
    });

    this.Etapas = PropertyEntity({ def: [], val: ko.observableArray([]), visible: ko.observable(false) });
}

function newEtapaBase() {
    let etapaBase = PropertyEntity({
        text: ko.observable(""),
        type: types.local,
        situacao: ko.observable(0),
        eventClick: null,
        idGrid: guid(),
        idTab: guid(),
        name: "",
        icon: ko.observable(""),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable(""),
        etapa: 0,
        visible: ko.observable(true),
        isActive: ko.observable(false),
        acaoTransportador: false
    })

    return etapaBase;
}
//#endregion Classes

// #region Funções de Inicialização
function loadEtapasGestaoDevolucao() {
    _etapasGestaoDevolucao = new EtapasGestaoDevolucao();
    KoBindings(_etapasGestaoDevolucao, "knockoutEtapasDevolucao");
}
// #endregion Funções de Inicialização

// #region Funções Públicas
function montarEtapaPadrao() {
    _etapasGestaoDevolucao.Etapas.val([]);

    $("#" + _etapasGestaoDevolucao.EtapaGestaoDevolucao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasGestaoDevolucao.EtapaGestaoDevolucao.idTab + " .step").attr("class", "step cyan");
    $("#" + _etapasGestaoDevolucao.EtapaGestaoDevolucao.idTab + " .step").removeClass('active');

    _etapasGestaoDevolucao.EtapaGestaoDevolucao.visible(true);
}

function montarEtapasDevolucao(registroSelecionado) {
    _etapasGestaoDevolucao.Etapas.val([]);

    $('#aviso-selecionar-registro').hide();
    $('#panel-etapas-devolucao').show();

    let listaEtapasMontar = [];

    if (!registroSelecionado.Etapas) return;

    let listaEtapasRegistroSelecionado = JSON.parse(registroSelecionado.Etapas);

    let nomesPropriedadesObjetoEtapas = Object.getOwnPropertyNames(_etapasGestaoDevolucao);

    let nomesPropriedadesObjetoEtapasFiltradas = nomesPropriedadesObjetoEtapas.filter(prop => listaEtapasRegistroSelecionado.some(etapa => etapa.Etapa == _etapasGestaoDevolucao[prop].etapa));
    nomesPropriedadesObjetoEtapasFiltradas.forEach(prop => listaEtapasMontar.push(_etapasGestaoDevolucao[prop]));

    let objEtapa = montarObjetoEtapas(listaEtapasMontar, listaEtapasRegistroSelecionado);
}

function limparEtapasDevolucao() {
    $('#aviso-selecionar-registro').show();
    $('#grid-devolucoes').show();
    $('#grid-notas-fiscais').hide();
    $("#panel-etapas-devolucao").hide();
    $("#aviso-selecionar-registro").show();
    $('#container-principal').hide();
    _informacoesDevolucao.CodigoDevolucao.val(0);
    _etapasGestaoDevolucao.Etapas.val([]);

    limparLinhasSelecionadasGridDevolucao();
}

function movimentarEtapa(codigoDevolucao) {
    let gestaoDevolucao = _gridGestaoDevolucaoDevolucoes.GridViewTable().row('#' + codigoDevolucao).data();
    if (!gestaoDevolucao) return;

    montarEtapasDevolucao(gestaoDevolucao);

    let novasEtapas = JSON.parse(gestaoDevolucao.Etapas);
    let etapaAtual = novasEtapas.find(e => e.SituacaoEtapa == 3);
    if (!etapaAtual) return;

    for (let etapa in _etapasGestaoDevolucao) {
        if (_etapasGestaoDevolucao[etapa].etapa == etapaAtual.Etapa) {
            if (typeof _etapasGestaoDevolucao[etapa].visible == 'function' && _etapasGestaoDevolucao[etapa].visible())
                $('#' + _etapasGestaoDevolucao[etapa].id).click();
            else
                limparEtapasDevolucao();
        }
    }
}
function atualizarMesmaEtapa(codigoDevolucao) {
    let gestaoDevolucao = _gridGestaoDevolucaoDevolucoes.GridViewTable().row('#' + codigoDevolucao).data();
    if (!gestaoDevolucao) return;

    montarEtapasDevolucao(gestaoDevolucao);

    let novasEtapas = JSON.parse(gestaoDevolucao.Etapas);
    let etapaAtual = novasEtapas.find(e => e.Etapa == gestaoDevolucao.EtapaAtual);
    if (!etapaAtual) return;

    for (let etapa in _etapasGestaoDevolucao) {
        if (typeof _etapasGestaoDevolucao[etapa].isActive == 'function' && _etapasGestaoDevolucao[etapa].isActive() && _etapasGestaoDevolucao[etapa].etapa == etapaAtual.Etapa) {
            $('#' + _etapasGestaoDevolucao[etapa].id).click();
        }
    }
}

function controlarAcoesContainerPrincipal(etapa, objetoKnockout) {
    let deveBloquear =
        etapa.situacao() != 3 ||
        (etapa.acaoTransportador && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) ||
        (!etapa.acaoTransportador &&
            (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe ||
                _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor));

    for (let prop in objetoKnockout) {
        let property = objetoKnockout[prop];
        if (property.hasOwnProperty('enable')) {
            if (typeof property.enable === 'function') {
                property.enable(!deveBloquear);
            } else if (typeof property.enable === 'boolean') {
                property.enable = !deveBloquear;
            }
        }
    }
}
// #endregion Funções Públicas

// #region Funções Privadas
function montarObjetoEtapas(etapas, listaEtapasRegistroSelecionado) {
    let listaEtapas = [];
    for (i = 0; i < etapas.length; i++) {
        let etapaObjetoKnockout = etapas[i];

        let etapa = listaEtapasRegistroSelecionado.find(e => e.Etapa == etapaObjetoKnockout.etapa);

        $.extend(etapaObjetoKnockout, {
            situacao: ko.observable(etapa.SituacaoEtapa),
            ordem: etapa.Ordem,
            class: ko.pureComputed(obterClasseCSSEtapa, etapaObjetoKnockout),
            isActive: ko.pureComputed(function () { return this.situacao() == 3; }, etapaObjetoKnockout),
            enable: ko.pureComputed(function () { return this.situacao() != 0; }, etapaObjetoKnockout),
        });

        if (etapaObjetoKnockout.visible)
            listaEtapas.push(etapaObjetoKnockout);
    }
    listaEtapas.sort(function (a, b) { return a.ordem - b.ordem });

    _etapasGestaoDevolucao.Etapas.val(listaEtapas);
}

function limparLinhasSelecionadasGridDevolucao() {
    let registrosSelecionados = _gridGestaoDevolucaoDevolucoes.ObterMultiplosSelecionados();

    for (let i = 0; i < registrosSelecionados.length; i++) {
        $('#' + registrosSelecionados[i].Codigo).removeClass('selected');
    }
}

function obterClasseCSSEtapa() {
    if (this.etapa == 0)
        return "step cyan";
    else if (this.situacao() == 0)
        return "step";
    else if (this.situacao() == 1)
        return "step green";
    else if (this.situacao() == 2)
        return "step red";
    else if (this.situacao() == 3)
        return "step yellow";
}

function formatDate(date) {
    return date.toLocaleString('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: false // Para usar o formato 24 horas
    }).replace(',', '');
}
// #endregion Funções Privadas