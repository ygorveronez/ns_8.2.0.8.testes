/// <reference path="EtapaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoAlteracaoFreteCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _integracaoValorPedagio;
var _cargaSelecionadaValorPedagio;
var _detalheValePedagio;
var _htmlValePedagio;
var _gridAprovacaoFreteSolicitacaoAnexo;

/*
 * Declaração das Classes
 */
var IntegracaoValorPedagio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(true), def: 0, getType: typesKnockout.int });

    this.SituacaoConsulta = PropertyEntity({ text: Localization.Resources.Cargas.Carga.SituacaoConsulta.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.Integradora = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Integradora.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.ValorPedagio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorPegadio.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.RotaFreteExclusiva = PropertyEntity({ text: Localization.Resources.Cargas.Carga.RotaFreteExcluciva.getFieldDescription(), val: ko.observable("") });
    this.DataIntegracao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataIntegracao.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.ProblemaIntegracao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Retorno.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.ListaArquivos = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Arquivos, type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.DetalhesIntegracao = PropertyEntity({ eventClick: detalhesIntegracaoClick, type: types.event, text: Localization.Resources.Cargas.Carga.DetalhesIntegracao, visible: ko.observable(true) });
    this.ReenviarIntegracao = PropertyEntity({ eventClick: reenviarIntegracaoClick, type: types.event, text: Localization.Resources.Cargas.Carga.Reenviar, visible: ko.observable(false) });
    this.LiberarIntegracao = PropertyEntity({ eventClick: liberarIntegracaoClick, type: types.event, text: Localization.Resources.Cargas.Carga.LiberarSemConsulta, visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadCargaIntegracaoValePedagio() {
    $.get("Content/Static/Carga/IntegracaoValePedagio.html?dyn=" + guid(), function (data) {
        _htmlValePedagio = data;
    });
}

function loadGridConsultaValePedagioSolicitacao() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: Localization.Resources.Cargas.Carga.Download, id: guid(), evento: "onclick", metodo: downloadArquivoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Data", title: Localization.Resources.Cargas.Carga.Data, width: "15%", className: "text-align-left" },
        { data: "DescricaoTipo", title: Localization.Resources.Cargas.Carga.Tipo, width: "10%", className: "text-align-left" },
        { data: "Mensagem", title: Localization.Resources.Cargas.Carga.Mensagem, width: "30%", className: "text-align-left" }
    ];

    _gridAprovacaoFreteSolicitacaoAnexo = new BasicDataTable(_integracaoValorPedagio.ListaArquivos.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.desc }, null, linhasPorPaginas);
    _gridAprovacaoFreteSolicitacaoAnexo.CarregarGrid(_integracaoValorPedagio.ListaArquivos.val());
}

function detalhesIntegracaoClick() {
    Global.abrirModal("divModalDetalhesIntegracaoArquivo");
}

function liberarIntegracaoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaLiberarSemConsultaDeIntegracao, function () {
        executarReST("CargaConsultaValorPedagioIntegracao/LiberarSemConsulta", { Carga: _cargaSelecionadaAprovacaoFrete.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    buscarIntegracaoValePedagioPorCarga();
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function reenviarIntegracaoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaReenviarIntegracao, function () {
        executarReST("CargaConsultaValorPedagioIntegracao/ReenviarIntegracaoConsultaValePedagio", { Codigo: _integracaoValorPedagio.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    buscarIntegracaoValePedagioPorCarga();
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function downloadArquivoClick(registroSelecionado) {
    executarDownload("CargaConsultaValorPedagioIntegracao/DownloadArquivosIntegracao", { Codigo: registroSelecionado.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function preencherCargaValoresValePedagio(carga) {
    if (_CONFIGURACAO_TMS.PermitirConsultaDeValoresPedagio || carga.TipoOperacao.permitirConsultaDeValoresPedagioSemParar) {
        _cargaSelecionadaAprovacaoFrete = carga;
        buscarIntegracaoValePedagioPorCarga();
    }
    else
        limparIntegracaoValePedagio(carga);
}

/*
 * Declaração das Funções Privadas
 */

function buscarIntegracaoValePedagioPorCarga() {
    limparIntegracaoValePedagio(_cargaSelecionadaAprovacaoFrete);

    executarReST("CargaConsultaValorPedagioIntegracao/PesquisaIntegracaoCarga", { Codigo: _cargaSelecionadaAprovacaoFrete.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var idIntegracaValePedagio = guid();
                var html = '<div id="' + idIntegracaValePedagio + '">' + _htmlValePedagio + '</div>';
                $("#tabValorValePedagio_" + _cargaSelecionadaAprovacaoFrete.DadosEmissaoFrete.id).html(html);
                _integracaoValorPedagio = new IntegracaoValorPedagio();
                KoBindings(_integracaoValorPedagio, idIntegracaValePedagio);
                PreencherObjetoKnout(_integracaoValorPedagio, retorno);
                $("#tabValorValePedagio_" + _cargaSelecionadaAprovacaoFrete.DadosEmissaoFrete.id + "_li").show();

                loadGridConsultaValePedagioSolicitacao();

                if (retorno.Data.Situacao == EnumSituacaoIntegracao.ProblemaIntegracao) {
                    _integracaoValorPedagio.ReenviarIntegracao.visible(true);
                    _integracaoValorPedagio.LiberarIntegracao.visible(true);
                }
            }
            else
                limparIntegracaoValePedagio(_cargaSelecionadaAprovacaoFrete);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function limparIntegracaoValePedagio(carga) {
    $("#tabValorValePedagio_" + carga.DadosEmissaoFrete.id + "_li").hide();
    $("#tabValorValePedagio_" + carga.DadosEmissaoFrete.id).html("");
}