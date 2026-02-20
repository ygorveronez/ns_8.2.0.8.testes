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
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />
/// <reference path="OcorrenciaIntegracao.js" />

var _gridIntegracaoEDIOcorrencia;
var _integracaoEDIOcorrencia;

var _situacaoIntegracaoEDI = [{ value: "", text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
{ value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
{ value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }];


var IntegracaoEDIOcorrencia = function () {

    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FilialEmissora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoEDI, text: Localization.Resources.Ocorrencias.Ocorrencia.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.Ocorrencia.TotalGeral.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.Ocorrencia.AguardandoIntegracao.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.Ocorrencia.ProblemasIntegracao.getFieldDescription()  });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.Ocorrencia.Integrados.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoEDIOcorrencia();
            _gridIntegracaoEDIOcorrencia.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoEDIOcorrencia();
        }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.ReenviarTodos, idGrid: guid(), visible: ko.observable(false)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoEDIOcorrencia();
        }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.ObterTotais, idGrid: guid(), visible: ko.observable(true)
    });
}

function LoadIntegracaoEDIOcorrencia(ocorrencia, idKnockoutIntegracaoEDI, filialEmissora) {
    if (filialEmissora === null)
        filialEmissora = false;

    _integracaoEDIOcorrencia = new IntegracaoEDIOcorrencia();

    _integracaoEDIOcorrencia.Ocorrencia.val(ocorrencia.Codigo.val());
    _integracaoEDIOcorrencia.FilialEmissora.val(filialEmissora);

    KoBindings(_integracaoEDIOcorrencia, idKnockoutIntegracaoEDI);

    ObterTotaisIntegracaoEDIOcorrencia();
    ConfigurarPesquisaIntegracaoEDIOcorrencia();

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasOcorrencia))
        _integracaoEDIOcorrencia.ReenviarTodos.visible(true);
}

function ConfigurarPesquisaIntegracaoEDIOcorrencia() {
    var download = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Download, id: guid(), metodo: DownloadIntegracaoEDIOcorrencia, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoDownloadIntegracaoEDIOcorrencia };
    var reenviar = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Reenviar, id: guid(), metodo: ReenviarIntegracaoEDIOcorrencia, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracaoEDIOcorrencia };
    var auditoria = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Auditoria, id: guid(), metodo: OpcaoAuditoria("OcorrenciaEDIIntegracao"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes, tamanho: 7, opcoes: [download, reenviar, auditoria] };

    _gridIntegracaoEDIOcorrencia = new GridView(_integracaoEDIOcorrencia.Pesquisar.idGrid, "OcorrenciaIntegracaoEDI/Pesquisa", _integracaoEDIOcorrencia, menuOpcoes);

    _gridIntegracaoEDIOcorrencia.CarregarGrid();
}

function VisibilidadeOpcaoDownloadIntegracaoEDIOcorrencia(data) {
    return VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_DownloadArquivoIntegracoes, _PermissoesPersonalizadasOcorrencia);
}

function VisibilidadeOpcaoReenviarIntegracaoEDIOcorrencia(data) {
    if (data.Tipo === EnumTipoIntegracao.NaoPossuiIntegracao || !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasOcorrencia))
        return false;

    return true;
}

function DownloadIntegracaoEDIOcorrencia(data) {
    executarDownload("OcorrenciaIntegracaoEDI/Download", { Codigo: data.Codigo });
}

function ReenviarIntegracaoEDIOcorrencia(data) {
    exibirConfirmacao("Atenção!", Localization.Resources.Ocorrencias.Ocorrencia.DesejaRealmenteReenviarArquivoEDI, function () {
        executarReST("OcorrenciaIntegracaoEDI/Reenviar", { Codigo: data.Codigo, FilialEmissora: _integracaoEDIOcorrencia.FilialEmissora.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.ReenvioSolicitadoComSucesso);
                _gridIntegracaoEDIOcorrencia.CarregarGrid();
                _resumoOcorrencia.Situacao.val(Localization.Resources.Ocorrencias.Ocorrencia.AgIntegracao);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function ReenviarTodosIntegracaoEDIOcorrencia() {
    exibirConfirmacao("Atenção!", Localization.Resources.Ocorrencias.Ocorrencia.DesejaRealmenteReenviarTodasIntegracoesEDI , function () {
        executarReST("OcorrenciaIntegracaoEDI/ReenviarTodos", { Ocorrencia: _integracaoEDIOcorrencia.Ocorrencia.val(), Situacao: _integracaoEDIOcorrencia.Situacao.val(), FilialEmissora: _integracaoEDIOcorrencia.FilialEmissora.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.ReenvioSolicitadoComSucesso);
                _gridIntegracaoEDIOcorrencia.CarregarGrid();
                _resumoOcorrencia.Situacao.val(Localization.Resources.Ocorrencias.Ocorrencia.AgIntegracao);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function ObterTotaisIntegracaoEDIOcorrencia() {
    executarReST("OcorrenciaIntegracaoEDI/ObterTotais", { Ocorrencia: _integracaoEDIOcorrencia.Ocorrencia.val() }, function (r) {
        if (r.Success) {
            _integracaoEDIOcorrencia.TotalGeral.val(r.Data.TotalGeral);
            _integracaoEDIOcorrencia.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoEDIOcorrencia.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoEDIOcorrencia.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

