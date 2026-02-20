/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoOcorrencia.js" />
/// <reference path="OcorrenciaIntegracao.js" />

var _tipoIntegracaoCTeOcorrencia = new Array();
var _gridIntegracaoCTeOcorrencia;
var _gridHistoricoIntegracaoCTeOcorrencia;
var _integracaoCTeOcorrencia;
var _pesquisaHistoricoIntegracaoCTeOcorrencia;

var PesquisaHistoricoIntegracaoCTeOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var IntegracaoCTeOcorrencia = function () {

    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: _tipoIntegracaoCTeOcorrencia, text: Localization.Resources.Ocorrencias.Ocorrencia.Integracao.getFieldDescription(), def: "", issue: 267, visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoIntegracaoCarga.Todas), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Ocorrencias.Ocorrencia.Situacao.getFieldDescription(), def: EnumSituacaoIntegracaoCarga.Todas, issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.Ocorrencia.TotalGeral.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.Ocorrencia.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.Ocorrencia.AguardandoRetorno.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.Ocorrencia.ProblemasIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.Ocorrencia.Integrados.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoCTeOcorrencia.CarregarGrid();
            ObterTotaisIntegracaoCTeOcorrencia();
        }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoCTeOcorrencia();
        }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.ReenviarTodos, idGrid: guid(), visible: ko.observable(true)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoCTeOcorrencia();
        }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.ObterTotais, idGrid: guid(), visible: ko.observable(true)
    });
}

function LoadIntegracaoCTeOcorrencia(ocorrencia, idKnockoutIntegracaoCTe) {

    _integracaoCTeOcorrencia = new IntegracaoCTeOcorrencia();
    _integracaoCTeOcorrencia.Ocorrencia.val(ocorrencia.Codigo.val());

    KoBindings(_integracaoCTeOcorrencia, idKnockoutIntegracaoCTe);

    ObterTotaisIntegracaoCTeOcorrencia();
    ConfigurarPesquisaIntegracaoCTeOcorrencia();
}

function ConfigurarPesquisaIntegracaoCTeOcorrencia() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes, tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Ocorrencias.Ocorrencia.Reenviar, id: guid(), metodo: ReenviarIntegracaoCTeOcorrencia, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracaoOcorrencia });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Ocorrencias.Ocorrencia.AnteciparEnvio, id: guid(), metodo: ReenviarAntecipadamenteIntegracaoCTeOcorrencia, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoEnviarAntecipadamenteIntegracaoOcorrencia });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Ocorrencias.Ocorrencia.HistoricoIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoCTeOcorrencia, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoHistoricoIntegracaoOcorrencia });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Ocorrencias.Ocorrencia.Auditoria, id: guid(), metodo: OpcaoAuditoria("OcorrenciaCTeIntegracao"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria });

    _gridIntegracaoCTeOcorrencia = new GridView(_integracaoCTeOcorrencia.Pesquisar.idGrid, "OcorrenciaIntegracaoCTe/Pesquisa", _integracaoCTeOcorrencia, menuOpcoes);
    _gridIntegracaoCTeOcorrencia.CarregarGrid();
}

function VisibilidadeOpcaoReenviarIntegracaoOcorrencia(data) {
    if (data.Tipo == EnumTipoIntegracao.Natura)
        return false;

    return true;
}

function VisibilidadeOpcaoEnviarAntecipadamenteIntegracaoOcorrencia(data) {
    const dataAtual = moment();
    if (data.Tipo == EnumTipoIntegracao.Natura && ((data.Situacao == EnumSituacaoIntegracao.AgIntegracao) && (data.DataEnvio < dataAtual) && (data.Tentativas == 0)))
        return false;

    return true;
}

function VisibilidadeOpcaoHistoricoIntegracaoOcorrencia(data) {
    if (data.Tipo == EnumTipoIntegracao.FTP)
        return false;

    return true;
}

function ObterTiposIntegracaoCTeOcorrencia() {
    var p = new promise.Promise();

    executarReST("TipoIntegracao/BuscarTodos", {
        Tipos: JSON.stringify([
            EnumTipoIntegracao.Avior,
            EnumTipoIntegracao.Avon,
            EnumTipoIntegracao.Natura,
            EnumTipoIntegracao.FTP,
            EnumTipoIntegracao.Michelin])
    }, function (r) {
        if (r.Success) {

            _tipoIntegracaoCTeOcorrencia.push({ value: "", text: Localization.Resources.Ocorrencias.Ocorrencia.Todas });

            for (var i = 0; i < r.Data.length; i++)
                _tipoIntegracaoCTeOcorrencia.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }

        p.done();
    });

    return p;
}

function ReenviarIntegracaoCTeOcorrencia(data) {
    executarReST("OcorrenciaIntegracaoCTe/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.ReenvioSolicitadoComSucesso);
            _gridIntegracaoCTeOcorrencia.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ReenviarAntecipadamenteIntegracaoCTeOcorrencia(data) {
    executarReST("OcorrenciaIntegracaoCTe/AnteciparEnvio", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.ReenvioSolicitadoComSucesso);
            _gridIntegracaoCTeOcorrencia.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoCTeOcorrencia() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.DesejaRealmenteReenviarTodasIntegracoesCTe, function () {
        executarReST("OcorrenciaIntegracaoCTe/ReenviarTodos", { Ocorrencia: _integracaoCTeOcorrencia.Ocorrencia.val(), Tipo: _integracaoCTeOcorrencia.Tipo.val(), Situacao: _integracaoCTeOcorrencia.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.ReenvioSolicitadoComSucesso);
                _gridIntegracaoCTeOcorrencia.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function ObterTotaisIntegracaoCTeOcorrencia() {
    executarReST("OcorrenciaIntegracaoCTe/ObterTotais", { Ocorrencia: _integracaoCTeOcorrencia.Ocorrencia.val() }, function (r) {
        if (r.Success) {
            _integracaoCTeOcorrencia.TotalGeral.val(r.Data.TotalGeral);
            _integracaoCTeOcorrencia.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoCTeOcorrencia.TotalAguardandoRetorno.val(r.Data.TotalAguardandoRetorno);
            _integracaoCTeOcorrencia.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoCTeOcorrencia.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ExibirHistoricoIntegracaoCTeOcorrencia(integracao) {
    BuscarHistoricoIntegracaoCTeOcorrencia(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoCTe");
}

function BuscarHistoricoIntegracaoCTeOcorrencia(integracao) {
    _pesquisaHistoricoIntegracaoCTeOcorrencia = new PesquisaHistoricoIntegracaoCTeOcorrencia();
    _pesquisaHistoricoIntegracaoCTeOcorrencia.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCTeOcorrencia, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoCTeOcorrencia = new GridView("tblHistoricoIntegracaoCTe", "OcorrenciaIntegracaoCTe/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoCTeOcorrencia, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoCTeOcorrencia.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoCTeOcorrencia(historicoConsulta) {
    executarDownload("OcorrenciaIntegracaoCTe/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}
