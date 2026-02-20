/// <reference path="../../../../js/Global/Buscas.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAprovacaoFreteSolicitacaoAnexo;
var _htmlAprovacaoFreteSolicitacao;
var _aprovacaoFreteSolicitacao;

/*
 * Declaração das Classes
 */

var AprovacaoFreteSolicitacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.MotivoSolicitacaoDeFrete.getFieldDescription() });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription() });
    this.ListaAnexo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCargaAprovacaoFreteSolicitacao() {
    $.get("Content/Static/Carga/AprovacaoFreteSolicitacao.html?dyn=" + guid(), function (data) {
        _htmlAprovacaoFreteSolicitacao = data;
        LocalizeCurrentPage();
    });
}

function loadGridAprovacaoFreteSolicitacaoAnexo() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), evento: "onclick", metodo: downloadAprovacaoFreteSolicitacaoAnexoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "30%", className: "text-align-left" }
    ];

    _gridAprovacaoFreteSolicitacaoAnexo = new BasicDataTable(_aprovacaoFreteSolicitacao.ListaAnexo.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, linhasPorPaginas);
    _gridAprovacaoFreteSolicitacaoAnexo.CarregarGrid(_aprovacaoFreteSolicitacao.ListaAnexo.val());
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function downloadAprovacaoFreteSolicitacaoAnexoClick(registroSelecionado) {
    executarDownload("CargaSolicitacaoFreteAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function preencherCargaAprovacaoFreteSolicitacao(carga) {
    if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoAlteracaoValorFrete && _CONFIGURACAO_TMS.ObrigarMotivoSolicitacaoFrete)
        buscarAprovacaoFreteSolicitacaoPorCarga(carga);
    else
        limparCargaAprovacaoFreteSolicitacao(carga);
}

/*
 * Declaração das Funções Privadas
 */

function buscarAprovacaoFreteSolicitacaoPorCarga(carga) {
    limparCargaAprovacaoFreteSolicitacao(carga);

    executarReST("CargaAprovacaoFrete/DetalhesSolicitacao", { Codigo: carga.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var idAprovacaoFreteSolicitacao = guid();
                var html = '<div id="' + idAprovacaoFreteSolicitacao + '">' + _htmlAprovacaoFreteSolicitacao + '</div>';

                $("#tabAprovacaoFreteSolicitacao_" + carga.DadosEmissaoFrete.id).html(html);

                _aprovacaoFreteSolicitacao = new AprovacaoFreteSolicitacao();
                KoBindings(_aprovacaoFreteSolicitacao, idAprovacaoFreteSolicitacao);

                PreencherObjetoKnout(_aprovacaoFreteSolicitacao, retorno);
                loadGridAprovacaoFreteSolicitacaoAnexo();

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && carga.TipoOperacao.naoExibirDetalhesDoFretePortalTransportador) {
                    $("#tabAprovacaoFreteSolicitacao_" + carga.DadosEmissaoFrete.id + "_li").hide();
                } else {
                    $("#tabAprovacaoFreteSolicitacao_" + carga.DadosEmissaoFrete.id + "_li").show();
                }
            }
            else if (retorno.Data === false)
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function limparCargaAprovacaoFreteSolicitacao(carga) {
    $("#tabAprovacaoFreteSolicitacao_" + carga.DadosEmissaoFrete.id + "_li").hide();
    $("#tabAprovacaoFreteSolicitacao_" + carga.DadosEmissaoFrete.id).html("");
}
