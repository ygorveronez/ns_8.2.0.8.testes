/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="../../../Enumeradores/EnumTipoIntegracao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridIntegracaoCargaTransportador;
var _gridHistoricoIntegracaoCargaTransportador;
var _integracaoCargaTransportador;
var _pesquisaHistoricoIntegracaoCarga;
var _tipoIntegracaoCargaTransportador = new Array();

/*
 * Declaração das Classes
 */

var PesquisaHistoricoIntegracaoCargaTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var IntegracaoCargaTransportador = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.IntegracaoTransportador = PropertyEntity({ type: types.bool, val: ko.observable(true), def: true, text: Localization.Resources.Cargas.Carga.SomenteIntegracaoDoTransportador, visible: false });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: _tipoIntegracaoCargaTransportador, text: Localization.Resources.Cargas.Carga.Integracao.getFieldDescription(), def: "", visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "" });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.TotalGeral.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.AguardandoRetorno.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.ProblemasNaIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.Integrados.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoCargaTransportador.CarregarGrid();
            obterTotaisIntegracaoCargaTransportador();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ReenviarTodas = PropertyEntity({ eventClick: reenviarTodasIntegracaoCargaTransportadorClick, type: types.event, text: Localization.Resources.Cargas.Carga.ReenviarTodos, idGrid: guid(), visible: ko.observable(false) });
    this.ObterTotais = PropertyEntity({ eventClick: obterTotaisIntegracaoCargaTransportador, type: types.event, text: Localization.Resources.Cargas.Carga.ObterTotais, idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridIntegracaoCargaTransportador() {
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.Reenviar, id: guid(), metodo: reenviarIntegracaoCargaTransportadorClick, tamanho: "20", icone: "", visibilidade: isOpcaoReenviarIntegracaoCargaTransportadorVisivel });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.HistoricoDeIntegracao, id: guid(), metodo: exibirHistoricoIntegracaoCargaTransportador, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Cargas.Carga.DownloadAutorizacaoEmbarque, id: guid(), metodo: DownloadAutorizacaoEmbarque, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoImpressaoAutorizacaoEmbarque });

    _gridIntegracaoCargaTransportador = new GridView(_integracaoCargaTransportador.Pesquisar.idGrid, "CargaIntegracaoCarga/Pesquisa", _integracaoCargaTransportador, menuOpcoes);
    _gridIntegracaoCargaTransportador.CarregarGrid();
}

function VisibilidadeOpcaoImpressaoAutorizacaoEmbarque(data) {
    if (data.Tipo == EnumTipoIntegracao.OpenTech)
        return true;

    return false;
}

function loadIntegracaoCargaTransportador(carga) {
    if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) && (_tipoIntegracaoCargaTransportador.length > 0)) {
        $("#KnockoutTransportadorIntegracao_" + carga.EtapaDadosTransportador.idGrid).html(_HTMLIntegracaoCargaTransportador);

        _integracaoCargaTransportador = new IntegracaoCargaTransportador();
        KoBindings(_integracaoCargaTransportador, "KnockoutTransportadorIntegracao_" + carga.EtapaDadosTransportador.idGrid);
        LocalizeCurrentPage();

        _integracaoCargaTransportador.Carga.val(carga.Codigo.val());

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga))
            _integracaoCargaTransportador.ReenviarTodas.visible(true);

        obterTotaisIntegracaoCargaTransportador();
        loadGridIntegracaoCargaTransportador();
        LoadCargaDadosTransporteIntegracao(carga);

        if (_tipoIntegracaoCargaTransportador.length > 1) {
            $("#" + _integracaoCargaTransportador.Pesquisar.id).removeClass("input-margin-top-24-md");
            $("#" + _integracaoCargaTransportador.ReenviarTodas.id).removeClass("input-margin-top-24-md");

            _integracaoCargaTransportador.Tipo.visible(true);
        }
    }
    else {
        $("#tabTransportadorIntegracao_" + carga.EtapaDadosTransportador.idGrid + "_li").hide();
        $("#KnockoutTransportadorIntegracao_" + carga.EtapaDadosTransportador.idGrid).html("");
    }
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function downloadArquivosHistoricoIntegracaoCargaTransportadorClick(historicoConsulta) {
    executarDownload("CargaIntegracaoCarga/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function reenviarIntegracaoCargaTransportadorClick(integracaoSelecionada) {
    executarReST("CargaIntegracaoCarga/Reenviar", { Codigo: integracaoSelecionada.Codigo, IntegracaoTransportador: true }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ReenvioSolicitadoComSucesso);
                _gridIntegracaoCargaTransportador.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
        }
    });
}

function reenviarTodasIntegracaoCargaTransportadorClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteReenviarTodasAsIntegracoes, function () {
        executarReST("CargaIntegracaoCarga/ReenviarTodos", { Carga: _integracaoCargaTransportador.Carga.val(), Tipo: _integracaoCargaTransportador.Tipo.val(), Situacao: _integracaoCargaTransportador.Situacao.val(), IntegracaoTransportador: true }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ReenvioSolicitadoComSucesso);
                    _gridIntegracaoCargaTransportador.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
            }
        });
    });
}

/*
 * Declaração das Funções
 */

function buscarHistoricoIntegracaoCargaTransportador(integracao) {
    _pesquisaHistoricoIntegracaoCargaTransportador = new PesquisaHistoricoIntegracaoCargaTransportador();
    _pesquisaHistoricoIntegracaoCargaTransportador.Codigo.val(integracao.Codigo);

    var opcaoDownload = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos, id: guid(), evento: "onclick", metodo: downloadArquivosHistoricoIntegracaoCargaTransportadorClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };

    _gridHistoricoIntegracaoCargaTransportador = new GridView("tblHistoricoIntegracaoCTe", "CargaIntegracaoCarga/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoCargaTransportador, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoCargaTransportador.CarregarGrid();
}

function isOpcaoReenviarIntegracaoCargaTransportadorVisivel(integracaoSelecionada) {
    if (_CONFIGURACAO_TMS.UsuarioAdministrador)
        return true;

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga) || integracaoSelecionada.SituacaoIntegracao != EnumSituacaoIntegracaoCarga.ProblemaIntegracao)
        return false;

    return true;
}

function obterTiposIntegracaoCargaTransportador() {
    var p = new promise.Promise();

    var dados = {
        IntegracaoTransportador: true
    };

    executarReST("TipoIntegracao/BuscarTodos", dados, function (r) {
        if (r.Success) {
            _tipoIntegracaoCargaTransportador.push({ value: "", text: Localization.Resources.Gerais.Geral.Todas });

            for (var i = 0; i < r.Data.length; i++)
                _tipoIntegracaoCargaTransportador.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);

        p.done();
    });

    return p;
}

function obterTotaisIntegracaoCargaTransportador() {
    executarReST("CargaIntegracaoCarga/ObterTotais", { Carga: _integracaoCargaTransportador.Carga.val(), IntegracaoTransportador: true }, function (r) {
        if (r.Success) {
            _integracaoCargaTransportador.TotalGeral.val(r.Data.TotalGeral);
            _integracaoCargaTransportador.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoCargaTransportador.TotalAguardandoRetorno.val(r.Data.TotalAguardandoRetorno);
            _integracaoCargaTransportador.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoCargaTransportador.TotalIntegrado.val(r.Data.TotalIntegrado);

            if (r.Data.TotalGeral > 0)
                $("#tabTransportadorIntegracao_" + _cargaAtual.EtapaDadosTransportador.idGrid + "_li").show();
            else {
                $("#tabTransportadorIntegracao_" + _cargaAtual.EtapaDadosTransportador.idGrid + "_li").hide();
                $("#KnockoutTransportadorIntegracao_" + _cargaAtual.EtapaDadosTransportador.idGrid).html("");
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function exibirHistoricoIntegracaoCargaTransportador(integracao) {
    buscarHistoricoIntegracaoCargaTransportador(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoCTe");
}

function recarregarIntegracaoCargaTransportadorViaSignalR(knoutCarga) {
    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id) {
        if ($("#tabTransportadorIntegracao_" + _cargaAtual.EtapaDadosTransportador.idGrid).is(":visible") && _gridIntegracaoCargaTransportador != null) {
            _gridIntegracaoCargaTransportador.CarregarGrid();
            obterTotaisIntegracaoCargaTransportador();
        }
    }
}