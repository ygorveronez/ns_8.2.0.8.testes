/// <reference path="CancelamentoProvisao.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoProvisao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoCarga;
var _historicoIntegracaoCancelamentoProvisao;
var _gridIntegracaoCarga;
var _gridHistoricoIntegracaoCancelamentoProvisao;

var _situacaoIntegracaoCarga = [
    { value: "", text: "Todas" },
    { value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
    { value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
    { value: EnumSituacaoIntegracaoCarga.AgRetorno, text: "Ag. Retorno" },
    { value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }
];

var IntegracaoCarga = function () {
    this.CancelamentoProvisao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoCarga, text: "Situação:", def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoCarga();
            _gridIntegracaoCarga.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoCarga();
        }, type: types.event, text: "Reenviar Todos", idGrid: guid(), visible: ko.observable(true)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoCarga();
        }, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true)
    });
}

var HistoricoIntegracaoCancelamentoProvisao = function () {
    this.Integracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ idGrid: guid()});
}

//*******EVENTOS*******
function loadIntegracaoCancelamentoProvisao() {
    _integracaoCarga = new IntegracaoCarga();
    KoBindings(_integracaoCarga, "knockoutIntegracaoCarga");

    _historicoIntegracaoCancelamentoProvisao = new HistoricoIntegracaoCancelamentoProvisao();
    KoBindings(_historicoIntegracaoCancelamentoProvisao, "knockoutHistoricoCancelamentoProvisao");
    CarregarHistoricoIntegracaoCancelamentoProvisao();

    if (!_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos) {
        $("#liIntegracaoEDI").show();
        $("#liIntegracaoEDI").addClass("active");

        $("#liIntegracaoCarga").hide();
        $("#liIntegracaoCarga").removeClass("active");

        $("#knockoutIntegracao").addClass("active in");
        $("#knockoutIntegracaoCarga").removeClass("active in");
    }

}

function DownloadIntegracaoCarga(data) {
    executarDownload("CancelamentoProvisaoIntegracao/Download", { Codigo: data.Codigo });
}

function ReenviarIntegracaoCarga(data) {
    executarReST("CancelamentoProvisaoIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridIntegracaoCarga.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function HistoricoIntegracaoCargaClick(data) {
    Global.abrirModal('divModalHistoricoIntegracaoCancelamentoProvisao');
    _historicoIntegracaoCancelamentoProvisao.Integracao.val(data.Codigo);
    _gridHistoricoIntegracaoCancelamentoProvisao.CarregarGrid();
}
function DownloadArquivosHistoricoIntegracaoCancelamentoProvisao(historicoConsulta) {
    executarDownload("CancelamentoProvisaoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo, Integracao: _historicoIntegracaoCancelamentoProvisao.Integracao.val() });
}


//*******MÉTODOS*******
function CarregaIntegracaoCarga() {
    _integracaoCarga.CancelamentoProvisao.val(_cancelamentoProvisao.Codigo.val());
    ObterTotaisIntegracaoCarga();
    ConfigurarPesquisaIntegracaoCarga();
}

function ObterTotaisIntegracaoCarga() {
    executarReST("CancelamentoProvisaoIntegracao/ObterTotais", { CancelamentoProvisao: _integracaoCarga.CancelamentoProvisao.val() }, function (r) {
        if (r.Success) {
            _integracaoCarga.TotalGeral.val(r.Data.TotalGeral);
            _integracaoCarga.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoCarga.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoCarga.TotalIntegrado.val(r.Data.TotalIntegrado);
         
            if (_integracaoCarga.TotalGeral.val() > 0)
                $("#liIntegracaoCarga").show();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoCarga() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações?", function () {
        executarReST("CancelamentoProvisaoIntegracao/ReenviarTodos", { CancelamentoProvisao: _integracaoCarga.CancelamentoProvisao.val(), Situacao: _integracaoCarga.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                _gridIntegracaoCarga.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function CarregarHistoricoIntegracaoCancelamentoProvisao() {
    var editar = { descricao: "Download Arquivos", id: "clasEditar", evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCancelamentoProvisao, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };
    _gridHistoricoIntegracaoCancelamentoProvisao = new GridView(_historicoIntegracaoCancelamentoProvisao.Grid.idGrid, "CancelamentoProvisaoIntegracao/PesquisaHistorico", _historicoIntegracaoCancelamentoProvisao, menuOpcoes);
}

function ConfigurarPesquisaIntegracaoCarga() {
    var historico = { descricao: "Histórico", id: guid(), metodo: HistoricoIntegracaoCargaClick, tamanho: "20", icone: "" };
    var download = { descricao: "Download", id: guid(), metodo: DownloadIntegracaoCarga, tamanho: "20", icone: "" };
    var reenviar = { descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoCarga, tamanho: "20", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [historico, download, reenviar] };

    _gridIntegracaoCarga = new GridView(_integracaoCarga.Pesquisar.idGrid, "CancelamentoProvisaoIntegracao/Pesquisa", _integracaoCarga, menuOpcoes);
    _gridIntegracaoCarga.CarregarGrid();
    //_gridIntegracaoCarga.CarregarGrid(function () {
    //    if (_gridIntegracaoCarga.NumeroRegistros() > 0)
    //        $("#liIntegracaoCarga").show();
    //    else
    //        $("#liIntegracaoCarga").hide();
    //});
}