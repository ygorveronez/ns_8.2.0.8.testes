/// <reference path="EtapaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoAlteracaoFreteCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _aprovacaoFrete;
var _cargaSelecionadaAprovacaoFrete;
var _cargaSelecionadaSituacaoRetornoDadosFrete;
var _detalheAutorizacaoFrete;
var _gridAutorizacoesFrete;
var _htmlAprovacaoFrete;

/*
 * Declaração das Classes
 */

var AprovacaoFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(_cargaSelecionadaAprovacaoFrete.Codigo.val()), def: 0, getType: typesKnockout.int });
    this.IdIconeAba = PropertyEntity({ val: ko.observable("#tabAprovacaoFreteIcone_" + _cargaSelecionadaAprovacaoFrete.DadosEmissaoFrete.id) });

    this.AprovacoesNecessarias = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Cargas.Carga.AprovacoesNecessarias.getFieldDescription(), enable: ko.observable(true) });
    this.Aprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Cargas.Carga.Aprovacoes.getFieldDescription(), enable: ko.observable(true) });
    this.CorIconeAba = PropertyEntity({ val: ko.observable("") });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Reprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: Localization.Resources.Cargas.Carga.Reprovacoes.getFieldDescription(), enable: ko.observable(true) });
    this.Autorizacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });

    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: Localization.Resources.Cargas.Carga.ReprocessarRegras });
}

var DetalheAutorizacaoFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regra = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Regra.getFieldDescription(), val: ko.observable("") });
    this.Data = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Data.getFieldDescription(), val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Situacao.getFieldDescription(), val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Usuario.getFieldDescription(), val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Motivo.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCargaAprovacaoFrete() {
    $.get("Content/Static/Carga/AprovacaoFrete.html?dyn=" + guid(), function (data) {
        _htmlAprovacaoFrete = data;
    });
}

function loadDetalheCargaAprovacaoFrete() {
    _detalheAutorizacaoFrete = new DetalheAutorizacaoFrete();
    KoBindings(_detalheAutorizacaoFrete, "knockoutDetalheAutorizacaoFrete");
}

function loadGridAutorizacoesFrete() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "Regra", visible: false },
        { data: "Data", visible: false },
        { data: "Motivo", visible: false },
        { data: "Usuario", title: Localization.Resources.Cargas.Carga.Usuario, width: "40%" },
        { data: "PrioridadeAprovacao", title: Localization.Resources.Cargas.Carga.Prioridade, width: "20%", className: "text-align-center" },
        { data: "Situacao", title: Localization.Resources.Cargas.Carga.Situacao, width: "20%" }
    ];

    var opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: "clasEditar", evento: "onclick", metodo: detalharAutorizacaoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    _gridAutorizacoesFrete = new BasicDataTable(_aprovacaoFrete.Autorizacoes.idGrid, header, menuOpcoes);
    _gridAutorizacoesFrete.CarregarGrid(_aprovacaoFrete.Autorizacoes.val());
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function detalharAutorizacaoClick(registroSelecionado) {
    _detalheAutorizacaoFrete.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_detalheAutorizacaoFrete, "CargaAprovacaoFrete/DetalhesAutorizacao", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                Global.abrirModal("divModalDetalhesAutorizacaoFrete");
                $("#divModalDetalhesAutorizacaoFrete").one('hidden.bs.modal', function () {
                    LimparCampos(_detalheAutorizacaoFrete);
                });
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function reprocessarRegrasClick() {
    executarReST("CargaAprovacaoFrete/ReprocessarRegras", { Codigo: _aprovacaoFrete.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                buscarAprovacaoFretePorCarga();
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function preencherCargaAprovacaoFrete(carga, retornoFrete) {
    if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoAlteracaoValorFrete) {
        _cargaSelecionadaAprovacaoFrete = carga;
        _cargaSelecionadaSituacaoRetornoDadosFrete = retornoFrete.situacao;

        buscarAprovacaoFretePorCarga();
    }
    else
        limparCargaAprovacaoFrete(carga);
}

/*
 * Declaração das Funções Privadas
 */

function buscarAprovacaoFretePorCarga() {
    limparCargaAprovacaoFrete(_cargaSelecionadaAprovacaoFrete);

    executarReST("CargaAprovacaoFrete/PesquisaAutorizacao", { Codigo: _cargaSelecionadaAprovacaoFrete.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var idAprovacaoFrete = guid();
                var html = '<div id="' + idAprovacaoFrete + '">' + _htmlAprovacaoFrete + '</div>';

                $("#tabAprovacaoFrete_" + _cargaSelecionadaAprovacaoFrete.DadosEmissaoFrete.id).html(html);

                _aprovacaoFrete = new AprovacaoFrete();
                KoBindings(_aprovacaoFrete, idAprovacaoFrete);
                LocalizeCurrentPage();

                PreencherObjetoKnout(_aprovacaoFrete, retorno);
                loadGridAutorizacoesFrete();

                $(_aprovacaoFrete.IdIconeAba.val()).css("color", _aprovacaoFrete.CorIconeAba.val());

                if (EnumSituacaoAlteracaoFreteCarga.isPermiteAvancarEtapa(retorno.Data.SituacaoAlteracaoFreteCarga))
                    _cargaSelecionadaAprovacaoFrete.AutorizarEmissaoDocumentos.enable(_cargaSelecionadaAprovacaoFrete.EtapaFreteEmbarcador.enable());
                else if (!EnumSituacaoRetornoDadosFrete.isPendenciaFrete(_cargaSelecionadaSituacaoRetornoDadosFrete)) {
                    if (retorno.Data.SituacaoAlteracaoFreteCarga == EnumSituacaoAlteracaoFreteCarga.AguardandoAprovacao) {
                        EtapaDadosTransportadorDesabilitada(_cargaSelecionadaAprovacaoFrete);
                        EtapaFreteEmbarcadorAguardando(_cargaSelecionadaAprovacaoFrete);
                    }
                    else
                        EtapaFreteEmbarcadorProblema(_cargaSelecionadaAprovacaoFrete);

                    _cargaSelecionadaAprovacaoFrete.AutorizarEmissaoDocumentos.enable(true);
                }

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && _cargaSelecionadaAprovacaoFrete.TipoOperacao.naoExibirDetalhesDoFretePortalTransportador) {
                    $("#tabAprovacaoFrete_" + _cargaSelecionadaAprovacaoFrete.DadosEmissaoFrete.id + "_li").hide();
                } else {
                    $("#tabAprovacaoFrete_" + _cargaSelecionadaAprovacaoFrete.DadosEmissaoFrete.id + "_li").show();
                }
                
            }
            else if (retorno.Data === false)
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function limparCargaAprovacaoFrete(carga) {
    $("#tabAprovacaoFrete_" + carga.DadosEmissaoFrete.id + "_li").hide();
    $("#tabAprovacaoFrete_" + carga.DadosEmissaoFrete.id).html("");
}
